using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectRemoter;
using XRouter.Common;

namespace XRouter.Broker
{
    // methods to be called by a gateway
    public interface IBrokerServiceForGateway : IRemotelyReferable
    {
        // - pass token from gateway to broker
        // - notify dispatcher
        void ReceiveToken(Token token);
    }
}
