﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Investment Toolbox")]
[assembly: AssemblyDescription("Bonanaza Investment Toolbox")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("DebuggingSoft")]
[assembly: AssemblyProduct("Investment Toolbox")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
[assembly: AssemblyTrademark("Power by DebuggingSoft")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3a18637b-cbc2-4724-aaec-cd920b0cdf5a")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("3.0.0.1")]
[assembly: AssemblyFileVersion("3.0.0.1")]

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log.config", Watch = true)]
