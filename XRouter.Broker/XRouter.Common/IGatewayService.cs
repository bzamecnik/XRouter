using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common
{
    public interface IGatewayService : IComponentService
    {
        SerializableXDocument SendMessageToOutputEndPoint(EndPointAddress address, SerializableXDocument message);
    }
}
