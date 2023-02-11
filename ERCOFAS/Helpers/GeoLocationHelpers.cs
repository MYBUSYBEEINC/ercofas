using System;
using System.Net;
using System.Net.Sockets;

namespace ERCOFAS.Helpers
{
    /// <summary>
    /// The helper class for geo location.
    /// </summary>
    public static class GeoLocationHelpers
    {
        /// <summary>
        /// Gets the client ip address.
        /// </summary>
        public static string GetClientIpAddress()
        {
            try
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    var endPoint = socket.LocalEndPoint as IPEndPoint;

                    return endPoint.Address.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}