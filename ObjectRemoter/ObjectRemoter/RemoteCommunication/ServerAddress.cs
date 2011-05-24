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
    internal class ServerAddress
    {
        private static readonly int PortRangeStart = 10000;
        private static readonly int PortRangeEnd = 30000;

        private static Random rnd = new Random();

        internal ServerAddress(Uri url)
        {
            // TODO: no test coverage
            Url = url;
            Port = url.Port;
            IPAddress = ChooseIPAddress(Dns.GetHostAddresses(url.Host));
        }

        private ServerAddress(Uri url, IPAddress ipAddress, int port)
        {
            Url = url;
            IPAddress = ipAddress;
            Port = port;
        }

        /// <summary>
        /// Url of server (contains host name/IP and port)
        /// </summary>
        public Uri Url { get; private set; }

        /// <summary>
        /// Port on which server is listening
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// IP address of server
        /// </summary>
        public IPAddress IPAddress { get; private set; }

        internal bool IsLocal
        {
            get
            {
                // TODO: no test coverage
                return Url.IsLoopback && (Port == ObjectServer.ServerAddress.Port);
            }
        }

        /// <summary>
        /// Generates a server address for local computer.
        /// </summary>
        /// <returns>Local server address</returns>
        public static ServerAddress GetLocalServerAddress()
        {
            int port = PortRangeStart + rnd.Next(PortRangeEnd - PortRangeStart);

            string localHostName = Dns.GetHostName();
            IPAddress ipAddress = ChooseIPAddress(Dns.GetHostAddresses(localHostName));

            Uri url = new Uri(string.Format("tcp://{0}:{1}", ipAddress, port));
            ServerAddress result = new ServerAddress(url, ipAddress, port);
            return result;
        }

        /// <summary>
        /// Deserialize address object from string.
        /// </summary>
        /// <param name="serialized">Serialized address</param>
        /// <returns>Deserialized object</returns>
        public static ServerAddress Deserialize(string serialized)
        {
            Uri url = new Uri(serialized);
            int port = url.Port;
            string host = url.Host;
            IPAddress ipAddress = ChooseIPAddress(Dns.GetHostAddresses(host));

            var result = new ServerAddress(url, ipAddress, port);
            return result;
        }

        /// <summary>
        /// Serialize this instance into string.
        /// </summary>
        /// <returns>Serialized address</returns>
        public string Serialize()
        {
            string result = Url.ToString();
            return result;
        }

        private static IPAddress ChooseIPAddress(IEnumerable<IPAddress> addresses)
        {
            // TODO:
            // - choosing IP addresses is currently restricted to private addresses
            // - it should be more general and parametrized
            var result = addresses
                            .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                            .FirstOrDefault(a => a.ToString().StartsWith("192.168."));

            // TODO:
            // - First() throws InvalidOperationException "Sequence contains
            //   no matching element"  if there is no such an address
            // - this situation should be handled better than using the
            //   loopback address
            if (result == null)
            {
                // TODO: no test coverage
                result = IPAddress.Parse("127.0.0.1");
            }
            return result;
        }
    }
}
