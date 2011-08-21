using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XRouter.Common;

namespace XRouter.Data
{
    public class MsSqlDataAccess : IDataAccess
    {
        private string connectionString;

        public MsSqlDataAccess () {}

        #region Public methods - interface IDataAccess

        public void Initialize(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void SaveConfiguration(string configXml)
        {
            ExecuteProcedure("SaveConfig",
                new SqlParameter[]
                {
                    new SqlParameter("Config", configXml)
                }).Close();
        }

        public string LoadConfiguration()
        {
            string config = null;

            Response response = ExecuteProcedure("GetConfig", new SqlParameter[] { });
            if (response.sqlDataReader.Read())
            {
                config = response.sqlDataReader[0].ToString();
            }
            response.Close();

            return config;
        }

        public void SaveToken(Guid tokenGuid, string tokenXml)
        {
            ExecuteProcedure("SaveToken",
                new SqlParameter[]
                {
                    new SqlParameter("MessageGUID", tokenGuid),
                    new SqlParameter("Token", tokenXml)
                }).Close();
        }

        public void SaveToken(Token token)
        {
            string tokenXml = token.Content.XDocument.ToString();
            string inputMessage = token.GetMessage("input").ToString();
            //TODO

            SaveToken(token.Guid, tokenXml);
        }

        public string LoadToken(Guid tokenGuid)
        {
            string token = null;

            Response response = ExecuteProcedure("GetToken",
                new SqlParameter[]
                {
                    new SqlParameter("MessageGUID", tokenGuid)
                });
            if (response.sqlDataReader.Read())
            {
                token = response.sqlDataReader[0].ToString();
            }
            response.Close();

            return token;
        }

        public IEnumerable<string> LoadTokens(int pageSize, int pageNumber)
        {
            List<string> tokens = new List<string>();

            Response response = ExecuteProcedure("GetTokens",
                new SqlParameter[]
                {
                    new SqlParameter("PageNumber", pageNumber),
                    new SqlParameter("PageSize", pageSize)
                });
            while (response.sqlDataReader.Read())
            {
                string token = response.sqlDataReader[0].ToString();

                tokens.Add(token);
            }
            response.Close();

            return tokens.ToArray();
        }

        public IEnumerable<string> LoadMatchingTokens(string xpath)
        {
            List<string> tokens = new List<string>();

            Response response = ExecuteProcedure("GetMatchingTokens",
                new SqlParameter[]
                {
                    new SqlParameter("XPath", xpath)
                });
            while (response.sqlDataReader.Read())
            {
                string token = response.sqlDataReader[0].ToString();

                tokens.Add(token);
            }
            response.Close();

            return tokens.ToArray();
        }

        #endregion

        private Response ExecuteProcedure(string procedure, SqlParameter[] parameters)
        {
            SqlDataReader sqlDataReader;
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = connectionString;
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
                throw; //TODO - specialized exception?
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
