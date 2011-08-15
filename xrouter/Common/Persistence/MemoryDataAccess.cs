using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Data;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;

namespace XRouter.Common.Persistence
{
    class MemoryDataAccess : IDataAccess
    {
        private object storageLock = new object();

        private string configurationXml;

        private Dictionary<Guid, string> tokens = new Dictionary<Guid, string>();

        private static readonly string ConfigFileName = @"app-config.xml";
        private DateTime LastModified = DateTime.Now;

        public void Initialize(string connectionString)
        {
            #region In memory configurationXml
            LastModified = File.GetLastWriteTime(ConfigFileName);
            configurationXml = File.ReadAllText(ConfigFileName);
            #endregion
        }

        public void SaveConfiguration(string configXml)
        {
            lock (storageLock) {
                configurationXml = configXml;
                File.WriteAllText(ConfigFileName, configurationXml);
                LastModified = File.GetLastWriteTime(ConfigFileName);
            }
        }

        public string LoadConfiguration()
        {
            lock (storageLock) {
                if (File.GetLastWriteTime(ConfigFileName) > LastModified)
                {
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
            lock (storageLock)
            {
                var result = tokens.Values.Where(tokenXml =>
                {
                    XDocument xToken = XDocument.Parse(tokenXml);
                    object matching = xToken.XPathEvaluate(xpath);
                    return matching != null;
                });
                return result;
            }
        }
    }
}
