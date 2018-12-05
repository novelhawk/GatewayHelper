using System;
using GatewayChanger.Native.Structures;

namespace GatewayChanger.Native
{
    public interface IIPHelper
    {
        int GetIpForwardTable(IntPtr pIpForwardTable, ref IntPtr pdwSize, bool bOrder);
        int CreateIpForwardEntry(ref IpForwardRow pRoute);
        int DeleteIpForwardEntry(ref IpForwardRow pRoute);
    }
}