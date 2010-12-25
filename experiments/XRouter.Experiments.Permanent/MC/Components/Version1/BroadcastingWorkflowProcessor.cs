using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Experiments.Permanent.MC.Components.Common;
using XRouter.Components.Core;

namespace XRouter.Experiments.Permanent.MC.Components.Version1
{
    public class BroadcastingWorkflowProcessor : WorkflowProcessor
    {
         public BroadcastingWorkflowProcessor(string name, int inputPoints, int outputPoints)
             : base(name, inputPoints, outputPoints)
         {         
         }

        protected override bool HandleMessage(Message message, WorkflowProcessor.InputPoint source)
        {
            foreach (var outputPoint in OutputPoints) {
                outputPoint.Send(message);
            }
            return true;
        }
    }
}
