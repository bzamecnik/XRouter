using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace XRouter.Remoting.RemoteCommunication
{
    public class ServerAddress
    {
        private static readonly int PortRangeStart = 10000;
        private static readonly int PortRangeEnd = 30000;

        public string Url { get; private set; }

        public int Port { get; private set; }

        public IPAddress IPAddress { get; private set; }

        private static Random rnd = new Random();

        private ServerAddress(string url, IPAddress ipAddress, int port)
        {
            Url = url;
            IPAddress = ipAddress;
            Port = port;            
        }

        public static ServerAddress GetLocalServerAddress()
        {
            int port = PortRangeStart + rnd.Next(PortRangeEnd - PortRangeStart);

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            string url = "127.0.0.1";
            try {
                string localHostName = Dns.GetHostName();
                ipAddress = ChooseIPAddress(Dns.GetHostAddresses(localHostName));
                url = ipAddress.ToString();
            } catch (Exception ex) {
            }
            url += ":" + port.ToString();

            ServerAddress result = new ServerAddress(url, ipAddress, port);
            return result;
        }

        private static IPAddress ChooseIPAddress(IEnumerable<IPAddress> addresses)
        {
            var result = addresses
                            .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                            .First(a => a.ToString().StartsWith("192.168."));
            return result;
        }

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

        public string Serialize()
        {
            string result = Url;
            return result;
        }
    }
}
