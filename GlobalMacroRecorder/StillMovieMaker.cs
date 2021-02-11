using Captura;
using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GlobalMacroRecorder
{
    class StillMovieMaker
    {
        static FourCC defaultCodec = SharpAvi.KnownFourCCs.Codecs.MotionJpeg;
        string FileName;
        string ImageDirectory;
        public int FramesPerSecond, Quality;
        FourCC Codec;
        int StartFrame;
        int EndFrame;
        string FileNameFormat;
        public StillMovieMaker(string directory, string fileName, double Scale, int framesPerSecond, int quality, int startFrame, int endFrame, string fileNameFormat, FourCC? codec = null)
        {
            Codec = codec ?? defaultCodec;
            ImageDirectory = directory;
            var di = new DirectoryInfo(directory);
            var first = di.GetFiles("*.png").First();
            Bitmap bmp = (Bitmap)Bitmap.FromFile(first.FullName);
            Width = bmp.Width;
            Height = bmp.Height;
            FileName = Path.Combine(di.FullName, Path.GetFileName(fileName));
            Params = new RecorderParams(FileName, 1, framesPerSecond, defaultCodec, 80, Height, Width);
            StartFrame = startFrame;
            EndFrame = endFrame;
            FileNameFormat = fileNameFormat;
        }

        RecorderParams Params;

        public int Height { get; private set; }
        public int Width { get; private set; }

        public void Run()
        {

            var di = new DirectoryInfo(ImageDirectory);


            var buffer = new byte[Params.Width * Params.Height * 4];
            //Task videoWriteTask = null;
            //1403-1458 - ready: aoc face removal
            //            1459 - 1509
            //1510 - 1529 - reay aoc face removal 2
            //1530 - 1572
            //1573 - 1597 - ready aoc face removal 3
            //1598 - 1633


            var timeTillNextFrame = TimeSpan.Zero;

            ManualResetEvent stopThread = new ManualResetEvent(false);

            using (var writer = Params.CreateAviWriter())
            {
                var frameInterval = TimeSpan.FromSeconds(1 / (double)writer.FramesPerSecond);
                var videoStream = Params.CreateVideoStream(writer);
                videoStream.Name = "Captura";
                var timedWriter = new TimedFrameWriter(videoStream, writer, Params);
                for (var i = StartFrame; /*!stopThread.WaitOne(timeTillNextFrame) &&*/ i >= EndFrame; i--)
                {
                    var timestamp = DateTime.Now;
                    //var fileName = $"facescan{i.ToString().PadLeft(3,'0')}-movie.png";
                    //ouA-2021-02-05_12-19-15001-movie-final.png
                    var fileName = FileNameFormat.Replace("{x}", i.ToString().PadLeft(3, '0'));// $"faceoff -{i.ToString().PadLeft(3, '0')}.png";
                    var path = Path.Combine(di.FullName, fileName);
                    if (File.Exists(path))
                    {
                        //buffer = File.ReadAllBytes(path);
                    }
                    else
                    {
                        fileName = $"aocfacecut-{i.ToString().PadLeft(4, '0')}.png";
                        path = Path.Combine(di.FullName, fileName);
                        //buffer = File.ReadAllBytes(path);
                    }
                    if (!File.Exists(path))
                    {
                        continue;
                        string bp = "Bad file";
                    }
                    GetBuffer(path, buffer);

                    //Screenshot(buffer);
                    videoStream.WriteFrame(true, buffer, 0, buffer.Length);
                    //timedWriter.WriteFrame(true, buffer, 0, buffer.Length);
                    timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                    if (timeTillNextFrame < TimeSpan.Zero)
                        timeTillNextFrame = TimeSpan.Zero;
                }
                timedWriter.Close();
                writer.Close();
            }
        }

        public void GetBuffer(string imageFileName, byte[] Buffer)
        {
            using (var src = (Bitmap)Bitmap.FromFile(imageFileName))
            //using (var BMP = new Bitmap(Params.Width, Params.Height))
            {
                using (var g = Graphics.FromImage(src))
                {

                    //g.CopyFromScreen(Point.Empty, Point.Empty, new Size(Params.Width, Params.Height), CopyPixelOperation.SourceCopy);

                    g.Flush();

                    var bits = src.LockBits(new Rectangle(0, 0, Params.Width, Params.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    Marshal.Copy(bits.Scan0, Buffer, 0, Buffer.Length);
                    src.UnlockBits(bits);
                }
            }
        }
    }
}
