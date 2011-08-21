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

        private object _updateTokenLock = new object();
        /// <summary>
        /// A lock for updating tokens to ensure atomicity.
        /// </summary>
        /// <remarks>
        /// Currently no two tokens can be updated in parallel. Better would
        /// be to enable updating tokens with different GUIDs in parallel.
        /// </remarks>
        private object UpdateTokenLock
        {
            get
            {
                if (_updateTokenLock == null)
                {
                    _updateTokenLock = new object();
                }
                return _updateTokenLock;
            }
        }

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
            // TODO: what about GUID of a non-existent token
            string tokenXml = dataAccess.LoadToken(tokenGuid);
            Token result = new Token(tokenGuid, tokenXml);
            return result;
        }

        /// <summary>
        /// Updates a token into new instance.
        /// </summary>
        /// <remarks>
        /// Loads a token specified by its GUID from the storage, updates it
        /// with a provided action, saves it and returns a *new* updated token
        /// instance.
        /// </remarks>
        /// <param name="tokenGuid"></param>
        /// <param name="updater"></param>
        /// <returns>updated token with the specified GUID; or null if no such
        /// token exists</returns>
        public Token UpdateToken(Guid tokenGuid, Action<Token> updater)
        {
            lock (UpdateTokenLock)
            {
                Token token = GetToken(tokenGuid);
                if (token == null)
                {
                    return null;
                }
                updater(token);
                SaveToken(token);
                return token;
            }
        }

        /// <summary>
        /// Updates a token and stores the content into an exiting instance.
        /// </summary>
        /// <remarks>
        /// Loads a token specified by its GUID from the storage, updates it
        /// with a provided action, saves it and returns the provided token
        /// instance only with updated content.
        /// </remarks>
        /// <param name="token"></param>
        /// <param name="updater"></param>
        public void UpdateToken(Token token, Action<Token> updater)
        {
            lock (UpdateTokenLock)
            {
                Token currentToken = GetToken(token.Guid);
                updater(currentToken);
                SaveToken(currentToken);
                token.UpdateContent(currentToken.Content);
            }
        }

        public Token[] GetTokens(int pageSize, int pageNumber)
        {
            return CreateTokens(dataAccess.LoadTokens(pageSize, pageNumber)).ToArray();
        }

        public Token[] GetUndispatchedTokens()
        {
            //todo: Determine correct xpath
            // <MessageFlowState><AssignedProcessor i:nil="true" />
            // 
            string xpath = "/token/messageflow-state/MessageFlowState/AssignedProcessor[@i:nil='true']";
            return CreateTokens(dataAccess.LoadMatchingTokens(xpath)).ToArray();
        }

        public Token[] GetActiveTokensAssignedToProcessor(string processorName)
        {
            //todo: Determine correct xpath
            // <MessageFlowState><AssignedProcessor z:Id="2">processor1</AssignedProcessor>
            // 
            string xpath = string.Format(
                "/token[@state='InProcessor']/messageflow-state/MessageFlowState/AssignedProcessor[.='{0}']",
                processorName);
            return CreateTokens(dataAccess.LoadMatchingTokens(xpath)).ToArray();
        }

        private IEnumerable<Token> CreateTokens(IEnumerable<string> tokensXml)
        {
            List<Token> result = new List<Token>();
            foreach (string tokenXml in tokensXml)
            {
                Token token = new Token(tokenXml);
                result.Add(token);
            }
            return result;
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
            if (xContainer != null)
            {
                result.Add(xContainer);
            }
            return result;
        }
        #endregion
    }
}