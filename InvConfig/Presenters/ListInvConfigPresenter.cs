using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InvConfig.Views;
using InvConfig.Models;

namespace InvConfig.Presenters
{
    public class ListInvConfigPresenter : Presenter<IView>
    {
        private readonly IListInvConfigView listConfigView;
        private readonly InvConfigMapper invConfigMapper;
        private Boolean isSelectConfig;
        private InvConfigModel selectConfig;

        public string[,] DisplayColumn
        {
            get
            {
                return new string[,]
	            {
                    //Column Name | Display Name 
	                {"configID", "Config ID"},
	                {"configName", "Config Name"},
                    {"configCategory", "Config Category"},
	                {"bnzDatabaseServer", "Database Server"},
	                {"bnzDatabaseName", "Database Name"}
	            };
            }
        }

        public Boolean IsSelectConfig
        {
            set { isSelectConfig = value; }
            get { return isSelectConfig; }
        }

        public InvConfigModel SelectConfig
        {
            set { selectConfig = value; }
            get { return selectConfig; }
        }

        public ListInvConfigPresenter(IListInvConfigView view) : base(view)
        {
            this.listConfigView = view;
            invConfigMapper = new InvConfigMapper();
            this.OnViewInitialize();
        }

        public override void OnViewInitialize()
        {
            base.OnViewInitialize();
            //Load Config 
            LoadInvConfig();
            IsSelectConfig = false;
            //Add Event
            this.listConfigView.SearchConfig += SearchConfig;
            this.listConfigView.DeleteConfig += DeleteConfig;
            this.listConfigView.SelectInvConfig += SelectInvConfig;
            this.listConfigView.ClearSearchConfig += ClearSearchConfig;
            
        }

        public override void OnViewFinalizeClose()
        {
            base.OnViewFinalizeClose();

        }
        private void SearchConfig()
        {
            try
            {
                //Get Data form grid
                List<InvConfigModel> invConfigList = this.listConfigView.InvConfigGridDataSource.Cast<InvConfigModel>().ToList<InvConfigModel>();
                if (invConfigList != null)
                {
                    string searchCriteria = this.listConfigView.SearchCriteria.ToUpper();
                    //Filter Data 
                    List<InvConfigModel> filterInvConfigList = invConfigList.FindAll(col => (col.configName.ToUpper().Contains(searchCriteria)) ||
                                                                                            (col.configCategory.ToUpper().Contains(searchCriteria)) ||
                                                                                            (col.bnzDatabaseServer.ToUpper().Contains(searchCriteria)) ||
                                                                                            (col.bnzDatabaseName.ToUpper().Contains(searchCriteria))
                                                                                    );
                    //Set Data to Grid
                    this.listConfigView.InvConfigGridDataSource = filterInvConfigList.OfType<Object>().ToList<Object>();
                    //Set Format
                    this.listConfigView.InvConfigDisplayColumn = DisplayColumn;
                }
            }
            catch (Exception ex)
            {
                //Keep Lop
                log.Error(ex.Message, ex);
            }
        }

        private void DeleteConfig(int p_ConfigID)
        {
            invConfigMapper.DeleteConfig(p_ConfigID);
        }
        private void ClearSearchConfig()
        {
            LoadInvConfig();
        }

        private void LoadInvConfig()
        {
            //Load Config 
            listConfigView.InvConfigGridDataSource = (invConfigMapper.GetAllInvConfig()).OfType<Object>().ToList<Object>();
            listConfigView.InvConfigDisplayColumn = DisplayColumn;
        }
        private void SelectInvConfig()
        {
            //Get Select Config
            InvConfigModel invConfigModel = (InvConfigModel)this.listConfigView.InvConfig;
            invConfigModel.isInstallDllAndOCX = this.listConfigView.RegisDLL;
            invConfigModel.isInstallInterfaceService = this.listConfigView.InstallInterfaceService;
            SelectConfig = invConfigModel;
            //Make Flag isSelect = true
            IsSelectConfig = true;
            //Close View
            this.CloseView();
        }  


    }
}
