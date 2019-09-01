using System.Runtime.InteropServices;
using GatewayHelper.Native.Enums;

namespace GatewayHelper.Native.Structures
{
    /// <summary>
    /// The MIB_IPFORWARDROW structure equivalent that contains information that describes an IPv4 network route.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct IpForwardRow
    {
        private readonly uint dwForwardDest;
        private readonly uint dwForwardMask;
        private readonly int dwForwardPolicy;
        private uint dwForwardNextHop;
        private readonly int dwForwardIfIndex;
        private readonly ForwardType dwForwardType;
        private readonly ForwardProtocol dwForwardProto;
        private readonly int dwForwardAge;
        private readonly int dwForwardNextHopAS;
        private readonly int dwForwardMetric1;
        private readonly int dwForwardMetric2;
        private readonly int dwForwardMetric3;
        private readonly int dwForwardMetric4;
        private readonly int dwForwardMetric5;

        public long Destination => dwForwardDest;

        public long Gateway
        {
            get => dwForwardNextHop;
            set => dwForwardNextHop = (uint) value;
        }
    }
}