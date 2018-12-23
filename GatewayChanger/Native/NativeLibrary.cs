namespace GatewayChanger.Native
{
    internal static class NativeLibrary
    {
        private static readonly IIPHelper _ipHelper;

        static NativeLibrary()
        {
            _ipHelper = new IPHelper();
        }
        
        public static IIPHelper IPHelper => _ipHelper;
    }
}