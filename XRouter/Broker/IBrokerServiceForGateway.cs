using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;

namespace XRouter.Broker
{
    // methods to be called by a gateway
    public interface IBrokerServiceForGateway : IBrokerServiceForComponent
    {
        // - pass token from gateway to broker
        // - notify dispatcher
        void ReceiveToken(Token token);
    }
}
