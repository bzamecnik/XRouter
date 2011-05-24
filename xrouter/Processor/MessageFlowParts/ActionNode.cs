using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common;

namespace XRouter.Processor.MessageFlowParts
{
    class ActionNode : Node
    {
        private ActionNodeConfiguration Config { get; set; }
        
        public override void InitializeCore(NodeConfiguration configuration)
        {
            Config = (ActionNodeConfiguration)configuration;
        }

        public override string Evaluate(Token token)
        {
            return Config.NextNode.Name;
        }
    }
}
