using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SchemaTron;

namespace XRouter.Prototype.Processor
{
    class CBR : INodeFunction
    {
        private String[] tests = null;

        private Int32[] steps = null;

        private Validator[] validators = null;

        private String name = null;

        public CBR(String name, String[] tests, Int32[] steps)
        {
            this.name = name;
            this.tests = tests;
            this.steps = steps;
        }

        public void Initialize()
        {
            Logger.LogInfo("Init CBR." + name);

            // vytvoří validátory pro kešovací scénář
            this.validators = new Validator[this.tests.Length - 1];
            for (Int32 i = 0; i < this.tests.Length - 1; i++)
            {
                XDocument xSchema = XDocument.Load(tests[i], LoadOptions.SetLineInfo);
                this.validators[i] = Validator.Create(xSchema);
            }

            Logger.LogInfo("Init CBR." + name + " done!");
        }

        public void Evaluate(Token token)
        {
            Logger.LogInfo("CBR." + name);

            for (Int32 i = 0; i < this.validators.Length; i++)
            {
                Validator validator = validators[i];
                ValidatorResults results = validator.Validate(token.Content, false);
                if (results.IsValid)
                {
                    token.Step = this.steps[i];
                    Logger.LogInfo("Evaluate CBR." + name + " done!");
                    return;
                }
                else
                {
                    AssertionInfo info = results.ViolatedAssertions.ElementAt(0);
                    Logger.LogInfo(String.Format("LineNumber={0} LinePosition={1} Location={2} UserMessage={3}.", info.LineNumber, info.LinePosition, info.Location, info.UserMessage));
                }
            }

            // pouzije default hranu
            token.Step = this.steps[this.steps.Length - 1];

            Logger.LogInfo("CBR." + name + " done!");
        }
    }
}
