using AdvancedDLSupport;

namespace GatewayChanger.Native
{
    internal static class NativeLibrary
    {
        private static readonly IIPHelper _ipHelper;

        static NativeLibrary()
        {
            var activator = new NativeLibraryBuilder();
            
            _ipHelper = activator.ActivateInterface<IIPHelper>("Iphlpapi.dll");
        }
        
        public static IIPHelper IPHelper => _ipHelper;
    }
}