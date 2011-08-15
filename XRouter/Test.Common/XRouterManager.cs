using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using DaemonNT;
using XRouter.Common.ComponentInterfaces;
using wcf = System.ServiceModel;
using XRouter.Manager;

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
        /// <summary>
        /// Proxy to broker component which allows to communite with a running
        /// XRouter instance.
        /// </summary>
        public IConsoleServer ConsoleServerProxy { get; private set; }

        private static readonly string DefaultDaemonNtConfigFile = @"DaemonNT.xml";

        private static readonly string DefaultXRouterServiceName = "xrouter";
        private static readonly string DefaultXRouterManagerServiceName = "xroutermanager";

        private IServiceRunner xRouterRunner;
        private IServiceRunner xRouterManagerRunner;

        public XRouterManager()
            : this(DefaultDaemonNtConfigFile, DefaultXRouterServiceName, DefaultXRouterManagerServiceName)
        {
        }

        public XRouterManager(string daemonNtConfigFile, string xRouterServiceName, string xRouterManagerServiceName)
        {
            xRouterRunner = new ProcessRunner(daemonNtConfigFile, xRouterServiceName);
            xRouterManagerRunner = new ProcessRunner(daemonNtConfigFile, xRouterManagerServiceName);
            xRouterManagerRunner.Start();
            ConsoleServerProxy = GetConsoleServerProxy();
        }

        #region IDisposable Members

        public void Dispose()
        {
            xRouterManagerRunner.Stop();
        }

        #endregion

        private IConsoleServer GetConsoleServerProxy()
        {
            // NOTE: code taken from XRouter.Gui.ConfigurationManager
            wcf.EndpointAddress endpointAddress = new wcf.EndpointAddress("http://localhost:9090/XRouter.ConsoleService/ConsoleServer");
            // set binding (WebService - SOAP/HTTP)
            wcf.WSHttpBinding binding = new wcf.WSHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas = new XmlDictionaryReaderQuotas() {
                MaxBytesPerRead = int.MaxValue,
                MaxArrayLength = int.MaxValue, MaxStringContentLength = int.MaxValue
            };
            wcf.ChannelFactory<IConsoleServer> channelFactory = new wcf.ChannelFactory<IConsoleServer>(binding, endpointAddress);
            return channelFactory.CreateChannel();
        }

        public void StartXRouter()
        {
            Console.WriteLine("Starting XRouter service in debug mode.");
            xRouterRunner.Start();
        }

        public void StopXRouter()
        {
            xRouterRunner.Stop();
            Console.WriteLine("XRouter service stopped.");
        }
    }
}
