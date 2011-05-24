using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common.MessageFlow;
using XRouter.Common;
using System.Xml.Linq;

namespace XRouter.Processor.MessageFlowParts
{
    class TerminatorNode : Node
    {
        private TerminatorNodeConfiguration Config { get; set; }

        public override void InitializeCore(NodeConfiguration configuration)
        {
            Config = (TerminatorNodeConfiguration)configuration;
        }

        public override string Evaluate(Token token)
        {
            XDocument resultMessage = null;
            if (Config.IsReturningOutput) {
                resultMessage = Config.OutputMessageSelection.GetSelectedDocument(token);
            }

            ProcessorService.FinishToken(token, resultMessage);
            return null;
        }
    }
}
