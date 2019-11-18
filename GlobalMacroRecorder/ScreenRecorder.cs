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


namespace Captura
{
    public class DesktopRecorder
    {
        static Recorder rec = null;
        static FourCC defaultCodec = SharpAvi.KnownFourCCs.Codecs.MotionJpeg;
        public static void Start(string fileName, double scale, FourCC? codec = null, int frameRate = 10, int quality = 70)
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            if (rec != null) return;
            if (codec == null)
                codec = defaultCodec;
            // Using MotionJpeg as Avi encoder,
            // output to 'out.avi' at 10 Frames per second, 70% quality
            //var rec = new Recorder(new RecorderParams("out.avi", 10, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, 70));
            var recorderParams = new RecorderParams(fileName, scale, frameRate, codec.Value, quality);
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

            Height = (int)( Screen.PrimaryScreen.Bounds.Height * scale);
            Width = (int) (Screen.PrimaryScreen.Bounds.Width * scale);
        }

        string FileName;
        public int FramesPerSecond, Quality;
        FourCC Codec;

        public int Height { get; private set; }
        public int Width { get; private set; }

        public AviWriter CreateAviWriter()
        {
            return new AviWriter(FileName)
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
        public void Dispose()
        {
            stopThread.Set();
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
            //Task videoWriteTask = null;
      
            var timeTillNextFrame = TimeSpan.Zero;
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

        public void Screenshot(byte[] Buffer)
        {
            using (var BMP = new Bitmap(Params.Width, Params.Height))
            {
                using (var g = Graphics.FromImage(BMP))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, new Size(Params.Width, Params.Height), CopyPixelOperation.SourceCopy);

                    g.Flush();

                    var bits = BMP.LockBits(new Rectangle(0, 0, Params.Width, Params.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    Marshal.Copy(bits.Scan0, Buffer, 0, Buffer.Length);
                    BMP.UnlockBits(bits);
                }
            }
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
