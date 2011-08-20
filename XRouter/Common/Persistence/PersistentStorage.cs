using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using XRouter.Common.Xrm;
using XRouter.Data;

namespace XRouter.Common.Persistence
{
    public class PersistentStorage : IXmlStorage
    {
        private IDataAccess dataAccess;
        private XDocument configXmlCache;

        private object tokensLocksSync = new object();
        private Dictionary<Guid, WeakReference> tokensLocks = new Dictionary<Guid, WeakReference>();

        public PersistentStorage(string connectionString)
        {
            dataAccess = new MsSqlDataAccess();
            //dataAccess = new MemoryDataAccess();
            dataAccess.Initialize(connectionString);
        }

        public XDocument GetApplicationConfiguration()
        {
            if (configXmlCache == null)
            {
                string configXml = dataAccess.LoadConfiguration();
                if (configXml == null)
                {
                    configXml = ApplicationConfiguration.InitialContent;
                }
                configXmlCache = XDocument.Parse(configXml);
            }
            return configXmlCache;
        }

        public void SaveApplicationConfiguration(XDocument config)
        {
            configXmlCache = config;
            string configXml = config.ToString();
            dataAccess.SaveConfiguration(configXml);
        }

        public void SaveToken(Token token)
        {
            string tokenXml = token.Content.XDocument.ToString();
            dataAccess.SaveToken(token.Guid, tokenXml);
        }

        public Token GetToken(Guid tokenGuid)
        {
            string tokenXml = dataAccess.LoadToken(tokenGuid);
            Token result = new Token(tokenGuid, tokenXml);
            return result;
        }

        public void UpdateToken(Guid tokenGuid, Action<Token> updater)
        {
            RunAtomicForToken(tokenGuid, delegate {
                Token token = GetToken(tokenGuid);
                updater(token);
                SaveToken(token);
            });
        }

        public Token[] GetTokens(int pageSize, int pageNumber)
        {
            return CreateTokens(dataAccess.LoadTokens(pageSize, pageNumber)).ToArray();
        }

        public Token[] GetUndispatchedTokens()
        {
            //todo: Determine correct xpath
            string xpath = "/token/messageflow-state/AssignedProcessor";  // AssignedProcessor!=null
            return CreateTokens(dataAccess.LoadMatchingTokens(xpath)).ToArray();
        }

        public Token[] GetActiveTokensAssignedToProcessor(string processorName)
        {
            //todo: Determine correct xpath
            string xpath = "/token/messageflow-state/AssignedProcessor"; // (t.MessageFlowState.AssignedProcessor == processorName) && (t.State == TokenState.InProcessor)
            return CreateTokens(dataAccess.LoadMatchingTokens(xpath)).ToArray();
        }

        private IEnumerable<Token> CreateTokens(IEnumerable<string> tokensXml)
        {
            List<Token> result = new List<Token>();
            foreach (string tokenXml in tokensXml) {
                Token token = new Token(tokenXml);
                result.Add(token);
            }
            return result;
        }

        private void RunAtomicForToken(Guid tokenGuid, Action action)
        {
            #region Get or create tokenLock for given token (atomicaly)
            object tokenLock = null;
            lock (tokensLocksSync) {
                if (tokensLocks.ContainsKey(tokenGuid)) {
                    tokenLock = tokensLocks[tokenGuid].Target;
                }
                if (tokenLock == null) {
                    tokenLock = new object();
                    tokensLocks.Add(tokenGuid, new WeakReference(tokenLock));
                }
            }
            #endregion

            // At this momement, there is globally exactly one tokenLock object for given token guid
            lock (tokenLock) {
                action();
            }

            #region Clear unused tokenLocks
            tokenLock = null;
            lock (tokensLocksSync) {
                foreach (var pair in tokensLocks.ToArray()) {
                    Guid guid = pair.Key;
                    WeakReference wr = pair.Value;
                    if (!wr.IsAlive) {
                        tokensLocks.Remove(guid);
                    }
                }
            }
            #endregion
        }

        #region Implementation of IXmlStorage
        void IXmlStorage.SaveXml(XDocument xml)
        {
            XDocument config = GetApplicationConfiguration();
            XElement xContainer = config.XPathSelectElement("/configuration/xml-resource-storage");
            xContainer.RemoveNodes();
            xContainer.Add(xml.Root);
            SaveApplicationConfiguration(config);
        }

        XDocument IXmlStorage.LoadXml()
        {
            XDocument config = GetApplicationConfiguration();
            XElement xContainer = config.XPathSelectElement("/configuration/xml-resource-storage");

            XDocument result = new XDocument();
            if (xContainer != null) {
                result.Add(xContainer);
            }
            return result;
        }
        #endregion
    }
}