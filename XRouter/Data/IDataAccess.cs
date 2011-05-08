using System;
using System.Collections.Generic;
using System.Xml;

namespace XRouter.Data
{
    // TODO:
    // - use XDocument instead of XmlDocument
    // - has Int32 enough range for identifying the messages and tokens?
    // - would it be reasonable to split the interface into several ones
    //   each containg methods of particular problem domain?
    //   - the data access object could be composed of multiple specialized
    //    objects

    public interface IDataAccess
    {
        #region For testing purposes

        // TODO: is it possible to filter and/or page the messages?

        List<XmlDocument> GetAllMessages();

        XmlDocument GetMessage(int messageId);

        #endregion

        #region Config

        void UpdateComponentConfigInterests(
            string componentName,
            IEnumerable<string> interests);

        IEnumerable<string> GetComponentConfigInterests(string componentName);

        void SaveConfig(XmlDocument config);

        XmlDocument GetConfig();

        #endregion

        #region Components

        void RegisterComponent(
            string name,
            IEnumerable<string> interests,
            string address);

        void UnregisterComponent(string name);

        #endregion

        #region Mesages/Tokens

        int SaveMessage(DateTime created, XmlDocument content, string source);

        // TODO: how to change token state without overwriting it?

        void SaveToken(
            int id,
            string componentName,
            DateTime created,
            XmlDocument content,
            TokenState state);

        #endregion

        #region Logs/Errors

        // TODO:
        // - why there is an integer log category?
        // - is there any way to read the logs (eg. for displaying in a GUI)?

        void SaveLog(
            string componentName,
            DateTime created,
            int category,
            string message);

        // TODO: why there's no category compared to SaveLog()?

        void SaveErrorAndLog(
            string componentName,
            DateTime created,
            XmlDocument errorContent,
            string logMessage);

        #endregion
    }

    public enum TokenState
    {
        Received = 1,
        InProgress = 2,
        Finished = 3
    }
}
