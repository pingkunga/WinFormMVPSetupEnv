using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InvConfig.Helper.Event
{
    public delegate void MessageEventHandle(object sender, MessageEvent e);
    public class MessageEvent
    {
        public string message; 
    }
}
