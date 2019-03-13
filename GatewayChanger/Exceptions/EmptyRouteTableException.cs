using System.ComponentModel;
using GatewayChanger.Native;
using GatewayChanger.Native.Constants;

namespace GatewayChanger.Exceptions
{
    public class EmptyRouteTableException : Win32Exception
    {
        public EmptyRouteTableException() : base(Error.NoData, "Cannot find any entries on the route table.")
        {
            
        }
    }
}