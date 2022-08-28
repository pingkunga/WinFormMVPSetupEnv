using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.OleDb;

namespace Helpers.DBProvider
{
    //NOTE: This class must reference System.configuration.dll on your project
    public static class DBProviderHelper
    {
        private const string PROVIDER_NOT_FOUND = "PROVIDER NOT FOUND";
        private const string CONSTRING_NOT_FOUND = "CONNECTION STRING NOT FOUND";
        public static string GetProvider(string p_ConnName)
        {
            string connectionString;
            try
            {
                ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[p_ConnName];
                connectionString = connectionStringSettings.ProviderName.ToString();
            }
            catch 
            {
                throw new ApplicationException(PROVIDER_NOT_FOUND);
            }
            return connectionString;
        }
        public static string GetConnectionString(string p_ConnName)
        {
            ConnectionStringSettings connectionStringSettings;
            try
            {
                connectionStringSettings = ConfigurationManager.ConnectionStrings[p_ConnName];
            }
            catch(Exception ex)
            {
                throw new ApplicationException(CONSTRING_NOT_FOUND, ex.InnerException);
            }
            return connectionStringSettings.ConnectionString.ToString();
        }
        public static string GetConnectionString(this string p_ConnName, string p_ServerName, string p_DatabaseName)
        {
            ConnectionStringSettings connectionStringSettings;
            OleDbConnectionStringBuilder connectionStringBuilder;
            try
            {
                connectionStringSettings = ConfigurationManager.ConnectionStrings[p_ConnName];
                //msdn.microsoft.com/en-us/library/system.data.odbc.odbcconnectionstringbuilder(v=vs.110).aspx
                connectionStringBuilder = new OleDbConnectionStringBuilder();
                connectionStringBuilder.ConnectionString = connectionStringSettings.ConnectionString.ToString();
                //modified data
                connectionStringBuilder["Server"] = p_ServerName;
                connectionStringBuilder["Database"] = p_DatabaseName;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(CONSTRING_NOT_FOUND, ex.InnerException);
            }
            return connectionStringBuilder.ToString();
        }
        public static string GetODBCConnectionString(this string p_ConnName, string p_DSN, string p_UID, string p_PWD)
        {
            ConnectionStringSettings connectionStringSettings;
            OleDbConnectionStringBuilder connectionStringBuilder;
            try
            {
                connectionStringSettings = ConfigurationManager.ConnectionStrings[p_ConnName];
                //msdn.microsoft.com/en-us/library/system.data.odbc.odbcconnectionstringbuilder(v=vs.110).aspx
                connectionStringBuilder = new OleDbConnectionStringBuilder();
                connectionStringBuilder.ConnectionString = connectionStringSettings.ConnectionString.ToString();
                //modified data
                connectionStringBuilder["Dsn"] = p_DSN;
                connectionStringBuilder["Uid"] = p_UID;
                connectionStringBuilder["Pwd"] = p_PWD;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(CONSTRING_NOT_FOUND, ex.InnerException);
            }
            return connectionStringBuilder.ToString();
        }
        public static string GetConnectionString(this string p_ConnName, string p_ServerName, string p_DatabaseName, string p_UserName, string p_Password)
        {
            ConnectionStringSettings connectionStringSettings;
            OleDbConnectionStringBuilder connectionStringBuilder;
            try
            {
                connectionStringSettings = ConfigurationManager.ConnectionStrings[p_ConnName];
                //msdn.microsoft.com/en-us/library/system.data.odbc.odbcconnectionstringbuilder(v=vs.110).aspx
                connectionStringBuilder = new OleDbConnectionStringBuilder();
                connectionStringBuilder.ConnectionString = connectionStringSettings.ConnectionString.ToString();
                //modified data
                connectionStringBuilder["Server"] = p_ServerName;
                connectionStringBuilder["Database"] = p_DatabaseName;
                connectionStringBuilder["User Id"] = p_UserName;
                connectionStringBuilder["Password"] = p_Password;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(CONSTRING_NOT_FOUND, ex.InnerException);
            }
            return connectionStringBuilder.ToString();
        }
    }
}
