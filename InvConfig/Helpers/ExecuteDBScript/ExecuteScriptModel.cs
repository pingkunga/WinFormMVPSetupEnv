using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InvConfig.Helper.ExcuteDBScript
{
    public class ExecuteScriptModel
    {
        private int scriptNumber;

        public int ScriptNumber
        {
            get { return scriptNumber; }
            set { scriptNumber = value; }
        }
        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private Boolean isError;

        public Boolean IsError
        {
            get { return isError; }
            set { isError = value; }
        }

        private string errorLog;

        public string ErrorLog
        {
            get { return errorLog; }
            set { errorLog = value; }
        }

        public ExecuteScriptModel(int p_ScriptNumber, string p_FileName, string p_ErrorLog)
        {
            this.ScriptNumber = p_ScriptNumber;
            this.FileName = p_FileName;
            this.IsError = false;           //Assume true
            this.ErrorLog = p_ErrorLog;
        }

    }
}
