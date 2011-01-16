using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Management;

namespace XRouter.MessageProcessor
{
    public interface IMessageProcessor : IXRouterComponent
    {
        void Process(Message message);
    }
}
