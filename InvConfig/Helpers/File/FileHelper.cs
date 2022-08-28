using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace Helpers.Files
{
    public static class FileHelper
    {
        private const string PATH_NO_FOUND = "Path not found !!!";
        public static void OpenWindowsExplorer(string p_Path)
        {
            try
            {
                Process.Start(p_Path);
            }
            catch
            {
                throw new ApplicationException(PATH_NO_FOUND);
            }
        }

        public static string ShowFolderDialog(string p_PrevPath)
        {
            using (FolderBrowserDialog DlgFolderBrowser = new FolderBrowserDialog())
            {
                String lstrPath = null;
                if (string.IsNullOrEmpty(p_PrevPath))
                {
                    DlgFolderBrowser.RootFolder = Environment.SpecialFolder.Desktop;
                }
                else
                {
                    DlgFolderBrowser.SelectedPath = p_PrevPath;
                }

                DialogResult dlgResult = DlgFolderBrowser.ShowDialog();
                if (dlgResult == DialogResult.OK)
                {
                    lstrPath = DlgFolderBrowser.SelectedPath;
                }
                else
                {
                    lstrPath = p_PrevPath;
                }
                return lstrPath;
            }
        }

        public static Boolean CheckIsFolder(string p_Path)
        {
            FileAttributes attr;
            Boolean result = false;
            try
            {
                //Check Path is File or folder
                attr = System.IO.File.GetAttributes(p_Path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        public static Boolean IsFolderExist(string p_Path)
        {
            try
            {
                return Directory.Exists(p_Path);
            }
            catch
            {
                return false;
            }
        }

        public static void WriteLog(string p_Message, string p_ExportPath)
        {
            System.IO.File.WriteAllText(p_ExportPath, p_Message);

        }

        public static void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }
    }
}
