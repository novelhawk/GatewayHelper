using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
// ReSharper disable All

namespace ConnectionSwitcher
{
    /// <summary>
    /// Native functions from user32.dll
    /// </summary>
    public interface IUser32
    {
        int GetMessageA(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
        bool TranslateMessage(ref MSG lpMsg);
        IntPtr DispatchMessageA(ref MSG lpMsg);
        bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers identifiers, Keys vk);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public UIntPtr wParam;
        public IntPtr lParam;
        public int time;
        public POINT pt;
#if _MAC
        public int lPrivate;
#endif
    }
    
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

    
    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }
}