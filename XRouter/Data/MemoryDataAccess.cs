﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using XRouter.Common;

namespace XRouter.Data
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

            if (!File.Exists(ConfigFileName)) {
                File.WriteAllText(ConfigFileName, ApplicationConfiguration.InitialContent);
            }
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

        private void SaveToken(Guid tokenGuid, string tokenXml)
        {
            lock (storageLock) {
                if (tokens.ContainsKey(tokenGuid)) {
                    tokens[tokenGuid] = tokenXml;
                } else {
                    tokens.Add(tokenGuid, tokenXml);
                }
            }
        }

        public void SaveToken(Token token)
        {
            SaveToken(token.Guid, token.Content.XDocument.ToString());
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
