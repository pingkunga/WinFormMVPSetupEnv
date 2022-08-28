using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using Microsoft.Win32;
using InvConfig.Models;

namespace InvConfig.Views
{
    public enum OpenAppData
    {
        LOG,
        SQLScript
    }
    public interface IInvConfigView : IView
    {
        //======== EVENT SECITON ========
        //>>ToolStripMain
        event Action ListConfig;
        event Action<string, Boolean> ImportConfig;
        event Action<string> ExportConfig;
        event Action<OpenAppData> OpenAppDataPath;
        event Action ValidateConfig;
        event Action About;
        //TAB
        event Action TabChange;
        event Action DefaultEnviroment;
        //>>TAB: Enviroment Setup
        #region TAB: Enviroment Setup
        event Action BrowseLocalPath;
        event Action BrowseServerPath;
        event Action OpenLocalPath;
        event Action OpenServerPath;
        event Action TestConnection;
        event Action SaveEnvironment;
        event Action DeleteEnvironment;
        event Action ClearEnvironment;
        event Action ShowWinPassword;
        //service 
        event Action ServiceStatus;
        event Action StartService;
        event Action StopService;
        event Action OpenBUC;
        event Action OpenOperation;
        event Action OpenAdministration;
        event Action OpenConnector;
        event Action OpenExtension;
        #endregion TAB: Enviroment Setup
        //>>TAB: InvestReg
        #region TAB: InvestReg
        event Action BrowseInvestRegPath;
        event Action OpenInvestRegPath;
        event Action ListInvestRegFile;
        event Action InvestRegisOperation;
        #endregion TAB: InvestReg
        //>>TAB: Cryptography
        #region TAB: Cryptography
        event Action ListUserInvest;
        event Action ClearUserInvest;
        event Action GetUserInvestInfo;

        event Action TestEncrypt;
        event Action TestDecrypt;
        //Test Cryptography

        #endregion TAB: Cryptography
        //>>TAB: Run Script
        #region TAB: Run Scriot
        event Action BrowseBatchPath;
        event Action OpenBatchPath;
        event Action RestoreDB;
        event Action BlurData;
        event Action ExcuteScriptInFolder;
        event Action ExcuteScriptQA;
        event Action ExcuteScriptByNo;
        #endregion TAB: Run Script
        //======== EVENT SECITON ========

        //======= PROPERTY SECTION ======
        //TAB
        int CurrentTab { get; }
        string CurrentDatabaseServer { set; }
        string CurrentDatabaseType { set; }
        string CurrentDatabaseName { set; }
        //>>TAB: Enviroment Setup
        #region TAB: Enviroment Setup
        int ConfigID { get; set; }
        string ConfigName { get; set; }
        string ConfigType { get; set; }
        string LocalPath { get; set; }
        string ServerPath { get; set; }

        string DatebaseType { get; set; }
        string DatabaseServer { get; set; }
        Boolean IsODBC { get; set; }
        string DatabaseName { get; set; }
        string DatabasePort { get; set; }
        string RPTDatabaseName { get; set; }
        IList<BNZDBVersionModel> BNZDBVersion { get; set; }

        string InterfaceServer { get; set; }
        string InterfacePort { get; set; }
        string DatabaseUsername { get; set; }
        string DatabasePassword { get; set; }
        string ScriptNumber { get; set; }
        string Remark { get; set; }
        string BaseRegistryPath { get; set; }
        Boolean IsUpdateBUCProperties { get; set; }
        string WindowsUsername { get; set; }
        string WindowsPassword { get; set; }
        //service
        string[] InterfaceStatus { set; }

        //Start Up App
        Dictionary<string, object> StartupAppDataSource { set; }
        string StartUpMSNetAs { get; set; }
        //Other
        Dictionary<string, object> DBTypeDataSource { set; }
        Dictionary<string, string> BaseRegistryDataSource { set; }

        Object DatabaseTypeValue { get; }
        Dictionary<string, int> ODBCDataSource { set; }
        Dictionary<string, int> RPTODBCDataSource { set; }
        string TabEnvConfigMsg { set; }
        #endregion TAB: Enviroment Setup
        //>>TAB: InvestReg
        #region TAB: InvestReg
        string InvestRegPath{ get; set;}
        string InvestRegOperation { get; }
        List<string> InvestRegLists { get; set; }
        string TabInvestRegMsg { set; }
        #endregion TAB: InvestReg
        //>>TAB: Cryptography
        #region TAB: Cryptography
        Boolean ISNewEncryption { get; set; }
        Dictionary<string, Object> ListUserInvestCombo { set; }
        Object UserInvest { get; }
        string DecryptPassword { set; }
        Boolean IsSupend { set; }
        DateTime LastLogin { set; }
        //Test Cryptography
        string InputText { get; }
        string SaltTextType { get; }
        string OtherSaltText { get; }
        string OutputText { set; }
        #endregion TAB: Cryptography
        //>>TAB: Run Script
        #region TAB: Run Script
        Boolean RunScriptControlState { set; }
        void AddConsoleLog(string p_Message);
        string GetConsolelog();
        void ClearConsoleLog();
        Boolean IsGenerateBATFile { get; }
        string GenerateBATFilePath { get; set; }
        Boolean IsMakeChangeOnDB { get; }
        Boolean IsOpenScriptLog { get; }
        #endregion TAB: Run Script
        //======= PROPERTY SECTION ======
        void ShowMessageBox(MessageBoxIcon p_MessageIcon, string p_Caption, string p_Message);
        void ShowProgressMarquee(Boolean p_IsMarqueeStyle);
        void ShowProgress(int p_ProgressPercentage);
    }
}
