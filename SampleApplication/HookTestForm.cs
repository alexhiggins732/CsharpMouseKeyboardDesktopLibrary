using System;
using System.Globalization;
using System.Windows.Forms;
using MouseKeyboardLibrary;

namespace SampleApplication
{
    public partial class HookTestForm : Form
    {
        private readonly KeyboardHook keyboardHook = new KeyboardHook();
        private readonly MouseHook mouseHook = new MouseHook();

        public HookTestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            mouseHook.MouseMove += mouseHook_MouseMove;
            mouseHook.MouseDown += mouseHook_MouseDown;
            mouseHook.MouseUp += mouseHook_MouseUp;
            mouseHook.MouseWheel += mouseHook_MouseWheel;

            keyboardHook.KeyDown += keyboardHook_KeyDown;
            keyboardHook.KeyUp += keyboardHook_KeyUp;
            keyboardHook.KeyPress += keyboardHook_KeyPress;

            mouseHook.Start();
            keyboardHook.Start();

            SetXYLabel(MouseSimulator.X, MouseSimulator.Y);
        }

        private void keyboardHook_KeyPress(object sender, KeyPressEventArgs e)
        {
            AddKeyboardEvent(
                "KeyPress",
                "",
                e.KeyChar.ToString(CultureInfo.InvariantCulture),
                "",
                "",
                ""
                );
        }

        private void keyboardHook_KeyUp(object sender, KeyEventArgs e)
        {
            AddKeyboardEvent(
                "KeyUp",
                e.KeyCode.ToString(),
                "",
                e.Shift.ToString(),
                e.Alt.ToString(),
                e.Control.ToString()
                );
        }

        private void keyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            AddKeyboardEvent(
                "KeyDown",
                e.KeyCode.ToString(),
                "",
                e.Shift.ToString(),
                e.Alt.ToString(),
                e.Control.ToString()
                );
        }

        private void mouseHook_MouseWheel(object sender, MouseEventArgs e)
        {
            AddMouseEvent(
                "MouseWheel",
                "",
                "",
                "",
                e.Delta.ToString(CultureInfo.InvariantCulture)
                );
        }

        private void mouseHook_MouseUp(object sender, MouseEventArgs e)
        {
            AddMouseEvent(
                "MouseUp",
                e.Button.ToString(),
                e.X.ToString(CultureInfo.InvariantCulture),
                e.Y.ToString(CultureInfo.InvariantCulture),
                ""
                );
        }

        private void mouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            AddMouseEvent(
                "MouseDown",
                e.Button.ToString(),
                e.X.ToString(CultureInfo.InvariantCulture),
                e.Y.ToString(CultureInfo.InvariantCulture),
                ""
                );
        }

        private void mouseHook_MouseMove(object sender, MouseEventArgs e)
        {
            SetXYLabel(e.X, e.Y);
        }

        private void SetXYLabel(int x, int y)
        {
            curXYLabel.Text = String.Format("Current Mouse Point: X={0}, y={1}", x, y);
        }

        private void AddMouseEvent(string eventType, string button, string x, string y, string delta)
        {
            listView1.Items.Insert(0,
                new ListViewItem(
                    new[]
                    {
                        eventType,
                        button,
                        x,
                        y,
                        delta
                    }));
        }

        private void AddKeyboardEvent(string eventType, string keyCode, string keyChar, string shift, string alt, string control)
        {
            listView2.Items.Insert(0,
                new ListViewItem(
                    new[]
                    {
                        eventType,
                        keyCode,
                        keyChar,
                        shift,
                        alt,
                        control
                    }));
        }
    }
}