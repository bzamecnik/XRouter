using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;

namespace XRouter.Broker
{
    class GatewayAccessor : ComponentAccessor
    {
        public GatewayAccessor(string componentName, ApplicationConfiguration configuration)
            : base(componentName, configuration)
        {
        }

        public SerializableXDocument SendMessageToOutputEndPoint(EndpointAddress address, SerializableXDocument message)
        {
            IGatewayService gateway = GetComponent<IGatewayService>();
            SerializableXDocument result = gateway.SendMessageToOutputEndPoint(address, message);
            return result;
        }

        public void ReceiveReturn(Guid tokenGuid, SerializableXDocument resultMessage)
        {
            IGatewayService gateway = GetComponent<IGatewayService>();
            gateway.ReceiveReturn(tokenGuid, resultMessage);
        }
    }
}
