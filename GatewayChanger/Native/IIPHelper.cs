using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using GatewayChanger.Native.Structures;

namespace GatewayChanger.Native
{
    public interface IIPHelper
    {
        [NativeSymbol(CallingConvention = CallingConvention.StdCall)]
        int GetIpForwardTable(IntPtr pIpForwardTable, ref IntPtr pdwSize, bool bOrder);
        
        [NativeSymbol(CallingConvention = CallingConvention.StdCall)]
        int CreateIpForwardEntry(ref IpForwardRow pRoute);
        
        [NativeSymbol(CallingConvention = CallingConvention.StdCall)]
        int DeleteIpForwardEntry(ref IpForwardRow pRoute);
    }
}