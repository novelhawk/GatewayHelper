using System;
using System.Runtime.InteropServices;

namespace GatewayChanger.Test.Native.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hWnd;
        public uint message;
        public UIntPtr wParam;
        public IntPtr lParam;
        public int time;
        public POINT point;
#if _MAC
        public int lPrivate;
#endif
    }
}