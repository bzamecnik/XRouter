using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Components.Core;
using XRouter.Experiments.Permanent.MC.Components.Common;

namespace XRouter.Experiments.Permanent.MC.Components.Samples
{
    public class FlowContext_v1 : IFlowContext_v1
    {
        public IMessageConsument EntrySource { get; private set; }
        public DateTime Created { get; private set; }

        public FlowContext_v1(IMessageConsument entrySource)
        {
            Created = DateTime.Now;
            EntrySource = entrySource;
        }
    }
}
