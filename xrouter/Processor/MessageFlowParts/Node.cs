using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common;

namespace XRouter.Processor.MessageFlowParts
{
    abstract class Node
    {
        public string Name { get; private set; }

        protected ProcessorServiceForNode ProcessorService { get; private set; }

        public void Initialize(NodeConfiguration configuration, ProcessorService processor)
        {
            Name = configuration.Name;
            ProcessorService = new ProcessorServiceForNode(processor, this);
            InitializeCore(configuration);
        }

        public abstract void InitializeCore(NodeConfiguration configuration);

        // returns next node name or null if terminated
        public abstract string Evaluate(Token token);
    }
}
