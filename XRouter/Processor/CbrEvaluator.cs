using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchemaTron;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Xrm;
using System.Xml.Linq;

namespace XRouter.Processor
{
    /// <summary>
    /// Determines the next message flow node based on the content of the
    /// input message and CBR node configuration. In other words it determines
    /// where the message should be routed to.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each router branch corresponds to one message flow node where the
    /// processing should continue. If the input message does not branch to
    /// any branch the default branch is used.
    /// </para>
    /// <para>
    /// Internally the input message (XML document) is validated using
    /// Schematron schemas. Each schema corresponds to a single branch.
    /// The CBR configuration connects each schema with a target message flow
    /// node. Branches are ordered and processed in this order. The first
    /// matching schema determines the matching branch.
    /// </para>
    /// </remarks>
    /// <seealso cref="Xrouter.Processor.MessageFlowParts.CbrNode"/>
    public class CbrEvaluator
    {
        /// <summary>
        /// Each branch assigns a validator with a target node name.
        /// </summary>
        private Dictionary<Validator, string> branches;

        private CbrNodeConfiguration cbrConfig;

        public CbrEvaluator(CbrNodeConfiguration cbrConfig, Func<XrmUri, XDocument> xrmResourceProvider)
        {
            this.cbrConfig = cbrConfig;

            if (cbrConfig.DefaultTarget == null) {
                throw new InvalidOperationException("The default CBR target is not specified.");
            }

            IInclusionResolver xrmInclusionResolver = new XrmInclusionResolver(xrmResourceProvider);
            branches = new Dictionary<Validator, string>();
            foreach (var branch in cbrConfig.Branches) {
                if (branch.Value == null) {
                    throw new InvalidOperationException(string.Format(
                        "CBR branch '{0}' does not specify the target node.",
                        branch.Key));
                }

                XDocument xSchema = xrmResourceProvider(branch.Key);
                Validator validator = Validator.Create(xSchema);
                ValidatorSettings validatorSettings = new ValidatorSettings {
                    InclusionsResolver = xrmInclusionResolver
                };
                branches.Add(validator, branch.Value.Name);
            }
        }

        /// <summary>
        /// Determines the name of the next message flow node based on the
        /// content of the provided XML document and CBR node configuration.
        /// </summary>
        /// <param name="testedDocument">XML document which should be
        /// examined</param>
        /// <returns>determined next node name or the default node name;
        /// never null</returns>
        public string GetTargetNodeName(XDocument testedDocument)
        {
            string targetNodeName = cbrConfig.DefaultTarget.Name;
            foreach (Validator validator in branches.Keys)
            {
                ValidatorResults results = validator.Validate(testedDocument, false);
                if (results.IsValid)
                {
                    targetNodeName = branches[validator];
                    break;
                }
            }
            return targetNodeName;
        }

        /// <summary>
        /// Schematron inclusion resolver which obtains the referenced
        /// resources from the XML resource manager.
        /// </summary>
        private class XrmInclusionResolver : IInclusionResolver
        {
            private Func<XrmUri, XDocument> xrmResourceProvider;

            public XrmInclusionResolver(Func<XrmUri, XDocument> xrmResourceProvider)
            {
                this.xrmResourceProvider = xrmResourceProvider;
            }

            public XDocument Resolve(string href)
            {
                XrmUri xrmUri = new XrmUri(href);
                XDocument result = xrmResourceProvider(xrmUri);
                return result;
            }
        }
    }
}
