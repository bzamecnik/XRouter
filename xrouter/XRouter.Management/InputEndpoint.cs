using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Management
{
    public delegate void MessageReceiveHandler(Message message);

    public class InputEndpoint : Endpoint
    {
        public event MessageReceiveHandler MessageReceived = delegate { };

        public InputEndpoint(EndpointAddress address, Action<MessageReceiveHandler> registerListener)
            : base(address)
        {
            registerListener(ReceiveMessage);
        }

        private void ReceiveMessage(Message message)
        {
            MessageReceived(message);
        }
    }
}
