using System;
using System.Runtime.InteropServices;
using ConnectionSwitcher.NativeLibraries;

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

        public void LoadCurrentGateway()
        {
            if (!InternalGetGateway(out Gateway[] gateways))
            {
                Console.WriteLine("[ERROR] Failed to load current gateway.");
                return;
            }

            if (gateways.Length == 0)
            {
                Console.WriteLine("[WARNING] Could not identify any gateway.");
                return;
            }
            
            if (gateways.Length > 1)
                Console.WriteLine("[WARNING] Multiple gateways identified. Using first.");

            _currentGateway = -1;
            for (int i = 0; i < _gateways.Length; i++)
            {
                if (_gateways[i] == gateways[0])
                {
                    _currentGateway = i;
                    Console.WriteLine("[INFO] Current gateway: {0}", gateways[0]);
                }
            }

            if (_currentGateway == -1)
            {
                Console.WriteLine("[WARNING] Current gateway ({0}) not present in the array. Switching to {1}", gateways[0], _gateways[0]);
                _currentGateway = 0;
                InternalChangeGateway(_gateways[0]);
            }
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

        private static bool GetForwardTable(out IpForwardTable forwardTable)
        {
            const int ERROR_INSUFFICIENT_BUFFER = 122;
            const int NO_ERROR = 0;

            IntPtr buffer = IntPtr.Zero;
            IntPtr bufferSize = IntPtr.Zero;

            var status = NativeLibrary.IPHelper.GetIpForwardTable(buffer, ref bufferSize, false);
            if (status == ERROR_INSUFFICIENT_BUFFER)
            {
                buffer = Marshal.AllocHGlobal(bufferSize);
                status = NativeLibrary.IPHelper.GetIpForwardTable(buffer, ref bufferSize, false);
            }
            
            if (status != NO_ERROR)
            {
                if (buffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(buffer);
                forwardTable = default;
                return false;
            }

            forwardTable = new IpForwardTable();
            forwardTable.Size = (uint) Marshal.ReadInt32(buffer);
            forwardTable.Table = new IpForwardRow[forwardTable.Size];

            IntPtr currentIndex = buffer + Marshal.SizeOf<uint>();
            for (int i = 0; i < forwardTable.Size; i++)
            {
                forwardTable.Table[i] = Marshal.PtrToStructure<IpForwardRow>(currentIndex);
                currentIndex += Marshal.SizeOf<IpForwardRow>();
            }
            
            if (buffer != IntPtr.Zero)
                Marshal.FreeHGlobal(buffer);

            return true;
        }

        private static bool InternalGetGateway(out Gateway[] gateways)
        {
            gateways = new Gateway[0];

            if (!GetForwardTable(out IpForwardTable forwardTable))
            {
                Console.WriteLine("[ERROR] Failed to get forward table.");
                return false;
            }
            
            for (int i = 0; i < forwardTable.Size; i++)
            {
                var row = forwardTable.Table[i];
                if (row.dwForwardDest == 0)
                {
                    if (gateways.Length == 0)
                    {
                        gateways = new[] { new Gateway(row.dwForwardNextHop) };
                    }
                    else
                    {
                        var newGateways = new Gateway[gateways.Length + 1];
                        Array.Copy(gateways, 0, newGateways, 1, gateways.Length);
                        newGateways[0] = new Gateway(row.dwForwardNextHop);
                    }
                }
            }

            return gateways.Length > 0;
        }

        private static bool InternalChangeGateway(in Gateway gateway)
        {
            const int NO_ERROR = 0;
            
            if (!GetForwardTable(out IpForwardTable forwardTable))
            {
                Console.WriteLine("[ERROR] Failed to get forward table.");
                return false;
            }
            
            int status;
            IpForwardRow? currentGateway = null;
            for (int i = 0; i < forwardTable.Size; i++)
            {
                var row = forwardTable.Table[i];
                if (row.dwForwardDest == 0)
                {
                    if (currentGateway == null)
                        currentGateway = row;
                        
                    status = NativeLibrary.IPHelper.DeleteIpForwardEntry(ref forwardTable.Table[i]);

                    if (status != NO_ERROR)
                    {
                        Console.WriteLine("[ERROR] Could not delete old gateway entry. (Win32Error: {0})", status);
                        return false;
                    }
                }
            }

            if (!currentGateway.HasValue)
            {
                Console.WriteLine("[ERROR] Could not find any gateway to change");
                return false;
            }

            var current = currentGateway.Value;
            current.dwForwardNextHop = gateway;
            
            status = NativeLibrary.IPHelper.CreateIpForwardEntry(ref current);

            if (status != NO_ERROR)
                Console.WriteLine("[ERROR] CreateIpForwardEntry failed with code {0}", status);
            
            return status == NO_ERROR;
        }

        public Gateway Gateway => _gateways[_currentGateway];
    }
}