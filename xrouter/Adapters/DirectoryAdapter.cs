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
using XRouter.Common;
using ObjectConfigurator;

namespace XRouter.Adapters
{
    [AdapterPlugin("Directory reader and writer", "Reads and write files from/into specified directories.")]
    public class DirectoryAdapter : Adapter
    {
        [ConfigurationItem("Checking interval (in seconds)", null, 0.1)]
        private double checkingIntervalInSeconds;

        [ConfigurationItem("Input directories", null, new[] { 
            "In", @"C:\XRouterTest\In"
        })]
        private ConcurrentDictionary<string, string> inputEndpointToPathMap;

        [ConfigurationItem("Output directories", null, new[] { 
            "OutA", @"C:\XRouterTest\OutA",
            "OutB", @"C:\XRouterTest\OutB",
            "OutC", @"C:\XRouterTest\OutC"
        })]
        private ConcurrentDictionary<string, string> outputEndpointToPathMap;

        protected override void Run()
        {
            #region Watch input directories
            while (IsRunning) {
                Thread.Sleep(TimeSpan.FromSeconds(checkingIntervalInSeconds));

                foreach (var enpointNameAndPath in inputEndpointToPathMap) {
                    string enpointName = enpointNameAndPath.Key;
                    string path = enpointNameAndPath.Value;

                    string[] newFiles;
                    try
                    {
                        newFiles = Directory.GetFiles(path);
                    }
                    catch (Exception ex)
                    {
                        EventLog.Error(string.Format(
                            "Cannot read files from directory: {0}. Details: {1}",
                            path, ex.Message));
                        continue;
                    }
                    foreach (string newFilePath in newFiles) {
                        try
                        {
                        string fileContent;
                        try
                        {
                            fileContent = File.ReadAllText(newFilePath);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        XDocument message = XDocument.Parse(fileContent);

                        XDocument metadata = new XDocument();
                        XElement xMetadata = new XElement(XName.Get("file-metadata"));
                        xMetadata.SetAttributeValue("filename", Path.GetFileName(newFilePath));
                        metadata.Add(xMetadata);

                        TraceLog.Info("Found input file " + Path.GetFileName(newFilePath));
                        
                        ReceiveMessage(message, enpointName, metadata);

                        // spolehlivost

                        File.Delete(newFilePath);
                        }
                        catch (Exception ex)
                        {
                            EventLog.Error(string.Format(
                                "Error while receiving file {0}. Details: {1}",
                                newFilePath, ex.Message));
                            // TODO: protentially store bad file contents into TraceLog
                        }
                    }
                }
            }
            #endregion
        }

        public override XDocument SendMessage(string endpointName, XDocument message, XDocument metadata)
        {
            string targetPath;
            if (outputEndpointToPathMap.TryGetValue(endpointName, out targetPath)) {
                string fileName;
                if (metadata != null) {
                    fileName = metadata.Element(XName.Get("file-metadata")).Attribute(XName.Get("filename")).Value;
                } else {
                    fileName = string.Format("output {0:00}_{1:00}_{2:00}_{3:000}.txt", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
                }
                TraceLog.Info(string.Format("Writing output file '{0}' into '{1}'", fileName, targetPath));
                File.WriteAllText(Path.Combine(targetPath, fileName), message.ToString());
            }
            return null;
        }
    }
}
