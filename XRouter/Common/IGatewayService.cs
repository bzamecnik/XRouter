using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common
{
    public interface IGatewayService : IComponentService
    {
        void ReceiveReturn(Guid tokenGuid, SerializableXDocument resultMessage);

        SerializableXDocument SendMessageToOutputEndPoint(EndpointAddress address, SerializableXDocument message);
    }
}
