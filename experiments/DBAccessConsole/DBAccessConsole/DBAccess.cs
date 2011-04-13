using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Xml;
using System.IO;

namespace XRouter
{
    class DBAccess : XRouter.DBAccessInterface
    {
        private string IPAddress;
        private string instance;
        private string DBName;

        private string login = "XRouter_AccessDB";
        private string password = "XRouter";

        private string ConnectionString;

        public DBAccess
        (
            string IPAddress = "localhost",
            string instance = "DOMA",
            string DBName = "XRouter"
        )
        {
            this.IPAddress = IPAddress;
            this.instance = instance;
            this.DBName = DBName;

            this.ConnectionString = String.Format("Server={0}\\{1};Database={2};User={3};Password={4}",
                IPAddress, instance, DBName, login, password);
        }

        private SqlConnection GetConnection()
        {
            //Will use SQL Server Connection Pooling -> effective for repeated use
            SqlConnection DB = new SqlConnection();
            DB.ConnectionString = ConnectionString;
            DB.Open();

            return DB;
        }

        private class SQLParameter
        {
            public SQLParameter(string name, object value)
            {
                this.name = name;
                this.value = value.ToString().Replace("'", "''");
            }

            public string name { get; private set; }
            public string value { get; private set; }
        }

        private SqlDataReader ExecuteProcedure(string procedure, SQLParameter[] parameters)
        {
            string parameterstring = "";
            bool isFirst = true;
            foreach (SQLParameter parameter in parameters)
            {
                if (!isFirst)
                {
                    parameterstring += ", ";
                }
                isFirst = false;
                parameterstring += "@" + parameter.name + "=";
                parameterstring += "'" + parameter.value + "'";
            }

            SqlDataReader sqlDataReader;
            try
            {
                SqlConnection DB = GetConnection();
                SqlCommand sqlCommand = DB.CreateCommand();
                sqlCommand.CommandText = String.Format("EXECUTE executable.{0} {1}", procedure, parameterstring);
                sqlDataReader = sqlCommand.ExecuteReader();

            }
            catch (Exception)
            {
                //TODO throw specialized exceptions...
                sqlDataReader = null;
            }

            return sqlDataReader;
        }

        private string GetXMLString(XmlDocument xml)
        {
            StringWriter stringWriter = new StringWriter();
            xml.WriteTo(new XmlTextWriter(stringWriter));

            return stringWriter.ToString();
        }

        public List<XmlDocument> GetAllMessages()
        {
            List<XmlDocument> messages = new List<XmlDocument>();

            SqlDataReader sqlDataReader = ExecuteProcedure("GetAllMessages", new SQLParameter[]{});
            while (sqlDataReader.Read())
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sqlDataReader[0].ToString());

                messages.Add(xml);
            }
            sqlDataReader.Close();

            return messages;
        }

        public XmlDocument GetMessage(int messageID)
        {
            XmlDocument message = new XmlDocument();

            SqlDataReader sqlDataReader = ExecuteProcedure("GetMessage",
                new SQLParameter[]
                {
                    new SQLParameter("MessageID", messageID)
                });
            if (sqlDataReader.Read())
            {
                message.LoadXml(sqlDataReader[0].ToString());
            }
            sqlDataReader.Close();

            return message;
        }

        public void UpdateComponentConfigInterests(string componentName, IEnumerable<string> interests)
        {
            string interestsString = "";
            string separator = ";:;";
            interestsString = String.Join(separator, interests);

            if ((componentName == "") || (interestsString == ""))
            {
                return;
            }

            ExecuteProcedure("UpdateComponentConfigInterests",
                new SQLParameter[]
                {
                    new SQLParameter("ComponentName", componentName),
                    new SQLParameter("Interests", interestsString),
                    new SQLParameter("Separator", separator)
                });
        }

        public IEnumerable<string> GetComponentConfigInterests(string componentName)
        {
            List<string> interests = new List<string>();

            string parameters = string.Format
                (
                    " @ComponentName='{0}'",
                    componentName
                );
            SqlDataReader sqlDataReader = ExecuteProcedure("GetComponentConfigInterests",
                new SQLParameter[]
                {
                    new SQLParameter("ComponentName", componentName)
                });
            while (sqlDataReader.Read())
            {
                string interest = sqlDataReader[0].ToString();

                interests.Add(interest);
            }
            sqlDataReader.Close();

            return interests;
        }

        public void SaveConfig(XmlDocument config)
        {
            ExecuteProcedure("SaveConfig",
                new SQLParameter[]
                {
                    new SQLParameter("Config", GetXMLString(config))
                });
        }

        public XmlDocument GetConfig()
        {
            XmlDocument config = new XmlDocument();

            SqlDataReader sqlDataReader = ExecuteProcedure("GetConfig", new SQLParameter[] { });
            if (sqlDataReader.Read())
            {
                config.LoadXml(sqlDataReader[0].ToString());
            }
            sqlDataReader.Close();

            return config;
        }

        public void RegisterComponent(string name, IEnumerable<string> interests, string address)
        {
            string interestsString = "";
            string separator = ";:;";
            interestsString = String.Join(separator, interests);

            if ((name == "") || (interestsString == ""))
            {
                return;
            }

            ExecuteProcedure("RegisterComponent",
                new SQLParameter[]
                {
                    new SQLParameter("ComponentName", name),
                    new SQLParameter("Interests", interestsString),
                    new SQLParameter("Separator", separator),
                    new SQLParameter("Address", address)
                });
        }

        public void UnregisterComponent(string name)
        {
            ExecuteProcedure("UnregisterComponent",
                new SQLParameter[]
                {
                    new SQLParameter("ComponentName", name)
                });
        }

        public int SaveMessage(DateTime created, XmlDocument content, string source)
        {
            int messageID = -1;

            SqlDataReader sqlDataReader = ExecuteProcedure("SaveMessage",
                new SQLParameter[]
                {
                    new SQLParameter("Created", created),
                    new SQLParameter("Content", GetXMLString(content)),
                    new SQLParameter("EndpointName", source)
                });
            if (sqlDataReader.Read())
            {
                messageID = Int32.Parse(sqlDataReader[0].ToString());
            }
            sqlDataReader.Close();

            return messageID;
        }

        public void SaveToken(int id, string componentName, DateTime created, XmlDocument content, TokenState state)
        {
            ExecuteProcedure("SaveToken",
                new SQLParameter[]
                {
                    new SQLParameter("MessagesID", id),
                    new SQLParameter("ComponentName", componentName),
                    new SQLParameter("Created", created),
                    new SQLParameter("Token", GetXMLString(content)),
                    new SQLParameter("TokenStatesID", (int)state)
                });
        }


        public void SaveLog(string componentName, DateTime created, int category, string message)
        {
            ExecuteProcedure("SaveLog",
                new SQLParameter[]
                {
                    new SQLParameter("ComponentName", componentName),
                    new SQLParameter("Created", created),
                    new SQLParameter("LogType", category),
                    new SQLParameter("LogMessage", message)
                });
        }

        public void SaveErrorAndLog(string componentName, DateTime created, XmlDocument errorContent, string logMessage)
        {
            ExecuteProcedure("SaveErrorAndLog",
                new SQLParameter[]
                {
                    new SQLParameter("ComponentName", componentName),
                    new SQLParameter("Created", created),
                    new SQLParameter("ErrorContent", GetXMLString(errorContent)),
                    new SQLParameter("LogMessage", logMessage)
                });
        }
    }
}
