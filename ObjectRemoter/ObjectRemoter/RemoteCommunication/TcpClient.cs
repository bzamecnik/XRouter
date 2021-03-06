﻿using System.IO;

namespace ObjectRemoter.RemoteCommunication
{
    /// <summary>
    /// A remote communication client using TCP.
    /// </summary>
    internal class TcpClient : IClient
    {
        /// <summary>
        /// Address of server to connect to.
        /// </summary>
        public ServerAddress ServerAddress { get; private set; }

        public TcpClient(ServerAddress serverAddress)
        {
            ServerAddress = serverAddress;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// SocketException is resolved at higher level in the
        /// RemoteObjectProxyProvider.ProxyInterceptor.Intercept() method.
        /// </remarks>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// When it is not possible to connect to the server.
        /// </exception>
        public string Request(string command, string[] data)
        {
            var client = new System.Net.Sockets.TcpClient(ServerAddress.IPAddress.ToString(), ServerAddress.Port);
            using (var clientStream = client.GetStream())
            {
                var sw = new StreamWriter(clientStream);

                command = command
                    .Replace("\r", TcpServer.CarriageReturnReplacement)
                    .Replace("\n", TcpServer.LineFeedReplacement);
                sw.WriteLine(command);

                sw.WriteLine(data.Length.ToString());
                foreach (string item in data)
                {
                    string encodedItem = item
                        .Replace("\r", TcpServer.CarriageReturnReplacement)
                        .Replace("\n", TcpServer.LineFeedReplacement);
                    sw.WriteLine(encodedItem);
                }

                sw.Flush();

                var sr = new StreamReader(clientStream);
                string result = sr.ReadLine();
                if (result != null)
                {
                    result = result
                        .Replace(TcpServer.CarriageReturnReplacement, "\r")
                        .Replace(TcpServer.LineFeedReplacement, "\n");
                }
                return result;
            }
        }
    }
}
