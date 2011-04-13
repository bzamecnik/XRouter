using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Xml;

namespace XRouter
{
    interface DBAccessInterface
    {
        //For testing purposes
        List<XmlDocument> GetAllMessages();
        XmlDocument GetMessage(int messageID);

        //Config
        void UpdateComponentConfigInterests(string componentName, IEnumerable<string> interests);
        IEnumerable<string> GetComponentConfigInterests(string componentName);
        void SaveConfig(XmlDocument config);
        XmlDocument GetConfig();

        //Components
        void RegisterComponent(string name, IEnumerable<string> interests, string address);
        void UnregisterComponent(string name);


        //Mesages/Tokens
        int SaveMessage(DateTime created, XmlDocument content, string source);
        void SaveToken(int id, string componentName, DateTime created, XmlDocument content, TokenState state);

        //Logs/Errors
        void SaveLog(string componentName, DateTime created, int category, string message);
        void SaveErrorAndLog(string componentName, DateTime created, XmlDocument errorContent, string logMessage);
    }

    public enum TokenState {Received = 1, InProgress = 2, Finished = 3}
}
