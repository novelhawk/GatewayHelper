using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using GatewayChanger.Exceptions;
using GatewayChanger.Native;
using GatewayChanger.Native.Structures;

namespace GatewayChanger
{
    public static class GatewayChanger
    {
        /// <summary>
        /// Gets the forward table
        /// </summary>
        /// <param name="forwardTable">The forward table</param>
        /// <exception cref="OutOfMemoryException">Could not allocate a buffer that can store the forward table</exception>
        /// <exception cref="EmptyRouteTableException">There are no routes in the forward table</exception>
        /// <exception cref="NotSupportedException">There is no IP stack installed</exception>
        /// <exception cref="Win32Exception">Error in unmanaged code</exception>
        private static void GetForwardTable(out IpForwardRow[] forwardTable)
        {
            const int ERROR_INSUFFICIENT_BUFFER = 122;
            const int ERROR_NOT_SUPPORTED = 50;
            const int ERROR_NO_DATA = 232;
            const int NO_ERROR = 0;

            IntPtr buffer = IntPtr.Zero;
            IntPtr bufferSize = IntPtr.Zero;

            //TODO: Use GetIpForwardTable2 to get only IPv4 entries (AF_INET)
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
                
                if (status == ERROR_NO_DATA)
                    throw new EmptyRouteTableException();
                
                if (status == ERROR_NOT_SUPPORTED)
                    throw new NotSupportedException("There is no IP stack installed on the local computer.");
                
                throw new Win32Exception(status);
            }

            var size = (uint) Marshal.ReadInt32(buffer);
            forwardTable = new IpForwardRow[size];
            
            IntPtr currentIndex = buffer + sizeof(uint);
            for (int i = 0; i < size; i++)
            {
                forwardTable[i] = Marshal.PtrToStructure<IpForwardRow>(currentIndex);
                currentIndex += Marshal.SizeOf<IpForwardRow>();
            }
            
            if (buffer != IntPtr.Zero)
                Marshal.FreeHGlobal(buffer);
        }

        /// <summary>
        /// Gets an array of all the gateways on the forward table
        /// </summary>
        /// <exception cref="OutOfMemoryException">Could not allocate a buffer that can store the forward table</exception>
        /// <exception cref="EmptyRouteTableException">There are no routes in the forward table</exception>
        /// <exception cref="NotSupportedException">There is no IP stack installed</exception>
        /// <exception cref="Win32Exception">Error in unmanaged code</exception>
        public static Gateway[] GetGateways()
        {
            GetForwardTable(out IpForwardRow[] forwardTable);

            var gateways = new Gateway[0];
            foreach (var row in forwardTable)
            {
                if (row.dwForwardDest == 0)
                {
                    var array = gateways;
                    gateways = new Gateway[gateways.Length + 1];
                    Array.Copy(array, 0, gateways, 1, array.Length);
                    
                    gateways[0] = new Gateway(row.dwForwardNextHop);
                }
            }
            return gateways;
        }

        /// <summary>
        /// Deletes every gateway entry from the forward table and creates a new one (with the first as a base) with the specified Gateway 
        /// </summary>
        /// <param name="gateway">The gateway that will be set</param>
        /// <exception cref="OutOfMemoryException">Could not allocate a buffer that can store the forward table</exception>
        /// <exception cref="EmptyRouteTableException">There are no routes in the forward table</exception>
        /// <exception cref="NotSupportedException">There is no IP stack installed</exception>
        /// <exception cref="GatewayNotFoundException">There are no gateway entries in the forward table</exception>
        /// <exception cref="UnauthorizedAccessException">The application is not running in an enhanced shell</exception>
        /// <exception cref="NotSupportedException">The IPv4 transport is not configured on the local computer</exception>
        /// <exception cref="Win32Exception">Error in unmanaged code</exception>
        public static void ChangeGateway(in Gateway gateway)
        {
            const int NO_ERROR = 0;
            const int ERROR_ACCESS_DENIED = 5;
            const int ERROR_NOT_SUPPORTED = 50;

            GetForwardTable(out IpForwardRow[] forwardTable);
            
            int status;
            IpForwardRow? currentGateway = null;
            for (int i = 0; i < forwardTable.Length; i++)
            {
                var row = forwardTable[i];
                if (row.dwForwardDest == 0)
                {
                    if (currentGateway == null)
                        currentGateway = row;
                        
                    // Might throw ERROR_INVALID_PARAMETER for IPv6
                    status = NativeLibrary.IPHelper.DeleteIpForwardEntry(ref forwardTable[i]);

                    if (status == ERROR_ACCESS_DENIED)
                        throw new UnauthorizedAccessException();
                    
                    if (status == ERROR_NOT_SUPPORTED)
                        throw new NotSupportedException("The IPv4 transport is not configured on the local computer.");
                    
                    if (status != NO_ERROR)
                        throw new Win32Exception(status);
                }
            }

            if (!currentGateway.HasValue)
                throw new GatewayNotFoundException();

            var current = currentGateway.Value;
            current.dwForwardNextHop = gateway;
            
            status = NativeLibrary.IPHelper.CreateIpForwardEntry(ref current);

            if (status == ERROR_ACCESS_DENIED)
                throw new UnauthorizedAccessException();
            
            if (status == ERROR_NOT_SUPPORTED)
                throw new NotSupportedException("The IPv4 transport is not configured on the local computer.");
            
            if (status != NO_ERROR)
                throw new Win32Exception(status);
        }
    }
}