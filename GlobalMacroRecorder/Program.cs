using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GlobalMacroRecorder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (bool.Parse(bool.FalseString))
            {
                string filePath = @"C:\Users\alexh\Source\Repos\alexhiggins732\CsharpMouseKeyboardDesktopLibrary\GlobalMacroRecorder\bin\Debug\ouP-2021-02-05_11-14-52.avi";
                extractFromMovie(filePath);
            }
            if (bool.Parse(bool.FalseString))
            {
                DrawMarker();
            }
            //extractAnimation();
            if (bool.Parse(bool.FalseString))
            {
                //var maker = new StillMovieMaker(@"C:\Users\alexh\Source\Repos\alexhiggins732\CsharpMouseKeyboardDesktopLibrary\GlobalMacroRecorder\bin\Debug\ouA-2021-02-05_12-19-15", "out-facemarker.avi", 1, 25, 80);
                //C:\Users\alexh\Downloads\ffmpeg\facecut
                var path = @"C:\Users\alexh\Downloads\ffmpeg\Face-on";
                int startFrame = 2801;
                int endFrame = 2734;
                string fileFormat = "faceon-{x}.png";
                var maker = new StillMovieMaker(path, "Face-on.avi", 1, 18, 80, startFrame, endFrame, fileFormat);

                maker.Run();
            }
            //return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MacroForm());


        }

        static int ColorDiff(Color c1, Color c2)
        {
            return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R)
                                   + (c1.G - c2.G) * (c1.G - c2.G)
                                   + (c1.B - c2.B) * (c1.B - c2.B));
        }
        private static void DrawMarker()
        {
            var di = new DirectoryInfo(@"C:\Users\alexh\Source\Repos\alexhiggins732\CsharpMouseKeyboardDesktopLibrary\GlobalMacroRecorder\bin\Debug\ouA-2021-02-05_06-49-38");

            var target1 = Color.FromArgb(255, 54, 11, 164);
            var target2 = Color.FromArgb(255, 58, 16, 169);
            var target3 = Color.FromArgb(255, 65, 22, 175);
            var target4 = Color.FromArgb(255, 73, 30, 183);
            var target5 = Color.FromArgb(255, 62, 22, 167);
            var targets = new[] { target1, target2, target3, target4, target5 };


            var pics = di.GetFiles("*-movie.png");
            List<Point> points = (new Point[]
                        {
                            new Point (252, 365),
                            new Point (305, 393),
                            new Point (390, 420),
                            new Point (486, 452),
                        }).ToList();
            foreach (var pic in pics)
            {


                var bmp = (Bitmap)Bitmap.FromFile(pic.FullName);
                var minDiff = int.MaxValue;
                var minPoint = new Point(0, 0);
                bool done = false;
                var l = points.Last();
                for (var x = 0; !done && x < bmp.Height; x++)
                {
                    for (var y = 0; !done && y < bmp.Height; y++)
                    {
                        var last = points.Last();
                        var endpoint = new Point(x, y);
                        var xroot = last.X - endpoint.X;
                        var yroot = last.Y - endpoint.Y;
                        var distance = Math.Sqrt(xroot * xroot + yroot * yroot);
                        if (distance > 20)
                            continue;
                        var c = bmp.GetPixel(x, y);
                        //var t = target.GetHue();
                        //var h = (int)c.GetHue();
                        //if (h >= 289 && h <= 291)
                        //{
                        //    string bp = "";
                        //}

                        var diff = targets.Min(cx => ColorDiff(c, cx));
                        if (diff < 10 || targets.Contains(c))
                        {
                            string bp = "";


                            if (!points.Contains(endpoint))
                                points.Add(endpoint);

                            var penColor1 = Color.FromArgb(120, 98, 67, 146);
                            var b = new Pen(penColor1, 8);
                            var p = new System.Drawing.Drawing2D.GraphicsPath();
                            var pts = points.ToArray();
                            p.AddCurve(pts);
                            var penColor2 = Color.FromArgb(150, 98, 67, 146);
                            var pen2 = new Pen(penColor2, 3);
                            using (var g = Graphics.FromImage(bmp))
                            {
                                g.DrawPath(b, p);
                                g.DrawPath(pen2, p);
                            }
                            //bmp.Save(@"C:\Users\alexh\OneDrive\Pictures\AOC Face Scan\aoc-face-line-scene-1-test.png");
                            var dest = pic.FullName.Replace("-movie.png", "-movie-final.png");
                            bmp.Save(dest);
                            //points.Remove(endpoint);
                            done = true;
                        }
                        else
                        {
                            //var diff = Math.Abs(c.ToArgb() - argb);
                            if (diff < minDiff)
                            {
                                minPoint = new Point(x, y);
                                minDiff = diff;
                            }
                        }
                    }
                }
                if (!done)
                {
                    string bp = "not found";
                    points.Add(minPoint);

                    var penColor1 = Color.FromArgb(120, 98, 67, 146);
                    var b = new Pen(penColor1, 8);
                    var p = new System.Drawing.Drawing2D.GraphicsPath();
                    var pts = points.ToArray();
                    p.AddCurve(pts);
                    var penColor2 = Color.FromArgb(150, 98, 67, 146);
                    var pen2 = new Pen(penColor2, 3);
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.DrawPath(b, p);
                        g.DrawPath(pen2, p);
                    }
                    //bmp.Save(@"C:\Users\alexh\OneDrive\Pictures\AOC Face Scan\aoc-face-line-scene-1-test.png");
                    var dest = pic.FullName.Replace("-movie.png", "-movie-final.png");
                    bmp.Save(dest);
                    done = true;
                    //points.Remove(minPoint);

                }
                bmp.Dispose();
            }
        }

        private static void extractFromMovie(string filePath)
        {
            //var src = @"C:\Users\alexh\Source\Repos\alexhiggins732\CsharpMouseKeyboardDesktopLibrary\GlobalMacroRecorder\bin\Debug\ouP-2021-02-05_08-44-40.avi";
            var src = filePath;
            var cmd = $"ffmpeg -i ../faceoff/faceoff.mp4 -r 24 ../faceoff/faceoff%03d.png";
            //var p = new Process();
            var info = new ProcessStartInfo(@"C:\Users\alexh\Downloads\ffmpeg\bin\ffmpeg.exe");
            var fileName = Path.GetFileNameWithoutExtension(src);
            var tempDir = Path.GetFullPath(fileName);
            var di = new DirectoryInfo(tempDir);
            di.Create();
            info.Arguments = $"-i \"{src}\" -r 24 \"{tempDir}\\{fileName}%03d.png\"";
            var p = Process.Start(info);
            p.WaitForExit();

            var files = di.GetFiles("*.png");
            foreach (var file in files)
            {
                var cropRect = new Rectangle(149, 143, 1280, 720);
                var cropped = GetCroppedBitmap(file.FullName, cropRect);
                var dest = file.FullName.Replace(".png", "-movie.png");
                cropped.Save(dest);
            }
            var un = new DirectoryInfo(@"C:\Users\alexh\Source\Repos\alexhiggins732\CsharpMouseKeyboardDesktopLibrary\GlobalMacroRecorder\bin\Debug\ouP-2021-02-05_11-14-52\Unique");
            un.Create();
            var files2 = di.GetFiles("*-movie.png").ToList();


            var Buffer = new byte[1280 * 720 * 4];
            var last= new byte[1280 * 720 * 4];
            foreach (var file in files2)
            {
                var current = (Bitmap)Bitmap.FromFile(file.FullName);
                using (var g = Graphics.FromImage(current))
                {
                    var bits = current.LockBits(new Rectangle(0, 0, 1280, 720), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    System.Runtime.InteropServices.Marshal.Copy(bits.Scan0, Buffer, 0, Buffer.Length);
                    current.UnlockBits(bits);
                }
                current.Dispose();
                if (!last.SequenceEqual(Buffer))
                {
                    var dest = Path.Combine(un.FullName, file.Name);
                    file.CopyTo(dest);
                    Array.Copy(Buffer, last, Buffer.Length);
                }
            }
        }

        private static Bitmap GetCroppedBitmap(string fileName, Rectangle cropRect)
        {
            var src = (Bitmap)Bitmap.FromFile(fileName);
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);
            //var result=  target.Clone(cropRect, bmpImage.PixelFormat);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);

            }
            return target;
        }

        static void extractAnimation()
        {
            var di = new DirectoryInfo(@"C:\Users\alexh\Downloads\ffmpeg\facescan");
            var files = di.GetFiles("*.png");

            foreach (var file in files)
            {
                var src = (Bitmap)Bitmap.FromFile(file.FullName);
                Rectangle cropRect = new Rectangle(0, 71, 780, 917);

                Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);
                //var result=  target.Clone(cropRect, bmpImage.PixelFormat);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                     cropRect,
                                     GraphicsUnit.Pixel);

                }
                var mult = 546.0 / 917;
                var rs = new Bitmap(target, (int)(mult * 780), 546);
                var halfRsWidth = rs.Width / 2;
                var halfRsHeight = rs.Height / 2;
                var bg = Bitmap.FromFile(@"C:\Users\alexh\OneDrive\Pictures\AOC Face Scan\grayscale-bg.png");
                var halfWidth = bg.Width / 2;
                var halfHeight = bg.Height / 2;
                using (Graphics g = Graphics.FromImage(bg))
                {


                    g.DrawImage(rs, halfWidth - halfRsWidth, halfHeight - halfRsHeight);

                }
                bg.Save(file.FullName.Replace(".png", "-movie.png"));
                //rs.Save(file.FullName.Replace(".png", "-cropped-sm.png"));

                //target.Save(file.FullName.Replace(".png", "-cropped.png"));
                target.Dispose();
                //1280 x 720

            }
        }
    }
}
