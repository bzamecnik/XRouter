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

        public PersistentStorage(string connectionString)
        {
            dataAccess = new MsSqlDataAccess();
            //dataAccess = new MemoryDataAccess();
            dataAccess.Initialize(connectionString);
        }

        public XDocument GetApplicationConfiguration()
        {
            string configXml = dataAccess.LoadConfiguration();
            if (configXml == null) {
                configXml = ApplicationConfiguration.InitialContent;
            }
            XDocument result = XDocument.Parse(configXml);
            return result;
        }

        public void SaveApplicationConfiguration(XDocument config)
        {
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

        /// <summary>
        /// Updates a token.
        /// </summary>
        /// <remarks>
        /// Loads a token specified by its GUID from the storage, updates it
        /// with a provided action, saves it and returns the updated token
        /// instance.
        /// </remarks>
        /// <param name="tokenGuid"></param>
        /// <param name="updater"></param>
        /// <returns>updated token</returns>
        public Token UpdateToken(Guid tokenGuid, Action<Token> updater)
        {
            Token token = GetToken(tokenGuid);
            updater(token);
            SaveToken(token);
            return token;
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