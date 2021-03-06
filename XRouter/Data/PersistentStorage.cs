﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using XRouter.Common;
using XRouter.Common.Xrm;

namespace XRouter.Data
{
    /// <summary>
    /// Implements a persistent storage of tokens, application configuration,
    /// and other XML resources (XRM). It wraps a plain data access object with
    /// some business logic.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Application configuration is assumed not to change during run-time,
    /// so it is cached and never updated after being loaded initially.
    /// </para>
    /// <para>
    /// The data access is prepared to be interchangable a only needs to
    /// implement the IDataAccess interface. Currently only the
    /// MsSqlDataAccess (MS SQL Server) is used.
    /// </para>
    /// <para>
    /// Token updates are atomic.
    /// </para>
    /// <para>
    /// Application configuration is currenlty assumed not to change during
    /// run-time of the XRouter service, so it is cached and after being
    /// loaded initially it can be only updated by calling the
    /// SaveApplicationConfiguration() method.
    /// </para>
    /// </remarks>
    public class PersistentStorage : IXmlStorage
    {
        private IDataAccess dataAccess;
        private XDocument configXmlCache;

        /// <summary>
        /// A lock for updating tokens to ensure atomicity.
        /// </summary>
        /// <remarks>
        /// Currently no two tokens can be updated in parallel. Better would
        /// be to enable updating tokens with different GUIDs in parallel.
        /// </remarks>
        private object updateTokenLock = new object();

        public PersistentStorage(string connectionString)
        {
            dataAccess = new MsSqlDataAccess();
            //dataAccess = new MemoryDataAccess();
            dataAccess.Initialize(connectionString);
        }

        #region Application configuration

        /// <summary>
        /// Obtains the current application configuration.
        /// </summary>
        /// <remarks>
        /// The configuration XML is cached and the method returns the same
        /// reference in case the configuration did not change.
        /// </remarks>
        /// <returns></returns>
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

        /// <summary>
        /// Updates the application configuration.
        /// </summary>
        /// <remarks>
        /// Also updates the cache.
        /// </remarks>
        /// <param name="config"></param>
        public void SaveApplicationConfiguration(XDocument config)
        {
            configXmlCache = config;
            string configXml = config.ToString();
            dataAccess.SaveConfiguration(configXml);
        }

        #endregion

        #region Tokens

        public void SaveToken(Token token)
        {
            dataAccess.SaveToken(token);
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
            lock (updateTokenLock) {
                Token token = GetToken(tokenGuid);
                if (token == null) {
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
            lock (updateTokenLock) {
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
            foreach (string tokenXml in tokensXml) {
                Token token = new Token(tokenXml);
                result.Add(token);
            }
            return result;
        }

        #endregion

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
