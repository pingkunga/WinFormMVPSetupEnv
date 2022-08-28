using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using Microsoft.SqlServer;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Data;
using System.Text.RegularExpressions;
using InvConfig.Helpers.ExecuteDBScript;
using System.Transactions;
using Helpers.Files;

namespace InvConfig.Helper.ExcuteDBScript
{
    public class MSSQLExcuteScript : ExcuteScript
    {
        private SqlConnection connection = null;
        private string connectionString = null;
        public override void SetConnection(string p_ConnString)
        {
            connectionString = p_ConnString;
            connection = new SqlConnection(p_ConnString);
        }

        public override void ExecuteScriptMain(ExcuteScriptType p_ExcuteScriptType, string p_BasePath, Boolean p_MakeChangeOnDB, string p_GenerateBATPath, Boolean p_GernerateBAT)
        {
            ExecuteScriptMain(p_ExcuteScriptType, p_BasePath, null, null, p_MakeChangeOnDB, p_GenerateBATPath, p_GernerateBAT);
        }
        public override void ExecuteScriptMain(ExcuteScriptType p_ExcuteScriptType, string p_BasePath, string p_ScriptNumber, string p_SearchPattern, Boolean p_MakeChangeOnDB, string p_GenerateBATPath, Boolean p_GernerateBAT)
        {
            #region RunScript
            try
            {
                if (p_MakeChangeOnDB == true)
                {
                    FireHeaderResult();
                    if (p_ExcuteScriptType == ExcuteScriptType.ExcuteSpecficScript)
                    {
                        ExecuteSpecficScript(p_BasePath);
                    }
                    else
                    {
                        CheckTableScriptLogExist();
                        lastExecuteList = GetLastExcuteScript();
                        switch (p_ExcuteScriptType)
                        {
                            case ExcuteScriptType.ExcuteScript:
                                ExecuteScriptFile(p_BasePath);
                                break;
                            case ExcuteScriptType.ExcuteScriptInFolder:
                                ExecuteScriptInFolder(p_BasePath);
                                break;
                            case ExcuteScriptType.ExecuteScriptByScriptNo:
                                ExecuteScriptByScriptNo(p_BasePath, p_ScriptNumber, p_SearchPattern);
                                break;
                            default:
                                throw new ApplicationException();
                        }
                        if (lastExecuteList.Count(n => n.IsNew == true) > 0)
                        {
                            UpdateLastExcuteScript();
                        }
                    }
                    FireSummaryResult();
                }
                else
                {
                    FireMessageEvent(">> No make change on DB");
                }
             }
            catch (Exception ex)
            {
                FireMessageEvent(">> " + ex.Message);
            }
            #endregion RunScript
            #region GenerateBATFile
            if (p_GernerateBAT == true)
            {
                try
                {
                    FireMessageEvent(GetHorizontalLine(50, '*'));
                    FireMessageEvent(">> Start Generate Script Process");
                    switch (p_ExcuteScriptType)
                    {
                        case ExcuteScriptType.ExcuteScript:
                        case ExcuteScriptType.ExcuteSpecficScript:
                        case ExcuteScriptType.ExcuteScriptInFolder:
                            GenerateScriptFile(p_BasePath, p_GenerateBATPath);
                            break;
                        case ExcuteScriptType.ExecuteScriptByScriptNo:
                            GenerateScriptFileByScriptNo(p_BasePath, p_ScriptNumber, p_SearchPattern, p_GenerateBATPath);
                            break;
                        default:
                            throw new ApplicationException();
                    }
                    FireMessageEvent(">> Finish Generate Script Process");
                }
                catch (Exception ex)
                {
                    FireMessageEvent(">> " + ex.Message);
                }
                finally
                {
                    FireMessageEvent(GetHorizontalLine(50, '*'));
                }
            }
            else
            {
                FireMessageEvent(">> No generate bat script");
            }
            #endregion GenerateBATFile
        }

        public override void ExecuteSpecficScript(string p_ScriptPath)
        {
            ExecuteScriptModel excuteResult = null;
            try
            {
                excuteResult = new ExecuteScriptModel(CurrentScriptNo, Path.GetFileName(p_ScriptPath), "");
                Server server = new Server(new ServerConnection(connection));
                string sqlCommand = GetScriptByFile(p_ScriptPath);
                server.ConnectionContext.ExecuteNonQuery(sqlCommand);
            }
            catch (Exception ex)
            {
                //Add Error Log
                excuteResult.IsError = true;
                excuteResult.ErrorLog = ex.InnerException.Message.ToString();
            }
            finally
            {
                FireExcuteResult(excuteResult);
                executeResultList.Add(excuteResult);
            }
        }
        public override void ExecuteScriptFile(string p_ScriptPath)
        {
            ExecuteScriptModel excuteResult = null;
            try
            {
                excuteResult = new ExecuteScriptModel(CurrentScriptNo, Path.GetFileName(p_ScriptPath), "");
                if ((lastExecuteList.FirstOrDefault(s => s.ScriptName.Equals(Path.GetFileName(p_ScriptPath))) ?? null) != null)
                {
                    throw new ApplicationException("Already run", new Exception("Already run"));
                }
                Server server = new Server(new ServerConnection(connection));
                string sqlCommand = GetScriptByFile(p_ScriptPath);
                //Excute SQL File 
                server.ConnectionContext.ExecuteNonQuery(sqlCommand);
                //Update Last Excute Script
                lastExecuteList.Add(new LastExecuteScriptModel(CurrentScriptNo, Path.GetFileName(p_ScriptPath), DateTime.Now, true));
            }
            catch (Exception ex)
            {
                //Add Error Log
                excuteResult.IsError = true;
                excuteResult.ErrorLog = ex.InnerException.Message.ToString();
            }
            finally
            {
                FireExcuteResult(excuteResult);
                executeResultList.Add(excuteResult);
            }
        }

        public override void ExecuteScriptInFolder(string p_ScriptPath)
        {
            string[] filePaths;
            try
            {
                if (Directory.Exists(p_ScriptPath))
                {
                    //If path is folder then Get List of .SQL File in folder
                    filePaths = Directory.GetFiles(p_ScriptPath, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase) || s.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)).ToArray();
                }
                else
                {
                    filePaths = new string[1];
                    filePaths[0] = p_ScriptPath;
                }

                //Execute Script
                foreach (string filePath in filePaths)
                {
                    ExecuteScriptFile(filePath);
                }
            }
            catch (Exception ex)
            {
                FireMessageEvent(">> Error occur: " + ex.Message + Environment.NewLine + ex.InnerException);
                FireMessageEvent(">> Strack Trace: " + ex.StackTrace.ToString());
            }
        }

        public override void ExecuteScriptByScriptNo(string p_BasePath, string p_ScriptNumber, string p_SearchPattern)
        {
            string[] subFolders;
            SortedDictionary<int, string> subFolderDic = new SortedDictionary<int, string>();
            #region Get sub folder in path
            if (string.IsNullOrEmpty(p_SearchPattern))
            {
                subFolders = Directory.GetDirectories(p_BasePath);
            }
            else
            {
                subFolders = Directory.GetDirectories(p_BasePath, p_SearchPattern);
            }
            #endregion Get sub folder in path
            for (int i = 0; i < subFolders.Length; i++)
            {
                int id = Convert.ToInt16(Regex.Match(new DirectoryInfo(subFolders[i]).Name, @"\d+").Value);
                subFolderDic.Add(id, subFolders[i]);
            }

            var subFolderDicFilter = subFolderDic.Where(s => s.Key >= Convert.ToInt16(p_ScriptNumber)).ToDictionary(p => p.Key, p => p.Value);
            foreach (KeyValuePair<int, string> item in subFolderDicFilter)
            {
                CurrentScriptNo = item.Key;
                ExecuteScriptInFolder(item.Value);
            }
        }

        public override Boolean CheckTableScriptLogExist()
        {
            Server server = new Server(new ServerConnection(connection));
            Database database = server.Databases[0];
            if (!database.Tables.Contains("SCRIPT_LOG"))
            {
                Table table = new Table(database, "SCRIPT_LOG");

                Column scriptNoCol = new Column(table, "SCRIPT_NO", DataType.SmallInt);
                scriptNoCol.Nullable = true;
                table.Columns.Add(scriptNoCol);

                Column scriptNameCol = new Column(table, "SCRIPT_NAME", DataType.NVarChar(256));
                scriptNameCol.Nullable = true;
                table.Columns.Add(scriptNameCol);

                Column lastScriptRunCol = new Column(table, "LAST_SCRIPT_RUN", DataType.DateTime);
                lastScriptRunCol.Nullable = true;
                table.Columns.Add(lastScriptRunCol);

                table.Create();
            }
            return true;
        }
        public override List<LastExecuteScriptModel> GetLastExcuteScript()
        {
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM SCRIPT_LOG", sqlConn))
                {
                    DataTable dt = new DataTable();
                    dt.Load(sqlCommand.ExecuteReader());
                    sqlConn.Close();
                    return (from item in dt.AsEnumerable()
                            select new LastExecuteScriptModel()
                            {
                                ScriptNo= (Int16)item["SCRIPT_NO"],
                                ScriptName = (string)item["SCRIPT_NAME"],
                                LastScriptRun = (DateTime)item["LAST_SCRIPT_RUN"],
                                IsNew = false
                            }
                           ).ToList();
                }
            }
        }

        public override void UpdateLastExcuteScript()
        {
            List<LastExecuteScriptModel> newExecuteScriptList = lastExecuteList.Where(s => s.IsNew == true).ToList<LastExecuteScriptModel>();
            //http://stackoverflow.com/questions/6374005/how-quickly-to-write-listobject-to-database
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                //Create Connection
                sqlConn.Open();
                using (TransactionScope scope = new TransactionScope())
                {
                    string sqlInsert = "INSERT INTO SCRIPT_LOG (SCRIPT_NO, SCRIPT_NAME, LAST_SCRIPT_RUN) VALUES (@SCRIPT_NO, @SCRIPT_NAME, @LAST_SCRIPT_RUN)";

                    SqlCommand sqlCommandInsert = new SqlCommand(sqlInsert, sqlConn);

                    foreach (LastExecuteScriptModel newExecuteScript in newExecuteScriptList)
                    {
                        sqlCommandInsert.Parameters.Clear();
                        sqlCommandInsert.Parameters.AddWithValue("@SCRIPT_NO", newExecuteScript.ScriptNo);
                        sqlCommandInsert.Parameters.AddWithValue("@SCRIPT_NAME", newExecuteScript.ScriptName);
                        sqlCommandInsert.Parameters.AddWithValue("@LAST_SCRIPT_RUN", newExecuteScript.LastScriptRun);
                        sqlCommandInsert.ExecuteNonQuery();
                    }
                    scope.Complete();
                }
                sqlConn.Close();
            }
        }
        public override void GenerateScriptFile(string p_BasePath,string p_GenerateScriptPath)
        {
            string[] filePaths;
            int i = 0;
            StringBuilder BATRunScript = new StringBuilder();
            StringBuilder BATScript = new StringBuilder();
            StringBuilder BATHeader = new StringBuilder();

            if (string.IsNullOrEmpty(p_GenerateScriptPath))
            {
                throw new ApplicationException(PATH_NO_FOUND + " in Generate Script Path");
            }

            if (string.IsNullOrEmpty(p_BasePath))
            {
                throw new ApplicationException(PATH_NO_FOUND + " in Base Path");
            }

            BATHeader.AppendLine("@echo off");
            BATHeader.AppendLine("cls");
            BATHeader.AppendLine();
            BATHeader.AppendLine("set ServerName=localhost");
            BATHeader.AppendLine("set UserName=invest");
            BATHeader.AppendLine("set Pwd=1nvest@WMSL");
            BATHeader.AppendLine("set DbName=BNZCUST");
            BATHeader.AppendLine();
            BATHeader.AppendLine("echo EXECUTE SCRIPTS to %DbName%");
            BATHeader.AppendLine();
            BATHeader.AppendLine(":begin");
            BATHeader.AppendLine("echo Begin - EXECUTE SCRIPTS to %DbName% %DATE% %TIME% >> ResultSummary.txt");
            BATHeader.AppendLine();
            BATHeader.AppendLine("@echo on");
            BATHeader.AppendLine();
            BATHeader.AppendLine("if not %ERRORLEVEL% == 0  goto Failure");

            if (Directory.Exists(p_BasePath))
            {
                //If path is folder then Get List of .SQL File in folder
                //copy file folder to destination folder
                FileHelper.CopyFolder(p_BasePath, p_GenerateScriptPath);
                filePaths = Directory.GetFiles(p_GenerateScriptPath, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase) || s.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            else
            {
                File.Copy(p_BasePath, p_GenerateScriptPath + "\\" + Path.GetFileName(p_BasePath), true);
                filePaths = new string[1];
                filePaths[0] = p_GenerateScriptPath;
            }
            foreach (string filePath in filePaths)
            {
                BATRunScript.Append("sqlcmd -S %ServerName% -U %UserName% -P %Pwd% -d %DbName% -b -I -i ");
                BATRunScript.Append(filePath);
                BATRunScript.Append(" -f 874 -o Result" + i + ".txt");
                BATRunScript.AppendLine();
                i++;
            }



            StringBuilder BATFooter = new StringBuilder();
            BATFooter.AppendLine(":end");
            BATFooter.AppendLine("echo End - EXECUTE SCRIPTS to %DbName% Success on %DATE% %TIME%  >> ResultSummary.txt");
            BATFooter.AppendLine("exit");
            BATFooter.AppendLine();
            BATFooter.AppendLine(":Failure");
            BATFooter.AppendLine("echo End - EXECUTE SCRIPTS to %DbName% Failure on %DATE% %TIME%  >> ResultSummary.txt");
            BATFooter.AppendLine("exit");

            BATScript.Append(BATHeader.ToString());
            BATScript.Append(BATRunScript.ToString());
            BATScript.Append(BATFooter.ToString());
            File.WriteAllText(p_GenerateScriptPath + "\\GenerateScriptBAT.bat", BATScript.ToString());
        }

        public override void GenerateScriptFileByScriptNo(string p_BasePath, string p_ScriptNumber, string p_SearchPattern, string p_GenerateScriptPath)
        {
            string[] filePaths;
            int i;

            if (string.IsNullOrEmpty(p_GenerateScriptPath))
            {
                throw new ApplicationException(PATH_NO_FOUND + " in Generate Script Path");
            }

            if (string.IsNullOrEmpty(p_BasePath))
            {
                throw new ApplicationException(PATH_NO_FOUND + " in Base Path(Local Path)");
            }
            StringBuilder BATRunScript = new StringBuilder();
            StringBuilder BATScript = new StringBuilder();
            StringBuilder BATHeader = new StringBuilder();

            BATHeader.AppendLine("@echo off");
            BATHeader.AppendLine("cls");
            BATHeader.AppendLine();
            BATHeader.AppendLine("set ServerName=localhost");
            BATHeader.AppendLine("set UserName=invest");
            BATHeader.AppendLine("set Pwd=1nvest@WMSL");
            BATHeader.AppendLine("set DbName=BNZCUST");
            BATHeader.AppendLine();
            BATHeader.AppendLine("echo EXECUTE SCRIPTS to %DbName%");
            BATHeader.AppendLine();
            BATHeader.AppendLine(":begin");
            BATHeader.AppendLine("echo Begin - EXECUTE SCRIPTS to %DbName% %DATE% %TIME% >> ResultSummary.txt");
            BATHeader.AppendLine();
            BATHeader.AppendLine("@echo on");
            BATHeader.AppendLine();
            BATHeader.AppendLine("if not %ERRORLEVEL% == 0  goto Failure");

            string[] subFolders;
            SortedDictionary<int, string> subFolderDic = new SortedDictionary<int, string>();
            #region Get sub folder in path
            if (string.IsNullOrEmpty(p_SearchPattern))
            {
                subFolders = Directory.GetDirectories(p_BasePath);
            }
            else
            {
                subFolders = Directory.GetDirectories(p_BasePath, p_SearchPattern);
            }
            #endregion Get sub folder in path
            for (i = 0; i < subFolders.Length; i++)
            {
                int id = Convert.ToInt16(Regex.Match(new DirectoryInfo(subFolders[i]).Name, @"\d+").Value);
                subFolderDic.Add(id, subFolders[i]);
            }

            var subFolderDicFilter = subFolderDic.Where(s => s.Key >= Convert.ToInt16(p_ScriptNumber)).ToDictionary(p => p.Key, p => p.Value);
            i = 0;
            foreach (KeyValuePair<int, string> item in subFolderDicFilter)
            {
                FileHelper.CopyFolder(item.Value, Path.Combine(p_GenerateScriptPath, Path.GetFileName(item.Value)));
                filePaths = Directory.GetFiles(Path.Combine(p_GenerateScriptPath, Path.GetFileName(item.Value)), "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase) || s.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)).ToArray();
                
                foreach (string filePath in filePaths)
                {
                    BATRunScript.Append("sqlcmd -S %ServerName% -U %UserName% -P %Pwd% -d %DbName% -b -I -i ");
                    BATRunScript.Append(Path.Combine(Path.GetFileName(Path.GetDirectoryName(filePath)), Path.GetFileName(filePath)));
                    BATRunScript.Append(" -f 874 -o Result" + i + ".txt");
                    BATRunScript.AppendLine();
                    i++;
                }
            }
            StringBuilder BATFooter = new StringBuilder();
            BATFooter.AppendLine(":end");
            BATFooter.AppendLine("echo End - EXECUTE SCRIPTS to %DbName% Success on %DATE% %TIME%  >> ResultSummary.txt");
            BATFooter.AppendLine("exit");
            BATFooter.AppendLine();
            BATFooter.AppendLine(":Failure");
            BATFooter.AppendLine("echo End - EXECUTE SCRIPTS to %DbName% Failure on %DATE% %TIME%  >> ResultSummary.txt");
            BATFooter.AppendLine("exit");

            BATScript.Append(BATHeader.ToString());
            BATScript.Append(BATRunScript.ToString());
            BATScript.Append(BATFooter.ToString());
            File.WriteAllText(p_GenerateScriptPath + "\\GenerateScriptBAT.bat", BATScript.ToString());
        }
    }
}
