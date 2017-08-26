using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LiveSplit.PetThePup;
using LiveSplit.UI.Components;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Pet the Pup Autosplitter")]
[assembly: AssemblyDescription("Allows LiveSplit to automatically split or reset based on the game state.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("LiveSplit")]
[assembly: AssemblyCopyright("Copyright © 2017 .start")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("91F034E9-8717-4F86-B6A4-EBC2F33FFD04")]

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
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// LiveSplit
[assembly: ComponentFactory(typeof(PetThePupFactory))]
