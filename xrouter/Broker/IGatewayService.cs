using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;

namespace XRouter.Broker
{
    public interface IGatewayService : IComponentService
    {
        void Start(string componentName, IBrokerServiceForGateway brokerService);
        void Stop();

        void ReceiveReturn(Guid tokenGuid, SerializableXDocument resultMessage);

        SerializableXDocument SendMessageToOutputEndPoint(EndpointAddress address, SerializableXDocument message);
    }
}
