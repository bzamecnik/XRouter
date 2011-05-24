using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common.MessageFlowConfig;
using SchemaTron;
using System.Xml.Linq;
using XRouter.Common.Xrm;
using XRouter.Common;

namespace XRouter.Processor.MessageFlowParts
{
    class CbrNode : Node
    {
        private CbrNodeConfiguration Config { get; set; }

        private Dictionary<Validator, string/*targetNodeName*/> branches;

        public override void InitializeCore(NodeConfiguration configuration)
        {
            Config = (CbrNodeConfiguration)configuration;

            branches = new Dictionary<Validator, string>();
            foreach (var branche in Config.Branches) {
                XDocument xSchema = ProcessorService.GetXmlResource(branche.Key);
                Validator validator = Validator.Create(xSchema);
                branches.Add(validator, branche.Value.Name);
            }
        }

        public override string Evaluate(Token token)
        {
            XDocument testedDocument = Config.TestedSelection.GetSelectedDocument(token);

            #region Determine targetNodeName
            string targetNodeName = Config.DefaultTarget.Name;
            foreach (Validator validator in branches.Keys) {
                ValidatorResults results = validator.Validate(testedDocument, false);
                if (results.IsValid) {
                    targetNodeName = branches[validator];
                }
            }
            #endregion

            return targetNodeName;
        }
    }
}
