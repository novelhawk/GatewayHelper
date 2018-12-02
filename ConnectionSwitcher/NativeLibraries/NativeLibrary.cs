using AdvancedDLSupport;

namespace ConnectionSwitcher.NativeLibraries
{
    public static class NativeLibrary
    {
        private static readonly IIPHelper _ipHelper;
        private static readonly IUser32 _user32;
        private static readonly IKernel32 _kernel32;

        static NativeLibrary()
        {
            var activator = new NativeLibraryBuilder();
            
            _user32 = activator.ActivateInterface<IUser32>("user32.dll");
            _kernel32 = activator.ActivateInterface<IKernel32>("kernel32.dll");
            _ipHelper = activator.ActivateInterface<IIPHelper>("Iphlpapi.dll");
        }
        
        public static IIPHelper IPHelper => _ipHelper;
        public static IUser32 User32 => _user32;
        public static IKernel32 Kernel32 => _kernel32;
    }
}