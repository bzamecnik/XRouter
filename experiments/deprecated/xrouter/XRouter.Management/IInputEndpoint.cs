using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Remoting;

namespace XRouter.Management
{
    public delegate void ReceivedMessageHandler(Message message);

    public interface IInputEndpoint : IEndpoint
    {
        event ReceivedMessageHandler MessageReceived;
    }
}
