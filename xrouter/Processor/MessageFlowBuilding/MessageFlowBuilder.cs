using System.Xml.Linq;
using ObjectConfigurator;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Xrm;
using XRouter.Processor.BuiltInActions;

namespace XRouter.Processor.MessageFlowBuilding
{
    public static class MessageFlowBuilder
    {
        static MessageFlowBuilder() {
            Configurator.CustomItemTypes.Add(new TokenSelectionConfigurationItemType());
            Configurator.CustomItemTypes.Add(new XrmUriConfigurationItemType());
            Configurator.CustomItemTypes.Add(new UriConfigurationItemType());
        }

        public static TerminatorNodeConfiguration CreateTerminator(string name)
        {
            return new TerminatorNodeConfiguration() { Name = name, IsReturningOutput = false };
        }

        public static ActionNodeConfiguration CreateSender(string name, string inputMessage, string outputEndpoint, NodeConfiguration nextNode)
        {
            var configuration = XDocument.Parse(
    string.Format(
@"<objectConfig>
  <item name='targetEndpointName'>{0}</item>
  <item name='messageSelection'>token/messages/message[@name='{1}']/*[1]</item>
  <item name='targetGatewayName'>gateway1</item>
</objectConfig>", outputEndpoint, inputMessage));

            return new ActionNodeConfiguration() {
                Name = name, NextNode = nextNode, Actions = {
                    new ActionConfiguration("Message sender") {
                        Configuration = new SerializableXDocument(configuration)
                    }
                }
            };
        }

        public static ActionNodeConfiguration CreateTransformation(string name, string xslt, string inputMessage, string outputMessage, NodeConfiguration nextNode)
        {
            var configuration = XDocument.Parse(
                string.Format(
@"<objectConfig>
  <item name='xlstUri'>//item[@name='{0}']</item>
  <item name='inputMessageSelection'>token/messages/message[@name='{1}']/*[1]</item>
  <item name='outputMessageName'>{2}</item>
  <item name='isXsltTrusted'>false</item>
</objectConfig>", xslt, inputMessage, outputMessage));

            return new ActionNodeConfiguration() {
                Name = name, NextNode = nextNode, Actions = { 
                    new ActionConfiguration("Xslt transformer") { 
                        Configuration = new SerializableXDocument(configuration)
                    }
                }
            };
        }

        public static CbrNodeConfiguration CreateCbr(string name, string testedMessage, NodeConfiguration defaultTarget, params CbrCase[] cases)
        {
            var testedMessageSelection = new TokenSelection(
                string.Format(@"/token/messages/message[@name='{0}']/*[1]", testedMessage));
            var result = new CbrNodeConfiguration() {
                Name = name,
                TestedSelection = testedMessageSelection,
                DefaultTarget = defaultTarget
            };
            foreach (CbrCase cbrCase in cases) {
                result.Branches.Add(
                    new XrmUri(string.Format(@"//item[@name='{0}']", cbrCase.Schematron)),
                    cbrCase.TargetNode);
            }
            return result;
        }
    }
}
