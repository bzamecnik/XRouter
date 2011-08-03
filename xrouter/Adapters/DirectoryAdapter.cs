using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
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
            System.Diagnostics.Debug.Assert(inputEndpointToPathMap != null);
            System.Diagnostics.Debug.Assert(outputEndpointToPathMap != null);

            #region Watch input directories
            while (IsRunning)
            {
                Thread.Sleep(TimeSpan.FromSeconds(checkingIntervalInSeconds));

                foreach (var enpointNameAndPath in inputEndpointToPathMap)
                {
                    string enpointName = enpointNameAndPath.Key;
                    string path = enpointNameAndPath.Value;

                    string[] newFiles;
                    try
                    {
                        newFiles = Directory.GetFiles(path);
                    }
                    catch (Exception ex)
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
                            // TODO: if the document is not a valid XML, send it
                            // as raw data with ReceiveMessageData()
                            XDocument message = XDocument.Parse(fileContent);

                            XDocument metadata = new XDocument();
                            XElement xMetadata = new XElement(XName.Get("file-metadata"));
                            xMetadata.SetAttributeValue("filename", Path.GetFileName(newFilePath));
                            metadata.Add(xMetadata);

                            TraceLog.Info("Found input file " + Path.GetFileName(newFilePath));
                            try
                            {
                                ReceiveMessageXml(message, enpointName, metadata);
                            }
                            catch (Exception ex)
                            {
                                // message receiving failed
                                continue;
                            }

                            // tady by bylo dobre, aby nedochazelo ke ztrate nebo rozmnozovani zprav
                            // navrhuji udelat to, ze jakmile bude zprava ulozena v databazi, tak adpter dostane notifikaci
                            // a zavola se nasledujici prikaz (takova notifikace se muze hodit i u synchronnich adapteru, 
                            // ale u WebServices to neni nutne

                            File.Delete(newFilePath);
                        }
                        catch (Exception ex)
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
            string targetPath;
            if (outputEndpointToPathMap.TryGetValue(endpointName, out targetPath))
            {
                string fileName;

                if (metadata != null)
                {
                    // tohle by bylo dobre, ale chtelo by to, aby tam uzivatel mohl specifikovat substituce (poresime osobne)                    
                    fileName = metadata.Element(XName.Get("file-metadata")).Attribute(XName.Get("filename")).Value;
                }
                else
                {
                    // tady poresime ziskani defaultniho jednoznacneho 
                    fileName = string.Format(DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss.xml"));
                }

                TraceLog.Info(string.Format("Writing output file '{0}' into '{1}'", fileName, targetPath));

                // tady musime zase osetrit to, aby se neprepisovaly zpravy, udelame to generovanim jednoznacnych nazvu souboru
                File.WriteAllText(Path.Combine(targetPath, fileName), message.ToString());
            }
            return null;
        }
    }
}
