using System;
using System.Runtime.InteropServices;
using GatewayChanger.Test.Native.Enums;

namespace GatewayChanger.Test.Native.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyInfo
    {
        public Keys KeyCode;
        public uint ScanCode;
        public KBDLLHOOKSTRUCTFlags Flags;
        public uint Time;
        public UIntPtr ExtraInfo;
    }
}