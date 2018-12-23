using System;
using System.Runtime.InteropServices;
using GatewayChanger.Native.Structures;

namespace GatewayChanger.Native
{
    /// <summary>
    /// PInvoke alternative to IPHelper native calls
    /// </summary>
    public class IPHelper : IIPHelper
    {
        private static class PInvoke
        {
            [DllImport("Iphlpapi.dll")]
            public static extern int GetIpForwardTable(IntPtr pIpForwardTable, ref IntPtr pdwSize, bool bOrder);
            
            [DllImport("Iphlpapi.dll")]
            public static extern int CreateIpForwardEntry(ref IpForwardRow pRoute);
            
            [DllImport("Iphlpapi.dll")]
            public static extern int DeleteIpForwardEntry(ref IpForwardRow pRoute);
        }

        public int GetIpForwardTable(IntPtr pIpForwardTable, ref IntPtr pdwSize, bool bOrder)
        {
            return PInvoke.GetIpForwardTable(pIpForwardTable, ref pdwSize, bOrder);
        }

        public int CreateIpForwardEntry(ref IpForwardRow pRoute)
        {
            return PInvoke.CreateIpForwardEntry(ref pRoute);
        }

        public int DeleteIpForwardEntry(ref IpForwardRow pRoute)
        {
            return PInvoke.DeleteIpForwardEntry(ref pRoute);
        }
    }
}