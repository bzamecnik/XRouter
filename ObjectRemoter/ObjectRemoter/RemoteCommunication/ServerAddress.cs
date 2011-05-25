using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

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
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            // TODO: no test coverage
            Url = url;
            Port = url.Port;
            // TODO: the chosen IP address might not match with the one
            // provided in the URI
            IPAddress = ChooseIPAddress(url.Host);
        }

        private ServerAddress(Uri url, IPAddress ipAddress, int port)
        {
            Url = url;
            IPAddress = ipAddress;
            Port = port;
        }

        // TODO: rename to Uri or URI
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

        /// <summary>
        /// Indicates whether the server address corresponds with the locally
        /// available ObjectServer instance.
        /// </summary>
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
            int port = rnd.Next(PortRangeStart, PortRangeEnd);

            string localHostName = Dns.GetHostName();
            IPAddress ipAddress = ChooseIPAddress(localHostName);

            Uri url = new Uri(string.Format("tcp://{0}:{1}", ipAddress, port));
            ServerAddress result = new ServerAddress(url, ipAddress, port);
            return result;
        }

        /// <summary>
        /// Deserialize address object from string.
        /// </summary>
        /// <param name="serialized">Serialized address. Must not be null.</param>
        /// <returns>Deserialized object</returns>
        public static ServerAddress Deserialize(string serialized)
        {
            if (serialized == null)
            {
                throw new ArgumentNullException("serialized");
            }
            Uri url = new Uri(serialized);
            int port = url.Port;
            string host = url.Host;
            // TODO: the chosen IP address might not match with the one
            // provided in the URI
            IPAddress ipAddress = ChooseIPAddress(host);

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

        private static IPAddress ChooseIPAddress(string host)
        {
            if (Regex.Match(host, @"^(\d{1,3}\.){3}\d{1,3}$").Success)
            {
                // do not translate a host already containing an IP address
                return IPAddress.Parse(host);
            }
            else
            {
                return ChooseIPAddress(Dns.GetHostAddresses(host));
            }
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

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ServerAddress other = (ServerAddress)obj;
            return (Url == other.Url) && (Port == other.Port) &&
                (object.Equals(IPAddress, other.IPAddress));
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            throw new NotImplementedException();
            return base.GetHashCode();
        }
    }
}
