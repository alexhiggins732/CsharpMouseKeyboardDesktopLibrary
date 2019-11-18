using System;
using System.Windows.Forms;

namespace GlobalMacroRecorder
{

    /// <summary>
    /// All possible events that macro can record
    /// </summary>
    [Serializable]
    public enum MacroEventType
    {
        MouseMove,
        MouseDown,
        MouseUp,
        MouseWheel,
        KeyDown,
        KeyUp
    }

    /// <summary>
    /// Series of events that can be recorded any played back
    /// </summary>
    [Serializable]
    public class MacroEvent
    {
        
        public MacroEventType MacroEventType;
        public MouseEventArgs MouseArgs;
        public KeyEventArgs KeyArgs;
        public int TimeSinceLastEvent;
        /*public List<MacroEvent> events = new List<MacroEvent>();
        public List<MacroEvent> Events
        {
            get { return events; }
            set { events = value; }
        }*/

        public MacroEvent(MacroEventType macroEventType, EventArgs eventArgs, int timeSinceLastEvent)
        {
            MacroEventType = macroEventType;
            if (eventArgs is MouseEventArgs mouseArgs)
            {
                this.MouseArgs = mouseArgs;
            } else
            {
                this.KeyArgs = (KeyEventArgs)eventArgs;
            }
            TimeSinceLastEvent = timeSinceLastEvent;
        }
    }}
