using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Configuration.Install;
using System.IO;
using Microsoft.Win32;

namespace Helpers.Service
{
    //Reference: http://dotnetstep.blogspot.com/2009/06/programmatically-install-window-service.html
    //Must Reference Dll;
    //>> System.Configuration.Install
    public static class ServiceHelper
    {
        private const string IMAGE_PATH_NO_FOUND = "Image Path(Service execute path) not found !!!";
        //Install Service
        public static Boolean InstallService(string p_ServiceName, 
                                             string p_DisplayName, 
                                             string p_Description,
                                             string p_ImagePath,
                                             string p_ApplicationPath,
                                             ServiceStartMode StartMode,
    		                                 ServiceAccount p_Account, 
                                             string p_Username,
                                             string p_Password)
        {
            try
            {
                ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
                if (!File.Exists(p_ImagePath))
                {
                    throw new ApplicationException(IMAGE_PATH_NO_FOUND + "[" + p_ImagePath + "]"); 
                }
                
                serviceProcessInstaller.Account = p_Account;
                serviceProcessInstaller.Username = p_Username;
                serviceProcessInstaller.Password = p_Password;

                ServiceInstaller serviceInstaller = new ServiceInstaller();
                String path = String.Format("/assemblypath={0}", p_ImagePath);
                String[] cmdline = { path };

                InstallContext Context  = new InstallContext("", cmdline);
                serviceInstaller.Context = Context;
                serviceInstaller.ServiceName = p_ServiceName;
                serviceInstaller.DisplayName = p_DisplayName;
                serviceInstaller.Description = p_Description;
                serviceInstaller.StartType = StartMode;
                serviceInstaller.Parent = serviceProcessInstaller;

                // http://bytes.com/forum/thread527221.html
                //SINST.ServicesDependedOn = new String[] {};

                System.Collections.Specialized.ListDictionary state = new System.Collections.Specialized.ListDictionary();
                serviceInstaller.Install(state);
                
                if (!string.IsNullOrEmpty(p_ApplicationPath))
                {
                    AddServiceParameterSubKey(p_ServiceName, "APPLICATION", p_ApplicationPath);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        private static void AddServiceParameterSubKey(string p_ServiceName, string p_key, string p_Value)
        {
            RegistryKey service, config;
            try
            {
                //http://stackoverflow.com/questions/291519/how-does-currentcontrolset-differ-from-controlset001-and-controlset002
                service = Registry.LocalMachine.OpenSubKey("SYSTEM\\ControlSet001\\Services\\" + p_ServiceName,true);
                if (service == null)
                {
                    service = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\" + p_ServiceName,true);
                }
                config = service.CreateSubKey("Parameters");
                config.SetValue(p_key.ToUpper(), p_Value);
                config.Close();
                service.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        //Uninstall Service
        public static void UninstallService(string p_ServiceName)
        {
            System.ServiceProcess.ServiceInstaller SINST = new System.ServiceProcess.ServiceInstaller();

            InstallContext Context = new InstallContext();
            SINST.Context = Context;
            SINST.ServiceName = p_ServiceName;
            SINST.Uninstall(null);
        }

        public static Boolean IsServiceExist(string p_ServiceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            var service = services.FirstOrDefault(s => s.ServiceName == p_ServiceName);
            return service != null;
        }
        //Get Service Path
        public static void StartService(string p_ServiceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(p_ServiceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                // ...
            }
        }

        public static void StopService(string p_ServiceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(p_ServiceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch
            {
                // ...
            }
        }

        public static void RestartService(string p_ServiceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(p_ServiceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                // ...
            }
        }

        public static string[] GetServiceStatus(string p_ServiceName)
        {

            string[] serviceStatus = new string[2];
            if (IsServiceExist(p_ServiceName))
            {
                ServiceController service = new ServiceController(p_ServiceName);
                switch (service.Status)
                {
                    case ServiceControllerStatus.Running:
                        serviceStatus[0] = "Running";
                        serviceStatus[1] = "#008000";           //GREEN-008000
                        break;
                    case ServiceControllerStatus.Stopped:
                        serviceStatus[0] = "Stopped";
                        serviceStatus[1] = "#FF0000";           //RED-FF0000
                        break;
                    case ServiceControllerStatus.Paused:
                        serviceStatus[0] = "Paused";
                        serviceStatus[1] = "#FFFF00";           //YELLOW-FFFF00 
                        break;
                    case ServiceControllerStatus.StopPending:
                        serviceStatus[0] = "Stopping";
                        serviceStatus[1] = "FFFF00";            //YELLOW-FFFF00 
                        break;
                    case ServiceControllerStatus.StartPending:
                        serviceStatus[0] = "Starting";
                        serviceStatus[1] = "#FFFF00";           //YELLOW-FFFF00 
                        break;
                    default:
                        serviceStatus[0] = "Status Changing";
                        serviceStatus[1] = "#FFFF00";           //YELLOW-FFFF00 
                        break;
                }
            }
            else
            {
                serviceStatus[0] = "Not Install";
                serviceStatus[1] = "#0000FF";                   //BLUE-0000FF
            }
            return serviceStatus;
        }

    }
}
