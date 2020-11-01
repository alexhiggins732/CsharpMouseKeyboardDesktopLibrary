using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroStorageTests
{

    //[ComVisible(true)]
    public class KeyEventArgs : EventArgs
    {

        /// <summary> Contains key data for KeyDown and KeyUp events.  This is a combination of keycode and modifer flags.</summary>
        private readonly Keys keyData;


        /// <summary> Determines if this event has been handled by a handler and if it will be sent for processing. </summary>
        private bool handled = false;


        /// <summary />
        private bool suppressKeyPress = false;


        /// <summary> Initializes a new instance of the <see cref='KeyEventArgs'/> class. </summary>
        public KeyEventArgs(Keys keyData)
        {
            this.keyData = keyData;
        }


        /// <summary> Gets a value indicating whether the ALT key was pressed. </summary>
        public virtual bool Alt => (keyData & Keys.Alt) == Keys.Alt;


        /// <summary> Gets a value indicating whether the CTRL key was pressed. </summary>
        public bool Control => (keyData & Keys.Control) == Keys.Control;


        /// <summary> Gets or sets a value indicating whether the event was handled. </summary>
        public bool Handled { get => handled; set => handled = value; }


        /// <summary> Gets the keyboard code for a <see cref='KeyDown'/> or <see cref='KeyUp'/> event. </summary>
        public Keys KeyCode => keyData & Keys.KeyCode;


        /// <summary> Gets the keyboard value for a <see cref='KeyDown'/> or <see cref='KeyUp'/> event. </summary>
        public int KeyValue => (int)(keyData & Keys.KeyCode);


        /// <summary> Gets the key data for a <see cref='KeyDown'/> or <see cref='KeyUp'/> event. </summary>
        public Keys KeyData { get => keyData; }


        /// <summary> Gets the CTRL, SHIFT, and/or ALT modifier a <see cref='KeyDown'/> or <see cref='KeyUp'/> event. </summary>
        public Keys Modifiers => keyData & Keys.Modifiers;


        /// <summary> Gets  a value indicating whether the SHIFT key was pressed.</summary>
        public virtual bool Shift => (keyData & Keys.Shift) == Keys.Shift;


        /// <summary />
        public bool SuppressKeyPress { get => suppressKeyPress; set => suppressKeyPress = handled = value; }
    }

    //[ComVisible(true)]
    //[Editor("System.Windows.Forms.Design.ShortcutKeysEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
    //[Flags]
    //[TypeConverter(typeof(KeysConverter))]

    public enum Keys
    {
        Modifiers = -65536,
        None = 0,
        LButton = 1,
        RButton = 2,
        Cancel = 3,
        MButton = 4,
        XButton1 = 5,
        XButton2 = 6,
        Back = 8,
        Tab = 9,
        LineFeed = 10,
        Clear = 12,
        Return = 13,
        Enter = 13,
        ShiftKey = 16,
        ControlKey = 17,
        Menu = 18,
        Pause = 19,
        Capital = 20,
        CapsLock = 20,
        KanaMode = 21,
        HanguelMode = 21,
        HangulMode = 21,
        JunjaMode = 23,
        FinalMode = 24,
        HanjaMode = 25,
        KanjiMode = 25,
        Escape = 27,
        IMEConvert = 28,
        IMENonconvert = 29,
        IMEAccept = 30,
        IMEAceept = 30,
        IMEModeChange = 31,
        Space = 32,
        Prior = 33,
        PageUp = 33,
        Next = 34,
        PageDown = 34,
        End = 35,
        Home = 36,
        Left = 37,
        Up = 38,
        Right = 39,
        Down = 40,
        Select = 41,
        Print = 42,
        Execute = 43,
        Snapshot = 44,
        PrintScreen = 44,
        Insert = 45,
        Delete = 46,
        Help = 47,
        D0 = 48,
        D1 = 49,
        D2 = 50,
        D3 = 51,
        D4 = 52,
        D5 = 53,
        D6 = 54,
        D7 = 55,
        D8 = 56,
        D9 = 57,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        LWin = 91,
        RWin = 92,
        Apps = 93,
        Sleep = 95,
        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        Multiply = 106,
        Add = 107,
        Separator = 108,
        Subtract = 109,
        Decimal = 110,
        Divide = 111,
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,
        F13 = 124,
        F14 = 125,
        F15 = 126,
        F16 = 127,
        F17 = 128,
        F18 = 129,
        F19 = 130,
        F20 = 131,
        F21 = 132,
        F22 = 133,
        F23 = 134,
        F24 = 135,
        NumLock = 144,
        Scroll = 145,
        LShiftKey = 160,
        RShiftKey = 161,
        LControlKey = 162,
        RControlKey = 163,
        LMenu = 164,
        RMenu = 165,
        BrowserBack = 166,
        BrowserForward = 167,
        BrowserRefresh = 168,
        BrowserStop = 169,
        BrowserSearch = 170,
        BrowserFavorites = 171,
        BrowserHome = 172,
        VolumeMute = 173,
        VolumeDown = 174,
        VolumeUp = 175,
        MediaNextTrack = 176,
        MediaPreviousTrack = 177,
        MediaStop = 178,
        MediaPlayPause = 179,
        LaunchMail = 180,
        SelectMedia = 181,
        LaunchApplication1 = 182,
        LaunchApplication2 = 183,
        OemSemicolon = 186,
        Oem1 = 186,
        Oemplus = 187,
        Oemcomma = 188,
        OemMinus = 189,
        OemPeriod = 190,
        OemQuestion = 191,
        Oem2 = 191,
        Oemtilde = 192,
        Oem3 = 192,
        OemOpenBrackets = 219,
        Oem4 = 219,
        OemPipe = 220,
        Oem5 = 220,
        OemCloseBrackets = 221,
        Oem6 = 221,
        OemQuotes = 222,
        Oem7 = 222,
        Oem8 = 223,
        OemBackslash = 226,
        Oem102 = 226,
        ProcessKey = 229,
        Packet = 231,
        Attn = 246,
        Crsel = 247,
        Exsel = 248,
        EraseEof = 249,
        Play = 250,
        Zoom = 251,
        NoName = 252,
        Pa1 = 253,
        OemClear = 254,
        KeyCode = 65535,
        Shift = 65536,
        Control = 131072,
        Alt = 262144
    }


    /// <summary> Provides data for the MouseEvent. </summary>
    //[System.Runtime.InteropServices.ComVisible(true)]
    public class MouseEventArgs : EventArgs
    {
        /// <summary> Which button generated this event. </summary>
        private readonly MouseButtons button; // 6 bits

        /// <summary> If the user has clicked the mouse more than once, this contains the count of clicks so far. </summary>
        private readonly int clicks; //always zero

        /// <summary> The x portion of the coordinate where this event occurred. </summary>
        private readonly int x; // 14 bits 16,384

        /// <summary> The y portion of the coordinate where this event occurred. </summary>
        private readonly int y; // 14 bits 16,384

        private readonly int delta; // can use 1 bit for postive, negative

        /// <summary> Initializes a new instance of the <see cref='MouseEventArgs'/> class. </summary>
        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
        {
            this.button = button;
            this.clicks = clicks;
            this.x = x;
            this.y = y;
            this.delta = delta;
        }

        /// <summary> Gets which mouse button was pressed. </summary>
        public MouseButtons Button => button;

        /// <summary> Gets the number of times the mouse button was pressed and released. </summary>
        public int Clicks => delta;

        /// <summary> Gets the x-coordinate of a mouse click. </summary>
        public int X => x;

        /// <summary> Gets the y-coordinate of a mouse click. </summary>
        public int Y => y;

        /// <summary> Gets a signed count of the number of detents the mouse wheel has rotated. </summary>
        public int Delta => delta;

        /// <summary> Gets the location of the mouse during MouseEvent. </summary>
        public Point Location => new Point(x, y);
    }

    public struct Point
    {
        public int X;
        public int Y;
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    //[ComVisible(true)]
    [Flags]
    public enum MouseButtons
    {
        None = 0,
        Left = 1048576,
        Right = 2097152,
        Middle = 4194304,
        XButton1 = 8388608,
        XButton2 = 16777216
    }
}
