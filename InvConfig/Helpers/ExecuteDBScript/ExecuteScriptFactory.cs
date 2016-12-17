using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InvConfig.Helper.ExcuteDBScript;

namespace InvConfig.Helpers.ExecuteDBScript
{
    public enum DatabaseType
    {
        SQLServer,
        DB2
    }
    public static class ExecuteScriptFactory
    {
        private const string CANNOT_FIND_FACTORY = "Cannot find Factory Class for "; 
        /// <summary>
        /// Decides which class to instantiate.
        /// </summary>
        public static ExcuteScript GetFactory(DatabaseType p_DatabaseType)
        {
            switch (p_DatabaseType)
            {
                case DatabaseType.SQLServer:
                    return new MSSQLExcuteScript();
                case DatabaseType.DB2:
                    throw new ApplicationException(CANNOT_FIND_FACTORY + p_DatabaseType);
                default:
                    throw new ApplicationException(CANNOT_FIND_FACTORY + p_DatabaseType);
            }
        }
    }
}
