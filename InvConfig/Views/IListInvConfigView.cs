using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InvConfig.Views
{
    public interface IListInvConfigView : IView
    {
        //======== EVENT SECITON ========
        event Action SelectInvConfig;                        //Select config
        event Action<int> DeleteConfig;
        event Action SearchConfig;              
        event Action ClearSearchConfig;
        //======== EVENT SECITON ========
        //======= PROPERTY SECTION ======
        string SearchCriteria { get; }
        List<Object> InvConfigGridDataSource { set; get; }
        String[,] InvConfigDisplayColumn { set; }
        Object InvConfig{ get;}
        Boolean RegisDLL { get; }
        Boolean InstallInterfaceService { get; }
        //======= PROPERTY SECTION ======
    }
}
