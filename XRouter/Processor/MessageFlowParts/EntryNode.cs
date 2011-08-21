using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common;

namespace XRouter.Processor.MessageFlowParts
{
    class EntryNode : Node
    {
        private EntryNodeConfiguration Config { get; set; }

        /// <summary>
        /// Initializes an action node.
        /// </summary>
        /// <param name="configuration">EntryNodeConfiguration is expected
        /// </param>
        public override void InitializeCore(NodeConfiguration configuration)
        {
            Config = (EntryNodeConfiguration)configuration;
        }

        public override string Evaluate(Token token)
        {
            return Config.NextNode.Name;
        }
    }
}
