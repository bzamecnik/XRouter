using System;

namespace XRouter.Common.ComponentInterfaces
{
    /// <summary>
    /// API of a gateway component.
    /// </summary>
    public interface IGatewayService : IComponentService
    {
        void Start(string componentName, IBrokerServiceForGateway brokerService);
        void Stop();

        /// <summary>
        /// Returns a reply message to a token received by an adapter back to
        /// the original adapter after the token had been processed.
        /// </summary>
        /// <param name="tokenGuid">identifier of the token</param>
        /// <param name="resultMessage">reply message</param>
        /// <param name="sourceMetadata">metadata about the source of the token
        /// </param>
        void ReceiveReturn(Guid tokenGuid, SerializableXDocument resultMessage, SerializableXDocument sourceMetadata);

        /// <summary>
        /// Sends an output message to an output endpoint of an adapter with
        /// an optional reply.
        /// </summary>
        /// <param name="address">endpoint address</param>
        /// <param name="message">output message</param>
        /// <param name="metadata">message metadata</param>
        /// <returns>reply message if any; or null</returns>
        SerializableXDocument SendMessage(EndpointAddress address, SerializableXDocument message, SerializableXDocument metadata);
    }
}
