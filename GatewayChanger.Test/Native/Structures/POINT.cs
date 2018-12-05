using System.Runtime.InteropServices;

namespace GatewayChanger.Test.Native.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }
}