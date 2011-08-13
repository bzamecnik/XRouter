using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using XRouter.Management;
using XRouter.Remoting;

namespace XRouter.Gateway.Implementation
{
    class OutputEndpoint : Endpoint, IOutputEndpoint
    {
        private Action<Message, MessageSendResultHandler> sendAsync;

        public OutputEndpoint(EndpointAddress address, Action<Message, MessageSendResultHandler> sendAsync)
            : base(address)
        {
            this.sendAsync = sendAsync;
        }

        public bool Send(Message message)
        {
            bool result = false;
            ManualResetEvent completedEvent = new ManualResetEvent(false);
            MessageSendResultHandler onCompleted = delegate(bool isSucessfull) {
                completedEvent.Set();
                result = isSucessfull;
            };
            sendAsync(message, onCompleted);
            completedEvent.WaitOne();
            return result;
        }

        public void SendAsync(Message message, MessageSendResultHandler resultHandler)
        {
            sendAsync(message, resultHandler);
        }
    }
}
