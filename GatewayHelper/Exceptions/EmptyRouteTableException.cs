using System.ComponentModel;
using GatewayHelper.Native.Constants;

namespace GatewayHelper.Exceptions
{
    public class EmptyRouteTableException : Win32Exception
    {
        public EmptyRouteTableException() : base(Error.NoData, "Cannot find any entries on the route table.")
        {
            
        }
    }
}