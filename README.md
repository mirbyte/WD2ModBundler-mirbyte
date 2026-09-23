-------------------------------
WD2ModBundler v1.0.1 28.02.2026
-------------------------------

## About this fork

I made this fork of [WD2ModBundler](https://github.com/qotapac/WD2ModBundler) to add a few small changes that make mod bundling easier for end users while preserving the original project's purpose.

This fork targets .NET 10 for Windows. Framework-dependent builds require the .NET 10 Desktop Runtime.

-----
INFO:
-----
Quickly combine multiple single mods into a custom Watch Dogs 2 Patch3.dat/Patch3.fat.  
Automates unpacking and merging of multiple mods into a single mod pack. 

------------
PLEASE NOTE:
------------
There is no need to extract your mod archives!  
After downloading mods - simply put all mods archives into a single folder, then select this folder in step 5. as described bellow.
Current version overwrites any conflicting mod files by default. 

----------
IMPORTANT:
----------
Works only with Watch Dogs 2 Patch3.dat and Patch3.fat files. 

Current version overwrites any conflicting mod files when it puts them into Patch folder.

Needs 7-Zip installed﻿. https://www.7-zip.org/download.html

Requires official WD2 Patch Tools﻿ by Me1ton﻿. https://www.nexusmods.com/watchdogs2/mods/28

Removes the Temp folder when a run finishes. The Patch folder is left behind and can be deleted after usage. 

-----------
How to Use:
-----------
1. Install 7-Zip﻿ if it is not already installed. https://www.7-zip.org/download.html

2. Download and extract the official WD2 Patch Tools﻿ by Me1ton﻿. https://www.nexusmods.com/watchdogs2/mods/28

3. Run WD2ModBundler.exe. It will be either in the root of the WD2ModBundler folder or inside win-x64. (depending on what version you download)

4. On startup the app searches for 7z.exe in the usual install folders and on PATH. If the path is shown in green, skip this step. Otherwise press "Select 7-Zip.exe" and choose 7z.exe.

5. Press button "Select folder with mods archives" and select the folder containing your mod archives.

6. Press button "Select folder with WD2 Patch Tools" and select the folder containing WD2Extract.exe and WD2Pack.exe (from the Combine Mods﻿ mod aka WD2 Patch Tools). 

7. Press button "Combine Mods" — this will:
        a. Extract all mod archives.
        b. Unpack each Mod`s Patch3.dat using WD2Extract.exe.
        c. Merge everything into a single package of Patch3.fat and Patch3.dat.
        d. Save the final combined package in the "MyModsBundle" folder in the WD2ModBundler running directory. 
8. Check the log output to follow progress and confirm completion. 

------------------------
Why Use WD2ModBundler?
------------------------
    Streamlines the creation of custom mod packs.
    Eliminates manual extraction and patch merging.
    Ideal for both new and experienced modders.
