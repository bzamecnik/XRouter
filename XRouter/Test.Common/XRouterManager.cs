using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using DaemonNT;
using XRouter.Common.ComponentInterfaces;
using wcf = System.ServiceModel;

namespace XRouter.Test.Common
{
    /// <summary>
    /// Start and stops an XRouter instance and provides access to it.
    /// </summary>
    /// <remarks>
    /// Usage: the service instance is started in debug mode in constructor
    /// and stopped in Dispose() method.</remarks>
    public class XRouterManager : IDisposable
    {
        private static readonly string DefaultDaemonNtConfigFile = @"..\..\..\ComponentHosting\Misc\DaemonNT_config.xml";

        /// <summary>
        /// DaemonNT configuration file containing the XRouter service.
        /// </summary>
        public string DaemonNtConfigFile { get; private set; }

        private static readonly string DefaultServiceName = "xrouter";

        /// <summary>
        /// XRouter service name as configured in the DaemonNT configuration
        /// file.
        /// </summary>
        public string ServiceName { get; private set; }

        /// <summary>
        /// Proxy to broker component which allows to communite with a running
        /// XRouter instance.
        /// </summary>
        public IBrokerServiceForManagement BrokerProxy { get; private set; }

        private ServiceCommands daemonNt;

        public XRouterManager()
            : this(DefaultDaemonNtConfigFile, DefaultServiceName)
        {
        }

        public XRouterManager(string daemonNtConfigFile, string serviceName)
        {
            ServiceName = serviceName;
            DaemonNtConfigFile = daemonNtConfigFile;
            daemonNt = new ServiceCommands() { ConfigFile = daemonNtConfigFile };
            Start();
            BrokerProxy = GetBrokerServiceProxy();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Stop();
        }

        #endregion

        private IBrokerServiceForManagement GetBrokerServiceProxy()
        {
            // NOTE: code taken from XRouter.Gui.ConfigurationManager
            wcf.EndpointAddress endpointAddress = new wcf.EndpointAddress("net.pipe://localhost/XRouter.ServiceForManagement");
            var binding = new wcf.NetNamedPipeBinding(wcf.NetNamedPipeSecurityMode.None) { MaxReceivedMessageSize = int.MaxValue };
            binding.ReaderQuotas = new XmlDictionaryReaderQuotas() { MaxBytesPerRead = int.MaxValue, MaxArrayLength = int.MaxValue, MaxStringContentLength = int.MaxValue };
            wcf.ChannelFactory<IBrokerServiceForManagement> channelFactory = new wcf.ChannelFactory<IBrokerServiceForManagement>(binding, endpointAddress);
            return channelFactory.CreateChannel();
        }

        private void Start()
        {
            Console.WriteLine("Starting XRouter service in debug mode.");
            // start the debug host in a new thread because it blocks while
            // reading from the console
            // TODO: this will go to the test class constructor
            Task.Factory.StartNew(() => daemonNt.DebugStart(ServiceName));

            // TODO: let the service start
            Thread.Sleep(1000);
        }

        private void Stop()
        {
            if (daemonNt != null)
            {
                // TODO: this will go to the test class Dispose()
                daemonNt.DebugStop(ServiceName);
                Console.WriteLine("XRouter service stopped.");
            }
        }
    }
}
