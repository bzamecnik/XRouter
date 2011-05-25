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
    class DirectoryAdapter : IAdapter
    {
        private static readonly string BinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private IAdapterService service;

        private TimeSpan checkingInterval;
        private ConcurrentDictionary<string, string> outputEndpointToPathMap;
        private ConcurrentDictionary<string, string> inputEndpointToPathMap;

        private volatile bool isStopped;

        public void Start(IAdapterService service)
        {
            this.service = service;

            #region Emulate reading configuration (will be automatic later)
            string inputDir1 = Path.Combine(BinPath, "inputDir1");
            string inputDir2 = Path.Combine(BinPath, "inputDir2");
            string outputDir1 = Path.Combine(BinPath, "output1");
            string outputDir2 = Path.Combine(BinPath, "output2");
            if (!Directory.Exists(inputDir1)) { Directory.CreateDirectory(inputDir1); }
            if (!Directory.Exists(inputDir2)) { Directory.CreateDirectory(inputDir2); }
            if (!Directory.Exists(outputDir1)) { Directory.CreateDirectory(outputDir1); }
            if (!Directory.Exists(outputDir2)) { Directory.CreateDirectory(outputDir2); }

            inputEndpointToPathMap = new ConcurrentDictionary<string, string>();
            inputEndpointToPathMap.AddOrUpdate("inputDir1", inputDir1, (key, oldValue) => null);
            inputEndpointToPathMap.AddOrUpdate("inputDir2", inputDir2, (key, oldValue) => null);

            outputEndpointToPathMap = new ConcurrentDictionary<string, string>();
            outputEndpointToPathMap.AddOrUpdate("outputDir1", outputDir1, (key, oldValue) => null);
            outputEndpointToPathMap.AddOrUpdate("outputDir2", outputDir2, (key, oldValue) => null);

            checkingInterval = TimeSpan.FromMilliseconds(100);
            #endregion

            #region Watch input directories
            while (!isStopped) {
                Thread.Sleep(checkingInterval);

                foreach (var enpointNameAndPath in inputEndpointToPathMap) {
                    string enpointName = enpointNameAndPath.Key;
                    string path = enpointNameAndPath.Value;

                    string[] newFiles = Directory.GetFiles(path);
                    foreach (string newFilePath in newFiles) {

                        string fileContent = File.ReadAllText(newFilePath);
                        XDocument message = new XDocument();
                        XElement root = new XElement(XName.Get("content"), fileContent);
                        message.Add(root);

                        try {
                            service.ReceiveMessage(message, enpointName);
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

        public void Stop()
        {
            isStopped = true;
        }

        public XDocument SendMessage(string endpointName, XDocument message, XDocument metadata)
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
