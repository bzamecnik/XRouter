using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Xml;

namespace XRouter.Manager
{
    /// <summary>
    /// Provides a remote proxy to the ConsoleServer of an XRouter Manager instance.
    /// </summary>
    public class ConsoleServerProxyProvider
    {
        /// <summary>
        /// Creates a remote proxy to the ConsoleServer web-service running at
        /// the specified URI.
        /// </summary>
        /// <remarks>
        /// NOTE: Whether it is possible to connect to the ConsoleServer or
        /// not can be figured out only at the first usage of the proxy, not
        /// as soon as it is created here.
        /// </remarks>
        /// <param name="consoleServerUri">URI of the ConsoleServer</param>
        /// <returns>a remote proxy to the ConsoleServer; not null</returns>
        public static IConsoleServer GetConsoleServerProxy(string consoleServerUri)
        {
            EndpointAddress endpointAddress = new EndpointAddress(consoleServerUri);

            // set binding (WebService - SOAP/HTTP)
            WSHttpBinding binding = new WSHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas = new XmlDictionaryReaderQuotas() {
                MaxBytesPerRead = int.MaxValue,
                MaxArrayLength = int.MaxValue, MaxStringContentLength = int.MaxValue
            };

            ChannelFactory<IConsoleServer> channelFactory = new ChannelFactory<IConsoleServer>(binding, endpointAddress);
            IConsoleServer result = channelFactory.CreateChannel();
            return result;
        }
    }
}
