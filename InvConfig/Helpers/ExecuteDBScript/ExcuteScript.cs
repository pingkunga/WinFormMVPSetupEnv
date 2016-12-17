using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InvConfig.Helper.Event;
using InvConfig.Helpers.ExecuteDBScript;

namespace InvConfig.Helper.ExcuteDBScript
{
    public enum ExcuteScriptType
    {
        ExcuteSpecficScript,
        ExcuteScript,
        ExcuteScriptInFolder,
        ExecuteScriptByScriptNo
    }

    public abstract class ExcuteScript
    {
        public const string PATH_NO_FOUND = "Path not found !!!";
        private int currentScriptNo;

        public int CurrentScriptNo
        {
            get { return currentScriptNo; }
            set { currentScriptNo = value; }
        }

        private string errorMessagePath;

        public string ErrorMessagePath
        {
            get { return errorMessagePath; }
            set { errorMessagePath = value; }
        }
         
        public List<ExecuteScriptModel> executeResultList = new List<ExecuteScriptModel>();
        public List<LastExecuteScriptModel> lastExecuteList;
        private string resultFormat = "{0,-10}|{1,-60}|{2,-800}";
        private string summaryFormat = "{0,-20}:  {1,-60}";
        public event MessageEventHandle MsgEventHandle;
        public string GetScriptByFile(string p_scriptPath)
        {
            FileInfo file = new FileInfo(p_scriptPath);
            return file.OpenText().ReadToEnd();
        }
        #region FireMessageEvent
        public void FireMessageEvent(string message)
        {
            MessageEvent msgEvent = new MessageEvent();
            msgEvent.message = message;
            if (MsgEventHandle != null)
            {
                MsgEventHandle(this, msgEvent);
            }
            //MsgEventHandle = null;
        }

        public void FireHeaderResult()
        {
            FireMessageEvent("<<-----EXCUTE SCRIPT RESULT----->>");
            FireMessageEvent(GetHorizontalLine(400));
            FireMessageEvent(string.Format(resultFormat,"SCRIPT NO", "FILE NAME", "ERROR LOG"));
            FireMessageEvent(GetHorizontalLine(400));
        }

        public void FireExcuteResult(ExecuteScriptModel excuteResult)
        {
            if (excuteResult.ScriptNumber == 0)
            {
                FireMessageEvent(string.Format(resultFormat, "-", excuteResult.FileName, excuteResult.ErrorLog));
            }
            else
            {
                FireMessageEvent(string.Format(resultFormat, excuteResult.ScriptNumber.ToString(), excuteResult.FileName, excuteResult.ErrorLog));
            }
        }

        public void FireSummaryResult()
        {
            FireMessageEvent(GetHorizontalLine(400));
            FireMessageEvent("<<--------SUMMARY RESULT-------->>");
            int totalFiles = executeResultList.Count;
            FireMessageEvent(string.Format(summaryFormat, "Total File", totalFiles.ToString() + " files"));
            int folderCounts = (from c in executeResultList select c).Distinct().Count();
            FireMessageEvent(string.Format(summaryFormat, "Total Folder", folderCounts.ToString() + " folders"));
            int errorCounts = executeResultList.Count(error => error.IsError == true);
            FireMessageEvent(string.Format(summaryFormat, "Pass Excute Script", (totalFiles - errorCounts).ToString()));
            FireMessageEvent(string.Format(summaryFormat, "Error Excute Script", (errorCounts).ToString()));
            FireMessageEvent(GetHorizontalLine(400));
        }

        public string GetHorizontalLine(int p_Length)
        {
            StringBuilder sb = new StringBuilder();
            return sb.Append('=', p_Length).ToString();
        }

        public string GetHorizontalLine(int p_Length, char p_LineChar)
        {
            StringBuilder sb = new StringBuilder();
            return sb.Append(p_LineChar, p_Length).ToString();
        }
        #endregion FireMessageEvent

        #region CodeTemplete Excute Script
        //public abstract string Title { get; }
        public abstract void SetConnection(string p_ConnString);


        #region This region will execute script and save log into database except method ExecuteSpecficScript
        
        public abstract void ExecuteScriptMain(ExcuteScriptType p_ExcuteScriptType,string p_BasePath,  Boolean p_MakeChangeOnDB, string p_GenerateScriptPath, Boolean p_GernerateBAT);
        public abstract void ExecuteScriptMain(ExcuteScriptType p_ExcuteScriptType, string p_BasePath,string p_ScriptNumber, string p_SearchPattern, Boolean p_MakeChangeOnDB, string p_GenerateScriptPath, Boolean p_GernerateBAT);
        public abstract void ExecuteSpecficScript(string p_ScriptPath); 
        public abstract void ExecuteScriptFile(string p_ScriptPath);
        public abstract void ExecuteScriptInFolder(string p_ScriptPath);
        public abstract void ExecuteScriptByScriptNo(string p_BasePath, string p_ScriptNumber, string p_SearchPattern);

        public abstract Boolean CheckTableScriptLogExist();
        public abstract List<LastExecuteScriptModel> GetLastExcuteScript();
        public abstract void UpdateLastExcuteScript();
        #endregion This region will execute script and save log into database except method ExecuteSpecficScript

        public abstract void GenerateScriptFile(string p_BasePath, string p_GenerateScriptPath);
        public abstract void GenerateScriptFileByScriptNo(string p_BasePath, string p_ScriptNumber, string p_SearchPattern, string p_GenerateScriptPath);
        #endregion CodeTemplate Excute Script
    }
}
