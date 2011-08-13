using System.Xml.Linq;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;

namespace XRouter.Processor.MessageFlowParts
{
    /// <summary>
    /// Represents a node in the message flow in which processing of the token
    /// is finished.
    /// </summary>
    /// <remarks>
    /// The terminator node might return one of the messages in the token as
    /// an output message (if configured so).
    /// </remarks>
    /// <seealso cref="XRouter.Common.MessageFlowConfig.TerminatorNodeConfiguration"/>
    class TerminatorNode : Node
    {
        private TerminatorNodeConfiguration Config { get; set; }

        /// <summary>
        /// Initializes a terminator node.
        /// </summary>
        /// <param name="configuration">TerminatorNodeConfiguration is expected
        /// </param>
        public override void InitializeCore(NodeConfiguration configuration)
        {
            Config = (TerminatorNodeConfiguration)configuration;
        }

        public override string Evaluate(Token token)
        {
            TraceLog.Info("Entering terminator: " + Name);
            XDocument resultMessage = null;
            if (Config.IsReturningOutput)
            {
                resultMessage = Config.ResultMessageSelection.GetSelectedDocument(token);
            }

            ProcessorService.FinishToken(token, resultMessage);
            return null;
        }
    }
}
