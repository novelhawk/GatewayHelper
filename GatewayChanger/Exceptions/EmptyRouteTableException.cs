using System.ComponentModel;

namespace GatewayChanger.Exceptions
{
    public class EmptyRouteTableException : Win32Exception
    {
        public EmptyRouteTableException() : base(232, "Cannot find any entries on the route table.")
        {
            
        }
    }
}