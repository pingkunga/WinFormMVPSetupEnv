using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InvConfig.Views;

namespace InvConfig.Presenters
{
    public abstract class Presenter<V> : IPresenter<V> where V : IView
    {
        //Create for logging
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public Presenter(V view)
        {
            if (view == null) throw new ArgumentNullException("view");

            View = view;
            //
            View.OnViewInitialize += OnViewInitialize;
            View.OnViewFinalizeClose += OnViewFinalizeClose;
            //
            OnCloseView += View.CloseView;
            OnShowView += View.ShowView;
        }
        public virtual V View { get; private set; }

      
        public virtual void OnViewInitialize() { }
        public virtual void OnViewFinalizeClose() 
        {
            //Clear view
            this.CloseView();
            //Clear Event
            View.OnViewInitialize -= OnViewInitialize;
            View.OnViewFinalizeClose -= OnViewFinalizeClose;
            OnCloseView -= View.CloseView;
            OnShowView -= View.ShowView;
            //Clear presenter
            
            //Clear GC
            //GC.Collect();
        }
        
        public virtual void ShowView() { RaiseVoidEvent(OnShowView); }
        public virtual void CloseView() { RaiseVoidEvent(OnCloseView); }
        public void RaiseVoidEvent(VoidEventHandler @event) { if (@event != null) @event(); }

        public virtual event VoidEventHandler OnCloseView;
        public virtual event VoidEventHandler OnShowView;
    }
}
