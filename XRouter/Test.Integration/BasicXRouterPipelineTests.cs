using System;
using System.Xml.Linq;
using System.Xml.XPath;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.MessageFlowConfig;
using XRouter.Processor.MessageFlowBuilding;
using Xunit;

namespace XRouter.Test.Integration
{
    public class BasicXRouterPipelineTests
    {
        /// <summary>
        /// Loads and prints the current XRouter configuration.
        /// </summary>
        [Fact]
        public void LoadCurrentConfiguration()
        {
            IBrokerServiceForManagement broker = ConfigurationManager.GetBrokerServiceProxy();
            var configuration = broker.GetConfiguration();
            Console.WriteLine(configuration.Content.XDocument.ToString());
        }

        /// <summary>
        /// Modifies the current XRouter configuration with a custom message
        /// flow and XML resource storage.
        /// </summary>
        [Fact]
        public void CreateSimpleMessageFlow()
        {
            #region Create message flow
            var terminator = MessageFlowBuilder.CreateTerminator("termination");
            var send = MessageFlowBuilder.CreateSender("sendToA",
                inputMessage: "input", outputEndpoint: "OutA", nextNode: terminator);
            var messageFlowConfig = new MessageFlowConfiguration("sendToA", 1)
            {
                Nodes = { send, terminator },
                RootNode = send
            };
            #endregion
            
            IBrokerServiceForManagement broker = ConfigurationManager.GetBrokerServiceProxy();
            // load current configuration
            var configuration = broker.GetConfiguration();
            var xConfig = configuration.Content.XDocument;

            // remove all message flows
            xConfig.XPathSelectElements("/configuration/messageflows/messageflow").Remove();
            
            // add current message flow
            configuration.AddMessageFlow(messageFlowConfig);
            configuration.SetCurrentMessageFlowGuid(messageFlowConfig.Guid);

            // remove all previous XRM items
            xConfig.XPathSelectElements("/configuration/xml-resource-storage/item").Remove();

            // add needed XRM items
            configuration.SaveXrmContent(XElement.Parse(@"
<xml-resource-storage>
    <item name='schematron1'>
        <xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:S='http://www.w3.org/2003/05/soap-envelope'>
            <xsl:template match='node()|@*'>
                <xsl:copy>
                    <xsl:apply-templates select='node()|@*' />
                </xsl:copy>
            </xsl:template>
            <xsl:template match='S:Header' />
        </xsl:stylesheet>
    </item>
</xml-resource-storage>
          "));

            // update the configuration

            broker.ChangeConfiguration(configuration);
        }
    }
}
