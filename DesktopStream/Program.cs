using SuperSocket.SocketBase;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopStream
{
    /// <summary>
    /// This example captures the current desktop and sends it over a websocket stream.
    /// </summary>
    class Program
    {
        static bool running = false;
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start the WebSocketServer!");

            Console.ReadKey();
            Console.WriteLine();

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
                Console.ReadKey();
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
                bytes = ScreenShotUtility.TakeScreenshot();
                var segment = new ArraySegment<byte>(bytes);
                session.Send(segment);
                var diff = DateTime.Now.Subtract(start).TotalMilliseconds;
                var sleep = Math.Max(0, fps - diff);
                System.Threading.Thread.Sleep((int)sleep);
            }

        }

        static void appServer_NewMessageReceived(WebSocketSession session, string message)
        {
            //Send the received message back
            session.Send("Server: " + message);
        }
    }

    public class ScreenShotUtility
    {
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
        public static byte[] TakeScreenshot(bool CaptureMouse = true)
        {
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (var ms = new MemoryStream())
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                    if (CaptureMouse)
                    {
                        CURSORINFO pci;
                        pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));

                        if (GetCursorInfo(out pci))
                        {
                            if (pci.flags == CURSOR_SHOWING)
                            {
                                DrawIcon(g.GetHdc(), pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor);
                                g.ReleaseHdc();
                            }
                        }
                    }
                    bmp.Save(ms, ImageFormat.Png);
                }
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
}