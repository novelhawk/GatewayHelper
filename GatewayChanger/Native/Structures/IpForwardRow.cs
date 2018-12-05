using System.Runtime.InteropServices;
using GatewayChanger.Native.Enums;

namespace GatewayChanger.Native.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct IpForwardRow
    {
        public uint dwForwardDest;
        public uint dwForwardMask;
        public int dwForwardPolicy;
        public uint dwForwardNextHop;
        public int dwForwardIfIndex;
        public ForwardType dwForwardType;
        public ForwardProtocol dwForwardProto;
        public int dwForwardAge;
        public int dwForwardNextHopAS;
        public int dwForwardMetric1;
        public int dwForwardMetric2;
        public int dwForwardMetric3;
        public int dwForwardMetric4;
        public int dwForwardMetric5;
    }
}