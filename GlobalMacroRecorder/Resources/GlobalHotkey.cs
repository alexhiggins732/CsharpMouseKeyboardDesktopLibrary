using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Hotkeys
{
    public class GlobalHotkey
    {
        private readonly int modifier;
        private readonly int key;
        private readonly IntPtr hWnd;
        private readonly int id;

        public GlobalHotkey(int modifier, Keys key, IWin32Window form)
        {
            this.modifier = modifier;
            this.key = (int)key;
            hWnd = form.Handle;
            id = GetHashCode();
        }

        public bool Register()
        {
            return RegisterHotKey(hWnd, id, modifier, key);
        }

        public bool Unregiser()
        {
            return UnregisterHotKey(hWnd, id);
        }

        public override sealed int GetHashCode()
        {
            return modifier ^ key ^ hWnd.ToInt32();
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
