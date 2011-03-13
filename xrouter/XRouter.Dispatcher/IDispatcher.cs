using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Management;

namespace XRouter.Dispatcher
{
    public interface IDispatcher : IXRouterComponent
    {
        void DispatchMessage(Message message);
    }
}
