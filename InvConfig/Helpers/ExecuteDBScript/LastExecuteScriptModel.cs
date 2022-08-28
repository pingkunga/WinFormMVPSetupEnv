using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InvConfig.Helpers.ExecuteDBScript
{
    public  class LastExecuteScriptModel
    {
        public int ScriptNo { get; set; }
        public string ScriptName { get; set; }
        public DateTime LastScriptRun { get; set; }
        public Boolean IsNew { get; set; }

        public LastExecuteScriptModel()
        {
        }
        public LastExecuteScriptModel(int p_ScriptNo, string p_ScriptName, DateTime p_LastScriptRun, Boolean p_IsNew)
        {
            if (p_ScriptNo > 0)
            {
                ScriptNo = p_ScriptNo;
            }
            else
            {
                ScriptNo = 0;
            }
            ScriptName = p_ScriptName;
            LastScriptRun = p_LastScriptRun;
            IsNew = p_IsNew;
        }
    }
}
