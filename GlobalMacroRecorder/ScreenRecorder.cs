using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;
using System.IO;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using GlobalMacroRecorder;

namespace Captura
{
    public class DesktopRecorder
    {
        static Recorder rec = null;
        static FourCC defaultCodec = SharpAvi.KnownFourCCs.Codecs.MotionJpeg;
        public static void Start(string fileName, double scale, FourCC? codec = null, int frameRate = 25, int quality = 25, Rectangle area = default(Rectangle))
        {
            if (area == default(Rectangle))
            {
                area = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            }
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            if (rec != null) return;
            if (codec == null)
                codec = defaultCodec;
            // Using MotionJpeg as Avi encoder,
            // output to 'out.avi' at 10 Frames per second, 70% quality
            //var rec = new Recorder(new RecorderParams("out.avi", 10, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, 70));
            var recorderParams = new RecorderParams(fileName, scale, frameRate, codec.Value, quality, area);
            rec = new Recorder(recorderParams);
            rec.StartAsync();
        }
        public static void Stop()
        {

            rec?.Dispose();
            rec = null;
        }
    }
    // Used to Configure the Recorder
    public class RecorderParams
    {

        public RecorderParams(string filename, double scale, int FrameRate, FourCC Encoder, int Quality)
        {
            FileName = filename;
            FramesPerSecond = FrameRate;
            Codec = Encoder;
            this.Quality = Quality;

            Height = (int)(Screen.PrimaryScreen.Bounds.Height * scale);
            Width = (int)(Screen.PrimaryScreen.Bounds.Width * scale);
        }

        public RecorderParams(string filename, double scale, int FrameRate, FourCC Encoder, int Quality, Rectangle area)
        {
            FileName = filename;
            FramesPerSecond = FrameRate;
            Codec = Encoder;
            this.Quality = Quality;

            this.Height = area.Height;
            this.Width = area.Width;
            this.Location = area.Location;
            //Height = (int)(Screen.PrimaryScreen.Bounds.Height * scale);
            //Width = (int)(Screen.PrimaryScreen.Bounds.Width * scale);
        }
        public RecorderParams(string filename, double scale, int FrameRate, FourCC Encoder, int Quality, int height, int width)
        {
            FileName = filename;
            FramesPerSecond = FrameRate;
            Codec = Encoder;
            this.Quality = Quality;

            Height = (int)(height * scale);
            Width = (int)(width * scale);
        }

        string FileName;
        public int FramesPerSecond, Quality;
        FourCC Codec;

        public int Height { get; private set; }
        public int Width { get; private set; }
        public Point Location { get; set; }
        public AviWriter CreateAviWriter()
        {
            var fs = File.OpenWrite(FileName);
            return new AviWriter(fs, false)
            {
                FramesPerSecond = FramesPerSecond,
                EmitIndex1 = true,
            };
        }

        public IAviVideoStream CreateVideoStream(AviWriter writer)
        {
            // Select encoder type based on FOURCC of codec
            if (Codec == KnownFourCCs.Codecs.Uncompressed)
                return writer.AddUncompressedVideoStream(Width, Height);
            else if (Codec == KnownFourCCs.Codecs.MotionJpeg)
                return writer.AddMotionJpegVideoStream(Width, Height, Quality);
            else
            {
                return writer.AddMpeg4VideoStream(Width, Height, (double)writer.FramesPerSecond,
                    // It seems that all tested MPEG-4 VfW codecs ignore the quality affecting parameters passed through VfW API
                    // They only respect the settings from their own configuration dialogs, and Mpeg4VideoEncoder currently has no support for this
                    quality: Quality,
                    codec: Codec,
                    // Most of VfW codecs expect single-threaded use, so we wrap this encoder to special wrapper
                    // Thus all calls to the encoder (including its instantiation) will be invoked on a single thread although encoding (and writing) is performed asynchronously
                    forceSingleThreadedAccess: true);
            }
        }
    }

    public class Recorder : IDisposable
    {
        #region Fields

        RecorderParams Params;
        //IAviVideoStream videoStream;
        Thread screenThread;
        ManualResetEvent stopThread = new ManualResetEvent(false);
        #endregion

        public Recorder(RecorderParams Params)
        {

            this.Params = Params;

            // Create AVI writer and specify FPS

        }

        public void StartAsync()
        {
            screenThread = new Thread(Start)
            {
                Name = typeof(Recorder).Name + ".RecordScreen",
                IsBackground = true
            };
            screenThread.Start();
        }
        public void Start()
        {
            RecordScreen();
        }
        private bool closed = false;
        public void Dispose()
        {
            stopThread.Set();
            while (!closed)
            {
                System.Threading.Thread.Sleep(100);
            }
            screenThread.Join();
            screenThread.Abort();

            // Close writer: the remaining data is written to a file and file is closed
            Params = null;
            //videoStream = null;

            stopThread = null;
            screenThread = null;
        }



        void RecordScreen()
        {
            var buffer = new byte[Params.Width * Params.Height * 4];
            var timeTillNextFrame = TimeSpan.Zero;
            Control.CheckForIllegalCrossThreadCalls = false;

            bool creatingNewFile = false;
            while (!closed)
            {
                creatingNewFile = false;
                var startDate = DateTime.Now;
                using (var writer = Params.CreateAviWriter())
                {
                    var frameInterval = TimeSpan.FromSeconds(1 / (double)writer.FramesPerSecond);
                    var videoStream = Params.CreateVideoStream(writer);
                    videoStream.Name = "Captura";
                    var timedWriter = new TimedFrameWriter(videoStream, writer, Params);
                    while (!stopThread.WaitOne(timeTillNextFrame) && !creatingNewFile)
                    {
                        var timestamp = DateTime.Now;
                        bool captured= Screenshot(buffer);
                        if (captured)
                        {
                            timedWriter.WriteFrame(true, buffer, 0, buffer.Length);
                        }
                        //videoStream.WriteFrame(true, buffer, 0, buffer.Length);
                        timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                        if (timeTillNextFrame < TimeSpan.Zero)
                            timeTillNextFrame = TimeSpan.Zero;
                        if (DateTime.Now.Subtract(startDate).Hours == 1)
                        {
                            creatingNewFile = true;
                        }
                    }
                    writer.Close();
                    if (!creatingNewFile)
                    {
                        closed = true;
                    }
                }
            }
        }

        void RecordScreenOriginal()
        {
            var buffer = new byte[Params.Width * Params.Height * 4];
            var timeTillNextFrame = TimeSpan.Zero;
            Control.CheckForIllegalCrossThreadCalls = false;
            using (var writer = Params.CreateAviWriter())
            {
                var frameInterval = TimeSpan.FromSeconds(1 / (double)writer.FramesPerSecond);
                var videoStream = Params.CreateVideoStream(writer);
                videoStream.Name = "Captura";
                while (!stopThread.WaitOne(timeTillNextFrame))
                {
                    var timestamp = DateTime.Now;
                    Screenshot(buffer);
                    videoStream.WriteFrame(true, buffer, 0, buffer.Length);
                    timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                    if (timeTillNextFrame < TimeSpan.Zero)
                        timeTillNextFrame = TimeSpan.Zero;
                }
                writer.Close();
            }
        }


        void RecordScreenWithAction()
        {

            var buffer = new byte[Params.Width * Params.Height * 4];
            //Task videoWriteTask = null;

            var timeTillNextFrame = TimeSpan.Zero;
            var grabs = new ConcurrentQueue<ScreenGrab>();
            var timedGrabs = new ConcurrentQueue<ScreenGrab>();
            Control.CheckForIllegalCrossThreadCalls = false;
            using (var writer = Params.CreateAviWriter())
            {
                var frameInterval = TimeSpan.FromSeconds(1 / (double)writer.FramesPerSecond);
                var videoStream = Params.CreateVideoStream(writer);
                videoStream.Name = "Captura";
                var current = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyy HH:mm:ss"));
                var LastProcessed = current.AddSeconds(1);
                var millisecondsPerFrame = (1.0 / (double)writer.FramesPerSecond) * 1000;
                var frameTimes = Enumerable.Range(0, (int)writer.FramesPerSecond).Select(i => (int)(i * millisecondsPerFrame)).ToList();
                bool stillWriting = true;
                DateTime doneWritingTime = DateTime.MaxValue;
                Action process = () =>
                {
                    while (stillWriting || grabs.Count > 0)
                    {
                        while (stillWriting && (DateTime.Now < LastProcessed || grabs.Count == 0))
                        {
                            System.Threading.Thread.Sleep(1);
                        }
                        if (!stillWriting)
                            break;
                        var copy = new List<ScreenGrab>();
                        while (grabs.Count > 0 && grabs.TryPeek(out ScreenGrab screenGrab) && screenGrab.Timestamp < LastProcessed)
                        {
                            grabs.TryDequeue(out screenGrab);
                            if (screenGrab.Timestamp >= current && screenGrab.Timestamp < LastProcessed)
                            {
                                copy.Add(screenGrab);
                            }
                        }
                        if (copy.Count > 0)
                        {
                            var frameGrabs = frameTimes.Select(x =>
                            {
                                ScreenGrab result = copy.First();
                                DateTime frameTime = current.AddMilliseconds(x);
                                var minDelta = Math.Abs(result.Timestamp.Subtract(frameTime).TotalMilliseconds);
                                for (var i = 1; i < copy.Count; i++)
                                {
                                    if (copy[i].Timestamp > doneWritingTime)
                                        break;
                                    var delta = Math.Abs(copy[i].Timestamp.Subtract(frameTime).TotalMilliseconds);
                                    if (delta < minDelta)
                                    {
                                        delta = minDelta;
                                        result = copy[i];
                                    }
                                }
                                result.FrameTime = frameTime;
                                return result;
                            }).ToList();
                            frameGrabs.ForEach(x => timedGrabs.Enqueue(x));
                        }

                        current = LastProcessed;
                        LastProcessed = LastProcessed.AddSeconds(1);
                    }
                };

                var t = Task.Run(() => process());
                //int written = 0;
                while (!stopThread.WaitOne(timeTillNextFrame))
                {
                    var timestamp = DateTime.Now;
                    Screenshot(buffer);
                    var grab = new ScreenGrab(timestamp, buffer.ToArray());
                    grabs.Enqueue(grab);

                    while (timedGrabs.Count > 0 && timedGrabs.TryDequeue(out ScreenGrab x))
                    {
                        //Console.WriteLine($"{++written}: {x.FrameTime.ToString("HH:mm:ss:fff")} {x.Timestamp.ToString("HH:mm:ss:fff")}");
                        videoStream.WriteFrame(true, x.Buffer, 0, x.Buffer.Length);
                    }
                    //videoStream.WriteFrame(true, buffer, 0, buffer.Length);
                    timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                    if (timeTillNextFrame < TimeSpan.Zero)
                        timeTillNextFrame = TimeSpan.Zero;
                }
                stillWriting = false;
                doneWritingTime = DateTime.Now;
                t.Wait();
                writer.Close();
                closed = true;
            }

        }

        public class ScreenGrab
        {
            public ScreenGrab(DateTime timestamp, byte[] buffer)
            {
                Timestamp = timestamp;
                Buffer = buffer;
            }

            public DateTime Timestamp { get; }
            public byte[] Buffer { get; }
            public DateTime FrameTime { get; set; }
        }


        public bool Screenshot(byte[] Buffer)
        {
            try
            {
                using (var BMP = new Bitmap(Params.Width, Params.Height))
                {
                    using (var g = Graphics.FromImage(BMP))
                    {
                        //g.CopyFromScreen(Point.Empty, Point.Empty, new Size(Params.Width, Params.Height), CopyPixelOperation.SourceCopy);
                        g.CopyFromScreen(Params.Location, Point.Empty, new Size(Params.Width, Params.Height), CopyPixelOperation.SourceCopy);
                        g.Flush();

                        var bits = BMP.LockBits(new Rectangle(0, 0, Params.Width, Params.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                        Marshal.Copy(bits.Scan0, Buffer, 0, Buffer.Length);
                        BMP.UnlockBits(bits);
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }
    }
}
namespace GlobalMacroRecorder
{
    class ScreenRecorder
    {
        private void takeScreenShot()
        {
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                var ts = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-zzz");

                bmp.Save("screenshot-{ts}.png");  // saves the image
            }
        }
    }
}
