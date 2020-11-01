using System;
using System.Windows.Forms;

namespace MouseKeyboardEvents
{
    /// <summary>
    /// Series of events that can be recorded any played back
    /// </summary>
    [Serializable]
    public class MouseKeyEvent
    {

        public MouseKeyEventType MacroEventType;
        public MouseEventArgs MouseArgs;
        public KeyEventArgs KeyArgs;
        public int TimeSinceLastEvent;
        protected ulong data;

        public MouseKeyEvent()
        {

        }
        public MouseKeyEvent(ulong eventData)
        {
            this.data = eventData;
            var CompactMacroEvent = new CompactMouseKeyEvent((int)(uint)eventData);
            this.MacroEventType = CompactMacroEvent.MacroEventType;
            this.TimeSinceLastEvent = CompactMacroEvent.TimeSinceLastEvent;
            switch (CompactMacroEvent.MacroEventType)
            {
                case MouseKeyEventType.KeyDown:
                case MouseKeyEventType.KeyUp:
                    var keyData = (int)(uint)(eventData >> 32);
                    var mke = new CompactKeyEvent(keyData, CompactMacroEvent.MacroEventType, CompactMacroEvent.TimeSinceLastEvent);
                    KeyArgs = mke.KeyEvent;
                    break;
                default:
                    var mouseData = (int)(uint)(eventData >> 32);
                    var mme = new CompactMouseEvent(mouseData, CompactMacroEvent.MacroEventType, CompactMacroEvent.TimeSinceLastEvent);
                    MouseArgs = mme.MouseEvent;
                    break;

            }
        }

        public MouseKeyEvent(CompactMouseEvent macroEvent)
        {
            this.data = macroEvent.EventData;
            this.MacroEventType = macroEvent.MacroEventType;
            this.MouseArgs = macroEvent.MouseEvent;
            this.TimeSinceLastEvent = macroEvent.TimeSinceLastEvent;
        }
        public MouseKeyEvent(CompactKeyEvent macroEvent)
        {
            this.data = macroEvent.EventData;
            this.MacroEventType = macroEvent.MacroEventType;
            this.KeyArgs = macroEvent.KeyEvent;
            this.TimeSinceLastEvent = macroEvent.TimeSinceLastEvent;
        }

        public MouseKeyEvent(MouseKeyEventType macroEventType, EventArgs eventArgs, int timeSinceLastEvent)
        {
            MacroEventType = macroEventType;
            TimeSinceLastEvent = timeSinceLastEvent;
            if (eventArgs is MouseEventArgs mouseArgs)
            {
                this.MouseArgs = mouseArgs;
            }
            else
            {
                this.KeyArgs = (KeyEventArgs)eventArgs;
            }
            SetData();
        }

        private void SetData()
        {
            if (MouseArgs != null)
            {
                var mme = new CompactMouseEvent(MouseArgs, MacroEventType, TimeSinceLastEvent);
                this.data = mme.EventData;
            }
            else
            {
                var mke = new CompactKeyEvent(KeyArgs, MacroEventType, TimeSinceLastEvent);
                this.data = mke.EventData;
            }
        }
        public ulong ToUlong()
        {
            if (data == 0)
            {
                SetData();
            }
            return data;
        }

        public static explicit operator MouseKeyEvent(ulong value) => new MouseKeyEvent(value);
        public static explicit operator ulong(MouseKeyEvent value) => value.ToUlong();
    }
}

