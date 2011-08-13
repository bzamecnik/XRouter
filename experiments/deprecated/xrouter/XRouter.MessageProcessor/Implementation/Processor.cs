using System.Threading.Tasks;
using System.Xml.Linq;
using XRouter.Management;

namespace XRouter.Processor.Implementation
{
    public class Processor : IProcessor
    {
        public string Name { get; private set; }

        private IXRouterManager XRouterManager { get; set; }

        private IProcessingProvider ProcessingProvider { get; set; }

        public Processor(IXRouterManager xrouterManager, string name)
        {
            Name = name;
            XRouterManager = xrouterManager;
            XRouterManager.ConnectComponent<IProcessor>(Name, this);
        }

        #region IXRouterComponent interface

        public void Initialize()
        {
            XElement configuration = XRouterManager.GetConfigData(string.Format("/xrouter/components/processor[@name=\"{0}\"]", Name)).XElement;

            ProcessingProvider = GetProcessingProvider(configuration);
        }

        private IProcessingProvider GetProcessingProvider(XElement configuration)
        {
            var processingProvider = configuration.Element("processingProvider");
            var processingProviderName = processingProvider.Attribute(XName.Get("name")).Value;
            return XRouterManager.GetComponent<IProcessingProvider>(processingProviderName);
        }

        #endregion

        #region IProcessor interface

        public void Process(Message message)
        {
            IProcessingProvider processingProvider = ProcessingProvider;
            // run the workers in a thread pool
            Task.Factory.StartNew(() => processingProvider.Process(message));
            // TODO: better create a shared queue of messages
        }

        #endregion
    }
}
