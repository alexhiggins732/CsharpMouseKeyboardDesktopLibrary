using System.Windows.Forms;

namespace MouseKeyboardEvents
{
    public class CompactKeyEvent : CompactMouseKeyEvent
    {
        public KeyEventArgs KeyEvent;

        public CompactKeyEvent(int keyData, MouseKeyEventType eventType, int timeSinceLastEvent)
            : base(eventType, timeSinceLastEvent)
        {
            base.Word2 = keyData;
            KeyEvent = new KeyEventArgs((Keys)keyData);
        }
        public CompactKeyEvent(KeyEventArgs keyEvent, MouseKeyEventType eventType, int timeSinceLastEvent)
            : base(eventType, timeSinceLastEvent)
        {
            base.Word2 = (int)keyEvent.KeyData;
            KeyEvent = keyEvent;
        }

        public static explicit operator int(CompactKeyEvent evt) => (int)evt.KeyEvent.KeyData;
        public static implicit operator KeyEventArgs(CompactKeyEvent keyEvent) => keyEvent.KeyEvent;
        public static implicit operator ulong(CompactKeyEvent keyEvent) => keyEvent.data;
        public static implicit operator MouseKeyEvent(CompactKeyEvent keyEvent) => new MouseKeyEvent(keyEvent);
    }
}

