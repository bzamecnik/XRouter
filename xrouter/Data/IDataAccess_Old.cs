using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace XRouter.Data
{
    /// <summary>
    /// Provides an API for services provided to XRouter components by a
    /// persistent data storage, such as a database. In general it contains
    /// methods for working with tokens, component configuration and logs.
    /// </summary>
    public interface IDataAccess_Old
    {
        #region Messages / Tokens

        /// <summary>
        /// Gets all messages sent into the system from outside.
        /// </summary>
        /// <returns>All messages.</returns>
        List<XDocument> GetAllMessages();

        // TODO:
        // - has Int32 enough range for identifying the messages and tokens?
        //   - probably GUID should be used

        /// <summary>
        /// Gets the message with the given ID.
        /// </summary>
        /// <param name="messageId">Number that is the ID of the wanted message.</param>
        /// <returns>Wanted message.</returns>
        XDocument GetMessage(int messageId);

        /// <summary>
        /// Stores the given message into the system.
        /// </summary>
        /// <param name="created">Date/time the message was received into the system.</param>
        /// <param name="content">Message itself (XML).</param>
        /// <param name="source">Source of the message.</param>
        /// <returns>ID of the message in the system.</returns>
        int SaveMessage(DateTime created, XDocument content, string source);

        /// <summary>
        /// Stores the given token into the system.
        /// </summary>
        /// <param name="id">ID of the message the token belongs to.</param>
        /// <param name="componentName">Component where the token was changed.</param>
        /// <param name="created">When the token was changed.</param>
        /// <param name="content">Token itself (XML).</param>
        /// <param name="state">New state of the token.</param>
        void SaveToken(
            int id,
            string componentName,
            DateTime created,
            XDocument content,
            TokenState state);

        #endregion

        #region Components / Interests / Cofigs

        /// <summary>
        /// Updates interests of the given component.
        /// </summary>
        /// <param name="componentName">Component to be updated.</param>
        /// <param name="interests">New set of interests (old ones are discarded).</param>
        void UpdateComponentConfigInterests(
            string componentName,
            IEnumerable<string> interests);

        /// <summary>
        /// Gets the given components interests.
        /// </summary>
        /// <param name="componentName">Given component.</param>
        /// <returns>Complete set of interests.</returns>
        IEnumerable<string> GetComponentConfigInterests(string componentName);

        /// <summary>
        /// Saves the new configuration.
        /// </summary>
        /// <param name="config">New config.</param>
        void SaveConfig(XDocument config);

        /// <summary>
        /// Gets the current config.
        /// </summary>
        /// <returns>Current config.</returns>
        XDocument GetConfig();

        /// <summary>
        /// Registers the given component.
        /// </summary>
        /// <param name="name">Given component.</param>
        /// <param name="interests">Interests of the new component.</param>
        /// <param name="address">Address of the new component.</param>
        void RegisterComponent(
            string name,
            IEnumerable<string> interests,
            string address);

        /// <summary>
        /// Sets the given component as no longer valid.
        /// </summary>
        /// <param name="name">Given component.</param>
        void UnregisterComponent(string name);

        #endregion

        #region Logs/Errors

        /// <summary>
        /// Stores the given log entry.
        /// </summary>
        /// <param name="componentName">Where the given log entry comes from.</param>
        /// <param name="created">When was the given log entry generated.</param>
        /// <param name="category">What is the log about.</param>
        /// <param name="message">The log entry itself.</param>
        void SaveLog(
            string componentName,
            DateTime created,
            int category,
            string message);

        /// <summary>
        /// Stores the given error and the corresponding log entry.
        /// </summary>
        /// <param name="componentName">Where the given error comes from.</param>
        /// <param name="created">When was the given error generated.</param>
        /// <param name="errorContent">The error itself.</param>
        /// <param name="logMessage">Corresponding log entry.</param>
        void SaveErrorAndLog(
            string componentName,
            DateTime created,
            XDocument errorContent,
            string logMessage);

        #endregion
    }

    /// <summary>
    /// Represents the state the token is in (how far in being processed it got).
    /// </summary>
    public enum TokenState
    {
        Received = 1,
        InProgress = 2,
        Finished = 3
    }
}
