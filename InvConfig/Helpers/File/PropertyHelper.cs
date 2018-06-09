/*
 * =============================================================
 * Author         :   TM-WMSL
 * Date           :   
 * Description    :   
 * =======================================================================================================================================================
 * Doc No#                      Edit By                 Date            Status          Desc.
 * =======================================================================================================================================================
 *
 * =======================================================================================================================================================
*/
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.file.Config
{
    public class PropertyHelper
    {
        private const string subProductFormat = @"SOFTWARE\{0}\{1}";
        private const string productFormat = @"SOFTWARE\{0}";
        
        private const string product = "Bonanza";
        

        private static bool os64Bit = IntPtr.Size == 8;

        public static Dictionary<string, string> LoadConfig(string filename)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            if (!File.Exists(@filename))
                return properties;

            StreamReader reader = new StreamReader(File.OpenRead(@filename));

            while (!reader.EndOfStream)
            {
                string property = reader.ReadLine();
                string[] keyvalue = property.Split('=');
                if(keyvalue.Length == 2)
                    properties.Add(keyvalue[0], keyvalue[1]);
            }

            reader.Close();

            return properties;
        }

        public static void ClearOldConfig(string filename)
        {
            File.WriteAllText(filename, String.Empty);
        }

        public static void WriteConfig(string filename, IDictionary<string, string> properties)
        {
            StreamWriter writer = new StreamWriter(File.OpenWrite(filename));

            string pattern = "{0}={1}";
            foreach (string key in properties.Keys)
            {
                writer.WriteLine(string.Format(pattern, key, properties[key]));
            }
            writer.Close();
        }

        public static void WriteConfig(string filename, IDictionary<string, IDictionary<string, IDictionary<string, string>>> properties)
        {
            StreamWriter writer = new StreamWriter(File.OpenWrite(filename));

            string categoryPattern = "<=={0}=================================================>";
            string keyPattern = "<======================{0}======================>";
            string pattern = "{0}={1}";
            foreach (string category in properties.Keys)
            {
                writer.WriteLine(string.Format(categoryPattern, category));
                IDictionary<string, IDictionary<string, string>> innerProperties = properties[category];
                foreach (string key in innerProperties.Keys)
                {
                    IDictionary<string, string> valueProperties = innerProperties[key];
                    writer.WriteLine(string.Format(keyPattern, key));
                    foreach (string innerkey in valueProperties.Keys)
                    {
                        writer.WriteLine(string.Format(pattern, innerkey, valueProperties[innerkey]));
                    }
                }
            }
            writer.Close();
        }

        private static bool writeToLocalMachine
        {
            get
            {
                string value = ConfigurationManager.AppSettings["WriteToLocalMachine"];
                return bool.Parse(value);
            }
        }

        private static RegistryKey getRootKey(bool is64)
        {

            if (writeToLocalMachine)
            {
                return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (os64Bit && is64) ? RegistryView.Registry64 : RegistryView.Registry32);
            }
            else
            {
                return RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, (os64Bit && is64) ? RegistryView.Registry64 : RegistryView.Registry32);
            }
        }


        private static RegistryKey getSubKey(RegistryKey root, string subProduct)
        {

            RegistryKey subKey = root.OpenSubKey(string.Format(subProductFormat, product, subProduct), true);
            if (subKey == null)
            {

                RegistryKey productKey = root.OpenSubKey(string.Format(productFormat, product), false);
                if (productKey == null)
                {
                    RegistryKey createKey = root.CreateSubKey(string.Format(productFormat, product));
                }

                RegistryKey subProductKey = root.OpenSubKey(string.Format(subProductFormat, product, subProduct), false);
                if (subProductKey == null)
                {
                    RegistryKey createKey = root.CreateSubKey(string.Format(subProductFormat, product, subProduct));
                }

                subKey = root.OpenSubKey(string.Format(subProductFormat, product, subProduct), true);
            }
            return subKey;
        }

        public static void InitialRegistry(string subProduct,bool is64, IDictionary<string,string> properties)
        {
            RegistryKey rootKey = getRootKey(is64);
            RegistryKey subKey = getSubKey(rootKey, subProduct);

            foreach (string key in properties.Keys)
            {
                subKey.SetValue(key, properties[key].Trim());
            }
        }

        static bool is64BitProcess = (IntPtr.Size == 8);
        public static bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
            else
            {
                return false;
            }
        }

        public static Dictionary<string, string> FilterProperties(IDictionary<string, string> pProperties, string pPrefix)
        {
            Dictionary<string, string> filterProp = new Dictionary<string, string>();

            foreach (string key in pProperties.Keys)
            {
                if (key.StartsWith(pPrefix))
                {
                    filterProp.Add(key.Substring(pPrefix.Length).Trim(), pProperties[key]);
                }
            }

            return filterProp;
        }
    }
}
