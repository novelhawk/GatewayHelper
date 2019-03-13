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
        /// Retrieves the IPv4 routing table
        /// </summary>
        /// <param name="forwardTable">The IPv4 routing table</param>
        /// <exception cref="OutOfMemoryException">Could not allocate a buffer that can store the routing table</exception>
        /// <exception cref="EmptyRouteTableException">There are no routes in the routing table</exception>
        /// <exception cref="NotSupportedException">There is no IP stack installed</exception>
        /// <exception cref="Win32Exception">Unexpected error</exception>
        private static void GetForwardTable(out IpForwardRow[] forwardTable)
        {
            const int ERROR_INSUFFICIENT_BUFFER = 122;
            const int ERROR_NOT_SUPPORTED = 50;
            const int ERROR_NO_DATA = 232;
            const int NO_ERROR = 0;

            IntPtr buffer = IntPtr.Zero;
            IntPtr bufferSize = IntPtr.Zero;

            try
            {
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
                    buffer = IntPtr.Zero;
                    
                    if (status == ERROR_NO_DATA)
                        throw new EmptyRouteTableException();
                    if (status == ERROR_NOT_SUPPORTED)
                        throw new NotSupportedException("There is no IP stack installed on the local computer.");

                    throw new Win32Exception(status);
                }

                var size = (uint) Marshal.ReadInt32(buffer);
                forwardTable = new IpForwardRow[size];

                IntPtr currentPointer = buffer + sizeof(uint);
                for (int i = 0; i < size; i++)
                {
                    forwardTable[i] = Marshal.PtrToStructure<IpForwardRow>(currentPointer);
                    currentPointer += Marshal.SizeOf<IpForwardRow>();
                }
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// Retrieves all the gateways on the IPv4 routing table
        /// </summary>
        /// <returns>An array of <see cref="Gateway"/> containing every found gateway</returns>
        /// <exception cref="OutOfMemoryException">Could not allocate a buffer that can store the routing table</exception>
        /// <exception cref="EmptyRouteTableException">There are no routes in the routing table</exception>
        /// <exception cref="NotSupportedException">There is no IP stack installed</exception>
        /// <exception cref="Win32Exception">Unexpected error</exception>
        public static Gateway[] GetGateways()
        {
            GetForwardTable(out IpForwardRow[] forwardTable);

            var gateways = new Gateway[0];
            foreach (var row in forwardTable)
            {
                if (row.Destination == 0)
                {
                    var array = gateways;
                    gateways = new Gateway[gateways.Length + 1];
                    Array.Copy(array, 0, gateways, 1, array.Length);
                    
                    gateways[0] = new Gateway(row.Gateway);
                }
            }
            return gateways;
        }

        /// <summary>
        /// Deletes every gateway entry from the IPv4 routing table and creates a new one with the specified <paramref name="gateway"/> 
        /// </summary>
        /// <param name="gateway">The gateway that will be set</param>
        /// <exception cref="OutOfMemoryException">Could not allocate a buffer that can store the routing table</exception>
        /// <exception cref="EmptyRouteTableException">There are no routes in the routing table</exception>
        /// <exception cref="NotSupportedException">There is no IP stack installed on the local computer</exception>
        /// <exception cref="GatewayNotFoundException">There are no gateway entries in the routing table</exception>
        /// <exception cref="UnauthorizedAccessException">The application is not running in an enhanced shell</exception>
        /// <exception cref="NotSupportedException">The IPv4 transport is not configured on the local computer</exception>
        /// <exception cref="Win32Exception">Unexpected error</exception>
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
                if (row.Destination == 0)
                {
                    if (currentGateway == null)
                        currentGateway = row;
                        
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
            current.Gateway = gateway.Address;
            
            status = NativeLibrary.IPHelper.CreateIpForwardEntry(ref current);

            if (status != NO_ERROR)
            {
                if (status == ERROR_ACCESS_DENIED)
                    throw new UnauthorizedAccessException();
                if (status == ERROR_NOT_SUPPORTED)
                    throw new NotSupportedException("The IPv4 transport is not configured on the local computer.");
                
                throw new Win32Exception(status);
            }
        }
    }
}