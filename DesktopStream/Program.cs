using SuperSocket.SocketBase;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MouseKeyboardEvents;
using MouseKeyboardLibrary;
using Newtonsoft.Json;
using System.IO.Ports;
using System.Diagnostics;
using System.Timers;
using System.Diagnostics.Contracts;
using System.Collections.Concurrent;

namespace DesktopStream
{
    /// <summary>
    /// This example captures the current desktop and sends it over a websocket stream.
    /// </summary>
    class Program
    {
        static bool running = false;
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        static void Main(string[] args)
        {
            //TestRender();
            if (args.Length != 0)
            {
                Console.WriteLine("Exectuing as interactive");
                WindowsLoginHelper.ExecuteAsInteractive();
                Console.ReadKey();
                return;

            }
            Console.WriteLine($"IsInteractive: {Environment.UserInteractive}");
            Console.WriteLine($"IPAddress: {GetLocalIPAddress()}");

            var scale = DesktopUtility.Scale;
            // Console.WriteLine("Press any key to start the WebSocketServer!");

            // Console.ReadKey();
            // Console.WriteLine();

            var appServer = new WebSocketServer();

            //Setup the appServer
            if (!appServer.Setup(2012)) //Setup with listening port
            {
                Console.WriteLine("Failed to setup!");
                Console.ReadKey();
                return;
            }

            appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(appServer_NewMessageReceived);
            appServer.NewSessionConnected += AppServer_NewSessionConnected;
            Console.WriteLine();

            //Try to start the appServer
            if (!appServer.Start())
            {
                Console.WriteLine("Failed to start!");
                // Console.ReadKey();
                return;
            }
            running = true;
            var ep = appServer.Listeners.First().EndPoint;
            Console.WriteLine($"The server started successfully {ep.Address}:{ep.Port}, press key 'q' to stop it!");

            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                continue;
            }

            //Stop the appServer

            running = false;
            Console.WriteLine();
            Console.WriteLine("The server was stopped!");
            Console.ReadKey();
        }

        private static void TestRender()
        {
            int ticks = 0;
            int frames = 0;
            long renderedBytes = 0;
            var t = new System.Timers.Timer();
            var fps = 1000 / 14.0;
            t.Interval = fps;
            t.Elapsed += (sender, e) =>
            {
                ticks++;
                var bytes = ScreenShotUtility.TakeScreenshot();
                frames++;
                var segment = new ArraySegment<byte>(bytes);
                renderedBytes += (long)segment.Count;
            };
            var sw = Stopwatch.StartNew();
            t.Start();
            while (bool.Parse(bool.TrueString))
            {
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine($"[{DateTime.Now}] Ticks: {ticks} Frames: {frames} bytes: {renderedBytes} Elapsed: {sw.Elapsed}");
            }
        }

        private static void AppServer_NewSessionConnected(WebSocketSession session)
        {


            byte[] bytes = null;
            var fps = 1000 / 24;
            var bounds = Screen.PrimaryScreen.Bounds;
            var boundsJson = JsonConvert.SerializeObject(new { bounds.Width, bounds.Height });
            session.Send(boundsJson);
            var latest = DateTime.MinValue;

            int ticks = 0;
            int frames = 0;
            int sent = 0;
            int failed = 0;
            //long renderedBytes = 0;
            //long sentBytes = 0, failed = 0;
            var sw = Stopwatch.StartNew();
            var logTimer = new System.Timers.Timer();
            logTimer.Interval = 1000;
            logTimer.Elapsed += (sender, e) =>
             {
                 Console.WriteLine($"[{DateTime.Now}] Ticks: {ticks} Frames: {frames}  Sent: {sent} Failed: {failed} Elapsed: {sw.Elapsed}");
             };

            var tickTimer = new System.Timers.Timer();
            tickTimer.Interval = fps;
            tickTimer.Elapsed += (sender, e) => ticks++;


            var queue = new ConcurrentQueue<byte[]>();
            var renderTimer = new System.Timers.Timer();
            renderTimer.Interval = fps;
            renderTimer.Elapsed += (sender, e) =>
            {
                if (running && session.Connected)
                {
                    queue.Enqueue(ScreenShotUtility.TakeScreenshot());
                    frames++;
                }
            };
            renderTimer.Start();

            bool serving = false;
            var serveTimer = new System.Timers.Timer();
            serveTimer.Interval = fps;
            serveTimer.Elapsed += (sender, e) =>
            {
                if (serving) return;
                serving = true;
                byte[] result = null;
                var skipped = -1;
                while (queue.Count > 0)
                {
                    queue.TryDequeue(out result);
                    skipped++;
                }
                failed += skipped;
                try
                {
                    session.Send(result, 0, result.Length);
                    sent++;
                }
                catch
                {
                    failed++;
                }
                serving = false;
            };


            //var renderTimer = new System.Timers.Timer();
            //renderTimer.Interval = fps;
            //renderTimer.Elapsed += (sender, e) =>
            //{

            //    if (running && session.Connected)
            //    {
            //        var tickBytes = ScreenShotUtility.TakeScreenshot();
            //        frames++;
            //        var segment = new ArraySegment<byte>(tickBytes);
            //        renderedBytes += segment.Count;
            //        try
            //        {
            //            session.Send(segment);
            //            sentBytes += segment.Count;
            //        }
            //        catch
            //        {
            //            failed += segment.Count;
            //        }


            //    }
            //};
            renderTimer.Start();
            serveTimer.Start();
            logTimer.Start();
            tickTimer.Start();
            //renderTimer.Start();
            while (running && session.Connected)
            {
                System.Threading.Thread.Sleep(1);
            }
            //while (running && session.Connected)
            //{
            //    DateTime start = DateTime.Now;
            //    frames++;
            //    if (frames <= ticks)
            //    {
            //        failed += bytes.Length;
            //    }
            //    else
            //    {

            //        bytes = ScreenShotUtility.TakeScreenshot();

            //        var segment = new ArraySegment<byte>(bytes);
            //        renderedBytes += segment.Count;
            //        try
            //        {
            //            session.Send(segment);
            //            sentBytes += segment.Count;
            //        }
            //        catch
            //        {
            //            failed += segment.Count;
            //        }
            //    }
            //    var diff = DateTime.Now.Subtract(start).TotalMilliseconds;
            //    var sleep = Math.Max(0, fps - diff);
            //    System.Threading.Thread.Sleep((int)sleep);

            //}





            while (running && session.Connected)
            {
                System.Threading.Thread.SpinWait((int)100);
            }
            serveTimer.Stop();
            serveTimer = null;
            renderTimer.Stop();
            renderTimer = null;
            tickTimer.Stop();
            tickTimer = null;
            logTimer.Stop();
            logTimer = null;

        }

        static void appServer_NewMessageReceived(WebSocketSession session, string message)
        {
            //Send the received message back
            var browserEvent = BrowserEventFactory.Parse(message);
            processEvent(browserEvent);

#if DEBUG
            string result = "";
            switch (browserEvent.MacroEventType)
            {
                case MouseKeyEventType.MouseMove:
                    {
                        var mouseArgs = (MouseEventArgs)browserEvent.MouseArgs;
                        //Console.WriteLine(result = $"MouseMove: {mouseArgs.X} {mouseArgs.Y}");
                    }
                    break;
                case MouseKeyEventType.MouseDown:
                    {
                        var mouseArgs = (MouseEventArgs)browserEvent.MouseArgs;
                        Console.WriteLine(result = $"MouseDown({mouseArgs.Button}): {mouseArgs.X} {mouseArgs.Y}");
                    }
                    break;
                case MouseKeyEventType.MouseUp:
                    {
                        var mouseArgs = (MouseEventArgs)browserEvent.MouseArgs;
                        Console.WriteLine(result = $"MouseUp({mouseArgs.Button}): {mouseArgs.X} {mouseArgs.Y}");
                    }
                    break;
                case MouseKeyEventType.MouseWheel:
                    {
                        var mouseArgs = (MouseEventArgs)browserEvent.MouseArgs;
                        Console.WriteLine(result = $"MouseWheel: {mouseArgs.Delta}");
                    }
                    break;
                case MouseKeyEventType.KeyDown:
                    {
                        var keyArgs = (KeyEventArgs)browserEvent.KeyArgs;
                        Console.WriteLine(result = $"KeyDown: {keyArgs.KeyCode}");
                    }
                    break;
                case MouseKeyEventType.KeyUp:
                    {
                        var keyArgs = (KeyEventArgs)browserEvent.KeyArgs;
                        Console.WriteLine(result = $"KeyUp: {keyArgs.KeyCode}");
                    }
                    break;
            }
            //var json = JsonConvert.SerializeObject(browserEvent, Formatting.Indented);
            //Console.WriteLine(json);
            if (!string.IsNullOrEmpty(result))
                session.Send("Server: " + result);
#endif
        }


        //todo:
        static double ScaleX = 1;
        static double ScaleY = 1;
        static void processEvent(MouseKeyEvent browserEvent)
        {
            switch (browserEvent.MacroEventType)
            {
                case MouseKeyEventType.MouseMove:
                    {
                        var mouseArgs = (MouseEventArgs)browserEvent.MouseArgs;
                        MouseSimulator.X = (int)(mouseArgs.X / ScaleX);
                        MouseSimulator.Y = (int)(mouseArgs.Y / ScaleY);
                    }
                    break;
                case MouseKeyEventType.MouseDown:
                    {
                        var mouseArgs = (MouseEventArgs)browserEvent.MouseArgs;
                        MouseSimulator.MouseDown(mouseArgs.Button);
                    }
                    break;
                case MouseKeyEventType.MouseUp:
                    {
                        var mouseArgs = (MouseEventArgs)browserEvent.MouseArgs;
                        MouseSimulator.MouseUp(mouseArgs.Button);
                    }
                    break;
                case MouseKeyEventType.MouseWheel:
                    {
                        var mouseArgs = (MouseEventArgs)browserEvent.MouseArgs;
                        MouseSimulator.MouseWheel(mouseArgs.Delta);
                    }
                    break;
                case MouseKeyEventType.KeyDown:
                    {
                        var keyArgs = (KeyEventArgs)browserEvent.KeyArgs;

                        KeyboardSimulator.KeyDown(keyArgs.KeyCode);
                    }
                    break;
                case MouseKeyEventType.KeyUp:
                    {
                        var keyArgs = (KeyEventArgs)browserEvent.KeyArgs;
                        KeyboardSimulator.KeyUp(keyArgs.KeyCode);
                    }
                    break;
            }
        }
    }


    public class ScreenShotUtility
    {

        public float DesktopScale;
        public int OriginalWidth;
        public int OriginalHeight;
        public int ScaledWidth;
        public int ScaledHeight;
        public byte[] CaptureBuffer;
        public byte[] ScaledBuffer;
        public ScreenShotUtility()
        {
            this.DesktopScale = DesktopUtility.Scale;
            this.OriginalWidth = Screen.PrimaryScreen.Bounds.Width;
            this.OriginalHeight = Screen.PrimaryScreen.Bounds.Height;
            this.ScaledWidth = (int)(OriginalWidth * DesktopScale);
            this.ScaledHeight = (int)(OriginalHeight * DesktopScale);
            this.CaptureBuffer = new byte[ScaledWidth * ScaledHeight * 4];
            this.ScaledBuffer = new byte[OriginalWidth * OriginalHeight * 4];
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        const Int32 CURSOR_SHOWING = 0x00000001;

        static float Scale = DesktopUtility.Scale;
        public static List<FileInfo> FindFiles(DirectoryInfo parent, string pattern)
        {
            var result = new List<FileInfo>();
            try
            {
                result.AddRange(parent.GetFiles(pattern));
            }
            catch (Exception ex)
            {

            }
            try
            {
                foreach (var child in parent.GetDirectories())
                {
                    try
                    {
                        result.AddRange(FindFiles(child, pattern));
                    }
                    catch { }
                }
            }
            catch { }
            return result;
        }
        public static IntPtr deviceHdc;
        // may need to start process interactively using winapi: https://blog.cjwdev.co.uk/2011/06/10/vb-net-start-process-in-console-session-from-windows-service-on-windows-7/
        public static byte[] TakeScreenshot(bool CaptureMouse = true)
        {
            int scaledWidth = (int)(Screen.PrimaryScreen.Bounds.Width * Scale);
            var scaledHeight = (int)(Screen.PrimaryScreen.Bounds.Height * Scale);

            Bitmap bmp = new Bitmap(scaledWidth, scaledHeight);
            int destinationX = 0;
            int destinationY = 0;
            int sourceX = 0;
            int sourceY = 0;
            int destWidth = scaledWidth;
            int destHeight = scaledHeight;

            using (var ms = new MemoryStream())
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, new Size(scaledWidth, scaledHeight), CopyPixelOperation.SourceCopy);
                    var dc = IntPtr.Zero;
                    if (Scale != 1)
                    {
                        bmp = new Bitmap(bmp, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                    }

                }

                if (CaptureMouse)
                {
                    CURSORINFO pci;
                    pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));

                    if (GetCursorInfo(out pci))
                    {
                        //if (pci.flags == CURSOR_SHOWING)
                        {
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                DrawIcon(g.GetHdc(), (int)(pci.ptScreenPos.x), (int)(pci.ptScreenPos.y), pci.hCursor);
                                g.ReleaseHdc();
                            }
                        }
                    }
                }

                bmp.Save(ms, ImageFormat.Jpeg);
                bmp.Dispose();
                return ms.ToArray();
            }

        }


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

    internal class SafeNativeMethods
    {
        //[DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth,
                int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern bool DeleteObject(IntPtr hObject);

        public enum TernaryRasterOperations : uint
        {
            SRCCOPY = 0x00CC0020,
            SRCPAINT = 0x00EE0086,
            SRCAND = 0x008800C6,
            SRCINVERT = 0x00660046,
            SRCERASE = 0x00440328,
            NOTSRCCOPY = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            MERGECOPY = 0x00C000CA,
            MERGEPAINT = 0x00BB0226,
            PATCOPY = 0x00F00021,
            PATPAINT = 0x00FB0A09,
            PATINVERT = 0x005A0049,
            DSTINVERT = 0x00550009,
            BLACKNESS = 0x00000042,
            WHITENESS = 0x00FF0062,
            CAPTUREBLT = 0x40000000 //only if WinVer >= 5.0.0 (see wingdi.h)
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    IntPtr pTarget = e.Graphics.GetHdc();
        //    IntPtr pSource = CreateCompatibleDC(pTarget);
        //    IntPtr pOrig = SelectObject(pSource, bmp.GetHbitmap());
        //    BitBlt(pTarget, 0, 0, bmp.Width, bmp.Height, pSource, 0, 0, TernaryRasterOperations.SRCCOPY);
        //    DeleteDC(pSource);
        //    e.Graphics.ReleaseHdc(pTarget);
        //}
    }

    public class DesktopUtility
    {
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }
        static DesktopUtility()
        {

        }
        public static float Scale => getScalingFactor();
        private static float getScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            return ScreenScalingFactor; // 1.25 = 125%
        }

    }
}