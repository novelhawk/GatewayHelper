using AdvancedDLSupport;

namespace GatewayHelper.Native
{
    internal static class NativeLibrary
    {
        public static readonly IIPHelper IPHelper;

        static NativeLibrary()
        {
            var activator = new NativeLibraryBuilder();

            IPHelper = activator.ActivateInterface<IIPHelper>("Iphlpapi.dll");
        }
    }
}