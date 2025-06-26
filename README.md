**CloneFinder** is a duplicate file finder for Windows. Check the [releases](https://github.com/stchan/CloneFinder/releases) page to download a prebuilt MSI package.

**Requirements**<br/>
Windows OS with .NET 8.0 support.

**License**<br/>
CloneFinder is MIT X11 licensed.

**Command line Usage:**

		CloneFinder [-c] [-p] path

	Options:
		-c, --csv        Display results as comma separated values (CSV).
		-p, --progress   Show progress indicator.

	Example:

		CloneFinder -c c:\temp

	Searches for duplicates in c:\temp, and outputs results in CSV format.

**GUI Notes:**<br/>
The GUI app will retain window sizing on subsequent launches.
Select "Reset Default Sizes" from the "Tools" menu to reset sizes back to defaults.

**Third party components**<br/>
- Code from MSDN for a breadth-first directory traversal.
- Free or CC licensed artwork. (Icons (C) 2008 GoSquared Ltd.)
- Installer graphics from Open Clip Art (public domain):<br/>
&nbsp; - Amateur Astronomer<br/>
&nbsp; - Spyglass<br/>


**ChangeLog**<br/>
2.2.0 - Targets .NET 8.0. Upgrade WiX. No new functionality.<br/>
2.1.1 - Disabled high DPI scaling (issue #2).</br>
2.1.0 - Package updates. No new functionality.<br/>
2.0.1 - Maintenance release. No new functionality.<br/>
2.0.0 - Targets .NET 6.0. Single file deployment. No new functionality.<br/>
1.2.3 - Targets .NET 4.8. Replaced MD5 with SHA512.<br/>
1.2.2 - Fix for issue #1<br/>
1.2.1 - Upgrade code changed (GUID was accidentally duplicated from another project)<br/>
1.2.0 - Installer is now a WiX project. SQLite upgraded to v1.0.108<br/>
1.1.0 - Added command line interface<br/>
1.0.2 - Fixes for installer, and icon changes<br/>
1.0.1 - First usable public release<br/>
1.0.0 - Initial commit to github<br/>
