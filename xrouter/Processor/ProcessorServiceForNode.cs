using System;
using System.Xml.Linq;
using XRouter.Broker;
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.Xrm;
using XRouter.Processor.MessageFlowParts;

namespace XRouter.Processor
{
    /// <summary>
    /// Wrapper for a processor specific to a concrete message flow node which
    /// simplifies the interface a bit and takes care of serialization.
    /// </summary>
    class ProcessorServiceForNode : IProcessorServiceForAction
    {
        /// <summary>
        /// Reference to the processor itself.
        /// </summary>
        public ProcessorService Processor { get; private set; }

        /// <summary>
        /// Identifier of the processor.
        /// </summary>
        public string ProcessorName { get { return Processor.Name; } }

        /// <summary>
        /// Configuration of the processor.
        /// </summary>
        public ApplicationConfiguration Configuration { get { return Processor.Configuration; } }

        /// <summary>
        /// Message flow node.
        /// </summary>
        public Node Node { get; private set; }

        private IBrokerServiceForProcessor BrokerService { get { return Processor.BrokerService; } }

        public ProcessorServiceForNode(ProcessorService processor, Node node)
        {
            Processor = processor;
            Node = node;
        }

        /// <summary>
        /// Creates a new message within the token.
        /// </summary>
        /// <param name="targetTokenGuid">identifier of the token to which to
        /// add the new message</param>
        /// <param name="messageName">name of the new message</param>
        /// <param name="message">message content</param>
        public void CreateMessage(Guid targetTokenGuid, string messageName, XDocument message)
        {
            BrokerService.AddMessageToToken(ProcessorName, targetTokenGuid, messageName, new SerializableXDocument(message));
        }

        /// <summary>
        /// Adds an exception to the token along with the name of the node
        /// where the exception was thrown.
        /// </summary>
        /// <param name="targetTokenGuid">identifier of the token to which to
        /// add the exception</param>
        /// <param name="ex">exception to be added to the token</param>
        public void AddExceptionToToken(Guid targetTokenGuid, Exception ex)
        {
            BrokerService.AddExceptionToToken(ProcessorName, targetTokenGuid, Node.Name, ex.Message, ex.StackTrace);
        }

        /// <summary>
        /// Sends an output message to specified output endpoint.
        /// </summary>
        /// <param name="target">output endpoint address</param>
        /// <param name="message">output message content</param>
        /// <param name="metadata">output message metadata; can be null</param>
        /// <returns></returns>
        public XDocument SendMessage(EndpointAddress target, XDocument message, XDocument metadata = null)
        {
            var result = BrokerService.SendMessage(target, new SerializableXDocument(message), new SerializableXDocument(metadata));
            return result.XDocument;
        }

        /// <summary>
        /// Obtains a XML resource identified its XRM URI from a XML resource
        /// storage.
        /// </summary>
        /// <param name="target">XRM URI of the requested resource</param>
        /// <returns>the requested resource XML resource; or null if none was
        /// found</returns>
        /// <seealso cref="XRouter.Common.Xrm.XmlResourceManager"/>
        public XDocument GetXmlResource(XrmUri target)
        {
            var result = BrokerService.GetXmlResource(target);
            return result.XDocument;
        }

        /// <summary>
        /// Persistently updates the state of the message flow of a given
        /// token, ie. the state of its processing.
        /// </summary>
        /// <param name="token">token to be updated</param>
        /// <seealso cref="XRouter.Common.MessageFlowState"/>
        public void MakeMessageFlowStatePersistent(Token token)
        {
            BrokerService.UpdateTokenMessageFlowState(ProcessorName, token.Guid, token.GetMessageFlowState());
        }

        /// <summary>
        /// Finishes the token processing. It changes only the token state, not
        /// the message flow. Also it might send a reply message back to the
        /// source gateway.
        /// </summary>
        /// <param name="token">token to be finished</param>
        /// <param name="resultMessage">reply message going back the the original
        /// gateway; can be null</param>
        public void FinishToken(Token token, XDocument resultMessage)
        {
            SerializableXDocument serializableResultMessage =  new SerializableXDocument(resultMessage);
            BrokerService.FinishToken(ProcessorName, token.Guid, serializableResultMessage);
        }
    }
}
