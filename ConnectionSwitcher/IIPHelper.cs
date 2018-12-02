using System;
using System.Runtime.InteropServices;

namespace ConnectionSwitcher
{
    public interface IIPHelper
    {
        int GetIpForwardTable(IntPtr pIpForwardTable, ref IntPtr pdwSize, bool bOrder);
        int SetIpForwardEntry(ref IpForwardRow pRoute);
        int CreateIpForwardEntry(ref IpForwardRow pRoute);
        int DeleteIpForwardEntry(ref IpForwardRow pRoute);
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct IpForwardTable {
        public uint Size;
        
        [MarshalAs(UnmanagedType.ByValArray)]
        public IpForwardRow[] table;
    }
    
    public enum ForwardType
    {
        Other = 1,
        Invalid = 2,
        Direct = 3,
        Indirect = 4
    }

    public enum ForwardProtocol
    {
        Other = 1,
        Local = 2,
        NetMGMT = 3,
        ICMP = 4,
        EGP = 5,
        GGP = 6,
        Hello = 7,
        RIP = 8,
        IS_IS = 9,
        ES_IS = 10,
        CISCO = 11,
        BBN = 12,
        OSPF = 13,
        BGP = 14,
        NT_AUTOSTATIC = 10002,
        NT_STATIC = 10006,
        NT_STATIC_NON_DOD = 10007
    }

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