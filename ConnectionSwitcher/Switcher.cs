using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;

namespace ConnectionSwitcher
{
    public class Switcher
    {
        private Gateway[] _gateways;
        private int _currentGateway;

        public void LoadGatewaysFromFile(string gateways)
        {
            //TODO: Load from file
            _gateways = new[]
            {
                new Gateway(192, 168, 1, 1), 
                new Gateway(192, 168, 1, 254)
            };
        }

        public void ChangeGateway()
        {
            var nextIndex = (_currentGateway + 1) % _gateways.Length;
            var gateway = _gateways[nextIndex];

            if (InternalChangeGateway(gateway))
            {
                _currentGateway = nextIndex;
                Console.WriteLine("[INFO] Successfully changed gateway to {0}", gateway);
                return;
            }
            
            Console.WriteLine("[ERROR] Failed to change gateway to {0}", gateway);
        }

        private static bool InternalChangeGateway(in Gateway gateway)
        {
            Process p = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    FileName = "cmd.exe",
                    Arguments = $"/c route change 0.0.0.0 mask 0.0.0.0 {gateway}",
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Minimized
                }
            };

            try
            {
                p.Start();
            }
            catch (Win32Exception e)
            {
                Console.WriteLine("[ERROR] Win32 exception while changing gateway: {0:X} (HRESULT: {1})", e.NativeErrorCode, e.HResult);
                return false;
            }
            catch
            {
                Console.WriteLine("[ERROR] Unhandled exception while changing gateway");
                return false;
            }

            if (!p.HasExited && !p.WaitForExit(200))
            {
                p.Kill(); // Permission error
                return false;
            }
            
            return true;
        }

        public Gateway Gateway => _gateways[_currentGateway];
    }

    public struct Gateway
    {
        private readonly byte[] _bytes;

        public Gateway(byte first, byte second, byte third, byte fourth)
        {
            _bytes = new[]
            {
                first, second, third, fourth
            };
        }

        public override string ToString()
        {
            return $"{_bytes[0]}.{_bytes[1]}.{_bytes[2]}.{_bytes[3]}";
        }
    }
}