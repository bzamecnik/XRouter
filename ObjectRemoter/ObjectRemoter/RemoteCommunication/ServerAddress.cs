using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ObjectRemoter.RemoteCommunication
{
    /// <summary>
    /// Contains information about communication server address.
    /// </summary>
    class ServerAddress
    {
        private static readonly int PortRangeStart = 10000;
        private static readonly int PortRangeEnd = 30000;

        /// <summary>
        /// Url of server (contains host name/IP and port)
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Port on which server is listening
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// IP address of server
        /// </summary>
        public IPAddress IPAddress { get; private set; }

        private static Random rnd = new Random();

        private ServerAddress(string url, IPAddress ipAddress, int port)
        {
            Url = url;
            IPAddress = ipAddress;
            Port = port;            
        }

        /// <summary>
        /// Generates a server address for local computer.
        /// </summary>
        /// <returns>Local server address</returns>
        public static ServerAddress GetLocalServerAddress()
        {
            int port = PortRangeStart + rnd.Next(PortRangeEnd - PortRangeStart);

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            string localHostName = Dns.GetHostName();
            ipAddress = ChooseIPAddress(Dns.GetHostAddresses(localHostName));

            string url = string.Format("{0}:{1}", ipAddress, port);
            ServerAddress result = new ServerAddress(url, ipAddress, port);
            return result;
        }

        private static IPAddress ChooseIPAddress(IEnumerable<IPAddress> addresses)
        {
            var result = addresses
                            .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                            .First(a => a.ToString().StartsWith("192.168."));   // currently restricted to LAN
            return result;
        }

        /// <summary>
        /// Deserialize address object from string.
        /// </summary>
        /// <param name="serialized">Serialized address</param>
        /// <returns>Deserialized object</returns>
        public static ServerAddress Deserialize(string serialized)
        { 
            string url = serialized;
            int colonPos = serialized.LastIndexOf(':');
            int port = int.Parse(serialized.Substring(colonPos+1));
            string host = url.Substring(0, colonPos);
            IPAddress ipAddress = ChooseIPAddress(Dns.GetHostAddresses(host));

            var result = new ServerAddress(serialized, ipAddress, port);
            return result;
        }

        /// <summary>
        /// Serialize this instance into string.
        /// </summary>
        /// <returns>Serialized address</returns>
        public string Serialize()
        {
            string result = Url;
            return result;
        }
    }
}
