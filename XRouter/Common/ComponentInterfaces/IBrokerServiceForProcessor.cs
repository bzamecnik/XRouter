using System;
using XRouter.Common.Xrm;

namespace XRouter.Common.ComponentInterfaces
{
    /// <summary>
    /// API of a broker component to be used by a processor component.
    /// </summary>
    public interface IBrokerServiceForProcessor : IBrokerServiceForComponent
    {
        /// <summary>
        /// Updates the state of the message flow of a given token, ie.
        /// the state of its processing.
        /// </summary>
        /// <param name="updatingProcessorName">name of the processor which
        /// updates the token</param>
        /// <param name="targetToken">token to be updated</param>
        /// <param name="messageFlowState">new state of the token's message
        /// flow</param>
        /// <seealso cref="XRouter.Common.MessageFlowState"/>
        void UpdateTokenMessageFlowState(
            string updatingProcessorName,
            Token targetToken,
            MessageFlowState messageFlowState);

        /// <summary>
        /// Adds a message to the token.
        /// </summary>
        /// <remarks>
        /// NOTE: no message is ever removed from a token during its
        /// processing, messages can be only added.
        /// </remarks>
        /// <param name="updatingProcessorName">name of the processor which
        /// updates the token</param>
        /// <param name="targetToken">token to be updated</param>
        /// <param name="messageName">name of the message to be added</param>
        /// <param name="message">contents of the message to be added</param>
        void AddMessageToToken(
            string updatingProcessorName,
            Token targetToken,
            string messageName,
            SerializableXDocument message);

        /// <summary>
        /// Adds an exception to the token.
        /// </summary>
        /// <param name="updatingProcessorName">name of the processor which
        /// updates the token</param>
        /// <param name="targetToken">token to be updated</param>
        /// <param name="sourceNodeName">name of the message flow node where
        /// the exception was thrown</param>
        /// <param name="message"></param>
        /// <param name="stackTrace"></param>
        void AddExceptionToToken(
            string updatingProcessorName,
            Token targetToken,
            string sourceNodeName,
            string message,
            string stackTrace);

        /// <summary>
        /// Finishes the token processing. It changes only the token state, not
        /// the message flow. Also it might send a reply message back to the
        /// source gateway.
        /// </summary>
        /// <param name="updatingProcessorName">name of the processor which
        /// tries to finish the token</param>
        /// <param name="token">token to be finished
        /// </param>
        /// <param name="resultMessage">reply going back the the original
        /// gateway</param>
        void FinishToken(
            string updatingProcessorName,
            Token token,
            SerializableXDocument resultMessage);

        /// <summary>
        /// Sends an output message to a specified output endpoint -
        /// synchronously.
        /// </summary>
        /// <param name="address">output endpoint address</param>
        /// <param name="message">output message contents</param>
        /// <param name="metadata">output message metadata</param>
        /// <returns>optional reply message or null if there is no reply
        /// from the output endpoint</returns>
        SerializableXDocument SendMessage(
            EndpointAddress address,
            SerializableXDocument message,
            SerializableXDocument metadata);

        /// <summary>
        /// Obtains a XML resource identified its XRM URI from a XML resource
        /// storage.
        /// </summary>
        /// <param name="target">XRM URI of the requested resource</param>
        /// <returns>the requested resource XML resource; or null if none was
        /// found</returns>
        /// <seealso cref="XRouter.Common.Xrm.XmlResourceManager"/>
        SerializableXDocument GetXmlResource(XrmUri target);
    }
}
