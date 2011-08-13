using System;
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;

namespace XRouter.Broker
{
    /// <summary>
    /// A proxy for accessing a gateway. It does not add any extra behavior.
    /// </summary>
    /// <seealso cref="XRouter.Common.ComponentInterfaces.IGatewayService"/>
    class GatewayAccessor : ComponentAccessor
    {
        private IGatewayService gateway;

        public GatewayAccessor(string componentName, IGatewayService gateway, ApplicationConfiguration configuration)
            : base(componentName, gateway, configuration)
        {
            this.gateway = gateway;
        }

        public SerializableXDocument SendMessageToOutputEndPoint(EndpointAddress address, SerializableXDocument message, SerializableXDocument metadata)
        {
            SerializableXDocument result = gateway.SendMessage(address, message, metadata);
            return result;
        }

        public void ReceiveReturn(Guid tokenGuid, SerializableXDocument resultMessage, SerializableXDocument sourceMetadata)
        {
            gateway.ReceiveReturn(tokenGuid, resultMessage, sourceMetadata);
        }
    }
}
