using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Xml;

namespace XRouter.Manager
{
    public class ConsoleServerProxyProvider
    {
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
