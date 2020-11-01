using System;

namespace MouseKeyboardEvents
{
    /// <summary>
    /// All possible events that macro can record
    /// </summary>
    [Serializable]
    public enum MouseKeyEventType
    {
        MouseMove,
        MouseDown,
        MouseUp,
        MouseWheel,
        KeyDown,
        KeyUp
    }
}

