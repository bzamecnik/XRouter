using System.Collections.Generic;
using System.Xml.Linq;
using SchemaTron;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Xrm;
using System;

namespace XRouter.Processor.MessageFlowParts
{
    /// <summary>
    /// Represents a node in the message flow which performs content-based
    /// routing. It decides to which one of several possible nodes to go
    /// based on the contents of the token. SchemaTron is used as a validator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A CBR node has one input edge and several possible output edges
    /// (it is a kind of switch). Each output edge (branch) corresponds to a
    /// single validator. As evaluation of the token CBR validates a selected
    /// document from the token with the validators in a sequence. The first
    /// validator with positive result determines the next node.
    /// </para>
    /// <para>
    /// Resources referenced from Schematron schemas in validator to be
    /// included are searched for and obtained from the XRM.
    /// </para>
    /// <para>
    /// In case the validated document is not valid under none of the
    /// validator the flow can continue to a node configured as default.
    /// </para>
    /// </remarks>
    /// <seealso cref="XRouter.Common.MessageFlowConfig.CbrNodeConfiguration"/>
    /// <seealso cref="Xrouter.Processor.CbrEvaluator"/>
    class CbrNode : Node
    {
        private CbrNodeConfiguration Config { get; set; }

        private CbrEvaluator evaluator;

        /// <summary>
        /// Initializes a CBR node.
        /// </summary>
        /// <param name="configuration">CbrNodeConfiguration is expected
        /// </param>
        public override void InitializeCore(NodeConfiguration configuration)
        {
            Config = (CbrNodeConfiguration)configuration;

            evaluator = new CbrEvaluator(Config, ProcessorService.GetXmlResource);
        }

        public override string Evaluate(Token token)
        {
            TraceLog.Info("Entering CBR: " + Name);
            XDocument testedDocument = Config.TestedSelection.GetSelectedDocument(token);
            string targetNodeName = evaluator.GetTargetNodeName(testedDocument);
            return targetNodeName;
        }
    }
}
