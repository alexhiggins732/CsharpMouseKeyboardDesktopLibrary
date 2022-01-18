using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Hotkeys;
using MouseKeyboardEvents;
using MouseKeyboardLibrary;
using Newtonsoft.Json;

namespace GlobalMacroRecorder
{
    //TODO: Seperate functionality to class and libraries.
    /*
    - Desktop Hook
    - Keyboard hook
    - Mouse hook

    - Events: Implement buffer for storage and reading.
    -- Either partition into files or append to existing.
        EventStorage: Buffer - file.partion using guid and part and then combine?
            .AddEvents(params events); if(session == null) =session = new session {fileName='ts.ext', Format=Json|Binary}; eventBuffer.AddRange(events); (full?? full=StoreEvents())
            .StoreEvents() => File.AppendAllBytes(session.FileName, data)|File.AppendAllText(session.FileName,data);
            .Dispose()=> StoreEvents(); Session = null;
            


    */
    public partial class MacroForm : Form
    {
        public List<MouseKeyEvent> events = new List<MouseKeyEvent>();
        private readonly KeyboardHook keyboardHook = new KeyboardHook();
        private readonly MouseHook mouseHook = new MouseHook();
        private int lastTimeRecorded;
        private bool recording;
        private bool hotkeymessage;
        private bool foreverloopmessage;
        private readonly GlobalHotkey ghk;

        public double ScaleX;
        public double ScaleY;
        public MacroForm()
        {
            InitializeComponent();
            GetScale();
            mouseHook.MouseMove += mouseHook_MouseMove;
            mouseHook.MouseDown += mouseHook_MouseDown;
            mouseHook.MouseUp += mouseHook_MouseUp;
            mouseHook.MouseWheel += MouseHook_MouseWheel;
            keyboardHook.KeyDown += keyboardHook_KeyDown;
            keyboardHook.KeyUp += keyboardHook_KeyUp;
            keyboardHook.KeyPress += KeyboardHook_KeyPress;
            ghk = new GlobalHotkey(Constants.ESC, Keys.Escape, this);
            hotkeymessage = false;
            foreverloopmessage = false;
        }




        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }


        private float GetScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            return ScreenScalingFactor; // 1.25 = 125%
        }
        void GetScale()
        {
            var scalingFactor = GetScalingFactor();
            ScaleX = ScaleY = scalingFactor;
        }
        public Point ScalePoint(Point input)
        {

            int x = (int)(input.X / ScaleX);
            int y = (int)(input.Y / ScaleY);
            return new Point(x, y);
        }

        [DebuggerStepThrough()]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID) HandleHotkey();
            base.WndProc(ref m);
        }

        private void HandleHotkey()
        {
            if (recording)
            {
                StopClick();
            }
            else if (PlayWorker.IsBusy)
            {
                if (ForeverLoop.Checked)
                {
                    ForeverLoop.Checked = false;
                }
                PlayWorker.CancelAsync();
            }
            //runningTest = false;
        }


        private void MouseHook_MouseWheel(object sender, MouseEventArgs e)
        {
            events.Add(new MouseKeyEvent(MouseKeyEventType.MouseWheel, e, Environment.TickCount - lastTimeRecorded));
            lastTimeRecorded = Environment.TickCount;
        }
        private void mouseHook_MouseMove(object sender, MouseEventArgs e)
        {
            events.Add(new MouseKeyEvent(MouseKeyEventType.MouseMove, e, Environment.TickCount - lastTimeRecorded));
            lastTimeRecorded = Environment.TickCount;
        }

        private void mouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            events.Add(new MouseKeyEvent(MouseKeyEventType.MouseDown, e, Environment.TickCount - lastTimeRecorded));
            lastTimeRecorded = Environment.TickCount;
        }

        private void mouseHook_MouseUp(object sender, MouseEventArgs e)
        {
            events.Add(new MouseKeyEvent(MouseKeyEventType.MouseUp, e, Environment.TickCount - lastTimeRecorded));
            lastTimeRecorded = Environment.TickCount;
        }

        private void keyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) return;
            events.Add(new MouseKeyEvent(MouseKeyEventType.KeyDown, e, Environment.TickCount - lastTimeRecorded));
            lastTimeRecorded = Environment.TickCount;
        }

        private void KeyboardHook_KeyPress(object sender, KeyPressEventArgs e)
        {

        }


        private void keyboardHook_KeyUp(object sender, KeyEventArgs e)
        {
            events.Add(new MouseKeyEvent(MouseKeyEventType.KeyUp, e, Environment.TickCount - lastTimeRecorded));
            lastTimeRecorded = Environment.TickCount;
        }

        private void recordStartButton_Click(object sender, EventArgs e)
        {
            //MouseSimulator.X = 0;
            //MouseSimulator.Y = 0;
            events.Clear();
            lastTimeRecorded = Environment.TickCount;
            keyboardHook.Start();
            mouseHook.Start();
            recording = true;
            recordStartButton.Enabled = false;
            playBackMacroButton.Enabled = false;
            recordStopButton.Enabled = true;
        }

        private void recordStopButton_Click(object sender, EventArgs e)
        {
            StopClick();
        }

        private void playBackMacroButton_Click(object sender, EventArgs e)
        {
            if (Hidewindow.Checked)
            {
                Hide();
            }
            else
            {
                playBackMacroButton.Enabled = false;
                recordStartButton.Enabled = false;
                recordStopButton.Enabled = true;
                progressBar1.Maximum = events.Count();
                progressBar1.Visible = true;
            }
            PlayWorker.RunWorkerAsync();
        }

        private void PlayWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var eventpass = 0;
            //MouseSimulator.X = 0;
            //MouseSimulator.Y = 0;
            foreach (MouseKeyEvent macroEvent in events)
            {
                ++eventpass;
                PlayWorker.ReportProgress(eventpass);
                if (PlayWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                Thread.Sleep(macroEvent.TimeSinceLastEvent);
                switch (macroEvent.MacroEventType)
                {
                    case MouseKeyEventType.MouseMove:
                        {
                            var mouseArgs = (MouseEventArgs)macroEvent.MouseArgs;
                            MouseSimulator.X = (int)(mouseArgs.X / ScaleX);
                            MouseSimulator.Y = (int)(mouseArgs.Y / ScaleY);
                        }
                        break;
                    case MouseKeyEventType.MouseDown:
                        {
                            var mouseArgs = (MouseEventArgs)macroEvent.MouseArgs;
                            MouseSimulator.MouseDown(mouseArgs.Button);
                        }
                        break;
                    case MouseKeyEventType.MouseUp:
                        {
                            var mouseArgs = (MouseEventArgs)macroEvent.MouseArgs;
                            MouseSimulator.MouseUp(mouseArgs.Button);
                        }
                        break;
                    case MouseKeyEventType.MouseWheel:
                        {
                            var mouseArgs = (MouseEventArgs)macroEvent.MouseArgs;
                            MouseSimulator.MouseWheel(mouseArgs.Delta);
                        }
                        break;
                    case MouseKeyEventType.KeyDown:
                        {
                            var keyArgs = (KeyEventArgs)macroEvent.KeyArgs;

                            KeyboardSimulator.KeyDown(keyArgs.KeyCode);
                        }
                        break;
                    case MouseKeyEventType.KeyUp:
                        {
                            var keyArgs = (KeyEventArgs)macroEvent.KeyArgs;
                            KeyboardSimulator.KeyUp(keyArgs.KeyCode);
                        }
                        break;
                }
            }
        }

        private void PlayWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (!Hidewindow.Checked)
            {
                progressBar1.Value = e.ProgressPercentage;
            }
        }

        private void PlayWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (ForeverLoop.Checked)
            {
                PlayWorker.RunWorkerAsync();
            }
            else if (Hidewindow.Checked)
            {
                Show();
            }
            else
            {
                progressBar1.Visible = false;
                playBackMacroButton.Enabled = true;
                recordStartButton.Enabled = true;
                recordStopButton.Enabled = false;
            }
        }

        private void MacroForm_Click(object sender, EventArgs e)
        {
            if (stoponselect.Checked)
            {
                StopClick();
            }
        }

        public void StopClick()
        {
            if (recording)
            {

                keyboardHook.Stop();
                mouseHook.Stop();
                recording = false;
                recordStartButton.Enabled = true;
                playBackMacroButton.Enabled = true;
                recordStopButton.Enabled = false;
                SaveMacro_CheckedChanged(this, null);
            }
            else if (PlayWorker.IsBusy)
            {
                PlayWorker.CancelAsync();
            }
        }

        private void MacroForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (stoponselect.Checked)
            {
                StopClick();
            }
        }

        private void SaveMacro_CheckedChanged(object sender, EventArgs e)
        {
            if (!SaveMacro.Checked)
            {
                //MessageBox.Show(@"For all the people who want to see if this will work... it won't.", @"It will not work", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //savemacro.Checked = false;
                return;
            }
            try
            {
                var ts = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
                var json = JsonConvert.SerializeObject(events, Formatting.Indented);
                Directory.CreateDirectory("EventsJson");
                var fileName = $"EventsJson/data-{ts}";
                //File.WriteAllText($"EventsJson/data-{ts}.json", json);
                EventStorage.Save(events, fileName);
                //using (Stream stream = File.Open("data.bin", FileMode.Create))
                //{
                //    bin.Serialize(stream, events);
                //}// This will not work being that you can not serialize mouse events without creating seperate variables for x and y. 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file. Try again later. {ex}");
            }

        }

        private void HotkeyActivated_CheckedChanged(object sender, EventArgs e)
        {
            if (HotkeyActivated.Checked)
            {
                if (hotkeymessage == false)
                {
                    MessageBox.Show(@"The Global Hotkey for this application is 'ESC'. It will stop the recording of keyboard and mouse movements if activated. If you are playing the movements pressing it will stop the playback process. Using this you have the ability to forever loop the movements and drive your friend crazy! Have fun :)");
                    hotkeymessage = true;
                }
                ForeverLoop.Visible = true;
                ghk.Register();
            }
            else
            {
                if (ForeverLoop.Checked)
                {
                    ForeverLoop.Checked = false;
                    MessageBox.Show(@"Forever Loop was on. Deactivated it.", @"Deactivated Forever Loop", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                ForeverLoop.Visible = false;
                ghk.Unregiser();
            }
        }

        private void ForeverLoop_CheckedChanged(object sender, EventArgs e)
        {
            if (ForeverLoop.Checked)
            {
                if (foreverloopmessage == false)
                {
                    MessageBox.Show(@"This feature allows you to forever loop a movement playback. To get out of the playback loop please press 'ESC'. You will need to reactivate this feature when you come out.", @"Enabled Forever Loop", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    foreverloopmessage = true;
                }
            }
        }

        private void btnPlayJson_Click(object sender, EventArgs e)
        {
            bool loaded = false;
            using (var ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    events.Clear();
                    //old
                    //var json = File.ReadAllText(ofd.FileName);
                    //var JsonEvents = JsonConvert.DeserializeObject<List<MacroEvent>>(json);
                    //var binaryEvents = JsonEvents.Select(x => (ulong)x).ToList();
                    //var binaryData = binaryEvents.SelectMany(x => BitConverter.GetBytes(x)).ToList();
                    //File.Delete(ofd.FileName + ".bin");
                    //File.WriteAllBytes(ofd.FileName + ".bin", binaryData.ToArray());
                    //var binaryBytes = File.ReadAllBytes(ofd.FileName + ".bin");
                    //var binaryBytesData = Enumerable.Range(0, binaryBytes.Length / 8)
                    //    .Select(i => BitConverter.ToUInt64(binaryBytes, i * 8))
                    //    .ToArray();
                    //var binaryByteEvents = binaryBytesData.Select(x => new MacroEvent(x)).ToList();
                    //var binaryByteEventsJson= JsonConvert.SerializeObject(binaryByteEvents, Formatting.Indented);
                    //var objectEvents = binaryEvents.Select(x => new MacroEvent(x)).ToList();
                    //var objectJson = JsonConvert.SerializeObject(objectEvents, Formatting.Indented);
                    //bool isEqual2 = binaryByteEventsJson == objectJson;
                    //bool isEqual = json == objectJson;
                    //events.AddRange(JsonEvents);
                    var storedEvents = EventStorage.LoadFromJson(ofd.FileName);
                    events.AddRange(storedEvents);

                    loaded = true;
                }
            }
            if (loaded)
            {
                playBackMacroButton_Click(this, e);
            }
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            if (btnRecord.Text == "Record")
            {
                var fileName = DateTime.Now.ToString("out-yyyy-MM-dd_hh-mm-ss.avi");
                Captura.DesktopRecorder.Start(fileName, this.ScaleX);
                btnRecord.Text = "Stop";
            }
            else
            {
                Captura.DesktopRecorder.Stop();
                btnRecord.Text = "Record";

            }
        }
    }
}