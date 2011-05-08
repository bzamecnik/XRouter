using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Management;
using XRouter.Remoting;

namespace XRouter.Gateway
{
    public interface IEndpointsPluginService
    {
        IInputEndpoint CreateInputEndpoint(string endpointName, Action<ReceivedMessageHandler> registerListener);

        IOutputEndpoint CreateOutputEndpoint(string endpointName, Action<Message, MessageSendResultHandler> sendAsync);
    }
}
