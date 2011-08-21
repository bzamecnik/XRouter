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
                throw new InvalidOperationException("Default target is not specified.");
            }

            IInclusionResolver xrmInclusionResolver = new XrmInclusionResolver(xrmResourceProvider);
            branches = new Dictionary<Validator, string>();
            foreach (var branch in cbrConfig.Branches) {
                if (branch.Value == null) {
                    throw new InvalidOperationException("One or more branches has not specified target node.");
                }

                XDocument xSchema = xrmResourceProvider(branch.Key);
                Validator validator = Validator.Create(xSchema);
                ValidatorSettings validatorSettings = new ValidatorSettings {
                    InclusionsResolver = xrmInclusionResolver
                };
                branches.Add(validator, branch.Value.Name);
            }
        }

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
