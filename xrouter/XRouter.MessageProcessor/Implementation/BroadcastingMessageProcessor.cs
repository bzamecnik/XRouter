using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Management;
using System.Xml.Linq;

namespace XRouter.MessageProcessor.Implementation
{
    public class BroadcastingMessageProcessor : IMessageProcessor
    {
        public string Name { get; private set; }

        private IXRouterManager XRouterManager { get; set; }

        public BroadcastingMessageProcessor(IXRouterManager xrouterManager, string name)
        {
            XRouterManager = xrouterManager;
            Name = name;
            XRouterManager.ConnectComponent<IMessageProcessor>(Name, this);
        }

        public void Initialize()
        {
            XElement configuration = XRouterManager.GetConfigData(string.Format("/xrouter/components/messageProcessor[@name=\"{0}\"]", Name)).XElement;
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
