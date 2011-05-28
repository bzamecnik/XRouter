using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common.ComponentInterfaces
{
    public interface IGatewayService : IComponentService
    {
        void Start(string componentName, IBrokerServiceForGateway brokerService);
        void Stop();

        void ReceiveReturn(Guid tokenGuid, SerializableXDocument resultMessage, SerializableXDocument sourceMetadata);

        SerializableXDocument SendMessage(EndpointAddress address, SerializableXDocument message, SerializableXDocument metadata);
    }
}
