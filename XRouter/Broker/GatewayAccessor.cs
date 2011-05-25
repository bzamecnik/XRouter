using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;

namespace XRouter.Broker
{
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
            SerializableXDocument result = gateway.SendMessageToOutputEndPoint(address, message, metadata);
            return result;
        }

        public void ReceiveReturn(Guid tokenGuid, SerializableXDocument resultMessage, SerializableXDocument sourceMetadata)
        {
            gateway.ReceiveReturn(tokenGuid, resultMessage, sourceMetadata);
        }
    }
}
