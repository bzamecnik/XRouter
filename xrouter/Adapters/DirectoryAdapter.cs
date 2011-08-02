﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Gateway;
using System.Xml.Linq;
using System.Collections.Concurrent;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using ObjectConfigurator;
using XRouter.Common;
using XRouter.Gateway;

namespace XRouter.Adapters
{
    /// <summary>
    /// Adapter umoznuje realizovat file-transfer pres sdileny adresar souboroveho systemu. 
    /// Adapter tvori sluzba, ktera se periodicky dotazuje nad nastavenymi adresari a v 
    /// pripade vyskytu zpravy, soubor nacte a odstrani ze vstupu. 
    /// Adapter tvori client, ktery umoznuje exportovat zpravu do nastaveneho adresare. 
    /// </summary>
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
