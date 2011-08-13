using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Remoting;

namespace XRouter.Management
{
    public delegate void MessageSendResultHandler(bool isSucessful);

    public interface IOutputEndpoint : IEndpoint
    {
        bool Send(Message message);

        void SendAsync(Message message, MessageSendResultHandler resultHandler);
    }
}
