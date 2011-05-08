using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Experiments.Permanent.MC.Components.Common;
using XRouter.Components.Core;

namespace XRouter.Experiments.Permanent.MC.Components.Samples
{
    public class BroadcastingMessageFlowProcessor_v2 : MesageFlowProcessor_v1<IBasicMessageFlowManagement_v1>
    {
        public BroadcastingMessageFlowProcessor_v2(string name, int inputPoints, int outputPoints, IBasicMessageFlowManagement_v1 management)
            : base(name, inputPoints, outputPoints, management)
         {         
         }

        protected override bool HandleMessage(Message message, MesageFlowProcessor_v1<IBasicMessageFlowManagement_v1>.InputPoint source)
        {
            IFlowContext_v1 context = new FlowContext_v1(source);

            foreach (var outputPoint in OutputPoints) {
                outputPoint.Send(message);
                Management.Log(context, "Sending to " + outputPoint.Name);
            }
            return true;
        }
    }
}
