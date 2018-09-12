using System;
using System.Runtime.InteropServices;

namespace HiPic
{
    class WinApi
    {
        #region WindowsApi
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, HotKeyModifiers fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetCursorPos(out POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        //[DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        //public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        public static extern int SendMessage(IntPtr hwnd, WM wMsg, IntPtr wParam, IntPtr lParam);
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
    }

    enum WM
    {
        WM_CUT = 0x300,
        WM_COPY = 0x0301,
        WM_PASTE = 0x0302,
        WM_KEYDOWN = 0X100,
        WM_KEYUP = 0x101
    }
}
