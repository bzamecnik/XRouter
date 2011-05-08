using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace XRouter.Data
{
    public class MsSqlDataAccess : IDataAccess
    {
        private string IPAddress;
        private string instance;
        private string DBName;

        private string login = "XRouter_AccessDB";
        private string password = "XRouter";

        private string ConnectionString;

        public MsSqlDataAccess
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

        #region Public methods - interface IDataAccess

        public List<XDocument> GetAllMessages()
        {
            List<XDocument> messages = new List<XDocument>();

            Response response = ExecuteProcedure("GetAllMessages", new SqlParameter[] { });
            while (response.sqlDataReader.Read())
            {
                XDocument xml = XDocument.Parse(response.sqlDataReader[0].ToString());

                messages.Add(xml);
            }
            response.Close();

            return messages;
        }

        public XDocument GetMessage(int messageID)
        {
            XDocument message = new XDocument();

            Response response = ExecuteProcedure("GetMessage",
                new SqlParameter[]
                {
                    new SqlParameter("MessageID", messageID)
                });
            if (response.sqlDataReader.Read())
            {
                message = XDocument.Parse(response.sqlDataReader[0].ToString());
            }
            response.Close();

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
                new SqlParameter[]
                {
                    new SqlParameter("ComponentName", componentName),
                    new SqlParameter("Interests", interestsString),
                    new SqlParameter("Separator", separator)
                }).Close();
        }

        public IEnumerable<string> GetComponentConfigInterests(string componentName)
        {
            List<string> interests = new List<string>();

            string parameters = string.Format
                (
                    " @ComponentName='{0}'",
                    componentName
                );
            Response response = ExecuteProcedure("GetComponentConfigInterests",
                new SqlParameter[]
                {
                    new SqlParameter("ComponentName", componentName)
                });
            while (response.sqlDataReader.Read())
            {
                string interest = response.sqlDataReader[0].ToString();

                interests.Add(interest);
            }
            response.Close();

            return interests;
        }

        public void SaveConfig(XDocument config)
        {
            ExecuteProcedure("SaveConfig",
                new SqlParameter[]
                {
                    new SqlParameter("Config", GetXMLString(config))
                }).Close();
        }

        public XDocument GetConfig()
        {
            XDocument config = new XDocument();

            Response response = ExecuteProcedure("GetConfig", new SqlParameter[] { });
            if (response.sqlDataReader.Read())
            {
                config = XDocument.Parse(response.sqlDataReader[0].ToString());
            }
            response.Close();

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
                new SqlParameter[]
                {
                    new SqlParameter("ComponentName", name),
                    new SqlParameter("Interests", interestsString),
                    new SqlParameter("Separator", separator),
                    new SqlParameter("Address", address)
                }).Close();
        }

        public void UnregisterComponent(string name)
        {
            ExecuteProcedure("UnregisterComponent",
                new SqlParameter[]
                {
                    new SqlParameter("ComponentName", name)
                }).Close();
        }

        public int SaveMessage(DateTime created, XDocument content, string source)
        {
            int messageID = -1;

            Response response = ExecuteProcedure("SaveMessage",
                new SqlParameter[]
                {
                    new SqlParameter("Created", created),
                    new SqlParameter("Content", GetXMLString(content)),
                    new SqlParameter("EndpointName", source)
                });
            if (response.sqlDataReader.Read())
            {
                messageID = Int32.Parse(response.sqlDataReader[0].ToString());
            }
            response.Close();

            return messageID;
        }

        public void SaveToken(int id, string componentName, DateTime created, XDocument content, TokenState state)
        {
            ExecuteProcedure("SaveToken",
                new SqlParameter[]
                {
                    new SqlParameter("MessagesID", id),
                    new SqlParameter("ComponentName", componentName),
                    new SqlParameter("Created", created),
                    new SqlParameter("Token", GetXMLString(content)),
                    new SqlParameter("TokenStatesID", (int)state)
                }).Close();
        }


        public void SaveLog(string componentName, DateTime created, int category, string message)
        {
            ExecuteProcedure("SaveLog",
                new SqlParameter[]
                {
                    new SqlParameter("ComponentName", componentName),
                    new SqlParameter("Created", created),
                    new SqlParameter("LogType", category),
                    new SqlParameter("LogMessage", message)
                }).Close();
        }

        public void SaveErrorAndLog(string componentName, DateTime created, XDocument errorContent, string logMessage)
        {
            ExecuteProcedure("SaveErrorAndLog",
                new SqlParameter[]
                {
                    new SqlParameter("ComponentName", componentName),
                    new SqlParameter("Created", created),
                    new SqlParameter("ErrorContent", GetXMLString(errorContent)),
                    new SqlParameter("LogMessage", logMessage)
                }).Close();
        }

        #endregion

        private Response ExecuteProcedure(string procedure, SqlParameter[] parameters)
        {
            SqlDataReader sqlDataReader;
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = ConnectionString;
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = "executable." + procedure;
            foreach (SqlParameter parameter in parameters)
            {
                sqlCommand.Parameters.Add(parameter);
            }

            try
            {
                sqlDataReader = sqlCommand.ExecuteReader();
            }
            catch (Exception)
            {
                //TODO throw specialized exceptions...
                sqlDataReader = null;
            }

            return new Response(sqlConnection, sqlDataReader);
        }

        private string GetXMLString(XDocument xml)
        {
            StringBuilder sb = new StringBuilder();
            using (XmlWriter xw = XmlWriter.Create(sb))
            {
                xml.WriteTo(xw);
            }

            return sb.ToString();
        }

        private class Response
        {
            public Response(SqlConnection sqlConnection, SqlDataReader sqlDataReader)
            {
                this.sqlConnection = sqlConnection;
                this.sqlDataReader = sqlDataReader;
            }

            public void Close()
            {
                sqlDataReader.Close();
                sqlConnection.Close();
            }

            private SqlConnection sqlConnection;
            public SqlDataReader sqlDataReader { get; private set; }
        }
    }
}
