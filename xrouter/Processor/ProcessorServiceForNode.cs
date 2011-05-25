using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Common;
using XRouter.Broker;
using XRouter.Common.Xrm;
using XRouter.Common.ComponentInterfaces;

namespace XRouter.Processor
{
    class ProcessorServiceForNode : IProcessorServiceForAction
    {
        internal string ProcessorName { get; private set; }

        private IBrokerServiceForProcessor BrokerService { get; set; }

        public event Action ConfigurationChanged = delegate { };

        private ApplicationConfiguration _configuration;
        public ApplicationConfiguration Configuration {
            get { return _configuration; }
            internal set {
                _configuration = value;
                ConfigurationChanged();
            }
        }

        public ProcessorServiceForNode(string processorName, IBrokerServiceForProcessor brokerService, ApplicationConfiguration configuration)
        {
            ProcessorName = processorName;
            BrokerService = brokerService;
            Configuration = configuration;
        }

        public void CreateMessage(Guid targetTokenGuid, string messageName, XDocument message)
        {
            BrokerService.AddMessageToToken(ProcessorName, targetTokenGuid, messageName, new SerializableXDocument(message));
        }

        public XDocument SendMessage(EndpointAddress target, XDocument message, XDocument metadata = null)
        {
            var result = BrokerService.SendMessage(target, new SerializableXDocument(message), new SerializableXDocument(metadata));
            return result.XDocument;
        }

        public XDocument GetXmlResource(XrmTarget target)
        {
            var result = BrokerService.GetXmlResource(target);
            return result.XDocument;
        }

        public void MakeMessageFlowStatePersistent(Token token)
        {
            BrokerService.UpdateTokenMessageFlowState(ProcessorName, token.Guid, token.MessageFlowState);
        }

        public void FinishToken(Token token, XDocument resultMessage)
        {
            SerializableXDocument serializableResultMessage = null;
            if (resultMessage != null) {
                serializableResultMessage = new SerializableXDocument(resultMessage);
            }
            BrokerService.FinishToken(ProcessorName, token.Guid, serializableResultMessage);
        }
    }
}
