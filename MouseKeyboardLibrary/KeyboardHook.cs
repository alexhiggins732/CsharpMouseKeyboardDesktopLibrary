using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MouseKeyboardLibrary
{
    /// <summary>
    ///     Captures global keyboard events
    /// </summary>
    public class KeyboardHook : GlobalHook
    {
        #region Events

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
        public event KeyPressEventHandler KeyPress;

        #endregion

        #region Constructor

        public KeyboardHook()
        {
            _hookType = WH_KEYBOARD_LL;
        }

        #endregion

        #region Methods

        protected override int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
        {
            bool handled = false;

            if (nCode <= -1 || (KeyDown == null && KeyUp == null && KeyPress == null)) return CallNextHookEx(_handleToHook, nCode, wParam, lParam);
            var keyboardHookStruct = (KeyboardHookStruct) Marshal.PtrToStructure(lParam, typeof (KeyboardHookStruct));

            // Is Control being held down?
            bool control = ((GetKeyState(VK_LCONTROL) & 0x80) != 0) ||
                           ((GetKeyState(VK_RCONTROL) & 0x80) != 0);

            // Is Shift being held down?
            bool shift = ((GetKeyState(VK_LSHIFT) & 0x80) != 0) ||
                         ((GetKeyState(VK_RSHIFT) & 0x80) != 0);

            // Is Alt being held down?
            bool alt = ((GetKeyState(VK_LALT) & 0x80) != 0) ||
                       ((GetKeyState(VK_RALT) & 0x80) != 0);

            // Is CapsLock on?
            bool capslock = (GetKeyState(VK_CAPITAL) != 0);

            // Create event using keycode and control/shift/alt values found above
            var e = new KeyEventArgs(
                (Keys) (
                    keyboardHookStruct.vkCode |
                    (control ? (int) Keys.Control : 0) |
                    (shift ? (int) Keys.Shift : 0) |
                    (alt ? (int) Keys.Alt : 0)
                    ));

            // Handle KeyDown and KeyUp events
            switch (wParam)
            {
                case WM_KEYDOWN:
                case WM_SYSKEYDOWN:
                    if (KeyDown != null)
                    {
                        KeyDown(this, e);
                        handled = e.Handled;
                    }
                    break;
                case WM_KEYUP:
                case WM_SYSKEYUP:
                    if (KeyUp != null)
                    {
                        KeyUp(this, e);
                        handled = e.Handled;
                    }
                    break;
            }

            // Handle KeyPress event
            if (wParam != WM_KEYDOWN || handled || e.SuppressKeyPress || KeyPress == null) return handled ? 1 : CallNextHookEx(_handleToHook, nCode, wParam, lParam);
            var keyState = new byte[256];
            var inBuffer = new byte[2];
            GetKeyboardState(keyState);

            if (ToAscii(keyboardHookStruct.vkCode, keyboardHookStruct.scanCode, keyState, inBuffer, keyboardHookStruct.flags) != 1) return CallNextHookEx(_handleToHook, nCode, wParam, lParam);
            var key = (char) inBuffer[0];
            if ((capslock ^ shift) && Char.IsLetter(key))
                key = Char.ToUpper(key);
            var e2 = new KeyPressEventArgs(key);
            KeyPress(this, e2);
            handled = e.Handled;

            return handled ? 1 : CallNextHookEx(_handleToHook, nCode, wParam, lParam);
        }

        #endregion
    }
}