using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InvConfig.Views;

namespace InvConfig.Presenters
{
    public class WinPasswordPresenter : Presenter<IView>
    {
        private readonly IWinPasswordView WinPasswordView;
        #region Property
        public string WindowsUsername { get; set; }
        public string WindowsPassword { get; set; }
        #endregion Property
        public WinPasswordPresenter(IWinPasswordView view): base(view)
        {
            this.WinPasswordView = view;
        }

        public override void OnViewInitialize()
        {
            base.OnViewInitialize();
            //Set Value to from
            this.WinPasswordView.Username = WindowsUsername;
            this.WinPasswordView.Password = WindowsPassword;

            if (String.IsNullOrEmpty(WindowsUsername))
            {
                this.WinPasswordView.Username = Properties.Resources.DefaultWinUsername;
            }
            else
            {
                this.WinPasswordView.Username = WindowsUsername;
            }


            if (String.IsNullOrEmpty(WindowsPassword))
            {
                this.WinPasswordView.Password = Properties.Resources.DefaultWinPassword;
            }
            else
            {
                this.WinPasswordView.Password = WindowsPassword;
            }
            //Add Event
            this.WinPasswordView.OKWinPassword += OKWinPassword;
            this.ShowView();
        }

        public override void OnViewFinalizeClose()
        {
            base.OnViewFinalizeClose();
        }

        private void OKWinPassword()
        {
            this.WindowsUsername = this.WinPasswordView.Username;
            this.WindowsPassword = this.WinPasswordView.Password;
            //Close View
            this.CloseView();
        }
    }
}
