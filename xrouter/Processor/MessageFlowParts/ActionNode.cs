using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common;
using XRouter.Common.Utils;
using System.Threading.Tasks;

namespace XRouter.Processor.MessageFlowParts
{
    class ActionNode : Node
    {
        private ActionNodeConfiguration Config { get; set; }

        private List<IActionPlugin> actions = new List<IActionPlugin>();

        public override void InitializeCore(NodeConfiguration configuration)
        {
            Config = (ActionNodeConfiguration)configuration;

            foreach (ActionConfiguration actionConfig in Config.Actions) {
                IActionPlugin action = TypeUtils.CreateTypeInstance<IActionPlugin>(actionConfig.PluginTypeFullName);
                action.Initialize(ProcessorService);
                actions.Add(action);
            }
        }

        public override string Evaluate(Token token)
        {
            Parallel.ForEach(actions, delegate(IActionPlugin action) {
                action.Evaluate(token);
            });

            return Config.NextNode.Name;
        }
    }
}
