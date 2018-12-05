using System.ComponentModel;

namespace GatewayChanger.Exceptions
{
    public class EmptyRouteTableException : Win32Exception
    {
        public EmptyRouteTableException() : base(232, "There are no routes present on the local computer.")
        {
            
        }
    }
}