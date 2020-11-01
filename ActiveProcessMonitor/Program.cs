using GlobalMacroRecorder;
using MouseKeyboardEvents;
using ScreenRecorder;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ActiveProcessMonitor
{
    class Program
    {
        static double diffThreshold = .005;
        static void Main(string[] args)
        {
            var evt = new CompactMouseEvent(7 << 28, MouseKeyEventType.MouseWheel, 0);
            var macroEvent = new MouseKeyEvent(evt.MacroEventType, evt, 0);
            var delta = evt.MouseEvent.Delta;
            var network = new MouseKeyNetwork();
            var networkUpdater = new MouseKeyNetworkInitializer(network);
            networkUpdater.Update((ulong)evt);

            var monitor = new Monitor();
            var recorder = new ScreenUtil();
            string active = string.Empty;
            int activeId = 0;
            string title = string.Empty;
            int lastCaptureTime = Environment.TickCount;
            int diffDelta = 10000;
            while (true)
            {
                diffDelta = 60000;
                var current = monitor.GetActiveProcess();
                if (current != active || activeId != monitor.CurrentProcess.Id || monitor.CurrentProcess.MainWindowTitle != title)
                {
                    Console.WriteLine($"[{DateTime.Now}] {active = current} (Id={activeId = monitor.CurrentProcess.Id}) ({title = monitor.CurrentProcess.MainWindowTitle })");
                    //var bytes= recorder.TakeScreenShot();
                    var fileName = recorder.SaveScreenShot(active);
                    var windowFileName = recorder.SaveWindowScreenShot(active);
                    lastCaptureTime = Environment.TickCount;
                    //TODO: 
                    // 1) Take desktop screen shot. 
                    // 2) Take App Screen Shot
                    // 3) Feed to neural network to learn.
                    // 4) Correlate with keyboard/mouse event stream.

                }
                else
                {
                    bool checkDiff = false;
                    if (current == "explorer")
                    {
                        if (WinApi.GetActiveWindowTitle() != "screenshots")
                        {
                            checkDiff = Environment.TickCount - lastCaptureTime >= diffDelta;
                        }
                    }
                    else
                    {
                        checkDiff = Environment.TickCount - lastCaptureTime >= diffDelta;
                    }

                    if (checkDiff)
                    {
                        lastCaptureTime = Environment.TickCount;
                        diffThreshold = .005;
                        var lastWindow = recorder.WindowCapture;
                        var windowFileName = recorder.SaveWindowScreenShot(active, false);
                        var imageDiff = BitmapDiff(lastWindow, recorder.WindowCapture);
                        if (imageDiff >= diffThreshold)
                        {
                            Console.WriteLine($"[{DateTime.Now}] {active = current} (Id={activeId = monitor.CurrentProcess.Id}) ({title = monitor.CurrentProcess.MainWindowTitle} {imageDiff.ToString("P")})");
                            recorder.WindowCapture.Save(windowFileName, ImageFormat.Png);
                            recorder.SaveScreenShot(active);
                            lastCaptureTime = Environment.TickCount;
                        }
                    }

                }
                System.Threading.Thread.Sleep(1000);
            }

        }

        private static double BitmapDiff(System.Drawing.Bitmap lastWindow, System.Drawing.Bitmap windowCapture)
        {
            if (lastWindow.Width != windowCapture.Width || lastWindow.Height != windowCapture.Height) return 1;
            double sum = 0;
            for (var x = 0; x < lastWindow.Width; x++)
            {
                for (var y = 0; y < lastWindow.Height; y++)
                {
                    var src = lastWindow.GetPixel(x, y);
                    var dest = windowCapture.GetPixel(x, y);
                    if (src != dest)
                    {
                        var diffA = Math.Abs(src.A - dest.A);
                        var diffR = Math.Abs(src.R - dest.R);
                        var diffG = Math.Abs(src.G - dest.G);
                        var diffB = Math.Abs(src.B - dest.B);

                        var pctDiffA = diffA / 255.0;
                        var pctDiffR = diffR / 255.0;
                        var pctDiffG = diffG / 255.0;
                        var pctDiffB = diffB / 255.0;
                        var totalDiff = (pctDiffA + pctDiffR + pctDiffG + pctDiffB) / 4;
                        sum += totalDiff;
                    }

                }
            }
            int totalPixels = windowCapture.Width * windowCapture.Height;
            var result = sum / totalPixels;
            if (result < 1 && result > diffThreshold)
            {
                string bp = $"Diff {result}";
                //Console.WriteLine(bp);
            }
            return result;
        }
    }

    public class Monitor
    {

        public Process CurrentProcess;
        Process childWindowProcess;
        public string GetActiveProcess()
        {
            var foregroundProcess = Process.GetProcessById(WinApi.GetWindowProcessId(WinApi.GetForegroundWindow()));
            if (foregroundProcess.ProcessName == "ApplicationFrameHost")
            {
                WinApi.EnumChildWindows(foregroundProcess.MainWindowHandle, ChildWindowCallback, IntPtr.Zero);
                foregroundProcess = childWindowProcess ?? foregroundProcess;
            }
            return (CurrentProcess = foregroundProcess).ProcessName;
        }

        private bool ChildWindowCallback(IntPtr hwnd, IntPtr lparam)
        {
            var process = Process.GetProcessById(WinApi.GetWindowProcessId(hwnd));
            if (process.ProcessName != "ApplicationFrameHost")
            {
                childWindowProcess = process;
            }
            return true;
        }


    }

    public class WinApi
    {
        /// <summary>
        //  Returns the handle of the ForegroundWindow
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetForegroundWindow();



        /// <summary>
        ///  Gets the process id of a window handlle
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpdwProcessId"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        /// <summary>
        ///  Gets the process id of a window handlle
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        public static int GetWindowProcessId(IntPtr hwnd)
        {
            int processId;
            GetWindowThreadProcessId(hwnd, out processId);
            return processId;
        }

        public delegate bool WindowEnumProc(IntPtr hwnd, IntPtr lparam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc callback, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
    }

}
