using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace InvConfig.Helpers.Registry
{
    public static class RegisComponentHelper
    {
        //http://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
        public static string Register_COM(string p_Path)
        {
            try
            {
                using (Process regProcess = new Process())
                {
                    string output = "";
                    string regisExecute = "";
                    //'/s' : Specifies regsvr32 to run silently and to not display any message boxes.
                    string arg_fileinfo = "/s" + " " + "\"" + p_Path + "\"";
                    //This file registers .dll files as command components in the registry.
                    if (Environment.Is64BitOperatingSystem)
                    {
                        regisExecute = "C:\\Windows\\SysWOW64\\regsvr32.exe";
                    }
                    else
                    {
                        regisExecute = "C:\\Windows\\System32\\regsvr32.exe";
                    }
                    ProcessStartInfo processStartInfo = new ProcessStartInfo();
                    processStartInfo.FileName = regisExecute;
                    processStartInfo.Arguments = arg_fileinfo;
                    processStartInfo.UseShellExecute = false;
                    processStartInfo.CreateNoWindow = true;
                    processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;       //Hide result

                    processStartInfo.RedirectStandardOutput = true;
                    using (Process process = Process.Start(processStartInfo))
                    {
                        while (!process.StandardOutput.EndOfStream)
                        {
                            string line = process.StandardOutput.ReadLine();
                            // do something with line
                        }
                    }
                    return output;
                }
            }
            catch
            {
                throw;
            }
        }

        public static string UnRegister_COM(this string p_Path)
        {
            try
            {
                using (Process regProcess = new Process())
                {
                    string output = "";
                    string regisExecute = "";
                    //'/s' : Specifies regsvr32 to run silently and to not display any message boxes.
                    string arg_fileinfo = "/u /s" + " " + "\"" + p_Path + "\"";
                    Process reg = new Process();
                    //This file registers .dll files as command components in the registry.
                    if (Environment.Is64BitOperatingSystem)
                    {
                        regisExecute = "C:\\Windows\\SysWOW64\\regsvr32.exe";
                    }
                    else
                    {
                        regisExecute = "C:\\Windows\\System32\\regsvr32.exe";
                    }
                    regProcess.StartInfo.FileName = regisExecute;
                    regProcess.StartInfo.Arguments = arg_fileinfo;
                    regProcess.StartInfo.UseShellExecute = false;
                    regProcess.StartInfo.CreateNoWindow = true;
                    regProcess.StartInfo.RedirectStandardOutput = true;
                    regProcess.Start();
                    output = regProcess.StandardOutput.ReadToEnd();
                    regProcess.WaitForExit();
                    regProcess.Close();
                    return output;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
