using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InvConfig.Views;

namespace InvConfig.Presenters
{
    public class AboutPresenter : Presenter<IView>
    {
        public AboutPresenter(IAboutView view): base(view)
        {        
            this.OnViewInitialize();
            this.ShowView();
        }

        public override void OnViewInitialize()
        {
            base.OnViewInitialize();
        }

        public override void OnViewFinalizeClose()
        {
            base.OnViewFinalizeClose();

        }
    }
}
