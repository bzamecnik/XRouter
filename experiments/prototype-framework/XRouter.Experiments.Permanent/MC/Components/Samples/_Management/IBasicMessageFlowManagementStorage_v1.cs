using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Experiments.Permanent.MC.Components.Common;
using XRouter.ComponentWatching;

namespace XRouter.Experiments.Permanent.MC.Components.Samples
{
    public interface IBasicMessageFlowManagementStorage_v1 : IWatchableComponent
    {
        void Store(IFlowContext_v1 context, string message);
    }
}
