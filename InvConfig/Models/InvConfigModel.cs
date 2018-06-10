using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace InvConfig.Models
{
    [Serializable()]
    public class InvConfigModel
    {
        public int configID { get; set; }
        public string configName { get; set; }
        public string configCategory { get; set; }
        public string configRemark { get; set; }
        public string bnzServerPath { get; set; }
        public string bnzLocalPath { get; set; }
        public string databaseType { get; set; }
        public string bnzDatabaseServer { get; set; }
        public Boolean bnzISODBC { get; set; }
        public string bnzDatabaseName { get; set; }
        public string bnzDatabasePort { get; set; }
        public string bnzInterfaceServer { get; set; }
        public string bnzInterfacePort { get; set; }
        public string isIncludeRPTServer { get; set; }
        public string bnzRPTDatabaseName { get; set; }

        public string StartUpMSNetAs { get; set; }

        public string bnzUpdatePath { get; set; }
        public string bnzInstallPath { get; set; }
        public string bnzLastUpdateScript { get; set; }
        public string bnzLastUpdateScriptDate { get; set; }
        public string bnzDatabaseUsername { get; set; }
        public string bnzDatabasePassword { get; set; }
        public DateTime configCreateDate { get; set; }
        public DateTime configModifiedDate { get; set; }
        public string configBaseRegistry { get; set; }
        public Boolean bnzISNewEncryption { get; set; }
        public string bnzWindowsUsername { get; set; }
        public string bnzWindowsPassword { get; set; }

        public Boolean IsUpdateBUCProperties { get; set; }
        //Flag 
        public Boolean isInstallDllAndOCX { get; set; }
        public Boolean isInstallInterfaceService { get; set; }
        //

        
    }
}
