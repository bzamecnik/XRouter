using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Utils;
using XRouter.Processor.MessageFlowBuilding;
using Xunit;

namespace XRouter.Test.Integration
{
    // TODO: load message flow configs from files

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
            //#region Create message flow
            //var terminator = MessageFlowBuilder.CreateTerminator("termination");
            //var send = MessageFlowBuilder.CreateSender("sendToA",
            //    inputMessage: "input", outputEndpoint: "OutA", nextNode: terminator);
            //var messageFlowConfig = new MessageFlowConfiguration("sendToA", 1)
            //{
            //    Nodes = { send, terminator },
            //    RootNode = send
            //};
            //#endregion

            XElement xMessageFlow = ConfigurationManager.LoadMessageFlowFromFile("simple-flow", "Test1");
            MessageFlowConfiguration messageFlowConfig = XSerializer.Deserialize<MessageFlowConfiguration>(xMessageFlow);

            IBrokerServiceForManagement broker = ConfigurationManager.GetBrokerServiceProxy();
            // load current configuration
            var configuration = broker.GetConfiguration();
            var xConfig = configuration.Content.XDocument;

            // remove all message flows
            xConfig.XPathSelectElement("/configuration/messageflows").RemoveNodes();

            // add current message flow
            configuration.AddMessageFlow(messageFlowConfig);
            configuration.SetCurrentMessageFlowGuid(messageFlowConfig.Guid);

            // remove all previous XRM items
            xConfig.XPathSelectElement("/configuration/xml-resource-storage").RemoveNodes();

            // add needed XRM items
//            var xrm = XElement.Parse(@"
//<xml-resource-storage>
//    <item name='schematron1'>
//        <xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:S='http://www.w3.org/2003/05/soap-envelope'>
//            <xsl:template match='node()|@*'>
//                <xsl:copy>
//                    <xsl:apply-templates select='node()|@*' />
//                </xsl:copy>
//            </xsl:template>
//            <xsl:template match='S:Header' />
//        </xsl:stylesheet>
//    </item>
//</xml-resource-storage>
//          ");
            var xrm = ConfigurationManager.LoadXrmItems(
                new[] { "sample1a", "sample1b", "sample1c" }, "Test1");
            configuration.SaveXrmContent(xrm);

            // update the configuration

            broker.ChangeConfiguration(configuration);
        }
    }
}
