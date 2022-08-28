using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Helpers.Registrys;
using Helpers.file.Config;
using Helpers.Cryptography;

namespace InvConfig.Models
{
    public class InvConfigMapper
    {
        //CONSTANT SECTION
        private const string DATABASE = "Database";
        private const string DATABASE_FILE = "InvConfig.bin";
        private const string DATABASE_NO_FOUND = "Database File not exist !!!";
        
        //PROPERTY SECTION
        private string DatabaseFilePath
        {
            get{return Application.StartupPath + "\\" + DATABASE + "\\" + DATABASE_FILE;}
        }

        private string DatabasePath
        {
            get { return Application.StartupPath + "\\" + DATABASE; }
        }


        public void SetLocalEnviromentInvConfig(InvConfigModel p_InvConfigModel)
        {
            RegistryHelper.BaseRegistryKey = RegistryHelper.GetRegistryBaseFormString(p_InvConfigModel.configBaseRegistry);
            RegistryHelper.RegistryPath = "SOFTWARE\\Bonanza\\Investment";
            if (RegistryHelper.checkRegistryExist(true))
            {
                RegistryHelper.writeRegistryValue("LOCAL PATH", p_InvConfigModel.bnzLocalPath);
                RegistryHelper.writeRegistryValue("SERVER PATH", p_InvConfigModel.bnzServerPath);
                RegistryHelper.writeRegistryValue("DATABASE", p_InvConfigModel.databaseType);
                RegistryHelper.writeRegistryValue("DB SERVER NAME", p_InvConfigModel.bnzDatabaseServer);
                RegistryHelper.writeRegistryValue("ODBC SERVER", p_InvConfigModel.bnzDatabaseName);
                RegistryHelper.writeRegistryValue("ODBC REPORT", p_InvConfigModel.bnzRPTDatabaseName);
                RegistryHelper.writeRegistryValue("SERVER NAME", p_InvConfigModel.bnzInterfaceServer);
                RegistryHelper.writeRegistryValue("PORT", p_InvConfigModel.bnzInterfacePort);
                RegistryHelper.writeRegistryValue("WinUserName", p_InvConfigModel.bnzWindowsUsername);
                RegistryHelper.writeRegistryValue("WinPassword", p_InvConfigModel.bnzWindowsPassword);
            }
        }

        public InvConfigModel GetLocalEnviromentInvConfig()
        {
            InvConfigModel configEnvModel = new InvConfigModel();
            //RegistryHelper.BaseRegistryKey = RegistryHelper.GetRegistryBaseFormString(p_InvConfigModel.configBaseRegistry);
            RegistryHelper.RegistryPath = "SOFTWARE\\Bonanza\\Investment";
            if (RegistryHelper.checkRegistryExist())
            {
                configEnvModel.bnzLocalPath = RegistryHelper.readRegistryValue("LOCAL PATH");
                configEnvModel.bnzServerPath = RegistryHelper.readRegistryValue("SERVER PATH");
                configEnvModel.databaseType = RegistryHelper.readRegistryValue("DATABASE");
                configEnvModel.bnzDatabaseServer = RegistryHelper.readRegistryValue("DB SERVER NAME");
                configEnvModel.bnzDatabaseName = RegistryHelper.readRegistryValue("ODBC SERVER");
                configEnvModel.bnzRPTDatabaseName = RegistryHelper.readRegistryValue("ODBC REPORT");
                configEnvModel.bnzInterfaceServer = RegistryHelper.readRegistryValue("SERVER NAME");
                configEnvModel.bnzInterfacePort = RegistryHelper.readRegistryValue("PORT");
                configEnvModel.bnzWindowsUsername = RegistryHelper.readRegistryValue("WinUserName");
                configEnvModel.bnzWindowsPassword = RegistryHelper.readRegistryValue("WinPassword");
                //configEnvModel.bnzDatabaseUsername = Properties.Resources.DefaultDBUsername;
                //configEnvModel.bnzDatabasePassword = Properties.Resources.DefaultDBPassword;
            }
            return configEnvModel;
            //throw new NotImplementedException();
        }

        public List<InvConfigModel> GetAllInvConfig()
        {
            try
            {
                //=========================================
                //check database file exist
                if (File.Exists(DatabaseFilePath))
                {
                    //=========================================
                    //read data from file
                    using (Stream stream = File.Open(DatabaseFilePath, FileMode.Open))
                    {
                        BinaryFormatter bin = new BinaryFormatter();
                        return (List<InvConfigModel>)bin.Deserialize(stream);
                    }
                }
                else
                {
                    //=========================================
                    //return empty list
                    return new List<InvConfigModel>();
                }
            }
            catch (IOException)
            {
                throw new NotImplementedException();
            }
        }

        public InvConfigModel GetInvConfigByIDAndName(int p_ConfigID, string p_ConfigName)
        {
            try
            {
                //Get all database config
                List<InvConfigModel> InvConfigList = GetAllInvConfig();
                InvConfigModel invConfigModel = InvConfigList.FirstOrDefault(config => (config.configID == p_ConfigID && config.configName == p_ConfigName)) ?? null;
                if (invConfigModel == null)
                {
                    throw new ApplicationException(Properties.Resources.ExCannotGetSpecificEnvConfig);
                }
                return invConfigModel;
            }
            catch (IOException)
            {
                throw new NotImplementedException();
            }
        }

        public int SaveInvConfig(InvConfigModel p_InvConfigModel)
        {
            int lastConfigID = 0;
            try
            {
                //MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM
                //        SERIALIZE OBJECT TO FILE
                //WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW
                //Get all database config
                List<InvConfigModel> InvConfigList = GetAllInvConfig();
                //=========================================
                //Check Config ID
                if (p_InvConfigModel.configID == 0)
                {
                    //Get last Config ID
                    if (InvConfigList.Count > 0)
                    {
                        lastConfigID = InvConfigList.First(config => config.configID == InvConfigList.Max(obj => obj.configID)).configID;
                    }
                    //Generate new Config ID
                    lastConfigID = lastConfigID + 1;
                    p_InvConfigModel.configID = lastConfigID;
                    //Add new List
                    p_InvConfigModel.configCreateDate = DateTime.Now;
                    InvConfigList.Add(p_InvConfigModel);
                }
                else
                {
                    //Get index
                    int configIdx = InvConfigList.IndexOf(InvConfigList.First(config => config.configID == p_InvConfigModel.configID));
                    //Update Object in list
                    lastConfigID = p_InvConfigModel.configID;
                    p_InvConfigModel.configModifiedDate = DateTime.Now;
                    InvConfigList.RemoveAt(configIdx);
                    InvConfigList.Insert(configIdx, p_InvConfigModel);
                }
                //=========================================
                //Check Folder Exist
                if (!Directory.Exists(DatabasePath))
                {
                    Directory.CreateDirectory(DatabasePath);
                }
                //=========================================
                //Save List into file
                using (Stream stream = File.Open(DatabaseFilePath, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, InvConfigList);
                }
                //MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM
                //        SAVE CONFIG TO REGISTRY
                //WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW
                SetLocalEnviromentInvConfig(p_InvConfigModel);
                //MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM
                //        SAVE CONFIG TO BUC.propertiess
                //WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW
                if (p_InvConfigModel.IsUpdateBUCProperties)
                {
                    UpdateBUCProperties(p_InvConfigModel);
                }
            }
            catch (IOException Ex) {
                Console.WriteLine(Ex.Message);               
                throw new NotImplementedException();
            }
            return lastConfigID;
        }

        private void UpdateBUCProperties(InvConfigModel p_InvConfigModel)
        {
            //Default BUC Path
            String BUCPropertiesPath = p_InvConfigModel.bnzLocalPath + "\\Operation\\buc.properties";

            //Check File Exist
            if (!File.Exists(BUCPropertiesPath))
            {
                throw new Exception("Not found Config in path " + BUCPropertiesPath);
            }
            //Read BUC
            Dictionary<string, string> properties = PropertyHelper.LoadConfig(BUCPropertiesPath);

            if (properties == null)
                return;

            #region PATH
            if (properties.ContainsKey("INV_LOCAL PATH"))
                properties["INV_LOCAL PATH"] = p_InvConfigModel.bnzLocalPath;
            if (properties.ContainsKey("INV_SERVER PATH"))
                properties["INV_SERVER PATH"] = p_InvConfigModel.bnzServerPath;

            if (properties.ContainsKey("NET_BonanzaPath"))
                properties["NET_BonanzaPath"] = p_InvConfigModel.bnzServerPath;
            else
                properties["NET_BonanzaPath"] = @"C:\Bonanza\BFM\";

            if (properties.ContainsKey("INV_RUNAS64BIT"))
                properties["INV_RUNAS64BIT"] = p_InvConfigModel.StartUpMSNetAs;
            #endregion PATH

            #region Database
            if (properties.ContainsKey("INV_ODBC SERVER"))
                properties["INV_ODBC SERVER"] = p_InvConfigModel.bnzDatabaseName;
            if (properties.ContainsKey("INV_ODBC REPORT"))
                properties["INV_ODBC REPORT"] = p_InvConfigModel.bnzRPTDatabaseName;
            if (properties.ContainsKey("INV_DB SERVER NAME"))
                properties["INV_DB SERVER NAME"] = p_InvConfigModel.bnzDatabaseServer;
            #region >>.NET
            if (properties.ContainsKey("NET_DbProvider"))
            {
                if (p_InvConfigModel.databaseType.Equals(Properties.Resources.ConstSQLServer))
                {
                    properties["NET_DbProvider"] = "System.Data.SqlClient";
                }
                else if (p_InvConfigModel.databaseType.Equals(Properties.Resources.ConstDB2))
                {
                    properties["NET_DbProvider"] = "IBM.Data.DB2";
                }
                else
                {
                    properties["NET_DbProvider"] = "Error !!!" + p_InvConfigModel.databaseType;
                }
            }
                

            if (properties.ContainsKey("NET_Server"))
                properties["NET_Server"] = p_InvConfigModel.bnzDatabaseServer;
            if (properties.ContainsKey("NET_Port"))
                properties["NET_Port"] = p_InvConfigModel.bnzDatabasePort;

            if (properties.ContainsKey("NET_DatabaseName"))
                properties["NET_DatabaseName"] = p_InvConfigModel.bnzDatabaseName;

            if (properties.ContainsKey("NET_User"))
                properties["NET_User"] = Rijndael.EncryptData(p_InvConfigModel.bnzDatabaseUsername
                                                            , ""
                                                            , Rijndael.BlockSize.Block256
                                                            , Rijndael.KeySize.Key256
                                                            , Rijndael.EncryptionMode.ModeECB
                                                            , true);
            if (properties.ContainsKey("NET_Password"))
                properties["NET_Password"] = Rijndael.EncryptData(p_InvConfigModel.bnzDatabaseUsername
                                                            , ""
                                                            , Rijndael.BlockSize.Block256
                                                            , Rijndael.KeySize.Key256
                                                            , Rijndael.EncryptionMode.ModeECB
                                                            , true);

            if (properties.ContainsKey("NET_DefaultSchema"))
            {
                if (p_InvConfigModel.databaseType.Equals(Properties.Resources.ConstDB2))
                {
                    properties["NET_DefaultSchema"] = "INVEST";
                }
            }
            #endregion >>.NET

            #region >> INTERFACE
            if (properties.ContainsKey("NET_InterfacePort"))
                properties["NET_InterfacePort"] = p_InvConfigModel.bnzInterfacePort;
            if (properties.ContainsKey("NET_InterfaceServer"))
                properties["NET_InterfaceServer"] = p_InvConfigModel.bnzInterfaceServer;

            if (properties.ContainsKey("INV_PORT"))
                properties["INV_PORT"] = p_InvConfigModel.bnzInterfacePort;
            if (properties.ContainsKey("INV_SERVER NAME"))
                properties["INV_SERVER NAME"] = p_InvConfigModel.bnzInterfaceServer;
            if (properties.ContainsKey("INV_WinUserName"))
                properties["INV_WinUserName"] = p_InvConfigModel.bnzWindowsUsername;
            if (properties.ContainsKey("INV_WinPassword"))
                properties["INV_WinPassword"] = Rijndael.EncryptData(p_InvConfigModel.bnzWindowsPassword
                                                            , ""
                                                            , Rijndael.BlockSize.Block256
                                                            , Rijndael.KeySize.Key256
                                                            , Rijndael.EncryptionMode.ModeECB
                                                            , true);
            #endregion >> INTERFACE
            #endregion

            //Save Result
            PropertyHelper.ClearOldConfig(BUCPropertiesPath);

            PropertyHelper.WriteConfig(BUCPropertiesPath, properties);
        }

        public Boolean DeleteConfig(int p_ConfigID)
        {
            try
            {
                //Get all database config
                List<InvConfigModel> InvConfigList = GetAllInvConfig();
                //=========================================
                //Get index
                int configIdx = InvConfigList.IndexOf(InvConfigList.First(config => config.configID == p_ConfigID));
                //remove it
                InvConfigList.RemoveAt(configIdx);
                //=========================================
                //Save List into file
                using (Stream stream = File.Open(DatabaseFilePath, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, InvConfigList);
                }
            }
            catch (IOException)
            {
                throw new NotImplementedException();
            }
            return true;
        }


    }
}
