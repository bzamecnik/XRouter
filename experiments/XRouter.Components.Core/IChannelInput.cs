using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Components.Core
{
    interface IChannelInput : IMessagingComponent
    {
        void Send(Message message);
    }
}
