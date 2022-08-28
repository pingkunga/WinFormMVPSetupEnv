using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InvConfig.Views;
using Helpers.Files;
using Helpers.DataSource;
using InvConfig.Models;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using InvConfig.Helpers.Registry;
using System.Data;
using Helpers.Cryptography;
using System.ComponentModel;
using Helpers.cryptography;
using Helpers.Service;
using InvConfig.Helper.ExcuteDBScript;
using Helpers.DBProvider;
using InvConfig.Helper.Event;
using Helpers.Controls;
using InvConfig.Helpers.ExecuteDBScript;
using System.Windows.Forms;
using InvConfig.Helper.ImportExport;
using CsvHelper;
using System.Threading;

namespace InvConfig.Presenters
{
    public class InvConfigPresenter : Presenter<IView>
    {
        private readonly IInvConfigView configView;
        private readonly InvConfigMapper invConfigMapper;
        private readonly DBConnect dbConnect;
        private readonly BackgroundWorker databaseWorker;
        private DoWorkEventHandler databaseWorkerDoWork;
        private RunWorkerCompletedEventHandler databaseWorkerRunComplete;

        private InvConfigModel ViewInvConfig
        {
            get
            {
                InvConfigModel invConfigModel = new InvConfigModel();
                invConfigModel.configID = configView.ConfigID;
                invConfigModel.configName = configView.ConfigName;
                invConfigModel.configCategory = configView.ConfigType;
                invConfigModel.bnzLocalPath = configView.LocalPath;
                invConfigModel.bnzServerPath = configView.ServerPath;
                invConfigModel.databaseType = configView.DatebaseType;
                invConfigModel.bnzDatabaseServer = configView.DatabaseServer;
                invConfigModel.bnzISODBC = configView.IsODBC;

                invConfigModel.bnzDatabaseName = configView.DatabaseName;
                invConfigModel.bnzDatabasePort = configView.DatabasePort;

                invConfigModel.bnzRPTDatabaseName = configView.RPTDatabaseName;
                invConfigModel.bnzInterfaceServer = configView.InterfaceServer;
                invConfigModel.bnzInterfacePort = configView.InterfacePort;
                invConfigModel.bnzDatabaseUsername = configView.DatabaseUsername;
                invConfigModel.bnzDatabasePassword = configView.DatabasePassword;
                invConfigModel.bnzLastUpdateScript = configView.ScriptNumber;
                invConfigModel.configRemark = configView.Remark;
                invConfigModel.configBaseRegistry = configView.BaseRegistryPath;
                invConfigModel.IsUpdateBUCProperties = configView.IsUpdateBUCProperties;

                invConfigModel.bnzISNewEncryption = configView.ISNewEncryption;
                invConfigModel.bnzWindowsUsername = configView.WindowsUsername;
                invConfigModel.bnzWindowsPassword = configView.WindowsPassword;

                invConfigModel.StartUpMSNetAs = configView.StartUpMSNetAs;
                return invConfigModel;
            }
            set
            {
                configView.ConfigID = value.configID;
                configView.ConfigName = value.configName;
                configView.ConfigType = value.configCategory;
                configView.LocalPath = value.bnzLocalPath;
                configView.ServerPath = value.bnzServerPath;
                configView.DatebaseType = value.databaseType;
                configView.DatabaseServer = value.bnzDatabaseServer;
                configView.IsODBC = value.bnzISODBC;
                configView.DatabaseName = value.bnzDatabaseName;
                configView.DatabasePort = value.bnzDatabasePort;
                configView.RPTDatabaseName = value.bnzRPTDatabaseName;
                configView.InterfaceServer = value.bnzInterfaceServer;
                configView.InterfacePort = value.bnzInterfacePort;
                configView.DatabaseUsername = value.bnzDatabaseUsername;
                configView.DatabasePassword = value.bnzDatabasePassword;
                configView.ScriptNumber = value.bnzLastUpdateScript;
                configView.Remark = value.configRemark;
                configView.BaseRegistryPath = value.configBaseRegistry;
                configView.IsUpdateBUCProperties = value.IsUpdateBUCProperties;

                configView.ISNewEncryption = value.bnzISNewEncryption;
                configView.WindowsUsername = value.bnzWindowsUsername;
                configView.WindowsPassword = value.bnzWindowsPassword;

                configView.StartUpMSNetAs = value.StartUpMSNetAs;
            }
        }
        public InvConfigPresenter(IInvConfigView view) : base(view)
        {
            invConfigMapper = new InvConfigMapper();
            dbConnect = new DBConnect();
            //Background thread
            databaseWorker = new BackgroundWorker();
            databaseWorker.WorkerReportsProgress = true;
            databaseWorker.WorkerSupportsCancellation = true;
            //Background thread
            this.configView = view;
            this.OnViewInitialize();
            this.ShowView();
        }

        public override void OnViewInitialize()
        {
            try
            {
                base.OnViewInitialize();
                BindToolStripEvent();
                BindTabEvent();
                BindEnvConfigEvent();
                InitDatabaseType();
                InitStartupAppDataSource();
                InitODBCDataSource();
                InitBaseRegistryPath();
                BindInvestRegEvent();
                BindCryptographyEvent();
                BindRunScriptEvent();
                //=========================================
                //Load Last Config
                this.ViewInvConfig = invConfigMapper.GetInvConfigByIDAndName(Properties.Settings.Default.LastConfigID, Properties.Settings.Default.LastConfigName);
                LisBNZDBVersion();
            }
            catch(ApplicationException ex)
            {
                //Load config form registry
                if (ex.Message.Equals(Properties.Resources.ExCannotGetSpecificEnvConfig))
                {
                    this.ViewInvConfig = invConfigMapper.GetLocalEnviromentInvConfig();
                    log.Warn(ex.Message);
                }
            }
            catch(Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }
        private void InitDatabaseType()
        {
            this.configView.DBTypeDataSource = new Dictionary<string, object>{
                { Properties.Resources.ConstSQLServer , DatabaseType.SQLServer },
                { Properties.Resources.ConstDB2, DatabaseType.DB2 }
            };
        }

        private void InitStartupAppDataSource()
        {
            this.configView.StartupAppDataSource = new Dictionary<string, object>{
                { Properties.Resources.ConstStart32bitsKey , Properties.Resources.ConstStart32bitsValue },
                { Properties.Resources.ConstStart64bitsKey, Properties.Resources.ConstStart64bitsValue },
                { Properties.Resources.ConstStartAutoKey, Properties.Resources.ConstStartAutoValue }
            };
        }

        private void InitODBCDataSource()
        {
            this.configView.ODBCDataSource = DataSourceHelper.GetSystemDataSourceNames();
            this.configView.RPTODBCDataSource = DataSourceHelper.GetSystemDataSourceNames();
        }
        private void InitBaseRegistryPath()
        {
            this.configView.BaseRegistryDataSource = new Dictionary<string, string>{
                { "HKEY_LOCAL_MACHINE" ,  "HKEY_LOCAL_MACHINE"},
                { "HKEY_CURRENT_USER", "HKEY_CURRENT_USER" },
                { "HKEY_CURRENT_CONFIG", "HKEY_CURRENT_CONFIG" }
            };
        }
        private void BindToolStripEvent()
        {
            this.configView.ListConfig += ListConfig;
            this.configView.ImportConfig += ImportConfig;
            this.configView.ExportConfig += ExportConfig;
            this.configView.OpenAppDataPath += OpenAppDataPath;
            this.configView.ValidateConfig += ValidateConfig;
            this.configView.About += About;

            this.configView.DefaultEnviroment += DefaultEnviroment;
        }

        private void BindTabEvent()
        {
            this.configView.TabChange += TabChange;
        }
        private void TabChange()
        {
            switch (this.configView.CurrentTab)
            {
                case 1:
                    BindEnvConfigEvent();
                    break;
                case 2:
                    BindInvestRegEvent();
                    break;
            }

        }
        private void BindEnvConfigEvent()
        {
            this.configView.BrowseLocalPath += BrowseLocalPath;
            this.configView.BrowseServerPath += BrowseServerPath;

            this.configView.OpenLocalPath += OpenLocalPath;
            this.configView.OpenServerPath += OpenServerPath;

            this.configView.TestConnection += TestConnection;

            this.configView.SaveEnvironment += SaveEnvironment;
            this.configView.DeleteEnvironment += DeleteEnvironment;
            this.configView.ClearEnvironment += ClearEnvironment;

            this.configView.ServiceStatus += ServiceStatus;
            this.configView.ShowWinPassword += ShowWinPassword;
            this.configView.StartService += StartService;
            this.configView.StopService += StopSerivce;

            this.configView.OpenBUC += OpenBUC;
            this.configView.OpenOperation += OpenOperation;
            this.configView.OpenAdministration += OpenAdministration;
            this.configView.OpenConnector += OpenConnector;
            this.configView.OpenExtension += OpenExtension;
        }
        private void BindInvestRegEvent()
        {
            this.configView.BrowseInvestRegPath += BrowseInvestRegPath;
            this.configView.OpenInvestRegPath += OpenInvestRegPath;
            this.configView.ListInvestRegFile += ListInvestRegFile;
            this.configView.InvestRegisOperation += InvestRegisOperation;
        }
        private void BindCryptographyEvent()
        {
            this.configView.ListUserInvest += ListUserInvest;
            this.configView.ClearUserInvest += ClearUserInvest;
            this.configView.GetUserInvestInfo += GetUserInvestInfo;

            this.configView.TestEncrypt += TestEncrypt;
            this.configView.TestDecrypt += TestDecrypt;
        }

        private void BindRunScriptEvent()
        {
            this.configView.BrowseBatchPath += BrowseBatchPath;
            this.configView.OpenBatchPath += OpenBatchPath;
            this.configView.RestoreDB += RestoreDB;
            this.configView.BlurData += BlurData;
            this.configView.ExcuteScriptInFolder += ExcuteScriptInFolder;
            this.configView.ExcuteScriptQA += ExcuteScriptQA;
            this.configView.ExcuteScriptByNo += ExcuteScriptByNo;
        }
        #region ToolStrip: Main
        public void ListConfig()
        {
            InvConfigModel tmpConfigModel = null;
            try
            {
                ListInvConfigPresenter listInvConfigPresenter = new ListInvConfigPresenter(new ListInvConfigForm());
                listInvConfigPresenter.ShowView();
                
                //Backup Old Config
                tmpConfigModel = this.ViewInvConfig;
                if (listInvConfigPresenter.IsSelectConfig == true)
                {
                    //Set New Config
                    ViewInvConfig = listInvConfigPresenter.SelectConfig;
                    #region Check OS is 64 bit
                    if (Environment.Is64BitOperatingSystem)
                    {
                        if (!Directory.Exists(Properties.Resources.ConstExcel64Required))
                        {
                            Directory.CreateDirectory(Properties.Resources.ConstExcel64Required);
                        }
                    }
                    #endregion Check OS is 64 bit
                    #region ReInstall Component
                    if (listInvConfigPresenter.SelectConfig.isInstallDllAndOCX == true)
                    {
                        #region unregister component
                        List<string> unRegisterList = GetDllAndOcxList(tmpConfigModel.bnzServerPath, Properties.Resources.ConstUNRegDLL);
                        unRegisterList.AddRange(GetDllAndOcxList(tmpConfigModel.bnzServerPath, Properties.Resources.ConstUNRegOCX));
                        foreach (string filePath in unRegisterList)
                        {
                            RegisComponentHelper.UnRegister_COM(filePath);
                        }
                        #endregion unregister component
                        #region register component
                        List<string> RegisterList = GetDllAndOcxList(ViewInvConfig.bnzServerPath, Properties.Resources.ConstRegDLL);
                        RegisterList.AddRange(GetDllAndOcxList(ViewInvConfig.bnzServerPath, Properties.Resources.ConstRegOCX));
                        foreach (string filePath in RegisterList)
                        {
                            RegisComponentHelper.Register_COM(filePath);
                        }
                        #endregion register component
                    }
                    #endregion ReInstall Component
                    #region ReInstall Interface Service
                    if (listInvConfigPresenter.SelectConfig.isInstallInterfaceService == true)
                    {
                        string applicationPath = "";
                        if (ServiceHelper.IsServiceExist(Properties.Resources.ConstBnziServer))
                        {
                            ServiceHelper.StopService(Properties.Resources.ConstBnziServer,5);
                            ServiceHelper.UninstallService(Properties.Resources.ConstBnziServer);
                        }
                        if (File.Exists(ViewInvConfig.bnzServerPath + "\\Interface\\InterfaceQueue.exe"))
                        {
                            applicationPath = ViewInvConfig.bnzServerPath + "\\Interface\\InterfaceQueue.exe";
                        }
                        else
                        {
                            applicationPath = ViewInvConfig.bnzServerPath + "\\Interface\\InterfaceServer.exe";
                        }
                        ServiceHelper.InstallService(Properties.Resources.ConstBnziServer, 
                                                     Properties.Resources.ConstBnziServer, 
                                                     Properties.Resources.ConstBnziServer,
                                                     ViewInvConfig.bnzServerPath + "\\Interface\\srvany.exe",
                                                     applicationPath,
                                                     System.ServiceProcess.ServiceStartMode.Automatic,
                                                     System.ServiceProcess.ServiceAccount.LocalSystem,
                                                     null, 
                                                     null);
                        ServiceHelper.StartService(Properties.Resources.ConstBnziServer, 5);
                    }
                    #endregion ReInstall Interface Service
                    //Save Current Config
                    Properties.Settings.Default.LastConfigID = this.configView.ConfigID;
                    Properties.Settings.Default.LastConfigName = this.configView.ConfigName;
                    Properties.Settings.Default.Save();
                    //Save Config to Registry
                    invConfigMapper.SetLocalEnviromentInvConfig(ViewInvConfig);
                    Thread.Sleep(1000);
                    LisBNZDBVersion();
                    this.configView.TabEnvConfigMsg = Properties.Resources.MsgTab1LoadConfig;
                }
            }
            catch(Exception ex)
            {
                //Restore Last Config
                this.ViewInvConfig = tmpConfigModel;
                invConfigMapper.SetLocalEnviromentInvConfig(tmpConfigModel);
                //Keep Lop
                log.Error(ex.Message, ex);
                //Message Error
                this.configView.TabEnvConfigMsg = ex.Message;
            }
        }

        private void ImportConfig(string p_ImportPath, Boolean p_OverwriteConfig)
        {
            using (TextReader textReader = File.OpenText(p_ImportPath))
            {
                var csv = new CsvReader(textReader);
                List<InvConfigModel> importList = csv.GetRecords<InvConfigModel>().OfType<InvConfigModel>().ToList();
                #region Overwrite config
                if (p_OverwriteConfig == true)
                {
                    //Delete All Config
                    List<InvConfigModel> invConfigList   = invConfigMapper.GetAllInvConfig();
                    foreach (InvConfigModel invConfig in invConfigList)
                    {
                        invConfigMapper.DeleteConfig(invConfig.configID);
                    }
                }
                #endregion Overwrite config
                foreach (InvConfigModel import in importList)
                {
                    import.configID = 0;
                    invConfigMapper.SaveInvConfig(import);
                }
            }
            
        }
        private void ExportConfig(string p_ExportPath)
        {
            List<InvConfigModel> exportList = invConfigMapper.GetAllInvConfig();
            //CSVExport<InvConfigModel> csv = new CSVExport<InvConfigModel>(exportList);
            //csv.ExportToFile(p_ExportPath,true, null);
            //this.configView.ShowMessageBox(MessageBoxIcon.Information, "Export config", "Export config complete");
            using (TextWriter textWriter = File.CreateText(p_ExportPath))
            {
                var csv = new CsvWriter(textWriter);
                csv.WriteHeader<InvConfigModel>();
                csv.WriteRecords(exportList);
                this.configView.ShowMessageBox(MessageBoxIcon.Information, "Export config", "Export config complete");
            }
        }
        private void OpenAppDataPath(OpenAppData p_AppPath)
        {
            if (p_AppPath == OpenAppData.LOG)
            {
                FileHelper.OpenWindowsExplorer(Application.StartupPath+"\\Log");
            }
            else if (p_AppPath == OpenAppData.SQLScript)
            {
                FileHelper.OpenWindowsExplorer(Application.StartupPath + "\\SQLScript");
            }
        }
        private void ValidateConfig()
        {
            try
            {
                Process.Start(Application.StartupPath + "\\ValidateEnviroment.exe");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                this.configView.TabEnvConfigMsg = ex.Message;
            }
        }
        private void About()
        {
            AboutPresenter aboutPresenter = new AboutPresenter(new AboutForm());
        }

        private void DefaultEnviroment()
        {
            this.configView.CurrentDatabaseType = this.configView.DatebaseType;
            this.configView.CurrentDatabaseServer = this.configView.DatabaseServer;
            this.configView.CurrentDatabaseName = this.configView.DatabaseName;
            this.configView.InvestRegPath = this.configView.ServerPath;
        }
        #endregion ToolStrip: Main
        #region Tab: Enviroment Config
        private void BrowseLocalPath()
        {
            this.configView.LocalPath = FileHelper.ShowFolderDialog(this.configView.LocalPath);
        }

        private void BrowseServerPath()
        {
            this.configView.ServerPath = FileHelper.ShowFolderDialog(this.configView.ServerPath);
        }

        private void OpenLocalPath()
        {
            try
            {
                FileHelper.OpenWindowsExplorer(configView.LocalPath);
            }
            catch(Exception ex)
            {
                log.Error(ex.Message,ex);
                this.configView.TabEnvConfigMsg = ex.Message;
            }
        }

        private void OpenServerPath()
        {
            try
            {
                FileHelper.OpenWindowsExplorer(configView.ServerPath);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                this.configView.TabEnvConfigMsg = ex.Message;
            }
        }

        private void TestConnection()
        {
            try
            {
                databaseWorker.DoWork += new DoWorkEventHandler(_bwTestConnection_DoWork);
                databaseWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bwTestConnection_Completed);
                if (!databaseWorker.IsBusy)
                {
                    databaseWorker.RunWorkerAsync();
                }
                else
                {
                    throw new ApplicationException("Thread is Busy");
                }
            }
            catch (Exception ex)
            {
                //write log
                log.Error(ex.Message, ex);
                this.configView.TabEnvConfigMsg = ex.Message;
            }
        }
        #region Test connection thread
        private void _bwTestConnection_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.configView.ShowProgressMarquee(true);
                dbConnect.SetDatabaseProperties(this.configView.DatebaseType,
                                                this.configView.IsODBC,
                                                this.configView.DatabaseServer,
                                                this.configView.DatabaseName,
                                                this.configView.DatabaseUsername,
                                                this.configView.DatabasePassword);
                if (dbConnect.TestConnection())
                {
                    this.configView.ShowMessageBox(MessageBoxIcon.Information, "Test Connection", "Test Sucess [DBMS: " + dbConnect.CurrentDBConnVersion + "]");
                }
                else
                {
                    this.configView.ShowMessageBox(MessageBoxIcon.Exclamation, "Test Connection", "Test Fail [Reason: " + dbConnect.ExcepionMessage + "]");
                }
                e.Result = true;
            }
            catch (Exception ex)
            {
                //write log
                log.Error(ex.Message, ex);
                this.configView.TabEnvConfigMsg = ex.Message;
                e.Result = false;
            }
            finally
            {
                this.configView.ShowProgressMarquee(false);
            }
        }

        private void _bwTestConnection_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            databaseWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(_bwTestConnection_Completed);
            databaseWorker.DoWork -= new DoWorkEventHandler(_bwTestConnection_DoWork);

            if ((bool)e.Result)
            {
                this.configView.ShowProgress(100);
            }
            else
            {
                this.configView.ShowProgress(0);
            }
        }
        #endregion Test connection thread
        #region CRUD
        private void SaveEnvironment()
        {
            //Save
            int saveID = invConfigMapper.SaveInvConfig(ViewInvConfig);
            if (saveID > 0)
            {
                //Update Config ID
                this.configView.ConfigID = saveID;
                //Update Last Config
                Properties.Settings.Default.LastConfigID = this.configView.ConfigID;
                Properties.Settings.Default.LastConfigName = this.configView.ConfigName;
                Properties.Settings.Default.Save();
                //Show Result Message
                this.configView.TabEnvConfigMsg = Properties.Resources.MsgTab1SaveConfig;
            }
        }
            
        private void DeleteEnvironment()
        {
            if (invConfigMapper.DeleteConfig(this.configView.ConfigID) == true)
            {
                this.configView.TabEnvConfigMsg = Properties.Resources.MsgTab1DeleteConfig;
                ClearEnvironment();
            }
        }

        private void ClearEnvironment()
        {
            configView.ConfigID	= 0;
            configView.ConfigName = "";
            configView.ConfigType = "";
            //configView.LocalPath = "";
            //configView.ServerPath = "";
            configView.DatebaseType	= "";
            configView.DatabaseServer = "";
            configView.DatabaseName = "";
            configView.InterfaceServer = ""; 
            configView.InterfacePort = "";
            configView.DatabaseUsername = Properties.Resources.DefaultDBUsername;
            configView.DatabasePassword = Properties.Resources.DefaultDBPassword;
            configView.DatabasePort = "50000";
            configView.WindowsUsername = Properties.Resources.DefaultWinUsername;
            configView.WindowsPassword = Properties.Resources.DefaultWinPassword;

            configView.StartUpMSNetAs = Properties.Resources.ConstStartAutoValue;

            configView.ScriptNumber = "";
            configView.Remark = "";
            this.configView.TabEnvConfigMsg = Properties.Resources.MsgTab1ClearConfig;
        }
        #endregion CRUD
        #region service
        private void ServiceStatus()
        {
            try
            {
                this.configView.InterfaceStatus = ServiceHelper.GetServiceStatus(Properties.Resources.ConstBnziServer);
            }
            catch (Exception ex)
            {
                log.Warn(ex.InnerException);
            }
        }

        private void ShowWinPassword()
        {
            try
            {
                WinPasswordPresenter winPasswordPresenter = new WinPasswordPresenter(new WinPasswordForm());

                if (String.IsNullOrEmpty(this.configView.WindowsUsername))
                {
                    winPasswordPresenter.WindowsUsername = Properties.Resources.DefaultWinUsername;
                }
                else
                {
                    winPasswordPresenter.WindowsUsername = this.configView.WindowsUsername;
                }

                if (String.IsNullOrEmpty(this.configView.WindowsPassword))
                {
                    winPasswordPresenter.WindowsPassword = Properties.Resources.DefaultWinPassword;
                }
                else
                {
                    winPasswordPresenter.WindowsPassword = this.configView.WindowsPassword;
                    winPasswordPresenter.WindowsPassword = Rijndael.DecryptData(winPasswordPresenter.WindowsPassword,
                                                                           "",
                                                                           Rijndael.BlockSize.Block256,
                                                                           Rijndael.KeySize.Key256,
                                                                           Rijndael.EncryptionMode.ModeECB,
                                                                           true);
                }
                
                winPasswordPresenter.OnViewInitialize();
                this.configView.WindowsUsername = winPasswordPresenter.WindowsUsername;
                this.configView.WindowsPassword = Rijndael.EncryptData(winPasswordPresenter.WindowsPassword,
                                                                       "",
                                                                       Rijndael.BlockSize.Block256,
                                                                       Rijndael.KeySize.Key256,
                                                                       Rijndael.EncryptionMode.ModeECB,
                                                                       true);
            }
            catch (Exception ex)
            {
                log.Warn(ex.InnerException);
            }
        }
        private void StartService()
        {
            try
            {
                ServiceHelper.StartService(Properties.Resources.ConstBnziServer, 5);
            }
            catch (Exception ex)
            {
                log.Warn(ex.InnerException);
            }
        }
        private void StopSerivce()
        {
            try
            {
                ServiceHelper.StopService(Properties.Resources.ConstBnziServer, 5);
            }
            catch (Exception ex)
            {
                log.Warn(ex.InnerException);
            }
        }
        #endregion service

        #region Open Application
        private void OpenBUC()
        {
            //C:\Bonanza\BFM\Operation\Buc
            //Check File Exist
            string BUCPath = this.configView.LocalPath + "\\Operation\\Buc";
            string BUCExePath = BUCPath + "\\Wmsl.Incubator.Configuration.UI.exe";
            if (File.Exists(BUCExePath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = BUCPath;
                startInfo.FileName = "Wmsl.Incubator.Configuration.UI.exe";
                Process.Start(startInfo);
            }
            else
            {
                this.configView.TabEnvConfigMsg = "Not found Wmsl.Incubator.Configuration.UI.exe in Path "+ BUCExePath;
            }
            
        }
        private void OpenOperation()
        {
            //Check File Exist
            string Path = this.configView.LocalPath + "\\Operation";
            string ExeTSYPath = Path + "\\OperationBFM.exe";
            string ExeInvPath = Path + "\\Operation.exe";
            if (File.Exists(ExeTSYPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = Path;
                startInfo.FileName = "OperationBFM.exe";
                Process.Start(startInfo);
            }
            else if (File.Exists(ExeInvPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = Path;
                startInfo.FileName = "Operation.exe";
                Process.Start(startInfo);
            }
            else
            {
                this.configView.TabEnvConfigMsg = "Not found Operation in Path " + Path;
            }
        }
        private void OpenAdministration()
        {
            //Check File Exist
            string Path = this.configView.LocalPath + "\\Operation";
            string ExePath = Path + "\\Administration.exe";
            if (File.Exists(ExePath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = Path;
                startInfo.FileName = "Administration.exe";
                Process.Start(startInfo);
            }
            else
            {
                this.configView.TabEnvConfigMsg = "Not found Administration.exe in Path " + ExePath;
            }

        }
        private void OpenConnector()
        {
            string OperPath = this.configView.LocalPath + "\\Operation";
            string Path = OperPath + "\\Connector";
            string ExePath = Path + "\\Wmsl.Invest.Office.Connector.exe";

            if (File.Exists(ExePath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = OperPath;
                startInfo.FileName = "STARTConnector.bat";
                Process.Start(startInfo);
            }
            else
            {
                this.configView.TabEnvConfigMsg = "Not found Wmsl.Invest.Office.Connector.exe in Path " + ExePath;
            }
        }
        private void OpenExtension()
        {
            string Path32 = this.configView.LocalPath + "\\Operation\\AssetAlloc\\AssetAlloc32";
            string Path64 = this.configView.LocalPath + "\\Operation\\AssetAlloc\\AssetAlloc64";

            string ExePath32 = Path32 + "\\Wmsl.Invest.exe";
            string ExePath64 = Path64 + "\\Wmsl.Invest.exe";

            string Path = "";
            string ExePath = "";
            if (this.configView.StartUpMSNetAs.Equals(Properties.Resources.ConstStart32bitsValue))
            {
                Path = Path32;
                ExePath = ExePath32;
            }
            else if (this.configView.StartUpMSNetAs.Equals(Properties.Resources.ConstStart64bitsValue))
            {
                Path = Path64;
                ExePath = ExePath64;
            }
            else if (this.configView.StartUpMSNetAs.Equals(Properties.Resources.ConstStartAutoValue))
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    Path = Path64;
                    ExePath = ExePath64;
                }
                else
                {
                    Path = Path32;
                    ExePath = ExePath32;
                }
            }

            if (File.Exists(ExePath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = Path;
                startInfo.FileName = "Wmsl.Invest.exe";
                Process.Start(startInfo);
            }
            else
            {
                this.configView.TabEnvConfigMsg = "Not found Wmsl.Invest.exe in Path " + ExePath;
            }
        }
        #endregion Open Application
        #region bnzDBVersion
        private void LisBNZDBVersion()
        {
            try
            {
                this.configView.ShowProgressMarquee(true);
                databaseWorker.DoWork += new DoWorkEventHandler(_bwBNZDBVersion_DoWork);
                databaseWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bwBNZDBVersion_Completed);
                if (!databaseWorker.IsBusy)
                {
                    databaseWorker.RunWorkerAsync();
                }
                else
                {
                    throw new ApplicationException("Thread is Busy");
                }
            }
            catch (Exception ex)
            {
                //write log
                databaseWorker.CancelAsync();
                log.Error(ex.Message, ex);
                this.configView.TabEnvConfigMsg = ex.Message;
            }
            finally
            {
                this.configView.ShowProgressMarquee(false);
            }
        }
        private void _bwBNZDBVersion_DoWork(object sender, DoWorkEventArgs e)
        {
            IList<BNZDBVersionModel> BNZVersionls = new List<BNZDBVersionModel>();
            try
            {
                dbConnect.SetDatabaseProperties(this.configView.DatebaseType,
                                               this.configView.IsODBC,
                                               this.configView.DatabaseServer,
                                               this.configView.DatabaseName,
                                               this.configView.DatabaseUsername,
                                               this.configView.DatabasePassword);
                string sqlQuery = "";
                if (dbConnect.CheckTableExist("BNZDBVERSION"))
                {
                    sqlQuery = "SELECT * FROM INVEST.BNZDBVERSION ORDER BY CREATEDATE DESC";

                    
                    using (DataTable dbVersionData = dbConnect.RetrieveData(sqlQuery))
                    {
                        if (dbVersionData != null)
                        {
                            DataView dbVersionView = new DataView(dbVersionData);

                            foreach (DataRowView dbVersionRow in dbVersionView)
                            {
                                BNZDBVersionModel BnzVersion = new BNZDBVersionModel();
                                BnzVersion.AppBase = (dbVersionRow["APPBASE"] == DBNull.Value) ? string.Empty : dbVersionRow["APPBASE"].ToString().Trim();
                                BnzVersion.VersionId = (dbVersionRow["VERSIONID"] == DBNull.Value) ? string.Empty : dbVersionRow["VERSIONID"].ToString().Trim();
                                BnzVersion.CreateDate = (DateTime)dbVersionRow["CREATEDATE"];
                                BnzVersion.IsCompatible = (dbVersionRow["ISCOMPATIBLE"] == DBNull.Value) ? string.Empty : dbVersionRow["ISCOMPATIBLE"].ToString().Trim();
                                BNZVersionls.Add(BnzVersion);
                            }       
                        }

                    }
                }
                
                e.Result = true;
            }
            catch(Exception ex)
            {
                log.Error(ex.Message, ex);
                e.Result = false;
            }
            finally
            {
                this.configView.BNZDBVersion = BNZVersionls;
            }
        }

        private void _bwBNZDBVersion_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            databaseWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(_bwBNZDBVersion_Completed);
            databaseWorker.DoWork -= new DoWorkEventHandler(_bwBNZDBVersion_DoWork);

            if ((bool)e.Result)
            {
                this.configView.ShowProgress(100);
            }
            else
            {
                this.configView.ShowProgress(0);
            }
        }
        #endregion bnDBVersion
        #endregion Tab: Enviroment Config
        #region Tab: InvestReg
        private void BrowseInvestRegPath()
        {
            this.configView.InvestRegPath = FileHelper.ShowFolderDialog(this.configView.InvestRegPath);
        }
        private void OpenInvestRegPath()
        {
            try
            {
                FileHelper.OpenWindowsExplorer(this.configView.InvestRegPath);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                configView.TabInvestRegMsg = ex.Message;
            }
        }
        private void ListInvestRegFile()
        {
            string operation = this.configView.InvestRegOperation;
            if (operation.Equals(Properties.Resources.ConstListFileVersion))
            {
                this.configView.InvestRegLists = GetProductVersion();
            }
            else if (operation.Equals(Properties.Resources.ConstListRegisComponent))
            {
                this.configView.InvestRegLists = GetInstallDllAndOcxInMachine();
            }
            else
            {
                this.configView.InvestRegLists = GetDllAndOcxList(this.configView.InvestRegPath,operation);
            }
        }
        private void InvestRegisOperation()
        {
            string operationResult = "";
            string operationLog = "";
            string operation = this.configView.InvestRegOperation;
            List<String> selectComponentList = this.configView.InvestRegLists;
            string invRegisResultFormat = "{0,-30} {1,-60} {2,-256} {3}";
            StringBuilder invRegisResult = new StringBuilder();
            invRegisResult.AppendFormat(invRegisResultFormat, "FILE NAME", "RESULT", "PATH",Environment.NewLine);
            try
            {

                if (selectComponentList.Count <= 0)
                {
                    throw new ApplicationException(Properties.Resources.ErrMsgSelectALeast);
                }
                //=========================================
                //Regis or UnRegis Component
                if (operation.Equals(Properties.Resources.ConstRegOCX) || operation.Equals(Properties.Resources.ConstRegDLL))
                {
                    operationLog = Application.StartupPath + "\\Log\\result_Regis_" + this.configView.ConfigName + ".txt";
                    this.configView.TabInvestRegMsg = Properties.Resources.MsgRegisterDLL;
                    foreach (string filePath in selectComponentList)
                    {
                        operationResult = RegisComponentHelper.Register_COM(filePath);
                        invRegisResult.AppendFormat(invRegisResultFormat, Path.GetFileName(filePath), operationResult, filePath, Environment.NewLine);
                    }

                }
                else if (operation.Equals(Properties.Resources.ConstUNRegOCX) || operation.Equals(Properties.Resources.ConstUNRegDLL))
                {
                    operationLog = Application.StartupPath + "\\Log\\result_UnRegis_" + this.configView.ConfigName + ".txt";
                    this.configView.TabInvestRegMsg = Properties.Resources.MsgUnregisterDLL;
                    foreach (string filePath in selectComponentList)
                    {
                        operationResult = RegisComponentHelper.UnRegister_COM(filePath);
                        invRegisResult.AppendFormat(invRegisResultFormat, Path.GetFileName(filePath), operationResult, filePath, Environment.NewLine);
                    }
                }
                this.configView.TabInvestRegMsg = Properties.Resources.MsgComplete;
            }
            catch (ApplicationException ex)
            {
                this.configView.TabInvestRegMsg = ex.Message;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
            finally
            {
                File.WriteAllText(operationLog, invRegisResult.ToString());
                Process.Start("notepad.exe", operationLog);
            }
        }
        private List<String> GetDllAndOcxList(string p_BasePath, string p_Operation)
        {
            List<String> resultList = null;
            string[,] Type = new string[,]
	        {
                    //Software Const | Real Folder Name 
	                {Properties.Resources.ConstRegDLL, ".dll"},
	                {Properties.Resources.ConstRegOCX, ".ocx"},
                    {Properties.Resources.ConstUNRegDLL, ".dll"},
	                {Properties.Resources.ConstUNRegOCX, ".ocx"}
            };
            try
            {
                //Validate Base Path
                if(!Directory.Exists(p_BasePath))
                {
                    throw new ApplicationException("Base path not found!!!");
                }
                //GetDllAndOcxList
                string result = (from colIdx in Enumerable.Range(0, Type.GetLength(0))
                                 where Type[colIdx, 0] == p_Operation
                                 select Type[colIdx, 1]).FirstOrDefault() ?? null;
                if (result != null)
                {
                    resultList = Directory.GetFiles(p_BasePath, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(result, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }
            catch(Exception ex)
            {
                log.Error(ex.Message, ex);
            }
            return resultList;
        }
        private List<String> GetProductVersion()
        {
            List<String> resultList = new List<string>();
            try
            {
                //Validate Base Path
                if (!Directory.Exists(this.configView.InvestRegPath))
                {
                    throw new ApplicationException("Base path not found!!!");
                }
                //GetProductVersion
                List<string> subFolder = new List<string> { "Operation", "Interface", "Dlls" };
                List<string> ext = new List<string> { ".exe", ".dll", ".ocx" };
                var directorys = new DirectoryInfo(this.configView.InvestRegPath).GetDirectories("*", SearchOption.AllDirectories)
                                                  .Where(subF => subFolder.Any(e => subF.FullName.EndsWith(e)));
                foreach (var directory in directorys)
                {
                    resultList = resultList.Concat(from f in directory.GetFiles("*", SearchOption.AllDirectories)
                                    .Where(s => ext.Any(e => s.FullName.EndsWith(e)))
                                  select Path.GetFileName(f.Name) + " (" + FileVersionInfo.GetVersionInfo(f.FullName).ProductVersion + ")").ToList<String>();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                //Console.WriteLine(ex.Message);
            }
            return resultList;
        }
        private List<String> GetInstallDllAndOcxInMachine()
        {
            List<string> regisWMSLCompoment = new List<string>();
            try
            {
                RegistryKey t_clsidKey = Registry.ClassesRoot.OpenSubKey("CLSID");
                
                foreach (string subKey in t_clsidKey.GetSubKeyNames())
                {
                    //For each CLSID\GUID key we get the InProcServer32 sub-key .
                    RegistryKey t_clsidSubKey = Registry.ClassesRoot.OpenSubKey("CLSID\\" + subKey + "\\InProcServer32");

                    if ((t_clsidSubKey != null))
                    {
                        //in the case InProcServer32 exist we get the default value wich contains the path of the COM file.
                        //string t_valueName = (from value in t_clsidSubKey.GetValueNames() 
                        //                      where String.IsNullOrEmpty(value)
                        //                      select value).ToString(); 

                        string[] t_valueName = t_clsidSubKey.GetValueNames().Where(item => String.IsNullOrEmpty(item)).ToArray();
                        if ((t_valueName != null) && (t_valueName.Length > 0))
                        {
                            string t_value = t_clsidSubKey.GetValue(t_valueName[0]).ToString();
                            if (t_value.Contains("wmsl") || t_value.Contains("WMSL") || t_value.Contains("Tsy") || t_value.Contains("TSY"))
                            {
                                if (File.Exists(t_value))
                                {
                                    string version = FileVersionInfo.GetVersionInfo(t_value).ProductVersion;
                                    regisWMSLCompoment.Add(t_value + "(" + version + ")");
                                }
                                else
                                {
                                    regisWMSLCompoment.Add(t_value + "(File not exists on this machine)");
                                }
                            }
                        }
                    }
                }
                regisWMSLCompoment = regisWMSLCompoment.Distinct().ToList();
                regisWMSLCompoment.Sort();
            }
            catch(Exception ex)
            {
                log.Error(ex.Message, ex);
            }
            return regisWMSLCompoment;
        }
        #endregion Tab: InvestReg
        #region Tab: Cryptography
        private void ListUserInvest()
        {
            try
            {
                this.configView.ShowProgressMarquee(true);
                databaseWorker.DoWork += new DoWorkEventHandler(_bwListUserInvest_DoWork);
                databaseWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bwListUserInvest_Completed);
                if (!databaseWorker.IsBusy)
                {
                    databaseWorker.RunWorkerAsync();
                }
                else
                {
                    throw new ApplicationException("Thread is Busy");
                }
            }
            catch (Exception ex)
            {
                //write log
                databaseWorker.CancelAsync();
                log.Error(ex.Message, ex);
                this.configView.TabEnvConfigMsg = ex.Message;
            }
            finally
            {
                this.configView.ShowProgressMarquee(false);
            }
        }
        #region List UserInvest thread
        private void _bwListUserInvest_DoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<string, Object> UserInvestDic = new Dictionary<string, Object>();
            try
            {
                
                #region Query User Invest
                dbConnect.SetDatabaseProperties(this.configView.DatebaseType,
                                                this.configView.IsODBC,
                                                this.configView.DatabaseServer,
                                                this.configView.DatabaseName,
                                                this.configView.DatabaseUsername,
                                                this.configView.DatabasePassword);
                #region GetQuery
                string sqlQuery = "";
                if (dbConnect.CheckTableExist("USERINVEST"))
                {
                    sqlQuery = "SELECT * FROM INVEST.USERINVEST WHERE ACTIVEFLAG = 'A' AND (ISINACTIVE <> 'Y' OR ISINACTIVE IS NULL)";
                }
                else
                {
                    sqlQuery = "SELECT * FROM INVEST.USER WHERE ACTIVEFLAG = 'A' AND (ISSUSPENDED <> 'Y' OR ISSUSPENDED IS NULL)";
                }
                #endregion GetQuery
                using (DataTable userData = dbConnect.RetrieveData(sqlQuery))
                {
                    #region Convert Datatable to UserInvestModel
                    if (userData != null)
                    {
                        DataView userView = new DataView(userData);
                        userView.Sort = "USERNAME ASC";

                    
                        foreach (DataRowView userDataRow in userView)
                        {
                            UserInvestModel userInvest = new UserInvestModel();
                            userInvest.Username = (userDataRow["USERNAME"] == DBNull.Value) ? string.Empty : userDataRow["USERNAME"].ToString().Trim();
                            userInvest.Password = (userDataRow["PASSWORD"] == DBNull.Value) ? string.Empty : userDataRow["PASSWORD"].ToString().Trim();
                            userInvest.IsSupend = ((userDataRow["ISSUSPENDED"] == DBNull.Value) || ((string)userDataRow["ISSUSPENDED"]).Equals("N")) ? false : true;
                            userInvest.LastLogin = (userDataRow["LASTLOGIN"] == DBNull.Value) ? DateTime.Now : (DateTime)userDataRow["LASTLOGIN"];
                            UserInvestDic.Add(userInvest.Username, userInvest);
                        }
                    }
                    #endregion Convert Datatable to UserInvestModel
                }
                #endregion Query User Invest
                //Set Data to Combobox
                this.configView.ListUserInvestCombo = UserInvestDic;
                e.Result = true;
            }
            catch(Exception ex)
            {
                log.Error(ex.Message, ex);
                e.Result = false;
            }
          
        }

        private void _bwListUserInvest_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            databaseWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(_bwListUserInvest_Completed);
            databaseWorker.DoWork -= new DoWorkEventHandler(_bwListUserInvest_DoWork);

            if ((bool)e.Result)
            {
                this.configView.ShowProgress(100);
            }
            else
            {
                this.configView.ShowProgress(0);
            }
        }
        #endregion List UserInvest thread
        private void ClearUserInvest()
        {
            this.configView.ListUserInvestCombo = null;
            this.configView.DecryptPassword = "";
            this.configView.IsSupend = false;
            this.configView.LastLogin = DateTime.Now;
        }
        private void GetUserInvestInfo()
        {
            try
            {
                UserInvestModel userInvestModel = (UserInvestModel)this.configView.UserInvest;
                #region check null password
                if (String.IsNullOrEmpty(userInvestModel.Password))
                {
                    this.configView.DecryptPassword = "";
                    throw new ApplicationException("Password is null, Please check authentication type is LDAP?");
                }
                #endregion check null password
                #region Decrypt Password
                if (this.configView.ISNewEncryption != true)
                {
                    Enigma.keyString = userInvestModel.Username;
                    this.configView.DecryptPassword = Enigma.Encrypt(userInvestModel.Password);
                }
                else
                {
                    this.configView.DecryptPassword = Rijndael.DecryptData(userInvestModel.Password,
                                                                           "", 
                                                                           Rijndael.BlockSize.Block256,
                                                                           Rijndael.KeySize.Key256,
                                                                           Rijndael.EncryptionMode.ModeECB, 
                                                                           true);
                }
                #endregion Decrypt Password
                this.configView.IsSupend = userInvestModel.IsSupend;
                this.configView.LastLogin = userInvestModel.LastLogin;
            }
            catch(Exception ex)
            {
                log.Error(ex.Message, ex);
            }     
        }

        private void TestEncrypt()
        {

            if (this.configView.ISNewEncryption != true)
            {
                Enigma.keyString = GetSaltText();
                this.configView.OutputText = Enigma.Encrypt(this.configView.InputText);
            }
            else
            {
                Rijndael.SaltPassword = GetSaltText();
                this.configView.OutputText = Rijndael.EncryptData(this.configView.InputText,
                                                                  "",
                                                                  Rijndael.BlockSize.Block256,
                                                                  Rijndael.KeySize.Key256,
                                                                  Rijndael.EncryptionMode.ModeECB,
                                                                  true);
                Rijndael.SaltPassword = "";
            }
            
        }

        private void TestDecrypt()
        {
            if (this.configView.ISNewEncryption != true)
            {
                Enigma.keyString = GetSaltText();
                this.configView.OutputText = Enigma.Encrypt(this.configView.InputText);
            }
            else
            {
                Rijndael.SaltPassword = GetSaltText();
                this.configView.OutputText = Rijndael.DecryptData(this.configView.InputText,
                                                                  "",
                                                                  Rijndael.BlockSize.Block256,
                                                                  Rijndael.KeySize.Key256,
                                                                  Rijndael.EncryptionMode.ModeECB,
                                                                  true);
                Rijndael.SaltPassword = "";
            }
        }

        private string GetSaltText()
        {
            string result = null;
            switch(this.configView.SaltTextType)
            {
                case "Operation/Administrator":
                    result = "Operation/Administrator";
                    break;
                case "Interface":
                    result = "Interface";
                    break;
                case "Other : ":
                    result = this.configView.OtherSaltText;
                    break;
            }
            return result;
        }
        #endregion Tab: Cryptography
        #region Tab: Run Script
        private void BrowseBatchPath()
        {
            this.configView.GenerateBATFilePath = FileHelper.ShowFolderDialog(this.configView.GenerateBATFilePath);
        }
        private void OpenBatchPath()
        {
            try
            {
                FileHelper.OpenWindowsExplorer(configView.GenerateBATFilePath);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                this.configView.ShowMessageBox(MessageBoxIcon.Exclamation, "Generate batch file",ex.Message);
            }
        }
        #region >> Restore DB
        private void RestoreDB()
        {
            try
            {
                databaseWorkerDoWork = new DoWorkEventHandler(_bwRestoreDB_DoWork);
                databaseWorkerRunComplete = new RunWorkerCompletedEventHandler(_bwRunScript_Completed);
                databaseWorker.DoWork += databaseWorkerDoWork;
                databaseWorker.RunWorkerCompleted += databaseWorkerRunComplete;
                if (!databaseWorker.IsBusy)
                {
                    databaseWorker.RunWorkerAsync();
                }
                else
                {
                    throw new ApplicationException("Thread is Busy");
                }
            }
            catch (Exception ex)
            {
                //write log
                log.Error(ex.Message, ex);
                this.configView.TabEnvConfigMsg = ex.Message;
            }
        }

        private void _bwRestoreDB_DoWork(object sender, DoWorkEventArgs e)
        {
            //MSSQLExcuteScript MSSQLexcuteScript = new MSSQLExcuteScript();
            
            StringBuilder sbHeaderLog = new StringBuilder();
            string tmp;
            try
            {
                this.configView.ClearConsoleLog();
                this.configView.RunScriptControlState = false;
                this.configView.ShowProgressMarquee(true);
                var excuteScript = ExecuteScriptFactory.GetFactory((DatabaseType)this.configView.DatabaseTypeValue);

                tmp = "=======START PROCESS [Restore DB User] =======";
                sbHeaderLog.AppendLine(tmp);
                this.configView.AddConsoleLog(tmp);
                
                InvConfigModel invConfig = ViewInvConfig;
                tmp = ">> Create a connection by using Windows Authentication [" + invConfig.bnzDatabaseServer + ":" + invConfig.bnzDatabaseName + "]";
                sbHeaderLog.AppendLine(tmp);
                this.configView.AddConsoleLog(tmp);
                string constring = DBProviderHelper.GetConnectionString("MSSQLTrust",
                                                                         invConfig.bnzDatabaseServer,
                                                                         invConfig.bnzDatabaseName);
                excuteScript.SetConnection(constring);
                //Regis Event
                ExecuteScriptEventHandlers(excuteScript);
                excuteScript.ExecuteScriptMain(ExcuteScriptType.ExcuteSpecficScript,
                                               Application.StartupPath + "\\SQLScript\\SQLServer\\ScriptAddUserAfterRestoreBakFile.sql",
                                               this.configView.IsMakeChangeOnDB,
                                               this.configView.GenerateBATFilePath,
                                               this.configView.IsGenerateBATFile);
                tmp = ">> Closed a connection";
                sbHeaderLog.AppendLine(tmp);
                this.configView.AddConsoleLog(tmp);
                e.Result = true;
            }
            catch (Exception ex)
            {
                e.Result = false;
            }
            finally
            {
                tmp = "========END PROCESS [Restore DB User] ========";
                sbHeaderLog.AppendLine(tmp);
                this.configView.AddConsoleLog(tmp);
                this.configView.ShowProgressMarquee(false);
                this.configView.RunScriptControlState = true;
            }            
        }
        #endregion >> Restore DB
        #region >> BlurData
        private void BlurData()
        {
            try
            {
                databaseWorkerDoWork = new DoWorkEventHandler(_bwBlurData_DoWork);
                databaseWorkerRunComplete = new RunWorkerCompletedEventHandler(_bwRunScript_Completed);
                databaseWorker.DoWork += databaseWorkerDoWork;
                databaseWorker.RunWorkerCompleted += databaseWorkerRunComplete;
                if (!databaseWorker.IsBusy)
                {
                    databaseWorker.RunWorkerAsync();
                }
                else
                {
                    throw new ApplicationException("Thread is Busy");
                }
            }
            catch (Exception ex)
            {
                //write log
                log.Error(ex.Message, ex);
                this.configView.TabEnvConfigMsg = ex.Message;
            }
        }

        private void _bwBlurData_DoWork(object sender, DoWorkEventArgs e)
        {
            StringBuilder sbHeaderLog = new StringBuilder();
            string tmp;
            try
            {
                this.configView.ClearConsoleLog();
                this.configView.RunScriptControlState = false;
                this.configView.ShowProgressMarquee(true);
                var excuteScript = ExecuteScriptFactory.GetFactory((DatabaseType)this.configView.DatabaseTypeValue);

                tmp = "=======START PROCESS [Blur User Data] =======";
                sbHeaderLog.AppendLine(tmp);
                this.configView.AddConsoleLog(tmp);
                InvConfigModel invConfig = ViewInvConfig;
                tmp = ">> Create a connection [" + invConfig.bnzDatabaseServer + ":" + invConfig.bnzDatabaseName + "]";
                sbHeaderLog.AppendLine(tmp);
                this.configView.AddConsoleLog(tmp);

                string constring = DBProviderHelper.GetConnectionString("SQL Server",
                                                                        invConfig.bnzDatabaseServer,
                                                                        invConfig.bnzDatabaseName,
                                                                        invConfig.bnzDatabaseUsername,
                                                                        invConfig.bnzDatabasePassword);
                excuteScript.SetConnection(constring);
                //Regis Event
                ExecuteScriptEventHandlers(excuteScript);
                excuteScript.ExecuteScriptMain(ExcuteScriptType.ExcuteSpecficScript,
                                               Application.StartupPath + "\\SQLScript\\SQLServer\\BlurUserData.txt",
                                               this.configView.IsMakeChangeOnDB,
                                               this.configView.GenerateBATFilePath,
                                               this.configView.IsGenerateBATFile);
                tmp = ">> Closed a connection";
                sbHeaderLog.AppendLine(tmp);
                this.configView.AddConsoleLog(tmp);
                e.Result = true;
            }
            catch (Exception ex)
            {
                //ErrorMsg = ex.Message + Environment.NewLine + ex.StackTrace.ToString();
                log.Error(ex.Message, ex);
                e.Result = false;
            }
            finally
            {
                tmp = "========END PROCESS [Blur User Data] ========";
                sbHeaderLog.AppendLine(tmp);
                this.configView.AddConsoleLog(tmp);
                this.configView.ShowProgressMarquee(false);
                this.configView.RunScriptControlState = true;
            }        
        }
        #endregion >> BlurData
        #region >> ExcuteScriptInFolder
        private void ExcuteScriptInFolder()
        {
            try
            {
                databaseWorkerDoWork = new DoWorkEventHandler(_bwExcuteScriptInFolder_DoWork);
                databaseWorkerRunComplete = new RunWorkerCompletedEventHandler(_bwRunScript_Completed);
                databaseWorker.DoWork += databaseWorkerDoWork;
                databaseWorker.RunWorkerCompleted += databaseWorkerRunComplete;
                if (!databaseWorker.IsBusy)
                {
                    databaseWorker.RunWorkerAsync();
                }
                else
                {
                    throw new ApplicationException("Thread is Busy");
                }
            }
            catch (Exception ex)
            {
                //write log
                log.Error(ex.Message, ex);
                this.configView.TabEnvConfigMsg = ex.Message;
            }
        }

        private void _bwExcuteScriptInFolder_DoWork(object sender, DoWorkEventArgs e)
        {
            StringBuilder sbHeaderLog = new StringBuilder();
            string tmp;
            try
            {
                this.configView.ClearConsoleLog();
                this.configView.RunScriptControlState = false;
                this.configView.ShowProgressMarquee(true);
                var excuteScript = ExecuteScriptFactory.GetFactory((DatabaseType)this.configView.DatabaseTypeValue);

                string scriptPath = null;
                tmp = "=======START PROCESS [Execute script in folder] =======";
                sbHeaderLog.AppendLine(tmp);
                this.configView.AddConsoleLog(tmp);

                if (DialogHelper.InputBox("Run Script", "Enter script folder path", ref scriptPath) == DialogResult.OK)
                {
                    if (Directory.Exists(scriptPath))
                    {
                        InvConfigModel invConfig = ViewInvConfig;
                        tmp = ">> Create a connection [" + invConfig.bnzDatabaseServer + ":" + invConfig.bnzDatabaseName + "]";
                        sbHeaderLog.AppendLine(tmp);
                        this.configView.AddConsoleLog(tmp);
                        string constring = DBProviderHelper.GetConnectionString("SQL Server",
                                                                                invConfig.bnzDatabaseServer,
                                                                                invConfig.bnzDatabaseName,
                                                                                invConfig.bnzDatabaseUsername,
                                                                                invConfig.bnzDatabasePassword);
                        excuteScript.SetConnection(constring);
                        //Regis Event
                        ExecuteScriptEventHandlers(excuteScript);
                        excuteScript.ExecuteScriptMain(ExcuteScriptType.ExcuteScriptInFolder,
                                                       scriptPath,
                                                        this.configView.IsMakeChangeOnDB,
                                                        this.configView.GenerateBATFilePath,
                                                        this.configView.IsGenerateBATFile);
                        tmp = ">> Closed a connection";
                        sbHeaderLog.AppendLine(tmp);
                        this.configView.AddConsoleLog(tmp);
                        e.Result = true;
                    }
                    else
                    {
                        tmp = ">> Directory not exist";
                        sbHeaderLog.AppendLine(tmp);
                        this.configView.AddConsoleLog(tmp);
                        e.Result = false;
                    }
                }
                else
                {
                    tmp = ">> Task canceled by user";
                    sbHeaderLog.AppendLine(tmp);
                    this.configView.AddConsoleLog(tmp);
                    e.Result = false;
                }
            }
            catch (Exception ex)
            {
                //ErrorMsg = ex.Message + Environment.NewLine + ex.StackTrace.ToString();
                e.Result = false;
            }
            finally
            {
                tmp = "========END PROCESS [Execute script in folder] ========";
                sbHeaderLog.AppendLine(tmp);
                this.configView.AddConsoleLog(tmp);
                this.configView.ShowProgressMarquee(false);
                this.configView.RunScriptControlState = true;
            }
        }
       
        #endregion >> ExcuteScriptInFolder
        #region >> ExcuteScriptQA
        private void ExcuteScriptQA()
        {
            try
            {
                databaseWorkerDoWork = new DoWorkEventHandler(_bwExcuteScriptQA_DoWork);
                databaseWorkerRunComplete = new RunWorkerCompletedEventHandler(_bwRunScript_Completed);
                databaseWorker.DoWork += databaseWorkerDoWork;
                databaseWorker.RunWorkerCompleted += databaseWorkerRunComplete;
                if (!databaseWorker.IsBusy)
                {
                    databaseWorker.RunWorkerAsync();
                }
                else
                {
                    throw new ApplicationException("Thread is Busy");
                }
            }
            catch (Exception ex)
            {
                //write log
                log.Error(ex.Message, ex);
                this.configView.TabEnvConfigMsg = ex.Message;
            }
        }
        private void _bwExcuteScriptQA_DoWork(object sender, DoWorkEventArgs e)
        {
            StringBuilder sbHeaderLog = new StringBuilder();
            string tmp;
            try
            {
                this.configView.ClearConsoleLog();
                this.configView.RunScriptControlState = false;
                this.configView.ShowProgressMarquee(true);
                var excuteScript = ExecuteScriptFactory.GetFactory((DatabaseType)this.configView.DatabaseTypeValue);

                string scriptNumber = "01";

                tmp = "=======START PROCESS [Execute script for QA] =======";
                sbHeaderLog.AppendLine(tmp);
                this.configView.AddConsoleLog(tmp);

                string startPath = configView.LocalPath + "\\Script#" + scriptNumber;
                if (Directory.Exists(startPath))
                {
                    tmp = ">> Create a connection [" + ViewInvConfig.bnzDatabaseServer + ":" + ViewInvConfig.bnzDatabaseName + "]";
                    sbHeaderLog.AppendLine(tmp);
                    this.configView.AddConsoleLog(tmp);
                    string constring = DBProviderHelper.GetConnectionString("SQL Server",
                                                                            ViewInvConfig.bnzDatabaseServer,
                                                                            ViewInvConfig.bnzDatabaseName,
                                                                            ViewInvConfig.bnzDatabaseUsername,
                                                                            ViewInvConfig.bnzDatabasePassword);
                    excuteScript.SetConnection(constring);
                    ExecuteScriptEventHandlers(excuteScript);
                    excuteScript.ExecuteScriptMain(ExcuteScriptType.ExecuteScriptByScriptNo,
                                                    ViewInvConfig.bnzLocalPath,
                                                    scriptNumber,
                                                    "Script#*",
                                                    this.configView.IsMakeChangeOnDB,
                                                    this.configView.GenerateBATFilePath,
                                                    this.configView.IsGenerateBATFile);
                    tmp = ">> Closed a connection";
                    sbHeaderLog.AppendLine(tmp);
                    this.configView.AddConsoleLog(tmp);
                    //result
                    e.Result = true;
                }
                else
                {
                    tmp = ">> Directory not exist";
                    sbHeaderLog.AppendLine(tmp);
                    this.configView.AddConsoleLog(tmp);
                    e.Result = false;
                }
            }
            catch (Exception ex)
            {
                e.Result = false;
            }
            finally
            {
                tmp = "========END PROCESS [Execute script by script number]  ========";
                sbHeaderLog.AppendLine(tmp);
                this.configView.AddConsoleLog(tmp);
                //m_view.ShowProgressMarquee(false);
                this.configView.ShowProgressMarquee(false);
                this.configView.RunScriptControlState = true;
            }
        }
        #endregion >> ExcuteScriptQA
        #region >> ExcuteScriptByNo
        private void ExcuteScriptByNo()
        {
            try
            {
                databaseWorkerDoWork = new DoWorkEventHandler(_bwExcuteScriptByNo_DoWork);
                databaseWorkerRunComplete = new RunWorkerCompletedEventHandler(_bwRunScript_Completed);
                databaseWorker.DoWork += databaseWorkerDoWork;
                databaseWorker.RunWorkerCompleted += databaseWorkerRunComplete;
                if (!databaseWorker.IsBusy)
                {
                    databaseWorker.RunWorkerAsync();
                }
                else
                {
                    throw new ApplicationException("Thread is Busy");
                }
            }
            catch (Exception ex)
            {
                //write log
                log.Error(ex.Message, ex);
                this.configView.TabEnvConfigMsg = ex.Message;
            }
        }
        private void _bwExcuteScriptByNo_DoWork(object sender, DoWorkEventArgs e)
        {
            StringBuilder sbHeaderLog = new StringBuilder();
            string tmp;
            try
            {
                this.configView.ClearConsoleLog();
                this.configView.RunScriptControlState = false;
                this.configView.ShowProgressMarquee(true);
                var excuteScript = ExecuteScriptFactory.GetFactory((DatabaseType)this.configView.DatabaseTypeValue);

                string scriptNumber = "";
                if ((string.IsNullOrEmpty(configView.ScriptNumber)) || (string.IsNullOrWhiteSpace(configView.ScriptNumber)))
                {
                    scriptNumber = "";
                }
                else
                {
                    scriptNumber = (Convert.ToInt16(configView.ScriptNumber) + 1).ToString();
                }
               
                tmp = "=======START PROCESS [Execute script by script number] =======";
                sbHeaderLog.AppendLine(tmp);
                this.configView.AddConsoleLog(tmp);

                //m_view.ShowProgressMarquee(true);
                if (DialogHelper.InputBox("Run Script", "Enter script number (Script#03) >> '03'", ref scriptNumber) == DialogResult.OK)
                {
                    string startPath = configView.LocalPath + "\\Script\\Script#" + scriptNumber;
                    if (Directory.Exists(startPath))
                    {
                        tmp = ">> Create a connection [" + ViewInvConfig.bnzDatabaseServer + ":" + ViewInvConfig.bnzDatabaseName + "]";
                        sbHeaderLog.AppendLine(tmp);
                        this.configView.AddConsoleLog(tmp);
                        string constring = DBProviderHelper.GetConnectionString("SQL Server",
                                                                                ViewInvConfig.bnzDatabaseServer,
                                                                                ViewInvConfig.bnzDatabaseName,
                                                                                ViewInvConfig.bnzDatabaseUsername,
                                                                                ViewInvConfig.bnzDatabasePassword);
                        excuteScript.SetConnection(constring);
                        ExecuteScriptEventHandlers(excuteScript);

                        excuteScript.ExecuteScriptMain(ExcuteScriptType.ExecuteScriptByScriptNo,
                                                       ViewInvConfig.bnzLocalPath + "\\Script",
                                                       scriptNumber,
                                                       null,
                                                       this.configView.IsMakeChangeOnDB,
                                                       this.configView.GenerateBATFilePath,
                                                       this.configView.IsGenerateBATFile);

                        tmp = ">> Closed a connection";
                        sbHeaderLog.AppendLine(tmp);
                        this.configView.AddConsoleLog(tmp);
                        if (this.configView.IsMakeChangeOnDB == true)
                        {
                            //get last script number
                            this.configView.ScriptNumber = excuteScript.CurrentScriptNo.ToString();
                            ViewInvConfig.bnzLastUpdateScriptDate = DateTime.Now.ToString();

                            //update data
                            ViewInvConfig.configID = invConfigMapper.SaveInvConfig(ViewInvConfig);
                        }
                        //result
                        e.Result = true;
                    }
                    else
                    {
                        tmp = ">> Directory not exist";
                        sbHeaderLog.AppendLine(tmp);
                        this.configView.AddConsoleLog(tmp);
                        e.Result = false;
                    }
                }
                else
                {
                    tmp = ">> Directory not exist";
                    sbHeaderLog.AppendLine(tmp);
                    this.configView.AddConsoleLog(tmp);
                    e.Result = false;
                }
            }
            catch (Exception ex)
            {
               
                e.Result = false;
            }
            finally
            {
                tmp = "========END PROCESS [Execute script by script number]  ========";
                sbHeaderLog.AppendLine(tmp);
                this.configView.AddConsoleLog(tmp);
                //m_view.ShowProgressMarquee(false);
                this.configView.ShowProgressMarquee(false);
                this.configView.RunScriptControlState = true;
            }
        }
        #endregion >> ExcuteScriptByNo
        private void _bwRunScript_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            databaseWorker.RunWorkerCompleted -= databaseWorkerRunComplete;
            databaseWorker.DoWork -= databaseWorkerDoWork;

            if ((bool)e.Result)
            {
                this.configView.ShowProgress(100);
            }
            else
            {
                this.configView.ShowProgress(0);
            }

            //Write log
            StringBuilder sbDefaultFilename = new StringBuilder();
            sbDefaultFilename.Append(Application.StartupPath + "\\Log\\");
            sbDefaultFilename.Append(ViewInvConfig.configName + "_" + ViewInvConfig.bnzDatabaseName);
            sbDefaultFilename.Append(".txt");
            FileHelper.WriteLog(this.configView.GetConsolelog(), sbDefaultFilename.ToString());
            if (this.configView.IsOpenScriptLog)
            {
                Process.Start("notepad.exe", sbDefaultFilename.ToString());
            }
        }
        #region HandleEvent form ExcuteScript Class
        private void ExecuteScriptEventHandlers(ExcuteScript mssqlExecute)
        {
            MessageEventHandle handler = new MessageEventHandle(OnHandler);
            mssqlExecute.MsgEventHandle += handler;
        }

        private void OnHandler(object sender, MessageEvent e)
        {
            this.configView.AddConsoleLog(e.message);
        }
        #endregion HandleEvent form ExcuteScript Class
        #endregion Tab: Run Script

    }
}