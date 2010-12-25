using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Experiments.Permanent.MC.Components.Common;
using XRouter.ComponentWatching;

namespace XRouter.Experiments.Permanent.MC.Components.Samples
{
    public interface IBasicMessageFlowManagement_v1 : IMessageFlowManagement_v1, IWatchableComponent
    {
        void Log(IFlowContext_v1 context, string message);
    }
}
