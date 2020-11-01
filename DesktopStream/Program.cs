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
        static dynamic GetContext()
        {
            var bmp = new Bitmap(5, 5);
            var g = Graphics.FromImage(bmp);
            var frm = new Form();
            frm.Show();
            frm.DrawToBitmap(bmp, new Rectangle(0, 0, 5, 5));
            var Assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("Drawing")).ToList();
            var type = Assemblies.SelectMany(x => x.DefinedTypes.Where(t => t.FullName == "System.Drawing.Internal.DeviceContext")).First();

            var types = Assemblies.SelectMany(x => x.DefinedTypes.Where(t => t.FullName == "System.Drawing.Internal.DeviceContexts")).First();
            var contextsField = types.GetField("activeDeviceContexts", BindingFlags.Static | BindingFlags.NonPublic);
            var contexts = contextsField.GetValue(null);


            var hwnd = frm.Handle;
            var m = type.GetMethod("FromHwnd");
            var mCurrentBmp = type.GetField("hCurrentBmp", BindingFlags.Instance | BindingFlags.NonPublic);
            var dc = type.GetField("hDC", BindingFlags.Instance | BindingFlags.NonPublic);
            dynamic result = m.Invoke(null, new object[] { hwnd });
            var bmpHwnd = mCurrentBmp.GetValue(result);
            var hDC = dc.GetValue(result);
            frm.Close();
            ScreenShotUtility.deviceHdc = hDC;
            return result;
        }
        static void Main(string[] args)
        {
            var ctx = GetContext();
            var p = System.Diagnostics.Process.GetCurrentProcess();
            var handle = p.Handle;
            PrivilegeHelper.EnablePrivilege((long)handle, Privileges.SeTcbPrivilege, true);


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
            Console.WriteLine("The server started successfully, press key 'q' to stop it!");

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

        private static void AppServer_NewSessionConnected(WebSocketSession session)
        {

            byte[] bytes = null;
            var fps = 1000 / 12.0;
            while (running && session.Connected)
            {

                DateTime start = DateTime.Now;
                fps = 1000.0 / 8;
                //bytes = util.TakeScreenShot(); // ScreenShotUtility.TakeScreenshot();

                try
                {
                    bytes = ScreenShotUtility.TakeScreenshot();
                    var segment = new ArraySegment<byte>(bytes);
                    session.Send(segment);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    session.Send(ex.Message);
                }

                var diff = DateTime.Now.Subtract(start).TotalMilliseconds;
                var sleep = Math.Max(0, fps - diff);
                System.Threading.Thread.Sleep((int)sleep);
                session.Close();
            }

        }

        static void appServer_NewMessageReceived(WebSocketSession session, string message)
        {
            //Send the received message back
            var browserEvent = BrowserEventFactory.Parse(message);
            session.Send("Server: " + message);
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

        public Byte[] TakeScreenShot(bool captureMouse = true)
        {
            using (var BMP = new Bitmap(ScaledWidth, ScaledHeight))
            {
                using (var g = Graphics.FromImage(BMP))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, new Size(ScaledWidth, ScaledHeight), CopyPixelOperation.SourceCopy);

                    g.Flush();

                    if (Scale == 1)
                    {
                        if (captureMouse)
                        {
                            CURSORINFO pci;
                            pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));

                            if (GetCursorInfo(out pci))
                            {
                                if (pci.flags == CURSOR_SHOWING)
                                {
                                    DrawIcon(g.GetHdc(), (int)(pci.ptScreenPos.x), (int)(pci.ptScreenPos.y), pci.hCursor);
                                    g.ReleaseHdc();
                                }
                            }
                        }
                        var bits = BMP.LockBits(new Rectangle(0, 0, ScaledWidth, ScaledWidth), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                        Marshal.Copy(bits.Scan0, CaptureBuffer, 0, CaptureBuffer.Length);
                        BMP.UnlockBits(bits);
                        return CaptureBuffer;
                    }
                    else
                    {
                        using (var bmp = new Bitmap(BMP, OriginalWidth, OriginalHeight))
                        {
                            using (var g2 = Graphics.FromImage(bmp))
                            {
                                if (captureMouse)
                                {
                                    CURSORINFO pci;
                                    pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));

                                    if (GetCursorInfo(out pci))
                                    {
                                        if (pci.flags == CURSOR_SHOWING)
                                        {
                                            DrawIcon(g2.GetHdc(), (int)(pci.ptScreenPos.x), (int)(pci.ptScreenPos.y), pci.hCursor);
                                            g2.ReleaseHdc();
                                        }
                                    }
                                }
                                var bits = bmp.LockBits(new Rectangle(0, 0, OriginalWidth, OriginalHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                                Marshal.Copy(bits.Scan0, ScaledBuffer, 0, ScaledBuffer.Length);
                                bmp.UnlockBits(bits);
                                return ScaledBuffer;
                            }
                        }
                    }
                }

            }
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
                var frm = new Form();
                var screen = Screen.FromControl(frm);
                frm.Show();
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    //g.CopyFromScreen(Point.Empty, Point.Empty, new Size(scaledWidth, scaledHeight), CopyPixelOperation.SourceCopy);
                    var dc = IntPtr.Zero;
                    //System.Drawing.Internal.DeviceContext;
                    // DeviceContext deviceContext = DeviceContext.FromHdc(IntPtr.Zero);
                    try
                    {
                        //HandleRef screenDC = new HandleRef(null, deviceHdc);
                        //HandleRef targetDC = new HandleRef(null, g.GetHdc());
                        try
                        {
                            //int result = SafeNativeMethods.BitBlt(targetDC, destinationX, destinationY, destWidth, destHeight,
                            //                                      screenDC, sourceX, sourceY, unchecked((int)CopyPixelOperation.SourceCopy));
                            bool result = SafeNativeMethods.BitBlt(g.GetHdc(), destinationX, destinationY, destWidth, destHeight,
                                                                  deviceHdc, sourceX, sourceY,
                                                                  (SafeNativeMethods.TernaryRasterOperations)(int)CopyPixelOperation.SourceCopy);
                            var error = Marshal.GetLastWin32Error();
                            //a zero result indicates a win32 exception has been thrown
                            if (result == false)
                            {
                                throw new Win32Exception(error);
                               
                            }
                        }
                        catch(Exception ex )
                        {
                            Console.WriteLine($"WinApiOperationfailed: {ex.Message}");
                        }
                        finally
                        {
                            g.ReleaseHdc();
                        }
                        // g.CopyFromScreen(Point.Empty, Point.Empty, new Size(scaledWidth, scaledHeight), CopyPixelOperation.SourceCopy);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception copying from screen {ex}");
                        throw;
                    }
                    finally
                    {
                       // g.ReleaseHdc();
                    }
                    if (Scale != 1)
                    {
                        bmp = new Bitmap(bmp, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                    }

                }
                frm.Close();
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

                bmp.Save(ms, ImageFormat.Png);
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