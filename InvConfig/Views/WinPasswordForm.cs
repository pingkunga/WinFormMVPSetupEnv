using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InvConfig.Presenters;
using Helpers.Controls;

namespace InvConfig.Views
{
    public partial class WinPasswordForm : Form, IWinPasswordView
    {
        //======== EVENT SECITON ========
        public event VoidEventHandler OnViewInitialize;
        public event VoidEventHandler OnViewFinalizeClose;
        public event Action OKWinPassword;

        public WinPasswordForm()
        {
            InitializeComponent();
            BindComponent();
            ValidateComponent();
        }

        public void BindComponent()
        {
            this.btnOK.Click += OnOK_Click;
            this.btnCancel.Click += OnCancel_Click;
        }
        #region Property
        public string Username
        {
            get
            {
                string username = null;
                txtUsername.InvokeIfRequired(() => { username = txtUsername.Text; });
                return username; 
            }
            set
            {
                txtUsername.InvokeIfRequired(() => { txtUsername.Text = value; });
            }
        }

        public string Password
        {
            get
            {
                string password = null;
                txtPassword.InvokeIfRequired(() => { password = txtPassword.Text; });
                return password;
            }
            set
            {
                txtPassword.InvokeIfRequired(() => { txtPassword.Text = value; });
            }
        }
        #endregion Property

        private void ValidateComponent()
        {
            //http://www.codeproject.com/Articles/11904/Extended-Error-Provider
            errorProviderExtended.Controls.Add((object)txtUsername, lblUsername.Text);
            errorProviderExtended.Controls.Add((object)txtPassword, lblPassword.Text);
            errorProviderExtended.SummaryMessage = "Following fields are required !!!";
        }
        #region Event Handle
        private void OnOK_Click(object sender, EventArgs e)
        {
            if ((errorProviderExtended.CheckAndShowSummaryErrorMessage() == true) && (this.OKWinPassword != null))
            {
                this.OKWinPassword();
            }
            //this.errorProviderExtended.ClearAllErrorMessages();
        }

        private void OnCancel_Click(object sender, EventArgs e)
        {
            if (this.OnViewFinalizeClose != null)
            {
                this.OnViewFinalizeClose();
            }
        }
        #endregion Event Handle


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




        public void RaiseVoidEvent(Presenters.VoidEventHandler @event)
        {
            throw new NotImplementedException();
        }

        
    }
}
