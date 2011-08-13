using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Components.Core
{
    public interface IMessageConsument : IMessagingComponent
    {
        void Send(Message message);
    }
}
