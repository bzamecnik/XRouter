using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common.MessageFlowConfig
{
    [Serializable]
    public class ActionNodeConfiguration : NodeConfiguration
    {
        public IList<ActionConfiguration> Actions { get; private set; }

        public NodeConfiguration NextNode { get; set; }

        public ActionNodeConfiguration()
        {
            Actions = new List<ActionConfiguration>();
        }
    }
}
