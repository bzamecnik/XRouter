using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XRouter.Data.Persistence
{
    public class MemoryDataAccess : IDataAccess
    {
        private object storageLock = new object();

        private string configurationXml;

        private Dictionary<Guid, string> tokens = new Dictionary<Guid, string>();

        public static readonly string DefaultConfigFileName = @"app-config.xml";

        public string ConfigFileName { get; set; }

        private DateTime FileLastModified = DateTime.Now;

        public MemoryDataAccess()
        {
            ConfigFileName = DefaultConfigFileName;
        }

        public void Initialize(string connectionString)
        {
            #region In memory configurationXml
            FileLastModified = File.GetLastWriteTime(ConfigFileName);
            configurationXml = File.ReadAllText(ConfigFileName);
            #endregion
        }

        public void SaveConfiguration(string configXml)
        {
            lock (storageLock) {
                configurationXml = configXml;
                File.WriteAllText(ConfigFileName, configurationXml);
                FileLastModified = File.GetLastWriteTime(ConfigFileName);
            }
        }

        public string LoadConfiguration()
        {
            lock (storageLock) {
                if (File.GetLastWriteTime(ConfigFileName) > FileLastModified) {
                    configurationXml = File.ReadAllText(ConfigFileName);
                }
                return configurationXml;
            }
        }

        public void SaveToken(Guid tokenGuid, string tokenXml)
        {
            lock (storageLock) {
                if (tokens.ContainsKey(tokenGuid)) {
                    tokens[tokenGuid] = tokenXml;
                } else {
                    tokens.Add(tokenGuid, tokenXml);
                }
            }
        }

        public string LoadToken(Guid tokenGuid)
        {
            lock (storageLock) {
                if (tokens.ContainsKey(tokenGuid)) {
                    return tokens[tokenGuid];
                } else {
                    return null;
                }
            }
        }

        public IEnumerable<string> LoadTokens(int pageSize, int pageNumber)
        {
            lock (storageLock) {
                int tokensToSkip = (pageNumber - 1) * pageSize;
                return tokens.Values.Skip(tokensToSkip).Take(pageNumber);
            }
        }

        public IEnumerable<string> LoadMatchingTokens(string xpath)
        {
            lock (storageLock) {
                var result = tokens.Values.Where(tokenXml => {
                    XDocument xToken = XDocument.Parse(tokenXml);
                    object matching = xToken.XPathEvaluate(xpath);
                    return matching != null;
                });
                return result;
            }
        }
    }
}
