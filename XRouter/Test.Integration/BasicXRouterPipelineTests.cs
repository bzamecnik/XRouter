using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Utils;
using XRouter.Test.Common;
using XRouter.Processor.MessageFlowBuilding;
using Xunit;
using System.Threading;

namespace XRouter.Test.Integration
{
    public class BasicXRouterPipelineTests
    {
        /// <summary>
        /// Path to original files - test configurations and data.
        /// </summary>
        private static readonly string OriginalsPath = @"..\..\Data\";
        /// <summary>
        /// Path to a directory managed by a directory adapter of a live XRouter service.
        /// </summary>
        private static readonly string WorkingPath = @"C:\XRouterTest\";

        public ConfigurationManager ConfigManager { get; set; }
        public BasicXRouterPipelineTests()
        {
            ConfigManager = new ConfigurationManager()
            {
                BasePath = OriginalsPath
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

        [Fact]
        public void ProvideInputFileAndCheckResults()
        {
            string source = Path.Combine(OriginalsPath, @"Test1\Input\RestaurantMenu_instance.xml");
            string dest = Path.Combine(WorkingPath, @"In\RestaurantMenu_instance.xml");
            // let the XRouter process the file
            File.Copy(source, dest);

            // TODO: find out how to correctly wait for XRouter until it
            // processes the provided messages
            Thread.Sleep(50);

            // verify that the file was processes well
            bool ok = XmlDirectoryComparer.Equals(
                Path.Combine(OriginalsPath, @"Test1\ExpectedOutput\OutA"),
                Path.Combine(WorkingPath, @"OutA"), true);
            Assert.True(ok);

            // tear down
            File.Delete(Path.Combine(WorkingPath, @"OutA\RestaurantMenu_instance.xml"));
        }
    }
}
