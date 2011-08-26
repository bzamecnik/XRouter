using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using ObjectConfigurator;
using XRouter.Common;
using XRouter.Gateway;

namespace XRouter.Adapters
{
    /// <summary>
    /// Directory adapter provides file input/output in a shared file system
    /// directory. It periodically polls for new files in directories under
    /// its control. Any succesfully loaded input file is received for further
    /// processing and removed from the file system. Also outgoing messages
    /// can be saved into files in a specified directory.
    /// </summary>
    /// <remarks>
    /// <para>Each input, resp. output directory is called an input, resp.
    /// output endpoint.</para>
    /// <para>XML files and text files (TODO) are supported.</para>
    /// <para>Unreadable files or directories are ignored.</para>
    /// </remarks>
    [AdapterPlugin("Directory I/O adapter", "Reads and writes files from/into specified directories.")]
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

        [ConfigurationItem("Save only text", null, false)]
        private bool saveOnlyText;

        protected override void Run()
        {
            System.Diagnostics.Debug.Assert(inputEndpointToPathMap != null);
            System.Diagnostics.Debug.Assert(outputEndpointToPathMap != null);

            #region Watch input directories
            while (IsRunning)
            {
                Thread.Sleep(TimeSpan.FromSeconds(checkingIntervalInSeconds));

                foreach (var enpointNameAndPath in inputEndpointToPathMap)
                {
                    if (!IsRunning)
                    {
                        return;
                    }

                    string enpointName = enpointNameAndPath.Key;
                    string path = enpointNameAndPath.Value;

                    string[] newFiles;
                    try
                    {
                        newFiles = Directory.GetFiles(path);
                    }
                    catch (Exception)
                    {
                        // Ignore input directories which cannot be read.
                        continue;
                    }
                    foreach (string newFilePath in newFiles)
                    {
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

                            XDocument metadata = new XDocument();
                            XElement xMetadata = new XElement(XName.Get("file-metadata"));
                            xMetadata.SetAttributeValue("filename", Path.GetFileName(newFilePath));
                            metadata.Add(xMetadata);

                            TraceLog.Info("Found input file " + Path.GetFileName(newFilePath));
                            try
                            {
                                try
                                {
                                    XDocument message = XDocument.Parse(fileContent);
                                    ReceiveMessageXml(message, enpointName, metadata);
                                }
                                catch (XmlException)
                                {
                                    ReceiveMessageData(fileContent, enpointName, metadata, null, null);
                                }
                            }
                            catch (Exception)
                            {
                                // message receiving failed
                                continue;
                            }

                            // NOTE: if ReceiveMessage*() fails the file is not deleted

                            File.Delete(newFilePath);
                        }
                        catch (Exception)
                        {
                            // Ignore input files which cannot be read.
                            continue;
                        }
                    }
                }
            }
            #endregion
        }

        public override XDocument SendMessage(string endpointName, XDocument message, XDocument metadata)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            string targetPath;
            if (outputEndpointToPathMap.TryGetValue(endpointName, out targetPath))
            {
                string fileName;

                if ((metadata != null) && (metadata.Root != null))
                {
                    fileName = metadata.Element(XName.Get("file-metadata")).Attribute(XName.Get("filename")).Value;
                }
                else
                {
                    fileName = string.Format("{0}_{1}.xml", DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss"), Guid.NewGuid().ToString());
                }

                TraceLog.Info(string.Format("Writing output file '{0}' into '{1}'", fileName, targetPath));

                string output;
                if (saveOnlyText) {
                    output = message.Root.Value;
                } else {
                    output = message.ToString();
                }

                // TODO: generate unique file name (in case the file with the
                // same name already exists)
                File.WriteAllText(Path.Combine(targetPath, fileName), output);
            }
            else
            {
                TraceLog.Warning(string.Format(
                    "Trying to write a file to a non-existent endpoint: {0}",
                    endpointName));
            }
            return null;
        }
    }
}
