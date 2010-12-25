using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Experiments.Permanent.MC.Components.Common;
using XRouter.Components.Core;

namespace XRouter.Experiments.Permanent.MC.Components.Samples
{
    public class BroadcastingMessageFlowProcessor_v1 : MesageFlowProcessor_v1
    {
         public BroadcastingMessageFlowProcessor_v1(string name, int inputPoints, int outputPoints)
             : base(name, inputPoints, outputPoints)
         {         
         }

        protected override bool HandleMessage(Message message, MesageFlowProcessor_v1.InputPoint source)
        {
            foreach (var outputPoint in OutputPoints) {
                outputPoint.Send(message);
            }
            return true;
        }
    }
}
