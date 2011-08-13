using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Management;
using XRouter.Remoting;

namespace XRouter.Gateway.Implementation
{    
    class InputEndpoint : Endpoint, IInputEndpoint
    {
        public event ReceivedMessageHandler MessageReceived = delegate { };

        public InputEndpoint(EndpointAddress address)
            : base(address)
        {
        }

        internal void ReceiveMessage(Message message)
        {
            MessageReceived(message);
        }
    }
}
