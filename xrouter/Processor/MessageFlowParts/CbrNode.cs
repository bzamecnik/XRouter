using System.Collections.Generic;
using System.Xml.Linq;
using SchemaTron;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Xrm;

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
    /// </remarks>
    /// <see cref="XRouter.Common.MessageFlowConfig.CbrNodeConfiguration"/>
    class CbrNode : Node
    {
        private CbrNodeConfiguration Config { get; set; }

        /// <summary>
        /// Each branch assigns a validator with a target node name.
        /// </summary>
        private Dictionary<Validator, string> branches;

        /// <summary>
        /// Initializes a CBR node.
        /// </summary>
        /// <param name="configuration">CbrNodeConfiguration is expected
        /// </param>
        public override void InitializeCore(NodeConfiguration configuration)
        {
            Config = (CbrNodeConfiguration)configuration;

            IInclusionResolver xrmInclusionResolver = new XrmInclusionResolver(ProcessorService);

            branches = new Dictionary<Validator, string>();
            foreach (var branch in Config.Branches)
            {
                XDocument xSchema = ProcessorService.GetXmlResource(branch.Key);
                Validator validator = Validator.Create(xSchema);
                ValidatorSettings validatorSettings = new ValidatorSettings
                {
                    InclusionsResolver = xrmInclusionResolver
                };
                branches.Add(validator, branch.Value.Name);
            }
        }

        public override string Evaluate(Token token)
        {
            TraceLog.Info("Entering CBR: " + Name);
            XDocument testedDocument = Config.TestedSelection.GetSelectedDocument(token);

            #region Determine targetNodeName
            string targetNodeName = Config.DefaultTarget.Name;
            foreach (Validator validator in branches.Keys)
            {
                ValidatorResults results = validator.Validate(testedDocument, false);
                if (results.IsValid)
                {
                    targetNodeName = branches[validator];
                    break;
                }
            }
            #endregion

            return targetNodeName;
        }

        /// <summary>
        /// Schematron inclusion resolver which obtains the referenced
        /// resources from the XML resource manager.
        /// </summary>
        private class XrmInclusionResolver : IInclusionResolver
        {
            private ProcessorServiceForNode processorService;

            public XrmInclusionResolver(ProcessorServiceForNode processorService)
            {
                this.processorService = processorService;
            }

            public XDocument Resolve(string href)
            {
                XrmUri xrmUri = new XrmUri(href);
                XDocument result = processorService.GetXmlResource(xrmUri);
                return result;
            }
        }
    }
}
