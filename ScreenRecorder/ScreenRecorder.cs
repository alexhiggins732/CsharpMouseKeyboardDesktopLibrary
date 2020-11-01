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

namespace ScreenRecorder
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

            Height = (int)(Screen.PrimaryScreen.Bounds.Height * scale);
            Width = (int)(Screen.PrimaryScreen.Bounds.Width * scale);
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

    public class ScalingUtil
    {
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
        }
        public static float GetScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            return ScreenScalingFactor; // 1.25 = 125%
        }
    }
    public class ScreenUtil
    {
        byte[] buffer;
        static FourCC defaultCodec = SharpAvi.KnownFourCCs.Codecs.MotionJpeg;

        public int Height { get; private set; }
        public int Width { get; private set; }

        public double Scale { get; private set; }
        public Rectangle Bounds;
        public ScreenUtil()
        {

            Scale = ScalingUtil.GetScalingFactor();
            Height = (int)(Screen.PrimaryScreen.Bounds.Height * Scale);
            Width = (int)(Screen.PrimaryScreen.Bounds.Width * Scale);
            Bounds = new Rectangle(0, 0, Width, Height);
            buffer = new byte[Height * Width * 4];
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private Bitmap lastDesktop;
        private Bitmap lastWindowCapture;
        public Bitmap DesktopCapture => (Bitmap)lastDesktop.Clone();

        public Bitmap WindowCapture => (Bitmap)lastWindowCapture.Clone();
        public byte[] TakeScreenShot()
        {

            using (var bmp = new Bitmap(Width, Height))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, Bounds.Size, CopyPixelOperation.SourceCopy);
                var bits = bmp.LockBits(Bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                Marshal.Copy(bits.Scan0, buffer, 0, buffer.Length);
                bmp.UnlockBits(bits);
                lastDesktop?.Dispose();
                lastDesktop = (Bitmap)bmp.Clone();
            }
            return buffer;
        }

        public string SaveScreenShot(string fileSuffix)
        {
            //TakeScreenShot();
            var fileName = $"screenshots\\{fileSuffix}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.png";
            Directory.CreateDirectory("screenshots");
            //File.WriteAllBytes(fileName, buffer);
            using (var bmp = new Bitmap(Width, Height))
            using (Graphics g = Graphics.FromImage(bmp))
            {

                g.CopyFromScreen(0, 0, 0, 0, Bounds.Size, CopyPixelOperation.SourceCopy);

                using (var bmp2 = new Bitmap(bmp, bmp.Width >> 1, bmp.Height >> 1))
                {
                    bmp2.Save(fileName, ImageFormat.Png);
                    lastDesktop?.Dispose();
                    lastDesktop = (Bitmap)bmp2.Clone();
                }

                //var bits = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                //Marshal.Copy(bits.Scan0, buffer, 0, buffer.Length);
                //bmp.UnlockBits(bits);

                //File.WriteAllBytes(fileName, buffer);
            }
            return fileName;
        }
        public string SaveWindowScreenShot(string fileSuffix, bool save = true)
        {

            //TakeScreenShot();
            var fileName = $"screenshots\\{fileSuffix}_window_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.png";
            Directory.CreateDirectory("screenshots");
            using (var bmp = ScreenRecorder.WindowCapture.CaptureActiveWindow())
            {
                if (bmp == null) return string.Empty;
                using (var bmp2 = new Bitmap(bmp, bmp.Width >> 1, bmp.Height >> 1))
                {
                    if (save) bmp2.Save(fileName, ImageFormat.Png);
                    lastWindowCapture?.Dispose();
                    lastWindowCapture = (Bitmap)bmp2.Clone();
                }

            }
            return fileName;

        }
    }

    public class WindowCapture
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        public static Image CaptureDesktop()
        {
            return CaptureWindow(GetDesktopWindow());
        }

        public static Bitmap CaptureActiveWindow()
        {
            return CaptureWindow(GetForegroundWindow());
        }

        private static float? scale;
        public static float Scale => (scale ?? (scale = ScalingUtil.GetScalingFactor())).Value;
        public static Bitmap CaptureWindow(IntPtr handle)
        {
            var rect = new Rect();
            GetWindowRect(handle, ref rect);
            var scale = ScalingUtil.GetScalingFactor();

            //account for invisible border

            //rect.Top += 0;
            //rect.Right += 7;
            //rect.Bottom += 7;
            ////border should be `7, 0, 7, 7`
            var left = (rect.Left + 6) * scale;
            var right = (rect.Right - 6) * scale;
            var top = rect.Top * scale;
            var bottom = (rect.Bottom - 6) * scale;
            var width = (right - left);
            var height = (bottom - top);
            if (width <= 0 || height <= 0) return null;
            var bounds = new Rectangle((int)left, (int)top, (int)width, (int)height);
            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var g = Graphics.FromImage(result))
            {
                g.CopyFromScreen((int)left, (int)top, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
                //graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size, CopyPixelOperation.);
            }

            return result;
        }
    }
}
