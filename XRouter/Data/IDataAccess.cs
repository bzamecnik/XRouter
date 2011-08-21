using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;

namespace XRouter.Data
{
    /// <summary>
    /// Provides an API for services provided to XRouter components by a
    /// persistent data storage, such as a database. In general it contains
    /// methods for working with application configuration and tokens.
    /// </summary>
    /// <seealso cref="XRouter.Common.Token"/>
    /// <seealso cref="XRouter.Common.ApplicationConfiguration"/>
    public interface IDataAccess
    {
        /// <summary>
        /// Initializes the data access object with a string containing data
        /// needed to connent to the storage, eg. a database connection
        /// string. The exact format depends on the particular storage used
        /// in implementation of this interface.
        /// </summary>
        /// <param name="connectionString">connection string</param>
        void Initialize(string connectionString);

        #region Application configuration

        /// <summary>
        /// Loads the application configuration.
        /// </summary>
        /// <returns>current application configuration XML document</returns>
        string LoadConfiguration();

        /// <summary>
        /// Saves the application configuration XML document.
        /// </summary>
        /// <remarks>
        /// It replaces the previous configuration.
        /// </remarks>
        /// <param name="configXml">application configuration</param>
        void SaveConfiguration(string configXml);

        #endregion

        #region Tokens

        // TODO: what about non-existent token?

        /// <summary>
        /// Loads the XML content of an existing token specified by its GUID.
        /// </summary>
        /// <param name="tokenGuid">unique identifier of the token</param>
        /// <returns>token contents</returns>
        string LoadToken(Guid tokenGuid);

        /// <summary>
        /// Saves the token identified by its GUID with a given content.
        /// </summary>
        /// <remarks>
        /// It adds or replaces the token.
        /// </remarks>
        /// <param name="tokenGuid">unique identifier of the token</param>
        /// <param name="tokenXml">token XML content</param>
        void SaveToken(Guid tokenGuid, string tokenXml);

        void SaveToken(Token token);

        // TODO: what about the order of tokens?

        /// <summary>
        /// Loads a page of all tokens in the storage matching the specified
        /// criteria.
        /// </summary>
        /// <param name="pageSize">size of the page of tokens</param>
        /// <param name="pageNumber">1-based index of the page</param>
        /// <returns>collection of contents of matching tokens</returns>
        IEnumerable<string> LoadTokens(int pageSize, int pageNumber);

        /// <summary>
        /// Loads all tokens in the storage matching he specified XPath query.
        /// </summary>
        /// <param name="xpath">XPath query over the token contents to
        /// select the matching tokens</param>
        /// <returns>collection of contents of matching tokens</returns>
        IEnumerable<string> LoadMatchingTokens(string xpath);

        #endregion
    }
}
