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
    /// <summary>
    /// Implements a data access object with a MS SQL Server being the data
    /// storage.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All the queries are performed by executing a SQL procedure on the
    /// database server.
    /// </para>
    /// <para>
    /// All SQL-related exceptions are propagated outside.
    /// </para>
    /// </remarks>
    public class MsSqlDataAccess : IDataAccess
    {
        private string connectionString;

        /// <summary>
        /// Creates a new instance of the data access object.
        /// NOTE: it must be initialized first via the Initialize() method!
        /// </summary>
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

        public void SaveToken(Token token)
        {
            string tokenXml = token.Content.XDocument.ToString();
            string inputMessage = token.GetMessage("input").ToString();
            EndpointAddress endpointAddress = token.GetSourceAddress();
            
            ExecuteProcedure("SaveToken",
                new SqlParameter[]
                {
                    new SqlParameter("InputGUID", token.Guid),
                    new SqlParameter("Token", tokenXml),
                    new SqlParameter("TokenState", token.State.ToString()),
                    new SqlParameter("Message", inputMessage),
                    (token.Created == DateTime.MinValue ? new SqlParameter("Created", DBNull.Value): new SqlParameter("Created", token.Created)),
                    (token.Received == DateTime.MinValue ? new SqlParameter("Received", DBNull.Value): new SqlParameter("Received", token.Received)),
                    (token.Dispatched == DateTime.MinValue ? new SqlParameter("Dispatched", DBNull.Value): new SqlParameter("Dispatched", token.Dispatched)),
                    (token.Finished == DateTime.MinValue ? new SqlParameter("Finished", DBNull.Value): new SqlParameter("Finished", token.Finished)),
                    new SqlParameter("IsPersistent", token.IsPersistent),
                    new SqlParameter("AdapterName", endpointAddress.AdapterName),
                    new SqlParameter("EndpointName", endpointAddress.EndpointName)
                }).Close();
        }

        public string LoadToken(Guid tokenGuid)
        {
            string token = null;

            Response response = ExecuteProcedure("GetToken",
                new SqlParameter[]
                {
                    new SqlParameter("InputGUID", tokenGuid)
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
                throw;
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
