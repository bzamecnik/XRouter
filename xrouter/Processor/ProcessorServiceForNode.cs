using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Common;
using XRouter.Broker;
using XRouter.Common.Xrm;
using XRouter.Common.ComponentInterfaces;
using XRouter.Processor.MessageFlowParts;

namespace XRouter.Processor
{
    class ProcessorServiceForNode : IProcessorServiceForAction
    {
        public ProcessorService Processor { get; private set; }

        public string ProcessorName { get { return Processor.Name; } }

        public ApplicationConfiguration Configuration { get { return Processor.Configuration; } }

        public Node Node { get; private set; }

        private IBrokerServiceForProcessor BrokerService { get { return Processor.BrokerService; } }

        public ProcessorServiceForNode(ProcessorService processor, Node node)
        {
            Processor = processor;
            Node = node;
        }

        public void CreateMessage(Guid targetTokenGuid, string messageName, XDocument message)
        {
            BrokerService.AddMessageToToken(ProcessorName, targetTokenGuid, messageName, new SerializableXDocument(message));
        }

        public void AddExceptionToToken(Guid targetTokenGuid, Exception ex)
        {
            BrokerService.AddExceptionToToken(ProcessorName, targetTokenGuid, Node.Name, ex.Message, ex.StackTrace);
        }

        public XDocument SendMessage(EndpointAddress target, XDocument message, XDocument metadata = null)
        {
            var result = BrokerService.SendMessage(target, new SerializableXDocument(message), new SerializableXDocument(metadata));
            return result.XDocument;
        }

        public XDocument GetXmlResource(XrmUri target)
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
