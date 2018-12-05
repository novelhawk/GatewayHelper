using AdvancedDLSupport;

namespace GatewayChanger.Test.Native
{
    internal static class NativeLibrary
    {
        private static readonly IUser32 _user32;
        private static readonly IKernel32 _kernel32;

        static NativeLibrary()
        {
            var activator = new NativeLibraryBuilder();
            
            _user32 = activator.ActivateInterface<IUser32>("user32.dll");
            _kernel32 = activator.ActivateInterface<IKernel32>("kernel32.dll");
        }
        
        public static IUser32 User32 => _user32;
        public static IKernel32 Kernel32 => _kernel32;
    }
}