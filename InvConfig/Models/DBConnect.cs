using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using Helpers.DBProvider;
using System.Net.NetworkInformation;

namespace InvConfig.Models
{
    public class DBConnect
    {
        private const string CANNOT_QUERY = "Cannot query on specific query: ";
        private string databaseType;
        private Boolean ISODBC;
        private string databaseServer;
        private string databaseName;
        private string databaseUsername;
        private string databasePassword;
        private string currentDBVersion;
        private string exceptionMessage;
        public string CurrentDBConnVersion
        {
            get { return currentDBVersion; }
            set { currentDBVersion = value; }
        }
        public string ExcepionMessage
        {
            get { return exceptionMessage; }
            set { exceptionMessage = value; }
        }
        public void SetDatabaseProperties(string p_DatabaseType, Boolean p_ISODBC, string p_DBServer, string p_DBName, string p_DBUsername, string p_DBPassword)
        {
            this.databaseType = p_DatabaseType;
            this.ISODBC = p_ISODBC;
            this.databaseServer = p_DBServer;
            this.databaseName = p_DBName;
            this.databaseUsername = p_DBUsername;
            this.databasePassword = p_DBPassword;
        }
        public DbConnection GetConnection()
        {
            DbConnection connection = null;
            try
            {
                string provider = null;
                string connString = null;
                //Check ODBC
                if (ISODBC == true)
                {
                    //connect to ODBC for DB2
                    provider = DBProviderHelper.GetProvider("ODBC");
                    connString = DBProviderHelper.GetODBCConnectionString("ODBC",
                                                                           databaseName,
                                                                           databaseUsername,
                                                                           databasePassword);                
                }
                else
                {
                    provider = DBProviderHelper.GetProvider(databaseType);
                    connString = DBProviderHelper.GetConnectionString(databaseType,
                                                                      databaseServer,
                                                                      databaseName,
                                                                      databaseUsername,
                                                                      databasePassword);
                }

                if (!String.IsNullOrEmpty(connString))
                {
                    DbProviderFactory factory = DbProviderFactories.GetFactory(provider);
                    connection = factory.CreateConnection();
                    connection.ConnectionString = connString;
                }
            }
            catch (Exception ex)
            {
                // Set the connection to null if it was created. 
                if (connection != null)
                {
                    connection = null;
                }
                throw new Exception(ex.Message);
            }
            // Return the connection. 
            return connection;
        }

        public Boolean TestServeravailability(string p_ServerAddress)
        {
            Boolean status = false;
            using (Ping ping = new Ping())
            {
                try
                {
                    PingReply reply = ping.Send(p_ServerAddress, 200);
                    if (reply.Status == IPStatus.Success)
                    {
                        status = true;
                        Console.WriteLine("Success - IP Address:{0}", reply.Address, reply.RoundtripTime);
                    }
                    else
                    {
                        status = false;
                        Console.WriteLine(reply.Status);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error ({0})",
                        ex.InnerException.Message);
                }
            }
            return status;
        }

		public Boolean TestConnection()
		{
			using(DbConnection connection = GetConnection())
			{
                try
                {
                    connection.Open();
                    #region DB Version
                    string serverVersion = connection.ServerVersion;
                    string[] serverVersionDetails = serverVersion.Split(new string[] { "." }, StringSplitOptions.None);
                    int versionNumber = int.Parse(serverVersionDetails[0]);
                    if (databaseType == Properties.Resources.ConstSQLServer)
                    {
                        switch (versionNumber)
                        {
                            case 8:
                            CurrentDBConnVersion = "SQL Server 2000";
                            break;
                            case 9:
                            CurrentDBConnVersion = "SQL Server 2005";
                            break;
                            case 10:
                            CurrentDBConnVersion = "SQL Server 2008";
                            break;
                            case 11:
                            CurrentDBConnVersion = "SQL Server 2012";
                            break;
                            case 12:
                            CurrentDBConnVersion = "SQL Server 2014";
                            break;
                            default:
                            CurrentDBConnVersion = string.Format("SQL Server {0}", versionNumber.ToString());
                            break;
                        }
                    }
                    else if (databaseType == Properties.Resources.ConstDB2)
                    {
                        CurrentDBConnVersion = string.Format("DB2 {0}", versionNumber.ToString());
                    }
                    #endregion DB Version
                    return true;
                }
                catch (DbException ex)
                {
                    ExcepionMessage = ex.Message;
                    //write log
                    return false;
                }
            }
		}

        public Boolean CheckTableExist(string p_TableName)
        {
            using (DbConnection connection = GetConnection())
            {
                bool tableExists;
                StringBuilder sqlQuery = new StringBuilder();
                try
                {
                    connection.Open();
                }
                catch
                {
                    throw new ApplicationException("Cannot create conection");
                }
                try
                {
                    using (DbCommand command = connection.CreateCommand())
                    {
                        // ANSI SQL way.  Works in PostgreSQL, MSSQL, MySQL. 
                        sqlQuery.Append("SELECT CASE ");
                        sqlQuery.Append("WHEN EXISTS((SELECT * ");
                        sqlQuery.Append("   FROM   information_schema.TABLES ");
                        sqlQuery.Append("   WHERE  TABLE_NAME = '" + p_TableName + "')) THEN 1 ");
                        sqlQuery.Append("ELSE 0 ");
                        sqlQuery.Append("END");
                        command.CommandText = sqlQuery.ToString();
                        command.CommandType = CommandType.Text;

                        tableExists = (int)command.ExecuteScalar() == 1;
                    }
                }
                catch
                {
                    try
                    {
                        // Other RDBMS.  Graceful degradation
                        tableExists = true;
                        using (DbCommand command = connection.CreateCommand())
                        {
                            sqlQuery.Clear();
                            sqlQuery.Append("SELECT 1 FROM " + p_TableName + " WHERE 1 = 0");
                            command.CommandText = sqlQuery.ToString();
                            command.CommandType = CommandType.Text;
                            command.ExecuteNonQuery();
                        }
                    }
                    catch
                    {
                        tableExists = false;
                    }
                }
                return tableExists;
            }
            
        }
        public DataTable RetrieveData(string p_Query)
        {
            using (DbConnection connection = GetConnection())
            {   
                try
                {
                    connection.Open();
                    using (DbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = p_Query;
                        command.CommandType = CommandType.Text;

                        DataTable resultData = new DataTable();
                        resultData.Load(command.ExecuteReader());
                        return resultData;
                    }
                }
                catch
                {
                    throw new ApplicationException(CANNOT_QUERY + p_Query); 
                    //Write Log
                }
            }
        }
    }
}
