using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InvConfig.Presenters;

namespace InvConfig.Views
{
    public partial class ListInvConfigForm : Form, IListInvConfigView
    {
        //======== EVENT SECITON ========
        #region EVENT
        public event VoidEventHandler OnViewInitialize;
        public event VoidEventHandler OnViewFinalizeClose;
        public event Action SelectInvConfig;                        //Select config
        public event Action<int> DeleteConfig;
        public event Action SearchConfig;
        public event Action ClearSearchConfig;
        #endregion EVENT
        //======== EVENT SECITON ========

        //======= PROPERTY SECTION ======
        #region PROPERTY
        public string SearchCriteria 
        {
            get{ return txtSearchCriteria.Text; }
        }
        public List<Object> InvConfigGridDataSource 
        {
            set
            {
                if (value != null)
                {
                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = value;
                    dgvList.DataSource = bindingSource;
                }
            }

            get
            {
                BindingSource bindingSource = new BindingSource();
                bindingSource.DataSource = dgvList.DataSource;
                return bindingSource.List.Cast<Object>().ToList();
            }
        }
        public String[,] InvConfigDisplayColumn
        {
            set
            {
                if (dgvList.DataSource != null)
                {
                    for (int i = 0; i < dgvList.Columns.Count; i++)
                    {
                        string result = (from colIdx in Enumerable.Range(0, value.GetLength(0))
                                         where value[colIdx, 0] == dgvList.Columns[i].Name
                                         select value[colIdx, 1]).FirstOrDefault() ?? null;
                        if (result != null)
                        {
                            dgvList.Columns[i].Visible = true;
                            dgvList.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                            dgvList.Columns[i].HeaderText = result;
                        }
                        else
                        {
                            dgvList.Columns[i].Visible = false;
                        }
                    }
                }
                //Add delete button
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListInvConfigForm));
                DataGridViewImageColumn deleteCol = new DataGridViewImageColumn();
                dgvList.Columns.Add(deleteCol);
                deleteCol.HeaderText = "Delete";
                deleteCol.Width = 50;
                deleteCol.Image = new Bitmap((System.Drawing.Image)(Properties.Resources.Delete), 15, 15);

                deleteCol.Name = "Delete";
            }
        }
        public Object InvConfig
        {
            get
            {
                return ((BindingSource)dgvList.DataSource)[dgvList.CurrentRow.Index];
            }
        }
        public Boolean RegisDLL 
        {
            get { return chkRegisDLL.Checked; } 
        }
        public Boolean InstallInterfaceService 
        {
            get { return chkRegisInterface.Checked; } 
        }
        #endregion PROPERTY
        //======= PROPERTY SECTION ======
        public ListInvConfigForm()
        {
            this.InitializeComponent();
            this.btnSearch.Click += OnSearch_Click;
            this.btnClearSearch.Click += OnClearSearch_Click;
            this.dgvList.CellClick += OnDataGridView1_CellClick;
            this.dgvList.DoubleClick += OnSelectInvConfig_Click;
            this.btnOK.Click += OnSelectInvConfig_Click;
        }

        private void OnSearch_Click(object sender, EventArgs e)
        {
            if (this.SearchConfig != null)
            {
                this.SearchConfig();
            }
        }
        private void OnClearSearch_Click(object sender, EventArgs e)
        {
            if (this.ClearSearchConfig != null)
            {
                this.ClearSearchConfig();
            }
        }

        private void OnDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Do nothing if a header is clicked
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            switch (dgvList.Columns[e.ColumnIndex].Name)
            {
                case "Delete":
                    DialogResult dialogResult = MessageBox.Show(Properties.Resources.ErrMsgConfirmDelete, Properties.Resources.ErrMsgConfirmCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if ((dialogResult == DialogResult.Yes) && (this.DeleteConfig != null))
                    {
                        this.DeleteConfig(Convert.ToInt16(dgvList.SelectedRows[0].Cells[1].Value));
                        dgvList.Rows.RemoveAt(dgvList.SelectedRows[0].Index);
                    }
                    break;
            }
        }
        private void OnSelectInvConfig_Click(object sender, EventArgs e)
        {
            if (this.SelectInvConfig != null)
            {
                if (dgvList.CurrentRow != null)
                {
                    this.SelectInvConfig();
                    this.OnViewFinalizeClose();
                }
            }
        }

        public void OnShowView()
        {
            ShowDialog(); 
        }

        public void CloseView()
        {
            this.Dispose();
        }

        public void ShowView()
        {
            this.ShowDialog();
        }
        public void RaiseVoidEvent(VoidEventHandler @event)
        {

        }
        #region hotkey
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Enter))
            {
                OnSearch_Click(null, null);
                return true;
            }
            else if (keyData == (Keys.Escape))
            {
                OnClearSearch_Click(null, null);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion hotkey
    }
}
