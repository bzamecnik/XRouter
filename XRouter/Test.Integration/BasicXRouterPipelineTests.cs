using System;
using System.Xml.Linq;
using System.Linq;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Utils;
using XRouter.Processor.MessageFlowBuilding;
using Xunit;

namespace XRouter.Test.Integration
{
    public class BasicXRouterPipelineTests
    {
        public ConfigurationManager ConfigManager { get; set; }
        public BasicXRouterPipelineTests()
        {
            ConfigManager = new ConfigurationManager()
            {
                BasePath = @"..\..\"
            };
        }

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

            // load message flow
            XElement xMessageFlow = ConfigManager.LoadMessageFlowFromFile("simple-flow", "Test1");
            MessageFlowConfiguration messageFlow = XSerializer.Deserialize<MessageFlowConfiguration>(xMessageFlow);

            // load XRM items
            var xrm = ConfigManager.LoadXrmItems(
                new[] { "sample1a", "sample1b", "sample1c" }, "Test1");

            ConfigurationManager.ReplaceConfiguration(messageFlow, xrm);
        }

        [Fact]
        public void LoadSingleCbr()
        {
            #region Create message flow
            var terminator = MessageFlowBuilder.CreateTerminator("termination");
            var sendToA = MessageFlowBuilder.CreateSender("sendToA",
                inputMessage: "input", outputEndpoint: "OutA", nextNode: terminator);
            var cbr = MessageFlowBuilder.CreateCbr(name: "restaurant", testedMessage: "input",
                defaultTarget: terminator, cases: new[] {
                    new CbrCase("RestaurantMenu_schematron", sendToA),
                });
            var messageFlow = new MessageFlowConfiguration("sendToA", 1)
            {
                Nodes = { sendToA, terminator, cbr },
                RootNode = sendToA
            };
            #endregion

            var xrm = ConfigManager.LoadXrmItems(
                new[] { "RestaurantMenu_schematron" }, "Test1");

            ConfigurationManager.ReplaceConfiguration(messageFlow, xrm);
        }

        [Fact]
        public void ValidateExampleDoc()
        {
            var xSch = XDocument.Load(@"Data\Common\Config\XRM\RestaurantMenu_schematron.xml");
            var xIn = XDocument.Load(@"Data\Test1\Input\RestaurantMenu_instance.xml");
            SchemaTron.Validator validator = SchemaTron.Validator.Create(xSch);
            SchemaTron.ValidatorResults results = validator.Validate(xIn, true);
            Console.WriteLine(results.IsValid ? "valid" : "invalid");
            Console.WriteLine(string.Join("\n", results.ViolatedAssertions.Select((info) => info.ToString())));
        }
    }
}
