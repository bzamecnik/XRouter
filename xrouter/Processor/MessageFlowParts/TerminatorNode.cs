using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common.MessageFlowConfig;
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
                resultMessage = Config.ResultMessageSelection.GetSelectedDocument(token);
            }

            ProcessorService.FinishToken(token, resultMessage);
            return null;
        }
    }
}
