using System.Xml.Linq;
using XRouter.Management;

namespace XRouter.Processor.Implementation
{
    public class Broadcaster : IProcessingProvider
    {
        public string Name { get; private set; }

        private IXRouterManager XRouterManager { get; set; }

        public Broadcaster(IXRouterManager xrouterManager, string name)
        {
            XRouterManager = xrouterManager;
            Name = name;
            XRouterManager.ConnectComponent<IProcessingProvider>(Name, this);
        }

        public void Initialize()
        {
            XElement configuration = XRouterManager.GetConfigData(string.Format("/xrouter/components/processingProvider[@name=\"{0}\"]", Name)).XElement;
        }

        public void Process(Message message)
        {
            var outputEndpoints = XRouterManager.GetOutputEndpoints();
            foreach (var outputEndpoint in outputEndpoints) {
                outputEndpoint.Send(message);
            }
        }
    }
}
