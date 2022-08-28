using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using InvConfig.Views;
using InvConfig.Presenters;
using Helpers.Controls;
using Microsoft.Win32;
using InvConfig.Models;
using System.Reflection;

namespace InvConfig.Views
{
    public partial class InvConfigForm : Form, IInvConfigView
    {
        //======== INTERFACE VAR ========
        public event VoidEventHandler OnViewInitialize;
        public event VoidEventHandler OnViewFinalizeClose;

        #region ======== EVENT SECITON ========
        //======== EVENT SECITON ========
        //>>ToolStripMain
        public event Action ListConfig;
        public event Action<string, Boolean> ImportConfig;
        public event Action<string> ExportConfig;
        public event Action<OpenAppData> OpenAppDataPath;
        public event Action ValidateConfig;
        public event Action About;
        //TAB
        public event Action TabChange;
        public event Action DefaultEnviroment;
        //>>TAB: Enviroment Setup
        public event Action BrowseLocalPath;
        public event Action BrowseServerPath;
        public event Action OpenLocalPath;
        public event Action OpenServerPath;
        public event Action TestConnection;
        public event Action SaveEnvironment;
        public event Action DeleteEnvironment;
        public event Action ClearEnvironment;
        public event Action ShowWinPassword;
        public event Action OpenBUC;
        public event Action OpenOperation;
        public event Action OpenAdministration;
        public event Action OpenConnector;
        public event Action OpenExtension;
        //>> Service
        public event Action ServiceStatus;
        public event Action StartService;
        public event Action StopService;
        //>>TAB: InvestReg
        public event Action BrowseInvestRegPath;
        public event Action OpenInvestRegPath;
        public event Action ListInvestRegFile;
        public event Action InvestRegisOperation;
        //>>TAB: Cryptography
        public event Action ListUserInvest;
        public event Action ClearUserInvest;
        public event Action GetUserInvestInfo;

        public event Action TestEncrypt;
        public event Action TestDecrypt;
        //>>TAB: Run Script
        public event Action BrowseBatchPath;
        public event Action OpenBatchPath;
        public event Action RestoreDB;
        public event Action BlurData;
        public event Action ExcuteScriptInFolder;
        public event Action ExcuteScriptQA;
        public event Action ExcuteScriptByNo;
        //======== EVENT SECITON ========
        #endregion ======== EVENT SECITON ========

        //======= PROPERTY SECTION ======
        public int CurrentTab
        {
            get { return tabMain.TabIndex; }
        }
        public string CurrentDatabaseServer
        {
            set { tstxtDBServer.Text = value; }
        }
        public string CurrentDatabaseType
        {
            set { tstxtDBType.Text = value; }
        }
        public string CurrentDatabaseName
        {
            set { tstxtDBName.Text = value; }
        }
        //>>TAB: Enviroment Setup
        #region TAB: Enviroment Setup
        public int ConfigID 
        {
            get
            { 
                int parsedInt = 0;
                tsMain.InvokeIfRequired(() => { int.TryParse(tstxtConfigID.Text, out parsedInt); });
                return parsedInt; 
            }
            set 
            { 
                tsMain.InvokeIfRequired(() => { tstxtConfigID.Text = value.ToString(); });
            }
        }
        public string ConfigName 
        {
            get 
            {
                string configName = null;
                tsMain.InvokeIfRequired(() => { configName = tstxtConfigName.Text; });
                return configName; 
            }
            set 
            { 
                tsMain.InvokeIfRequired(() => { tstxtConfigName.Text = value; });
            }
        }
        public string ConfigType 
        {
            get 
            {
                string configType = null;
                tsMain.InvokeIfRequired(() => { configType = tsComboConfigType.Text; });
                return configType; 
            }
            set 
            { 
                tsMain.InvokeIfRequired(() => { tsComboConfigType.Text = value; });
            }
        }
        public string LocalPath
        {
            get 
            { 
                string localPath = null;
                txtLocalPath.InvokeIfRequired(() => { localPath = txtLocalPath.Text; });
                return localPath; 
            }
            set 
            { 
                txtLocalPath.InvokeIfRequired(() => { txtLocalPath.Text = value; });
            }
        }
        public string ServerPath
        {
            get 
            { 
                string serverPath = null;
                txtServerPath.InvokeIfRequired(() => { serverPath = txtServerPath.Text; });
                return serverPath; 
            }
            set 
            { 
                txtServerPath.InvokeIfRequired(() => { txtServerPath.Text = value; });
            }
        }
        public string DatebaseType 
        {
            get 
            { 
                string datebaseType = null;
                cboDatabase.InvokeIfRequired(() => { datebaseType = cboDatabase.Text; });
                return datebaseType; 
            }
            set 
            { 
                cboDatabase.InvokeIfRequired(() => { cboDatabase.Text = value; });
            }
        }
        public string DatabaseServer 
        {
            get 
            { 
                string databaseServer = null;
                txtDBServer.InvokeIfRequired(() => { databaseServer = txtDBServer.Text; });
                return databaseServer; 
            }
            set 
            { 
                txtDBServer.InvokeIfRequired(() => { txtDBServer.Text = value; });
            }
        }
        public Boolean IsODBC 
        {
            get
            {
                Boolean IsODBC = false;
                chkISODBC.InvokeIfRequired(() => { IsODBC = chkISODBC.Checked; });
                return IsODBC; 
            }
            set
            {
                chkISODBC.InvokeIfRequired(() => { chkISODBC.Checked = value; });
            }
        }
        public string DatabaseName 
        {
            get 
            { 
                string databaseName = null;
                cboDBName.InvokeIfRequired(() => { databaseName = cboDBName.Text; });
                return databaseName; 
            }
            set 
            { 
                cboDBName.InvokeIfRequired(() => { cboDBName.Text = value; });
            }
        }

        public string DatabasePort
        {
            get
            {
                string databasePort = null;
                mtPort.InvokeIfRequired(() => { databasePort = mtPort.Text; });
                return databasePort;
            }
            set
            {
                mtPort.InvokeIfRequired(() => { mtPort.Text = value; });
            }
        }

        public string RPTDatabaseName
        {
            get
            {
                string RPTdatabaseName = null;
                cboRPTDBName.InvokeIfRequired(() => { RPTdatabaseName = cboRPTDBName.Text; });
                return RPTdatabaseName;
            }
            set
            {
                cboRPTDBName.InvokeIfRequired(() => { cboRPTDBName.Text = value; });
            }
        }

        public IList<BNZDBVersionModel> BNZDBVersion
        {
            set
            {
                if (value != null)
                {
                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = value;
                    dvgBNZVersion.InvokeIfRequired(() => {
                        dvgBNZVersion.DataSource = bindingSource;
                        //Set Property
                        dvgBNZVersion.Columns[0].HeaderText = "APPBASE";
                        dvgBNZVersion.Columns[0].Width = 60;
                        dvgBNZVersion.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dvgBNZVersion.Columns[1].HeaderText = "VERSION";
                        dvgBNZVersion.Columns[1].Width = 60;
                        dvgBNZVersion.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dvgBNZVersion.Columns[2].HeaderText = "UPDATE";
                        dvgBNZVersion.Columns[2].Width = 110;
                        dvgBNZVersion.Columns[2].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                        dvgBNZVersion.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dvgBNZVersion.Columns[3].HeaderText = "COMPAT";
                        dvgBNZVersion.Columns[3].Width = 55;
                        dvgBNZVersion.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    });
                }
            }

            get
            {
                BindingSource bindingSource = new BindingSource();
                dvgBNZVersion.InvokeIfRequired(() => { bindingSource.DataSource = dvgBNZVersion.DataSource; });
                return bindingSource.List.Cast<BNZDBVersionModel>().ToList();
            }
        }

        public string InterfaceServer 
        {
            get
            { 
                string interfaceServer = null;
                txtServerName.InvokeIfRequired(() => { interfaceServer = txtServerName.Text; });
                return interfaceServer; 
            }
            set 
            { 
                txtServerName.InvokeIfRequired(() => { txtServerName.Text = value; });
            }
        }
        public string InterfacePort 
        {
            get
            { 
                string interfacePort = null;
                txtMarkPort.InvokeIfRequired(() => { interfacePort = txtMarkPort.Text; });
                return interfacePort; 
            }
            set 
            { 
                txtMarkPort.InvokeIfRequired(() => { txtMarkPort.Text = value; });
            }
        }
       
        public string DatabaseUsername 
        {
            get
            { 
                string databaseUsername = null;
                txtDBUsername.InvokeIfRequired(() => { databaseUsername = txtDBUsername.Text; });
                return databaseUsername; 
            }
            set 
            {
                txtDBUsername.InvokeIfRequired(() => { txtDBUsername.Text = value; });
            }
        }
        public string DatabasePassword 
        {
            get
            { 
                string databasePassword = null;
                txtDBPassword.InvokeIfRequired(() => { databasePassword = txtDBPassword.Text; });
                return databasePassword; 
            }
            set 
            { 
                txtDBPassword.InvokeIfRequired(() => { txtDBPassword.Text = value; });
            }
        }
        public string ScriptNumber 
        {
            get
            { 
                string scriptNumber = null;
                txtMarkScriptNo.InvokeIfRequired(() => { scriptNumber = txtMarkScriptNo.Text; });
                return scriptNumber; 
            }
            set 
            { 
                txtMarkScriptNo.InvokeIfRequired(() => { txtMarkScriptNo.Text = value; });
            }
        }
        public string Remark 
        {
            get
            { 
                string remark = null;
                txtRemark.InvokeIfRequired(() => { remark = txtRemark.Text; });
                return remark; 
            }
            set 
            { 
                txtRemark.InvokeIfRequired(() => { txtRemark.Text = value; });
            }
        }
        public string WindowsUsername { get; set; }
        public string WindowsPassword { get; set; }
        public string BaseRegistryPath
        {
            get
            {
                string baseRegistryPath = null;
                cboBaseRegistry.InvokeIfRequired(() => { baseRegistryPath = cboBaseRegistry.SelectedValue.ToString(); });
                return baseRegistryPath;
            }
            set
            {
                cboBaseRegistry.InvokeIfRequired(() =>
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        cboBaseRegistry.SelectedValue = value;
                    }
                    else
                    {
                        cboBaseRegistry.SelectedIndex = 0;
                    }
                });
            }
        }

        public Boolean IsUpdateBUCProperties
        {
            get
            {
                Boolean IsUpdateBUC = false;
                chkUpdateBUC.InvokeIfRequired(() => { IsUpdateBUC = chkUpdateBUC.Checked; });
                return IsUpdateBUC;
            }
            set
            {
                chkUpdateBUC.InvokeIfRequired(() => { chkUpdateBUC.Checked = value; });
            }
        }
        public string[] InterfaceStatus 
        {
            set
            {
                txtStatus.InvokeIfRequired(() => { 
                    txtStatus.Text = value[0];
                    txtStatus.BackColor = System.Drawing.ColorTranslator.FromHtml(value[1]);
                });
                
            }
        }
        public Dictionary<string, object> DBTypeDataSource 
        {
            set
            {
                cboDatabase.DisplayMember = "key";
                cboDatabase.ValueMember = "value";
                cboDatabase.DataSource = new BindingSource(value, null);
                cboDatabase.SelectedIndex = -1;
            }
        }

        public Dictionary<string, object> StartupAppDataSource
        {
            set
            {
                cboStartMSNETAS.DisplayMember = "key";
                cboStartMSNETAS.ValueMember = "value";
                cboStartMSNETAS.DataSource = new BindingSource(value, null);
                cboStartMSNETAS.SelectedIndex = -1;
            }
        }

        public string StartUpMSNetAs
        {
            get
            {
                string StartUpMSNetAs = null;
                cboStartMSNETAS.InvokeIfRequired(() => { StartUpMSNetAs = cboStartMSNETAS.SelectedValue.ToString(); });
                return StartUpMSNetAs;
            }
            set
            {
                cboStartMSNETAS.InvokeIfRequired(() => {
                    if (value == null)
                        cboStartMSNETAS.SelectedValue = Properties.Resources.ConstStartAutoValue;
                    else
                        cboStartMSNETAS.SelectedValue = value;
                });
            }
        }
        public Dictionary<string, string> BaseRegistryDataSource 
        {
            set
            {
                cboBaseRegistry.DisplayMember = "key";
                cboBaseRegistry.ValueMember = "value";
                cboBaseRegistry.DataSource = new BindingSource(value, null);
                cboBaseRegistry.SelectedIndex = 1;
            }
        }
        public Object DatabaseTypeValue
        {
            get 
            { 
                object tmp = null;
                cboDatabase.InvokeIfRequired(() => { tmp = cboDatabase.SelectedValue; });
                return tmp;
            }
        }
        public Dictionary<string, int> ODBCDataSource 
        {
            set
            {
                cboDBName.DisplayMember = "key";
                cboDBName.ValueMember = "key";
                cboDBName.AutoCompleteSource = AutoCompleteSource.ListItems;
                cboDBName.DataSource = new BindingSource(value, null);
                cboDBName.SelectedIndex = -1;
            }
        }

        public Dictionary<string, int> RPTODBCDataSource
        {
            set
            {
                cboRPTDBName.DisplayMember = "key";
                cboRPTDBName.ValueMember = "key";
                cboRPTDBName.AutoCompleteSource = AutoCompleteSource.ListItems;
                cboRPTDBName.DataSource = new BindingSource(value, null);
                cboRPTDBName.SelectedIndex = -1;
            }
        }
        public string TabEnvConfigMsg
        {
            set{ txtMsgTabEnvConfig.InvokeIfRequired(() => { txtMsgTabEnvConfig.Text = value; });}
        }
        #endregion TAB: Enviroment Setup
        //>>TAB: InvestReg
        #region TAB: Invest Reg
        public string InvestRegPath
        {
            get { return txtInvRegPath.Text; }
            set { txtInvRegPath.Text = value; }
        }
        public string InvestRegOperation
        {
            get { return cboInvRegOperation.Text; }
        }
        public List<string> InvestRegLists
        {
            get { return chkListInvReg.CheckedItems.Cast<string>().ToList(); ; }
            set 
            { 
                chkListInvReg.DataSource = value;
                for (int i = 0; i < chkListInvReg.Items.Count; i++)
                {
                    chkListInvReg.SetItemChecked(i, true);
                }        
            }
        }

        public string TabInvestRegMsg 
        { 
            set
            {
                txtMsgTabInvestReg.Text = value;
            }
        }
        #endregion TAB: Invest Reg
        //>>TAB: Cryptography
        #region TAB: Crytography
        public Boolean ISNewEncryption 
        {
            get
            {
                return this.rBtnNewEncrypt.Checked;
            }
            set
            {
                if (value == true)
                {
                    this.rBtnNewEncrypt.Checked = value;
                }
                else
                {
                    this.rBtnOldEncrypt.Checked = !value;
                }
            }
        }
        public Dictionary<string, Object> ListUserInvestCombo 
        {
            set
            {
                if (cboUserInvest.InvokeRequired)
                {
                    cboUserInvest.Invoke(
                        new MethodInvoker(
                            delegate
                            {
                                cboUserInvest.BeginUpdate();
                                cboUserInvest.DataSource = null;
                                cboUserInvest.Items.Clear();
                                cboUserInvest.DataSource = new BindingSource(value, null);
                                cboUserInvest.DisplayMember = "Key";
                                cboUserInvest.ValueMember = "Value";
                                if (value != null)
                                {
                                    AutoCompleteStringCollection autoCompleteSource = new AutoCompleteStringCollection();
                                    cboUserInvest.AutoCompleteSource = AutoCompleteSource.CustomSource;
                                    foreach (KeyValuePair<string, Object> userInvestItem in value)
                                    {
                                        autoCompleteSource.Add(userInvestItem.Key);
                                    }
                                    cboUserInvest.AutoCompleteCustomSource = autoCompleteSource;
                                    cboUserInvest.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                                }
                                cboUserInvest.EndUpdate();
                            }
                        )
                    );
                }
                else
                {
                    cboUserInvest.BeginUpdate();
                    cboUserInvest.DataSource = null;
                    cboUserInvest.Items.Clear();
                    if (value != null)
                    {
                        cboUserInvest.DataSource = new BindingSource(value, null);
                        cboUserInvest.DisplayMember = "Key";
                        cboUserInvest.ValueMember = "Value";
                        AutoCompleteStringCollection autoCompleteSource = new AutoCompleteStringCollection();
                        cboUserInvest.AutoCompleteSource = AutoCompleteSource.CustomSource;
                        foreach (KeyValuePair<string, Object> userInvestItem in value)
                        {
                            autoCompleteSource.Add(userInvestItem.Key);
                        }
                        cboUserInvest.AutoCompleteCustomSource = autoCompleteSource;
                        cboUserInvest.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    }
                    cboUserInvest.EndUpdate();
                }
            }
        }
        public Object UserInvest
        {
            get
            {
                return ((KeyValuePair<string,Object>)cboUserInvest.SelectedItem).Value;
            }
        }
        public string DecryptPassword 
        {
            set
            {
                txtUserInvestPassword.Text = value;
            }
        }
        public Boolean IsSupend 
        {
            set
            {
                chkIsSupend.Checked = value;
            }
        }
        public DateTime LastLogin 
        {
            set
            {
                dtpLastLogin.Value = value;
            }
        }

        //Test Cryptography
        public string InputText 
        {
            get { return txtInput.Text; } 
        }
        public string SaltTextType 
        {
            get
            {
                return gpbCryptography.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            }
        }
        public string OtherSaltText 
        {
            get { return txtOtherSalt.Text; }
        }
        public string OutputText 
        {
            set { txtOutput.Text = value; }
        }
        #endregion TAB: Crytography
        //>>TAB: Run Script
        #region TAB: Run Script
        public Boolean IsGenerateBATFile 
        {
            get 
            {
                Boolean isGenerateBATFile = false;
                chkGenerateBatFile.InvokeIfRequired(() => { isGenerateBATFile = chkGenerateBatFile.Checked; });
                return isGenerateBATFile;
            }
        }
        public string GenerateBATFilePath 
        {
            get
            {
                string generateBATFilePath = "";
                txtBATScriptPath.InvokeIfRequired(() => { generateBATFilePath = txtBATScriptPath.Text; });
                return generateBATFilePath;
            }
            set
            {
                txtBATScriptPath.InvokeIfRequired(() => { txtBATScriptPath.Text = value; });
            }

        }
        public Boolean IsMakeChangeOnDB 
        {
            get
            {
                Boolean makeChangeOnDB = false;
                chkMakeChangeOnDB.InvokeIfRequired(() => { makeChangeOnDB = chkMakeChangeOnDB.Checked; });
                return makeChangeOnDB;
            }
        }
        public Boolean IsOpenScriptLog
        {
            get
            {
                Boolean openScriptLog = false;
                chkOpenLog.InvokeIfRequired(() => { openScriptLog = chkOpenLog.Checked; });
                return openScriptLog;
            }
        }
        public Boolean RunScriptControlState 
        {
            set
            {
                chkGenerateBatFile.InvokeIfRequired(() => { chkGenerateBatFile.Enabled = value; });
                txtBATScriptPath.InvokeIfRequired(() => { txtBATScriptPath.Enabled = value; });
                chkMakeChangeOnDB.InvokeIfRequired(() => { chkMakeChangeOnDB.Enabled = value; });
                chkOpenLog.InvokeIfRequired(() => { chkOpenLog.Enabled = value; });
            }
        }

        public void AddConsoleLog(string p_Message)
        {
            txtConsoleLog.InvokeIfRequired(() => { txtConsoleLog.AppendText(p_Message + Environment.NewLine); });
        }
        public string GetConsolelog()
        {
            string consoleLog="";
            txtConsoleLog.InvokeIfRequired(() => {
                Thread.SpinWait(1000);
                consoleLog = txtConsoleLog.Text;
            });
            return consoleLog;
        }
        public void ClearConsoleLog()
        {
            txtConsoleLog.InvokeIfRequired(() => { txtConsoleLog.Clear(); });
        }
        #endregion TAB: Run Script
        //======= PROPERTY SECTION ======

        public InvConfigForm()
        {
            InitializeComponent();
            this.Text = this.Text + "-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            BindToolStrip();
            BindComponent();
            ValidateComponent();
        }
        private void BindToolStrip()
        {
            this.tsListEnvConfig.Click += OnListConfig_Click;
            this.tsMenuImportConfig.Click += OnMenuImportConfig_Click;
            this.tsMenuExportConfig.Click += OnMenuExportConfig_Click;
            this.tsMenuAppDataLOG.Click += OnMenuAppDataLOG_Click;
            this.tsMenuAppDataSQLScript.Click += OnMenuAppDataSQLScript_Click;
            this.tsBtnValidate.Click += OnValidateEnviroment_Click;
            this.tsBtnAbout.Click += OnAbout_Click;

            this.txtServerPath.TextChanged += OnDefaultEnviroment_Change;
            this.txtDBServer.TextChanged += OnDefaultEnviroment_Change;
            this.cboDBName.TextChanged += OnDefaultEnviroment_Change;

        }
        private void BindComponent()
        {
            //TAB1
            this.btnBrowseLp.Click += OnBrowseLocalPath_Click;
            this.btnBrowseSp.Click += OnBrowseServerPath_Click;
            this.btnOpenLocalPath.Click += OnOpenLocalPath_Click;
            this.btnOpenServerPath.Click += OnOpenServerPath_Click;
            this.cboDatabase.SelectedIndexChanged += OnDatabase_SelectedIndexChanged;
            this.chkISODBC.CheckedChanged += OnISODBC_CheckedChanged;
            this.btnTestConnection.Click += OnTestConnection_Click;
            this.btnSaveEnviroment.Click += OnSaveEnvConfig_Click;
            this.btnDelEnviroment.Click += OnDeleteEnvConfig_Click;
            this.btnClearEnviroment.Click += OnClearEnvConfig_Click;
            this.btnWinPassword.Click += OnShowWinPassword_Click;

            this.timeService.Tick += OnTimeService_Tick;
            this.btnStartService.Click += OnStartService_Click;
            this.btnStopService.Click += OnStopService_Click;
            //-BUC Config
            this.btnOpenBUC.Click += OnBUC_Click;
            this.btnAppBUC.Click += OnBUC_Click;
            this.btnOperation.Click += OnOperation_Click;
            this.btnAdmin.Click += OnAdminsitrator_Click;
            this.btnConnector.Click += OnConnector_Click;
            this.btnExtension.Click += OnExtension_Click;
            //TAB2

            this.btnBrowseInvReg.Click += OnBrowseInvestRegis_Click;
            this.btnOpenInvRegPath.Click += OnOpenInvRegPath_Click;
            this.txtInvRegPath.TextChanged += OnListInvestRegFile;
            this.chkInvRegSelectAll.CheckedChanged += OnInvRegSelectAll_CheckedChanged;
            this.cboInvRegOperation.SelectedIndexChanged += OnListInvestRegFile;
            this.btnInvRegProceed.Click += OnInvRegProceed_Click;

            this.btnListUserInvest.Click += OnListUserInvest_Click;
            this.btnClearUserInvest.Click += OnClearUserInvest_Click;
            this.cboUserInvest.SelectedValueChanged += OnUserInvest_SelectedValueChanged;

            this.btnEncrypt.Click += OnTestEncrypt_Click;
            this.btnDecrypt.Click += OnTestDecrypt_Click;

            this.btnBrowseBATPath.Click += OnBrowseBatchPath_CLick;
            this.btnOpenScriptPath.Click += OnOpenBatchPath_Click;
            this.tsBtnRestoreDBUser.Click += OnRestoreDBUser_Click;
            this.tsBtnBlurData.Click += OnBlurData_Click;
            this.tsBtnRunScriptInFolder.Click += OnRunScriptInFolder_Click;
            this.tsBtnRunQAScript.Click += OnRunQAScript_Click;
            this.tsBtnRunScriptSharpNumber.Click += OnRunScriptSharpNumber_Click;

        }

        private void ValidateComponent()
        {
            //http://www.codeproject.com/Articles/11904/Extended-Error-Provider
            errorProviderExtended.Controls.Add((object)tstxtConfigID, tslblConfigID.Text);
            errorProviderExtended.Controls.Add((object)tstxtConfigName, tslblConfigName.Text);
            errorProviderExtended.Controls.Add((object)txtLocalPath, lblLocalPath.Text);
            errorProviderExtended.Controls.Add((object)txtServerPath, lblServerPath.Text);
            errorProviderExtended.Controls.Add((object)cboDatabase, lblDbType.Text);
            errorProviderExtended.Controls.Add((object)txtDBServer, lblDBPath.Text);
            errorProviderExtended.Controls.Add((object)cboDBName, lblDBName.Text);
            errorProviderExtended.Controls.Add((object)txtServerName, lblServerName.Text);
            errorProviderExtended.Controls.Add((object)txtMarkPort, lblServerPort.Text);
            errorProviderExtended.Controls.Add((object)txtDBUsername, lblDBUsername.Text);
            errorProviderExtended.Controls.Add((object)txtDBPassword, lblDBPassword.Text);
            errorProviderExtended.SummaryMessage = "Following fields are required !!!";
        }

        private void OnListConfig_Click(object sender, EventArgs e)
        {
            if (this.ListConfig != null)
            {
                this.ListConfig();
            }
            this.errorProviderExtended.ClearAllErrorMessages();
        }
        private void OnMenuImportConfig_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                openFileDialog.Title = "Open Import CSV Files";
                openFileDialog.CheckPathExists = true;
                openFileDialog.DefaultExt = "csv";
                openFileDialog.RestoreDirectory = true;

                if ((openFileDialog.ShowDialog() == DialogResult.OK) && (this.ImportConfig != null))
                {
                    DialogResult dialogResult = MessageBox.Show("Overwrite Existing Config" , Properties.Resources.ErrMsgConfirmCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (dialogResult == DialogResult.Yes)
                    {
                        this.ImportConfig(openFileDialog.FileName, true);
                    }
                    else
                    {
                        this.ImportConfig(openFileDialog.FileName, false);
                    }
                }
            }
        }
        private void OnMenuExportConfig_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                saveFileDialog.Title = "Save Export CSV Files";
                saveFileDialog.CheckPathExists = true;
                saveFileDialog.FileName = "ExportConfig";
                saveFileDialog.DefaultExt = "csv";
                saveFileDialog.RestoreDirectory = true;

                if ((saveFileDialog.ShowDialog() == DialogResult.OK) && (this.ExportConfig != null))
                {
                    this.ExportConfig(saveFileDialog.FileName);
                }
            }
        }
        private void OnMenuAppDataLOG_Click(object sender, EventArgs e)
        {
            if (this.OpenAppDataPath != null)
            {
                this.OpenAppDataPath(OpenAppData.LOG);
            }
        }

        private void OnMenuAppDataSQLScript_Click(object sender, EventArgs e)
        {
            if (this.OpenAppDataPath != null)
            {
                this.OpenAppDataPath(OpenAppData.SQLScript);
            }
        }

        private void OnValidateEnviroment_Click(object sender, EventArgs e)
        {
            if (this.ValidateConfig != null)
            {
                this.ValidateConfig();
            }
        }
        private void OnAbout_Click(object sender, EventArgs e)
        {
            if (this.About != null)
            {
                this.About();
            }
        }
        public void ShowMessageBox(MessageBoxIcon p_MessageIcon, string p_Caption, string p_Message)
        {
            MessageBox.Show(p_Message, p_Caption, MessageBoxButtons.OK, p_MessageIcon);
        }
        #region TabEvent: Enviroment Config
        #region Path
        private void OnBrowseLocalPath_Click(object sender, EventArgs e)
        {
            if (this.BrowseLocalPath != null)
            {
                this.BrowseLocalPath();
            }
        }

        private void OnBrowseServerPath_Click(object sender, EventArgs e)
        {
            if (this.BrowseServerPath != null)
            {
                this.BrowseServerPath();
            }
        }
        private void OnOpenLocalPath_Click(object sender, EventArgs e)
        {
            if (this.OpenLocalPath != null)
            {
                this.OpenLocalPath();
            }
        }
        private void OnOpenServerPath_Click(object sender, EventArgs e)
        {
            if (this.OpenServerPath != null)
            {
                this.OpenServerPath();
            }
        }
        #endregion Path

        private void OnDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDatabase.Text == Properties.Resources.ConstSQLServer)
            {
                rBtnNewEncrypt.Checked = true;
                if (!tabMain.TabPages.Contains(tabScript))
                {
                    tabMain.TabPages.Insert(3, tabScript);
                }
            }
            else if (cboDatabase.Text == Properties.Resources.ConstDB2)
            {
                rBtnOldEncrypt.Checked = true;
                tabMain.TabPages.Remove(tabScript);
            }
        }

        private void OnISODBC_CheckedChanged(object sender, EventArgs e)
        {
            txtDBServer.Enabled = !chkISODBC.Checked;
            errorProviderExtended.Controls[txtDBServer].Validate = !chkISODBC.Checked;
        }
        private void OnTestConnection_Click(object sender, EventArgs e)
        {
            if (this.TestConnection != null)
            {
                this.TestConnection();
            }
        }
        #region CRUD
        private void OnSaveEnvConfig_Click(object sender, EventArgs e)
        {
            if ((errorProviderExtended.CheckAndShowSummaryErrorMessage() == true) && (this.SaveEnvironment != null))
            {
                this.SaveEnvironment();
            }
        }

        private void OnDeleteEnvConfig_Click(object sender, EventArgs e)
        {
            if ((errorProviderExtended.CheckAndShowSummaryErrorMessage() == true) && (this.DeleteEnvironment != null))
            {
                DialogResult dialogResult = MessageBox.Show(Properties.Resources.ErrMsgConfirmDelete, Properties.Resources.ErrMsgConfirmCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dialogResult == DialogResult.Yes)
                {
                    this.DeleteEnvironment();
                }
            }
        }

        private void OnClearEnvConfig_Click(object sender, EventArgs e)
        {
            if (this.ClearEnvironment != null)
            {
                this.ClearEnvironment();
            }
            this.errorProviderExtended.ClearAllErrorMessages();
        }
        #endregion CRUD
        private void OnDefaultEnviroment_Change(object sender, EventArgs e)
        {
            if (this.DefaultEnviroment != null)
            {
                this.DefaultEnviroment();
            }
        }

        private void OnTimeService_Tick(object sender, EventArgs e)
        {
            if (this.ServiceStatus != null)
            {
                this.ServiceStatus();
            }
        }

        private void OnShowWinPassword_Click(object sender, EventArgs e)
        {
            if (this.ShowWinPassword != null)
            {
                this.ShowWinPassword();
            }
        }
        private void OnStartService_Click(object sender, EventArgs e)
        {
            if (this.StartService != null)
            {
                this.StartService();
            }
        }

        private void OnStopService_Click(object sender, EventArgs e)
        {
            if (this.StopService != null)
            {
                this.StopService();
            }
        }

        private void OnBUC_Click(object sender, EventArgs e)
        {
            if (this.OpenBUC != null)
            {
                this.OpenBUC();
            }
        }

        private void OnOperation_Click(object sender, EventArgs e)
        {
            if (this.OpenOperation != null)
            {
                this.OpenOperation();
            }
        }

        private void OnAdminsitrator_Click(object sender, EventArgs e)
        {
            if (this.OpenAdministration != null)
            {
                this.OpenAdministration();
            }
        }
        private void OnConnector_Click(object sender, EventArgs e)
        {
            if (this.OpenConnector != null)
            {
                this.OpenConnector();
            }
        }
        private void OnExtension_Click(object sender, EventArgs e)
        {
            if (this.OpenExtension != null)
            {
                this.OpenExtension();
            }
        }
        #endregion TabEvent: Enviroment Config

        #region TabEvent: Invest Regis
        private void OnBrowseInvestRegis_Click(object sender, EventArgs e)
        {
            if (this.BrowseInvestRegPath != null)
            {
                this.BrowseInvestRegPath();
            }
        }
        private void OnOpenInvRegPath_Click(object sender, EventArgs e)
        {
            if (this.OpenInvestRegPath != null)
            {
                this.OpenInvestRegPath();
            }
        }
        private void OnInvRegSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < chkListInvReg.Items.Count; i++)
            {
                chkListInvReg.SetItemChecked(i, chkInvRegSelectAll.Checked);
            }
        }
        private void OnListInvestRegFile(object sender, EventArgs e)
        {
            if (this.ListInvestRegFile != null)
            {
                this.ListInvestRegFile();
            }
            chkListInvReg.Focus();
        }
        private void OnInvRegProceed_Click(object sender, EventArgs e)
        {
            if (this.InvestRegisOperation != null)
            {
                this.InvestRegisOperation();
            }
        }
        #endregion TabEvent: Invest Regis

        #region TabEvent: Cryptography
        private void OnListUserInvest_Click(Object sender, EventArgs e)
        {
            if (this.ListUserInvest != null)
            {
                this.ListUserInvest();
            }
        }
        private void OnClearUserInvest_Click(Object sender, EventArgs e)
        {
            if (this.ClearUserInvest != null)
            {
                this.ClearUserInvest();
            }
        }
        private void OnUserInvest_SelectedValueChanged(Object sender, EventArgs e)
        {
            if (this.GetUserInvestInfo != null)
            {
                this.GetUserInvestInfo();
            }
        }

        private void OnTestEncrypt_Click(Object sender, EventArgs e)
        {
            if (this.TestEncrypt != null)
            {
                this.TestEncrypt();
            }
        }

        private void OnTestDecrypt_Click(Object sender, EventArgs e)
        {
            if (this.TestDecrypt != null)
            {
                this.TestDecrypt();
            }
        }

        #endregion TabEvent: Cryptography

        #region TabEvent: RunScript
        private void OnBrowseBatchPath_CLick(object sender, EventArgs e)
        {
            if (this.BrowseBatchPath != null)
            {
                this.BrowseBatchPath();
            }
        }
        private void OnOpenBatchPath_Click(object sender, EventArgs e)
        {
            if (this.OpenBatchPath != null)
            {
                this.OpenBatchPath();
            }
        }
        private void OnRestoreDBUser_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(Properties.Resources.MsgConfirmRunScript, Properties.Resources.ErrMsgConfirmCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if ((dialogResult == DialogResult.Yes) && (this.RestoreDB != null))
            {
                this.RestoreDB();
            }
        }
        private void OnBlurData_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(Properties.Resources.MsgConfirmRunScript, Properties.Resources.ErrMsgConfirmCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if ((dialogResult == DialogResult.Yes) && (this.BlurData != null))
            {
                this.BlurData();
            }
        }

       
        private void OnRunScriptInFolder_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(Properties.Resources.MsgConfirmRunScript, Properties.Resources.ErrMsgConfirmCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if ((dialogResult == DialogResult.Yes) && (this.ExcuteScriptInFolder != null))
            {
                this.ExcuteScriptInFolder();
            }
        }

        private void OnRunQAScript_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(Properties.Resources.MsgConfirmRunScript, Properties.Resources.ErrMsgConfirmCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if ((dialogResult == DialogResult.Yes) && (this.ExcuteScriptInFolder != null))
            {
                this.ExcuteScriptQA();
            }
        }

        private void OnRunScriptSharpNumber_Click(object sender, EventArgs e)
        {
             DialogResult dialogResult = MessageBox.Show(Properties.Resources.MsgConfirmRunScript, Properties.Resources.ErrMsgConfirmCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
             if ((dialogResult == DialogResult.Yes) && (this.ExcuteScriptByNo != null))
             {
                 this.ExcuteScriptByNo();
             }
        }
        #endregion TabEvent: RunScripgt


        public void CloseView()
        {
            throw new NotImplementedException();
        }

        public void ShowView()
        {
            this.ShowDialog();
        }

        public void RaiseVoidEvent(Presenters.VoidEventHandler @event)
        {
            throw new NotImplementedException();
        }

        public void ShowProgressMarquee(Boolean p_IsMarqueeStyle)
        {
            statusStrip.InvokeIfRequired(() => {
                if (p_IsMarqueeStyle)
                {
                    tsProgress.Style = ProgressBarStyle.Marquee;
                    tsProgress.MarqueeAnimationSpeed = 30;
                }
                else
                {
                    tsProgress.Style = ProgressBarStyle.Blocks;
                    tsProgress.Value = 0;
                }
            });
        }
        public void ShowProgress(int p_ProgressPercentage)
        {
            statusStrip.InvokeIfRequired(() => { tsProgress.Value = p_ProgressPercentage; });
        }
    }
}
