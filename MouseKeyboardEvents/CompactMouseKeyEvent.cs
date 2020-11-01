namespace MouseKeyboardEvents
{
    /// <summary>
    /// Efficient binary macro event storage.
    /// </summary>
    public class CompactMouseKeyEvent
    {

        protected ulong data;


        public CompactMouseKeyEvent(MouseKeyEventType eventType, int timeSinceLastEvent)
        {
            this.MacroEventType = eventType;
            this.TimeSinceLastEvent = timeSinceLastEvent;
        }
        public CompactMouseKeyEvent(int word)
        {
            Word1 = word;
        }

        public ulong EventData => data;
        protected int Word1
        {
            get => (int)(uint)data;
            set => data = (uint)value | (data & ~uint.MaxValue);
        }
        protected int Word2
        {
            get => (int)(uint)(data >> 32);
            set => data = (data & uint.MaxValue) | (((ulong)(uint)value) << 32);
        }

        public MouseKeyEventType MacroEventType
        {
            get => (MouseKeyEventType)(Word1 & 7);
            set => Word1 = (Word1 & ~7) | (int)value;
        }

        public int TimeSinceLastEvent //=> (int)(Word1 >> 3);
        {
            get => (int)(Word1 >> 3);
            set => Word1 = (Word1 & 7) | (value << 3);
        }

        public static int GetMacroData(MouseKeyEventType macroEventType, int timeSinceLastEvent)
            => new CompactMouseKeyEvent(macroEventType, timeSinceLastEvent).Word1;

    }
}

