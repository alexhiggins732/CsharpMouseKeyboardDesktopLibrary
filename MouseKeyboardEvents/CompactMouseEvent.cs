using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace MouseKeyboardEvents
{

    public class CompactMouseEvent : CompactMouseKeyEvent
    {
        public MouseEventArgs MouseEvent;
        const int locationMask = 0x3FFF; // (1 << 14) - 1; 16383;
        const int yOffset = 14;
        const int buttonDataOffset = 28;
        public int MouseData { get => base.Word2; private set => base.Word2 = value; }
        public CompactMouseEvent(int mouseData, MouseKeyEventType eventType, int timeSinceLastEvent)
            : base(eventType, timeSinceLastEvent)
        {
            MouseData = mouseData;
            int clicks = 1; //always 0
            int x = mouseData & locationMask;// 14 bits: 0-13
            int y = (mouseData >> yOffset) & locationMask;// 14 bits: 14-27;

            MouseButtons buttons = MouseButtons.None;
            int buttonData = mouseData >> buttonDataOffset; // 3: bits 28-30;
            int delta = 0;
            switch (buttonData)
            {
                case 1: buttons = MouseButtons.Left; break;
                case 2: buttons = MouseButtons.Right; break;
                case 3: buttons = MouseButtons.Middle; break;
                case 4: buttons = MouseButtons.XButton1; break;
                case 5: buttons = MouseButtons.XButton2; break;
                case 6: delta = 120; break;
                case 7: delta = -120; break;
            }

            MouseEvent = new MouseEventArgs(buttons, clicks, x, y, delta);
        }
        public CompactMouseEvent(MouseEventArgs mouseEvent, MouseKeyEventType eventType, int timeSinceLastEvent) :
            base(eventType, timeSinceLastEvent)
        {
            MouseEvent = mouseEvent;
            MouseData = GetMouseData(mouseEvent);
        }
        public static int GetMouseData(MouseEventArgs mouseEvent)
        {
            int xData = mouseEvent.X & locationMask; // bits 0-13
            int yData = (mouseEvent.Y & locationMask) << yOffset;// bits 14-27;
            int buttonData = 0;
            switch (mouseEvent.Button)
            {
                case MouseButtons.Left: buttonData = 1; break;
                case MouseButtons.Right: buttonData = 2; break;
                case MouseButtons.Middle: buttonData = 3; break;
                case MouseButtons.XButton1: buttonData = 4; break;
                case MouseButtons.XButton2: buttonData = 5; break;
                case MouseButtons.None:
                default:
                    switch (mouseEvent.Delta)
                    {
                        case 120: buttonData = 6; break;
                        case -120: buttonData = 7; break;
                    }
                    break;
            }
            buttonData <<= buttonDataOffset;
            int result = buttonData | yData | xData;
            return result;
        }

        public static implicit operator MouseEventArgs(CompactMouseEvent mouseEvent) => mouseEvent.MouseEvent;
        public static implicit operator int(CompactMouseEvent evt) => evt.MouseData;
        public static implicit operator ulong(CompactMouseEvent mouseEvent) => mouseEvent.data;
        public static implicit operator MouseKeyEvent(CompactMouseEvent mouseEvent) => new MouseKeyEvent(mouseEvent);

    }
}

