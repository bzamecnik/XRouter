using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Utils;
using XRouter.Processor.MessageFlowBuilding;
using XRouter.Test.Common;

namespace XRouter.Test.Integration
{
    /// <summary>
    /// Utilities for testing by hand. Each method is runnable separetely.
    /// </summary>
    class HandUtils : IDisposable
    {
        /// <summary>
        /// Path to original files - test configurations and data.
        /// </summary>
        private static readonly string OriginalsPath = @"..\..\Data\";
        /// <summary>
        /// Path to a directory managed by a directory adapter of a live XRouter service.
        /// </summary>
        private static readonly string WorkingPath = @"C:\XRouterTest\";

        private ConfigurationManager configManager;

        private XRouterManager xrouterManager;

        public HandUtils()
        {
            xrouterManager = new XRouterManager();
            //configManager = new ConfigurationManager(xrouterManager.ConsoleServerProxy)
            //{
            //    BasePath = OriginalsPath
            //};
        }

        /// <summary>
        /// Loads and prints the current XRouter configuration.
        /// </summary>
        //[Fact]
        public void LoadCurrentConfiguration()
        {
            var configuration = xrouterManager.ConsoleServerProxy.GetConfiguration();
            Console.WriteLine(configuration.Content.XDocument.ToString());
        }

        //[Fact]
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
            XElement xMessageFlow = configManager.LoadMessageFlowFromFile("simple-flow", "Test1");
            MessageFlowConfiguration messageFlow = XSerializer.Deserialize<MessageFlowConfiguration>(xMessageFlow);

            // load XRM items
            var xrm = configManager.LoadXrmItems(
                new[] { "sample1a", "sample1b", "sample1c" }, "Test1");

            configManager.ReplaceConfiguration(messageFlow, xrm);
        }

        //[Fact]
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
                Nodes = { sendToA, terminator, cbr }
            };
            messageFlow.GetEntryNode().NextNode = sendToA;
            #endregion

            var xrm = configManager.LoadXrmItems(
                new[] { "RestaurantMenu_schematron" }, "Test1");

            configManager.ReplaceConfiguration(messageFlow, xrm);
        }

        //[Fact]
        public void ValidateExampleDoc()
        {
            var xSch = XDocument.Load(Path.Combine(OriginalsPath, @"Common\Config\XRM\RestaurantMenu_schematron.xml"));
            var xIn = XDocument.Load(Path.Combine(OriginalsPath, @"Test1\Input\RestaurantMenu_instance.xml"));
            SchemaTron.Validator validator = SchemaTron.Validator.Create(xSch);
            SchemaTron.ValidatorResults results = validator.Validate(xIn, true);
            Console.WriteLine(results.IsValid ? "valid" : "invalid");
            Console.WriteLine(string.Join("\n", results.ViolatedAssertions.Select((info) => info.ToString())));
        }

        //[Fact]
        public void CreateFloodOfFiles()
        {
            int filesCount = 1000;
            int paddingZeros = 1 + (int)Math.Floor(Math.Log10(filesCount));
            string fileFormat = "input{0:" + new String('0', paddingZeros) + "}.xml";
            string basePath = Path.Combine(OriginalsPath, @"Test1\Input");
            TextGenerator gen = new TextGenerator()
            {
                MinMessageLength = 100,
                MaxMessageLength = 1000
            };
            for (int i = 0; i < filesCount; i++)
            {
                string fileName = string.Format(fileFormat, i);
                string filePath = Path.Combine(basePath, fileName);
                using (TextWriter writer = File.CreateText(filePath))
                {
                    writer.WriteLine("<message>");
                    writer.WriteLine("<a>");
                    writer.WriteLine(gen.GenerateMessage());
                    writer.WriteLine("</a>");
                    writer.WriteLine("</message>");
                }
            }
        }

        public void ConfigureWebServiceAdapter()
        {
            var httpServiceAdapter = new XRouter.Adapters.HttpServiceAdapter()
            {
                Uri = "http://localhost:8123/FloodConsumerWebService"
            };
            XDocument webServiceConfig = ObjectConfigurator.Configurator.SaveConfiguration(httpServiceAdapter);
            //            XDocument webServiceConfig = XDocument.Parse(
            //@"
            //<objectConfig>
            //  <item name='Uri'>localhost:8123</item>
            //  <item name='AreInputTokensPersistent'>False</item>
            //</objectConfig>
            //");
            var webService = new AdapterConfiguration(
                "floodConsumerWebService", "gateway", "httpServiceAdapter", webServiceConfig);
            XDocument config = new XDocument();
            config.Add(new XElement("config"));
            XSerializer.Serializer<AdapterConfiguration>(webService, config.Root);

            Console.WriteLine(config.ToString().Replace('"', '\''));
        }

        public void SaveConfig()
        {
            var dataAccess = new XRouter.Data.MsSqlDataAccess();
            dataAccess.Initialize("Server=192.168.10.1;Database=XRouter;User Id=XRouter_AccessDB;Password=XRouter;");
            string[] configLines = System.IO.File.ReadAllLines(@"Debug\config.xml");
            if (configLines[0].StartsWith("<?xml"))
            {
                configLines[0] = "";
            }
            string config = string.Join("\n", configLines);
            dataAccess.SaveConfiguration(config);
        }

        #region IDisposable Members

        public void Dispose()
        {
            xrouterManager.Dispose();
        }

        #endregion
    }
}
