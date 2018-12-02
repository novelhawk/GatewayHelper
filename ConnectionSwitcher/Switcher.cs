using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;

namespace ConnectionSwitcher
{
    public class Switcher
    {
        private readonly IIPHelper _ipHelper;
        private Gateway[] _gateways;
        private int _currentGateway;

        public Switcher(IIPHelper ipHelper)
        {
            _ipHelper = ipHelper;
        }
        
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

        private bool InternalGetGateway(out Gateway[] gateways)
        {
            const int ERROR_INSUFFICIENT_BUFFER = 122;
            const int NO_ERROR = 0;

            gateways = new Gateway[0];

            IntPtr tablePtr = IntPtr.Zero;
            IntPtr bufferSize = IntPtr.Zero;
            var status = _ipHelper.GetIpForwardTable(tablePtr, ref bufferSize, false);
            if (status == ERROR_INSUFFICIENT_BUFFER)
            {
                tablePtr = Marshal.AllocHGlobal(bufferSize);
                status = _ipHelper.GetIpForwardTable(tablePtr, ref bufferSize, false);
            }

            if (status != NO_ERROR)
            {
                Console.WriteLine("[ERROR] Failed to get forward table.");
                if (tablePtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(tablePtr);
                return false;
            }

            var forwardTable = Marshal.PtrToStructure<IpForwardTable>(tablePtr);
            forwardTable.table = new IpForwardRow[forwardTable.Size];
            for (int i = 0; i < forwardTable.Size; i++)
                forwardTable.table[i] = Marshal.PtrToStructure<IpForwardRow>(tablePtr + Marshal.SizeOf(forwardTable.Size) + i * Marshal.SizeOf<IpForwardRow>());
            
            for (int i = 0; i < forwardTable.Size; i++)
            {
                var row = forwardTable.table[i];
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

            if (tablePtr != IntPtr.Zero)
                Marshal.FreeHGlobal(tablePtr);
            return gateways.Length > 0;
        }

        private bool InternalChangeGateway(in Gateway gateway)
        {
            const int ERROR_INSUFFICIENT_BUFFER = 122;
            const int NO_ERROR = 0;

            IntPtr tablePtr = IntPtr.Zero;
            IntPtr bufferSize = IntPtr.Zero;
            var status = _ipHelper.GetIpForwardTable(tablePtr, ref bufferSize, false);
            if (status == ERROR_INSUFFICIENT_BUFFER)
            {
                tablePtr = Marshal.AllocHGlobal(bufferSize);
                status = _ipHelper.GetIpForwardTable(tablePtr, ref bufferSize, false);
            }

            if (status != NO_ERROR)
            {
                Console.WriteLine("[ERROR] Failed to get forward table.");
                if (tablePtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(tablePtr);
                return false;
            }

            var forwardTable = Marshal.PtrToStructure<IpForwardTable>(tablePtr);
            forwardTable.table = new IpForwardRow[forwardTable.Size];
            for (int i = 0; i < forwardTable.Size; i++)
                forwardTable.table[i] = Marshal.PtrToStructure<IpForwardRow>(tablePtr + Marshal.SizeOf(forwardTable.Size) + i * Marshal.SizeOf<IpForwardRow>());
            
            IpForwardRow? currentGateway = null;
            
            for (int i = 0; i < forwardTable.Size; i++)
            {
                var row = forwardTable.table[i];
                if (row.dwForwardDest == 0)
                {
                    if (currentGateway == null)
                        currentGateway = row;
                        
                    status = _ipHelper.DeleteIpForwardEntry(ref forwardTable.table[i]);

                    if (status != NO_ERROR)
                    {
                        Console.WriteLine("[ERROR] Could not delete old gateway entry. (Win32Error: {0})", status);
                        if (tablePtr != IntPtr.Zero)
                            Marshal.FreeHGlobal(tablePtr);
                        return false;
                    }
                }
            }

            if (!currentGateway.HasValue)
            {
                Console.WriteLine("[ERROR] Could not find any gateway to change");
                if (tablePtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(tablePtr);
                return false;
            }

            var current = currentGateway.Value;
            current.dwForwardNextHop = gateway;
            
            status = _ipHelper.CreateIpForwardEntry(ref current);

            if (status != NO_ERROR)
                Console.WriteLine("[ERROR] CreateIpForwardEntry failed with code {0}", status);
            
            if (tablePtr != IntPtr.Zero)
                Marshal.FreeHGlobal(tablePtr);
            
            return status == NO_ERROR;
        }

        public Gateway Gateway => _gateways[_currentGateway];
    }

    public struct Gateway : IEquatable<Gateway>
    {
        private readonly byte[] _bytes;

        public Gateway(byte first, byte second, byte third, byte fourth)
        {
            _bytes = new[]
            {
                first, second, third, fourth
            };
        }

        public Gateway(uint ip)
        {
            _bytes = BitConverter.GetBytes(ip);
        }
        
        public static implicit operator uint(Gateway gateway)
        {
            return BitConverter.ToUInt32(gateway._bytes, 0);
        }

        public override string ToString()
        {
            return $"{_bytes[0]}.{_bytes[1]}.{_bytes[2]}.{_bytes[3]}";
        }

        public bool Equals(Gateway other)
        {
            return Equals(_bytes, other._bytes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Gateway gateway && Equals(gateway);
        }

        public override int GetHashCode()
        {
            return _bytes != null ? _bytes.GetHashCode() : 0;
        }
    }
}