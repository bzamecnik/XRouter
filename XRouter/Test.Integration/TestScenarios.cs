using System;
using System.IO;
using System.Reflection;
using XRouter.Test.Common;
using Xunit;

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
            //TODO reconfigure XRouter with some neutral config

            string pathFrom = Path.Combine(testScenarioDirectory, scenarioName);
            string pathTo = Path.Combine(dataDirectory, scenarioName);

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
            string expectedRoot = Path.Combine(testScenarioDirectory, scenarioName);
            string actualRoot = Path.Combine(dataDirectory, scenarioName);

            Assert.True(XmlDirectoryComparer.Equals(expectedRoot, actualRoot));
        }

        private void waitUntilXRouterIsDone()
        {
            Console.WriteLine("Press a key when XRouter is done");
            Console.ReadKey();

            //TODO better way to find out...
            //string signalFile = Path.Combine(testScenarioDirectory, "XRouterIsDone");
            //while (!File.Exists(signalFile))
            //{
            //    Thread.Sleep(500);
            //}
            //File.Delete(signalFile);
        }

        //[Fact]
        public void TestWhichHasNoNameAsOfYet()
        {
            prepareTest(MethodInfo.GetCurrentMethod().Name);

            //reconfigure XRouter

            waitUntilXRouterIsDone(); //TODO after it is implemented properly, maybe combine waiting, comparing and some common tear down parts into one method

            compareResults(MethodInfo.GetCurrentMethod().Name);
        }

        //[Fact]
        public void TestMoreComplex()
        {
            prepareTest(MethodInfo.GetCurrentMethod().Name);

            //copy additional shared resources from Common...

            //launch any WS and other listeners...

            //reconfigure XRouter

            waitUntilXRouterIsDone();

            compareResults(MethodInfo.GetCurrentMethod().Name);

            //tear down ... quit listeners etc., but don't delete results - user might want to study them
        }
    }
}
