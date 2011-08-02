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
    /// <summary>
    /// Adapter umoznuje realizovat file-transfer pres sdileny adresar souboroveho systemu. 
    /// Adapter tvori sluzba, ktera se periodicky dotazuje nad nastavenymi adresari a v 
    /// pripade vyskytu zpravy, soubor nacte a odstrani ze vstupu. 
    /// Adapter tvori client, ktery umoznuje exportovat zpravu do nastaveneho adresare. 
    /// </summary>
    class DirectoryAdapter : Adapter
    {                                        
        private static readonly string BinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);       
        
        private TimeSpan checkingInterval;
       
        private ConcurrentDictionary<string, string> outputEndpointToPathMap;
                
        private ConcurrentDictionary<string, string> inputEndpointToPathMap;

        protected override void Run()
        {
            #region Emulate reading configuration (will be automatic later)
            string inputDir1 = @"C:\XRouterTest\In";
            string outputDir1 = @"C:\XRouterTest\OutA";
            string outputDir2 = @"C:\XRouterTest\OutB";
            string outputDir3 = @"C:\XRouterTest\OutC";

            inputEndpointToPathMap = new ConcurrentDictionary<string, string>();
            inputEndpointToPathMap.AddOrUpdate("In", inputDir1, (key, oldValue) => null);          

            outputEndpointToPathMap = new ConcurrentDictionary<string, string>();
            outputEndpointToPathMap.AddOrUpdate("OutA", outputDir1, (key, oldValue) => null);
            outputEndpointToPathMap.AddOrUpdate("OutB", outputDir2, (key, oldValue) => null);
            outputEndpointToPathMap.AddOrUpdate("OutC", outputDir3, (key, oldValue) => null);

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
                        XDocument message = XDocument.Parse(fileContent);

                        XDocument metadata = new XDocument();
                        XElement xMetadata = new XElement(XName.Get("file-metadata"));
                        xMetadata.SetAttributeValue("filename", Path.GetFileName(newFilePath));
                        metadata.Add(xMetadata);

                        TraceLog.Info("Found input file " + Path.GetFileName(newFilePath));
                        try
                        {
                            ReceiveMessage(message, enpointName, metadata);
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
                    // tohle by bylo dobre, ale chtelo by to, aby tam uzivatel mohl specifikovat substituce (poresime osobne)                    
                    fileName = metadata.Element(XName.Get("file-metadata")).Attribute(XName.Get("filename")).Value;
                } else {
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
