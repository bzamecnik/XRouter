using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Gateway;
using System.Xml.Linq;
using System.Collections.Concurrent;
using System.Reflection;
using System.IO;
using System.Threading;

namespace XRouter.Adapters
{
    class DirectoryAdapter : Adapter
    {
        private static readonly string BinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private TimeSpan checkingInterval;
        private ConcurrentDictionary<string, string> outputEndpointToPathMap;
        private ConcurrentDictionary<string, string> inputEndpointToPathMap;

        protected override void Run()
        {
            #region Emulate reading configuration (will be automatic later)
            string inputDir1 = @"C:\XRouterTest\IN";
            string outputDir1 = @"C:\XRouterTest\A";
            string outputDir2 = @"C:\XRouterTest\B";
            string outputDir3 = @"C:\XRouterTest\C";

            inputEndpointToPathMap = new ConcurrentDictionary<string, string>();
            inputEndpointToPathMap.AddOrUpdate("IN", inputDir1, (key, oldValue) => null);          

            outputEndpointToPathMap = new ConcurrentDictionary<string, string>();
            outputEndpointToPathMap.AddOrUpdate("A", outputDir1, (key, oldValue) => null);
            outputEndpointToPathMap.AddOrUpdate("B", outputDir2, (key, oldValue) => null);
            outputEndpointToPathMap.AddOrUpdate("C", outputDir3, (key, oldValue) => null);

            checkingInterval = TimeSpan.FromMilliseconds(100);
            #endregion

            #region Watch input directories
            while (IsRunning) {
                Thread.Sleep(checkingInterval);

                foreach (var enpointNameAndPath in inputEndpointToPathMap) {
                    string enpointName = enpointNameAndPath.Key;
                    string path = enpointNameAndPath.Value;

                    string[] newFiles = Directory.GetFiles(path);
                    foreach (string newFilePath in newFiles) {

                        string fileContent;
                        try
                        {
                            fileContent = File.ReadAllText(newFilePath);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        XDocument message = new XDocument();
                        XElement root = new XElement(XName.Get("content"), fileContent);
                        message.Add(root);

                        try {
                            ReceiveMessage(message, enpointName);
                        } catch (Exception ex) {
                            // message receiving failed
                            continue;
                        }

                        File.Delete(newFilePath);
                    }
                }
            }
            #endregion
        }

        public override XDocument SendMessage(string endpointName, XDocument message, XDocument metadata)
        {
            string targetPath;
            if (outputEndpointToPathMap.TryGetValue(endpointName, out targetPath)) {
                string fileName = string.Format("output {0:00}_{1:00}_{2:00}_{3:000}.txt", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
                string content = message.Root.Value;
                File.WriteAllText(Path.Combine(targetPath, fileName), content);
            }
            return null;
        }
    }
}
