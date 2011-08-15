using System;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;
using XRouter.Processor.MessageFlowBuilding;
using XRouter.Test.Common;
using Xunit;

namespace XRouter.Test.Integration
{
    // TODO: as changing configuration at run-time was disabled it is needed
    // to run change the code to start/stop XRouter for EACH test method.
    // It is needed to provide configuration before running XRouter.

    public class BasicXRouterPipelineTests : IDisposable
    {
        /// <summary>
        /// Path to original files - test configurations and data.
        /// </summary>
        private static readonly string OriginalsPath = @"..\XRouter\Test.Integration\Data\";
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
            configManager = new ConfigurationManager(xrouterManager.ConsoleServerProxy) {
                BasePath = OriginalsPath
            };
        }

        [Fact]
        public void ProvideInputFileAndCheckResults()
        {
            LoadSingleCbrRestaurantMenu();

            xrouterManager.StartXRouter();

            try
            {
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
            finally
            {
                xrouterManager.StopXRouter();
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

            var adapters = new AdapterConfiguration[] {
                new AdapterConfiguration("directoryAdapter", "directoryAdapter",
                    XDocument.Parse(
@"<objectConfig>
  <item name='checkingIntervalInSeconds'>0.1</item>
  <item name='inputEndpointToPathMap'>
    <pair>
      <key>In</key>
      <value>C:\XRouterTest\In</value>
    </pair>
  </item>
  <item name='outputEndpointToPathMap'>
    <pair>
      <key>OutB</key>
      <value>C:\XRouterTest\OutB</value>
    </pair>
    <pair>
      <key>OutA</key>
      <value>C:\XRouterTest\OutA</value>
    </pair>
    <pair>
      <key>OutC</key>
      <value>C:\XRouterTest\OutC</value>
    </pair>
  </item>
</objectConfig>"))
            };

            configManager.ReplaceConfiguration(messageFlow, xrm, adapters);
        }

        #region IDisposable Members

        public void Dispose()
        {
            xrouterManager.Dispose();
        }

        #endregion
    }
}
