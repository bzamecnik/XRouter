using System;

namespace XRouter.Common.ComponentInterfaces
{
    /// <summary>
    /// API of a gateway component.
    /// </summary>
    public interface IGatewayService : IComponentService
    {
        /// <summary>
        /// Starts the gateway component instance.
        /// </summary>
        /// <remarks>This method can be called only when the gateway instance
        /// is not running. It returns as soon as the gateway is started.
        /// </remarks>
        /// <param name="componentName">identifier of the gateway</param>
        /// <param name="brokerService">reference to the broker component
        /// </param>
        void Start(string componentName, IBrokerServiceForGateway brokerService);

        /// <summary>
        /// Stops a running gateway component.
        /// </summary>
        void Stop();

        // TODO: can be optionally renamed to ReceiveReply() for consistence with docs

        /// <summary>
        /// Returns a message which is a reply to a token received by an
        /// adapter back to the original adapter after the token has been
        /// processed.
        /// </summary>
        /// <param name="tokenGuid">identifier of the token</param>
        /// <param name="resultMessage">reply message</param>
        /// <param name="sourceMetadata">metadata about the source of the token
        /// </param>
        void ReceiveReturn(
            Guid tokenGuid,
            SerializableXDocument resultMessage,
            SerializableXDocument sourceMetadata);

        /// <summary>
        /// Sends an output message to an output endpoint of an adapter with
        /// an optional reply.
        /// </summary>
        /// <param name="address">endpoint address</param>
        /// <param name="message">output message</param>
        /// <param name="metadata">message metadata</param>
        /// <returns>reply message if any; or null</returns>
        SerializableXDocument SendMessage(
            EndpointAddress address,
            SerializableXDocument message,
            SerializableXDocument metadata);
    }
}
