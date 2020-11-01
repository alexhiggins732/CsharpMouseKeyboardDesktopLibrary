using System;
using System.Linq;
using System.Windows.Forms;

namespace MouseKeyNetwork
{
    public class KeysEnumNetwork<TNumeric>
    {
        public override string ToString()
        {{
            return string.Join(" | ",
                this.GetType()
                .GetFields()
                .Where(x => x.FieldType == typeof(TNumeric) && ToInt((TNumeric)x.GetValue(this)) != 0)
                .Select(x=> x.Name));
        }}
        public virtual int ToInt(TNumeric numeric) => Convert.ToInt32(numeric);
        public TNumeric None;
        public TNumeric LButton;
        public TNumeric RButton;
        public TNumeric Cancel;
        public TNumeric MButton;
        public TNumeric XButton1;
        public TNumeric XButton2;
        public TNumeric Back;
        public TNumeric Tab;
        public TNumeric LineFeed;
        public TNumeric Clear;
        public TNumeric Enter;
        public TNumeric Return;
        public TNumeric ShiftKey;
        public TNumeric ControlKey;
        public TNumeric Menu;
        public TNumeric Pause;
        public TNumeric CapsLock;
        public TNumeric Capital;
        public TNumeric HangulMode;
        public TNumeric HanguelMode;
        public TNumeric KanaMode;
        public TNumeric JunjaMode;
        public TNumeric FinalMode;
        public TNumeric KanjiMode;
        public TNumeric HanjaMode;
        public TNumeric Escape;
        public TNumeric IMEConvert;
        public TNumeric IMENonconvert;
        public TNumeric IMEAccept;
        public TNumeric IMEAceept;
        public TNumeric IMEModeChange;
        public TNumeric Space;
        public TNumeric Prior;
        public TNumeric PageUp;
        public TNumeric PageDown;
        public TNumeric Next;
        public TNumeric End;
        public TNumeric Home;
        public TNumeric Left;
        public TNumeric Up;
        public TNumeric Right;
        public TNumeric Down;
        public TNumeric Select;
        public TNumeric Print;
        public TNumeric Execute;
        public TNumeric Snapshot;
        public TNumeric PrintScreen;
        public TNumeric Insert;
        public TNumeric Delete;
        public TNumeric Help;
        public TNumeric D0;
        public TNumeric D1;
        public TNumeric D2;
        public TNumeric D3;
        public TNumeric D4;
        public TNumeric D5;
        public TNumeric D6;
        public TNumeric D7;
        public TNumeric D8;
        public TNumeric D9;
        public TNumeric A;
        public TNumeric B;
        public TNumeric C;
        public TNumeric D;
        public TNumeric E;
        public TNumeric F;
        public TNumeric G;
        public TNumeric H;
        public TNumeric I;
        public TNumeric J;
        public TNumeric K;
        public TNumeric L;
        public TNumeric M;
        public TNumeric N;
        public TNumeric O;
        public TNumeric P;
        public TNumeric Q;
        public TNumeric R;
        public TNumeric S;
        public TNumeric T;
        public TNumeric U;
        public TNumeric V;
        public TNumeric W;
        public TNumeric X;
        public TNumeric Y;
        public TNumeric Z;
        public TNumeric LWin;
        public TNumeric RWin;
        public TNumeric Apps;
        public TNumeric Sleep;
        public TNumeric NumPad0;
        public TNumeric NumPad1;
        public TNumeric NumPad2;
        public TNumeric NumPad3;
        public TNumeric NumPad4;
        public TNumeric NumPad5;
        public TNumeric NumPad6;
        public TNumeric NumPad7;
        public TNumeric NumPad8;
        public TNumeric NumPad9;
        public TNumeric Multiply;
        public TNumeric Add;
        public TNumeric Separator;
        public TNumeric Subtract;
        public TNumeric Decimal;
        public TNumeric Divide;
        public TNumeric F1;
        public TNumeric F2;
        public TNumeric F3;
        public TNumeric F4;
        public TNumeric F5;
        public TNumeric F6;
        public TNumeric F7;
        public TNumeric F8;
        public TNumeric F9;
        public TNumeric F10;
        public TNumeric F11;
        public TNumeric F12;
        public TNumeric F13;
        public TNumeric F14;
        public TNumeric F15;
        public TNumeric F16;
        public TNumeric F17;
        public TNumeric F18;
        public TNumeric F19;
        public TNumeric F20;
        public TNumeric F21;
        public TNumeric F22;
        public TNumeric F23;
        public TNumeric F24;
        public TNumeric NumLock;
        public TNumeric Scroll;
        public TNumeric LShiftKey;
        public TNumeric RShiftKey;
        public TNumeric LControlKey;
        public TNumeric RControlKey;
        public TNumeric LMenu;
        public TNumeric RMenu;
        public TNumeric BrowserBack;
        public TNumeric BrowserForward;
        public TNumeric BrowserRefresh;
        public TNumeric BrowserStop;
        public TNumeric BrowserSearch;
        public TNumeric BrowserFavorites;
        public TNumeric BrowserHome;
        public TNumeric VolumeMute;
        public TNumeric VolumeDown;
        public TNumeric VolumeUp;
        public TNumeric MediaNextTrack;
        public TNumeric MediaPreviousTrack;
        public TNumeric MediaStop;
        public TNumeric MediaPlayPause;
        public TNumeric LaunchMail;
        public TNumeric SelectMedia;
        public TNumeric LaunchApplication1;
        public TNumeric LaunchApplication2;
        public TNumeric OemSemicolon;
        public TNumeric Oem1;
        public TNumeric Oemplus;
        public TNumeric Oemcomma;
        public TNumeric OemMinus;
        public TNumeric OemPeriod;
        public TNumeric Oem2;
        public TNumeric OemQuestion;
        public TNumeric Oem3;
        public TNumeric Oemtilde;
        public TNumeric Oem4;
        public TNumeric OemOpenBrackets;
        public TNumeric OemPipe;
        public TNumeric Oem5;
        public TNumeric OemCloseBrackets;
        public TNumeric Oem6;
        public TNumeric OemQuotes;
        public TNumeric Oem7;
        public TNumeric Oem8;
        public TNumeric Oem102;
        public TNumeric OemBackslash;
        public TNumeric ProcessKey;
        public TNumeric Packet;
        public TNumeric Attn;
        public TNumeric Crsel;
        public TNumeric Exsel;
        public TNumeric EraseEof;
        public TNumeric Play;
        public TNumeric Zoom;
        public TNumeric NoName;
        public TNumeric Pa1;
        public TNumeric OemClear;
        public TNumeric KeyCode;
        public TNumeric Shift;
        public TNumeric Control;
        public TNumeric Alt;
        public TNumeric Modifiers;
    }


    public class KeysEnumNetworkOfFloat: KeysEnumNetwork<float>
    {
        public static KeysEnumNetworkOfFloat Create(Keys keys)
        {
            var thisType = typeof(KeysEnumNetworkOfFloat);
            var type = typeof(Keys);
            var result = new KeysEnumNetworkOfFloat();
            if ((int)keys == 0)
            {
                if (Enum.IsDefined(type, 0))
                {
                    var name = Enum.GetName(type, 0);
                    var field = thisType.GetField(name);
                    field.SetValue(result, 1.0f);
                }
            } 
            else
            {
                var names = Enum.GetNames(type);
                foreach (var name in names)
                {
                    var value = (Keys)Enum.Parse(type, name);
                    if ((int)value == 0) continue;
                    if (((keys & Keys.KeyCode) == (value & Keys.KeyCode)) || ((keys & Keys.Modifiers) == (value & Keys.Modifiers)))
                    {
                        var field = thisType.GetField(name);
                        field.SetValue(result, 1.0f);
                    }
                }
            }
            return result;
        }
    }
}
