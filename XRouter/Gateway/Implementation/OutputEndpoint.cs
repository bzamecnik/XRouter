//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using XRouter.Management;
//using XRouter.Remoting;

//namespace XRouter.Gateway.Implementation
//{
//    class OutputEndpoint : Endpoint, IOutputEndpoint
//    {
//        private Action<Token, MessageSendResultHandler> sendAsync;

//        public OutputEndpoint(EndpointAddress address, Action<Token, MessageSendResultHandler> sendAsync)
//            : base(address)
//        {
//            this.sendAsync = sendAsync;
//        }

//        public bool Send(Token message)
//        {
//            bool result = false;
//            ManualResetEvent completedEvent = new ManualResetEvent(false);
//            MessageSendResultHandler onCompleted = delegate(bool isSucessfull) {
//                result = isSucessfull;
//                completedEvent.Set();				
//            };
//            sendAsync(message, onCompleted);
//            completedEvent.WaitOne();
//            return result;
//        }

//        public void SendAsync(Token message, MessageSendResultHandler resultHandler)
//        {
//            sendAsync(message, resultHandler);
//        }
//    }
//}
