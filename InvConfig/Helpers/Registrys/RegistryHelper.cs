using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace Helpers.Registrys
{
    public static class RegistryHelper
    {
        private const string HKEY_LOCALMACHINE = "HKEY_LOCAL_MACHINE";
        private const string HKEY_CURRENT_USER = "HKEY_CURRENT_USER";
        private const string HKEY_CURRENT_CONFIG = "HKEY_CURRENT_CONFIG";
        private const string HKEY_CLASSES_ROOT = "HKEY_CLASSES_ROOT";
        private const string HKEY_USERS = "HKEY_USERS";
        //Data Member
        //1.registry subkey
        public static String RegistryPath { get; set; }
        //2.registry 
        private static RegistryKey baseRegistryKey = Registry.LocalMachine;
        /// <summary>
        /// A property to set the BaseRegistryKey value.
        /// (default = Registry.LocalMachine)
        /// </summary>
        public static RegistryKey BaseRegistryKey
        {
            get { return baseRegistryKey; }
            set { baseRegistryKey = value; }
        }
       
        public static RegistryKey GetRegistryBaseFormString(string p_BaseRegistryPath)
        {
            if (p_BaseRegistryPath.Equals(HKEY_LOCALMACHINE))
            {
                return Registry.LocalMachine;
            }
            else if (p_BaseRegistryPath.Equals(HKEY_CURRENT_USER))
            {
                return Registry.CurrentUser;
            }
            else if (p_BaseRegistryPath.Equals(HKEY_CURRENT_CONFIG))
            {
                return Registry.CurrentConfig;
            }
            else if (p_BaseRegistryPath.Equals(HKEY_CLASSES_ROOT))
            {
                return Registry.ClassesRoot;
            }
            else if (p_BaseRegistryPath.Equals(HKEY_USERS))
            {
                return Registry.Users;
            }
            else
            {
                return Registry.LocalMachine;
            }
        }
        //Fn check Registry in Path
        public static Boolean checkRegistryExist()
        {
            Boolean lblncheckRegistryExist = false;
            try
            {
                RegistryKey registryKey = baseRegistryKey;
                RegistryKey registrySubKey = registryKey.OpenSubKey(RegistryPath);
                // If the RegistryKey exists...
                if (registrySubKey != null)
                {
                    //if found key return true
                    lblncheckRegistryExist = true;
                }
                else
                {
                    //not found
                    lblncheckRegistryExist = false;
                }
            }
            catch
            {
                throw;
            }
            return lblncheckRegistryExist;
        }

        //Fn check Registry in Path
        public static Boolean checkRegistryExist(Boolean p_CreateSubkey)
        {
            Boolean lblncheckRegistryExist = false;
            try
            {
                RegistryKey registryKey = baseRegistryKey;
                RegistryKey registrySubKey = registryKey.OpenSubKey(RegistryPath);
                // If the RegistryKey exists...
                if (registrySubKey != null)
                {
                    //if found key return true
                    lblncheckRegistryExist = true;
                }
                else
                {
                    if (p_CreateSubkey == true)
                    {
                        registryKey.CreateSubKey(RegistryPath);
                    }
                    else
                    {
                        //not found
                        lblncheckRegistryExist = false;
                    }
                }
            }
            catch
            {
                throw;
            }
            return lblncheckRegistryExist;
        }
        //Fn to Add registry subkey 
        public static bool createSubkey(string p_basePath, string p_paramName)
        {
            //p_basePath clsReg.sKey = "SYSTEM\CurrentControlSet\Services\" & .ServiceName
            //p_paramName
            RegistryKey registryKey = baseRegistryKey;
            RegistryKey registrySubKey = registryKey.OpenSubKey(p_basePath, true);
            registrySubKey.CreateSubKey(p_paramName);
            return true;
        }
        //Fn to read Registry Value
        public static String readRegistryValue(String pStrRegistryKey)
        {
            String lStrRegistryValue = null;
            RegistryKey registryKey = baseRegistryKey;
            RegistryKey registrySubKey = registryKey.OpenSubKey(RegistryPath);
            // If the RegistrySubKey doesn't exist -> (null)
            if (registrySubKey == null)
            {
                lStrRegistryValue = null;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    lStrRegistryValue = (string)registrySubKey.GetValue(pStrRegistryKey.ToUpper());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return lStrRegistryValue;
        }
        //Fn to write Registry Value
        public static bool writeRegistryValue(string pStrRegistryKey, object pObjValue)
        {
            try
            {
                // Setting
                RegistryKey registryKey = baseRegistryKey;
                RegistryKey registrySubKey = registryKey.OpenSubKey(RegistryPath, true);
                // Save the value
                registrySubKey.SetValue(pStrRegistryKey.ToUpper(), pObjValue);
                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static List<string> GetRegisWMSLComponent()
        {
            RegistryKey t_clsidKey = Registry.ClassesRoot.OpenSubKey("CLSID");
            List<string> regisWMSLCompoment = new List<string>();
            foreach (string subKey in t_clsidKey.GetSubKeyNames())
            {
                //For each CLSID\GUID key we get the InProcServer32 sub-key .
                RegistryKey t_clsidSubKey = Registry.ClassesRoot.OpenSubKey("CLSID\\" + subKey + "\\InProcServer32");

                if ((t_clsidSubKey != null))
                {
                    //in the case InProcServer32 exist we get the default value wich contains the path of the COM file.
                    //string t_valueName = (from value in t_clsidSubKey.GetValueNames() 
                    //                      where String.IsNullOrEmpty(value)
                    //                      select value).ToString(); 

                    string[] t_valueName = t_clsidSubKey.GetValueNames().Where(item => String.IsNullOrEmpty(item)).ToArray();
                    if ((t_valueName != null) && (t_valueName.Length > 0))
                    {
                        string t_value = t_clsidSubKey.GetValue(t_valueName[0]).ToString();
                        if (t_value.Contains("wmsl") || t_value.Contains("WMSL"))
                        {
                            if (File.Exists(t_value))
                            {
                                string version = FileVersionInfo.GetVersionInfo(t_value).ProductVersion;
                                regisWMSLCompoment.Add(t_value + "(" + version + ")");
                            }
                            else
                            {
                                regisWMSLCompoment.Add(t_value + "(File not exists on this machine)");
                            }
                        }
                    }
                }
            }
            regisWMSLCompoment = regisWMSLCompoment.Distinct().ToList();
            regisWMSLCompoment.Sort();
            return regisWMSLCompoment;
        }
    }
}

