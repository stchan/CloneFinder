using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyCompany("Black Telescope Workshop")]
[assembly: AssemblyProduct("CloneFinder")]
[assembly: AssemblyCopyright("Copyright © Sherman Chan 2010 - 2011")]
[assembly: AssemblyTrademark("")]

// Make it easy to distinguish Debug and Release builds;
// for example, through the file properties window.
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyDescription("Clone Finder (Debug build)\r\n\r\nBreadth first directory traversal code from MSDN.\r\nIcons and graphics from Wikimedia Commons.\r\nSystem.Data.SQLite library provided by SQLite.org.")] // a.k.a. "Comments"
#else
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("Clone Finder\r\n\r\nBreadth first directory traversal code from MSDN.\r\nIcons and graphics from Wikimedia Commons.\r\nSystem.Data.SQLite library provided by SQLite.org.")] // a.k.a. "Comments"
#endif

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.0.1.*")]
