using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InvConfig.Views
{
    public interface IWinPasswordView: IView
    {
        //======== EVENT SECITON ========
        event Action OKWinPassword;                        
        //======== EVENT SECITON ========
        //======= PROPERTY SECTION ======
        string Username { get; set; }
        string Password { get; set; }
        //======= PROPERTY SECTION ======
    }
}
