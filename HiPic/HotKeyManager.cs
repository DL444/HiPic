using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace HiPic
{
    /// <summary>
    /// 为 Window 提供快捷键管理。
    /// </summary>
    sealed class HotKeyManager
    {
        const int WM_HOTKEY = 0x0312;

        /// <summary>
        /// 被管理 Window 的 HWND。
        /// </summary>
        readonly IntPtr hwnd;
        /// <summary>
        /// 从已注册的 Win32 快捷键 ID 到回调委托的字典。
        /// </summary>
        /// <remarks>
        /// 该字典中所有的值保证恒不为 null。
        /// </remarks>
        readonly Dictionary<int, Action> handlers =
            new Dictionary<int, Action>();

        /// <summary>
        /// 创建用于管理指定 Window 的快捷键的 HotKeyManager 的新实例。
        /// </summary>
        /// <param name="window">要管理快捷键的 Window。</param>
        public HotKeyManager(Window window)
        {
            hwnd = new WindowInteropHelper(window).Handle;
            HwndSource.FromHwnd(hwnd).AddHook(this.WndProc);
        }

        /// <summary>
        /// 为被管理的 Window 注册新的快捷键。
        /// </summary>
        /// <param name="fsModifiers">指定快捷键的修饰键部分。</param>
        /// <param name="key">指定快捷键的非修饰键部分。</param>
        /// <param name="handler">指定此快捷键按下时应当在 UI 线程上被调用的委托。</param>
        /// <exception cref="InvalidOperationException">当指定的快捷键已被注册时抛出。</exception>
        /// <exception cref="Exception">当注册快捷键遇到 Windows API 返回错误时抛出。</exception>
        public void Register(HotKeyModifiers fsModifiers, Key key, Action handler)
        {
            if (handler == null)
                return;

            int vk = KeyInterop.VirtualKeyFromKey(key);
            int id = GetIDFromKeyCombination(fsModifiers, vk);

            try
            {
                handlers.Add(id, handler);
            }
            catch (ArgumentException) // ID already registered
            {
                throw new InvalidOperationException("This hot key has already been registered.");
            }

            if (!WinApi.RegisterHotKey(hwnd, id, fsModifiers, unchecked((uint) vk)))
            {
                handlers.Remove(id);
                throw new Exception("Failed to register hot key.");
            }
        }

        /// <summary>
        /// 为被管理的 Window 注销指定的此前注册的快捷键。
        /// </summary>
        /// <param name="fsModifiers">指定快捷键的修饰键部分。</param>
        /// <param name="key">指定快捷键的非修饰键部分。</param>
        /// <exception cref="Exception">当注销快捷键遇到错误时抛出。</exception>
        public void Unregister(HotKeyModifiers fsModifiers, Key key)
        {
            int vk = KeyInterop.VirtualKeyFromKey(key);
            int id = GetIDFromKeyCombination(fsModifiers, vk);

            if (!WinApi.UnregisterHotKey(hwnd, id))
            {
                // TODO: Special case for "nonexistent hot key"
                throw new Exception("Failed to unregister hot key.");
            }
            // Since WinAPI succeeds, this cannot fail:
            handlers.Remove(id);
        }

        /// <summary>
        /// 从指定的快捷键组合计算唯一快捷键 ID。
        /// </summary>
        /// <param name="fsModifiers">指定快捷键的修饰键部分。</param>
        /// <param name="vk">指定快捷键的非修饰键部分的 Win32 虚拟键值。</param>
        /// <returns>快捷键 ID。</returns>
        /// <remarks>保证不同的快捷键组合与不同的快捷键 ID 一一对应。</remarks>
        static int GetIDFromKeyCombination(HotKeyModifiers fsModifiers, int vk)
        {
            // Valid ID range is [0x0000, 0xBFFF]
            // We are producing IDs within [0x0001, 0x0FFE]
            unchecked
            {
                return (((int) fsModifiers) << 8) | vk;
            }
        }

        /// <summary>
        /// 挂给 Window 的用于检测快捷键的钩子。
        /// </summary>
        /// <param name="hWnd">可以断定等于 this.hwnd。</param>
        /// <param name="msg">传来的消息的类型。我们在此仅处理 WM_HOTKEY。</param>
        /// <param name="wParam">其值等于被按下的快捷键 ID。</param>
        /// <param name="lParam">不用。</param>
        /// <param name="handled">总是不输出 true。</param>
        /// <returns>0。</returns>
        IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                handlers[id]();
            }
            return IntPtr.Zero;
        }
    }

    /// <summary>
    /// 表示快捷键的修饰键部分。
    /// </summary>
    [Flags]
    enum HotKeyModifiers
    {
        /// <summary>
        /// 无修饰键。
        /// </summary>
        None = 0,
        /// <summary>
        /// 左或右 ALT 键。
        /// </summary>
        Alt = 0x1,
        /// <summary>
        /// 左或右 CTRL 键。
        /// </summary>
        Control = 0x2,
        /// <summary>
        /// 左或右 SHIFT 键。
        /// </summary>
        Shift = 0x4,
        /// <summary>
        /// Windows 徽标键。该键为 Windows 功能保留；请勿在应用程序中使用。
        /// </summary>
        Win = 0x8,
    }
}
