using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace XRouter.Management
{
    public delegate void SendingResultHandler(bool isSucessfull);

    public class OutputEndpoint : Endpoint
    {
        private Action<Message, SendingResultHandler> sendAsync;

        public OutputEndpoint(EndpointAddress address, Action<Message, SendingResultHandler> sendAsync)
            : base(address)
        {
            this.sendAsync = sendAsync;
        }

        public bool Send(Message message)
        {
            bool result = false;
            ManualResetEvent completedEvent = new ManualResetEvent(false);
            SendingResultHandler onCompleted = delegate(bool isSucessfull) {
                completedEvent.Set();
                result = isSucessfull;
            };
            sendAsync(message, onCompleted);
            completedEvent.WaitOne();
            return result;
        }

        public void SendAsync(Message message, SendingResultHandler resultHandler)
        {
            sendAsync(message, resultHandler);
        }
    }
}
