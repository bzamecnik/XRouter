using System;
using System.IO;
using System.Threading;
using XRouter.Common.MessageFlowConfig;
using XRouter.Processor.MessageFlowBuilding;
using XRouter.Test.Common;
using Xunit;

namespace XRouter.Test.Integration
{
    public class BasicXRouterPipelineTests : IDisposable
    {
        /// <summary>
        /// Path to original files - test configurations and data.
        /// </summary>
        private static readonly string OriginalsPath = @"..\..\Data\";
        /// <summary>
        /// Path to a directory managed by a directory adapter of a live XRouter service.
        /// </summary>
        private static readonly string WorkingPath = @"C:\XRouterTest\";

        /// <summary>
        /// Specified whether the output file created during the tests should
        /// be deleted or left to the tester for further examination.
        /// </summary>
        public bool DeleteOutputFiles = true;

        private ConfigurationManager configManager;

        private XRouterManager xrouterManager;

        public BasicXRouterPipelineTests()
        {
            xrouterManager = new XRouterManager();
            configManager = new ConfigurationManager(xrouterManager.BrokerProxy) {
                BasePath = OriginalsPath
            };
        }

        [Fact]
        public void ProvideInputFileAndCheckResults()
        {
            LoadSingleCbrRestaurantMenu();

            string source = Path.Combine(OriginalsPath, @"Test1\Input\RestaurantMenu_instance.xml");
            string dest = Path.Combine(WorkingPath, @"In\RestaurantMenu_instance.xml");
            // let the XRouter process the file
            File.Copy(source, dest);

            WaitForProcessing();

            // verify that the file was processed well
            bool ok = XmlDirectoryComparer.Equals(
                Path.Combine(OriginalsPath, @"Test1\ExpectedOutput\OutA"),
                Path.Combine(WorkingPath, @"OutA"), true);
            Assert.True(ok);

            // tear down
            if (DeleteOutputFiles)
            {
                File.Delete(Path.Combine(WorkingPath, @"OutA\RestaurantMenu_instance.xml"));
            }
        }

        private static void WaitForProcessing()
        {
            // TODO: find out how to correctly wait for XRouter until it
            // processes the provided messages
            Thread.Sleep(1000);
        }

        public void LoadSingleCbrRestaurantMenu()
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

            var xrm = configManager.LoadXrmItems(
                new[] { "RestaurantMenu_schematron" }, "Test1");

            configManager.ReplaceConfiguration(messageFlow, xrm);
        }

        #region IDisposable Members

        public void Dispose()
        {
            xrouterManager.Dispose();
        }

        #endregion
    }
}
