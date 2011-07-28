using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Utils;
using XRouter.Processor.MessageFlowBuilding;
using XRouter.Test.Common;
using Xunit;
using System.Reflection;

namespace XRouter.Test.Integration
{
    class TestScenarios
    {
        public TestScenarios()
        {
            //TODO is XRouter running? ... or launch it?
        }

        private readonly string testScenarioDirectory = @"C:\XRouterTest\";
        private readonly string dataDirectory = @"..\..\Data\";

        private void prepareTest(string scenarioName)
        {
            string pathFrom = testScenarioDirectory + scenarioName;
            string pathTo = dataDirectory + scenarioName;

            //delete old files if they are there
            if (Directory.Exists(pathTo))
            {
                Directory.Delete(pathTo, true);
            }

            Directory.CreateDirectory(pathTo);
            string[] filePathsFrom = Directory.GetFiles(pathFrom, "*.*", SearchOption.AllDirectories);
            foreach (string filePathFrom in filePathsFrom)
            {
                string fileFromName = Path.GetFileName(filePathFrom);
                string filePathTo = Path.Combine(pathTo, fileFromName);
                File.Copy(filePathFrom, filePathTo, true);
            }
        }

        private void compareResults(string scenarioName)
        {
            string expectedRoot = testScenarioDirectory + scenarioName;
            string actualRoot = dataDirectory + scenarioName;

            Assert.True(XmlDirectoryComparer.Equals(expectedRoot, actualRoot));
        }

        [Fact]
        public void TestWhichHasNoNameAsOfYet()
        {
            prepareTest(MethodInfo.GetCurrentMethod().Name);

            //reconfigure XRouter and wait till ready...

            compareResults(MethodInfo.GetCurrentMethod().Name);
        }

        [Fact]
        public void TestMoreComplex()
        {
            prepareTest(MethodInfo.GetCurrentMethod().Name);

            //copy additional shared resources from Common...

            //launch any WS and other listeners...

            //reconfigure XRouter and wait till ready...

            compareResults(MethodInfo.GetCurrentMethod().Name);

            //tear down ... quit listeners etc., but don't delete results - user might want to study them
        }
    }
}
