# GPUPrefSwitcher
Enables seamless switching of game settings and targeting the dGPU/iGPU for gaming laptops when they plug in or out, making portable gaming more convenient. 

## [Download/install (directly downloads the latest release)](https://github.com/sharpjd/GPUPrefSwitcher/releases/download/v0.0.0-alpha/GPUPrefSwitcher.v0.0.0-alpha.zip)

## Features
* When switching between plugged in / on battery, this app can:
  * Automatically change the GPU preference (Power Saving/High Performance — i.e. iGPU/dGPU) on a per-app basis
  * Automatically save and swap specified settings files, enabling 2 sets of settings
  * Automatically execute Task Scheduler triggers or scripts for extended functionality
* A GUI to configure all features
* Activity and error logging + notifying the user of errors
* Runs as a service

## Installation
#### Prerequisites:
- A 64-bit system
- Windows 11 or newer versions of Windows 10

#### Install (Wizard):
- Download the latest [Release](https://github.com/sharpjd/GPUPrefSwitcher/releases).
- Extract all .zip contents and run `setup.exe` (not the .msi file)

#### Install (Manual): [see step 4 of this section](https://github.com/sharpjd/GPUPrefSwitcher/edit/main/README.md#manual-installation-and-assembly--extra-notes)

## Planned features (high priority)
- [ ] Option to preserve user data when uninstalling and updating with the installer
- [ ] Stalling file swapping if the target .exe is still running (in case the target app saves settings *when it closes*)
- [ ] Finish developer/detailed documentation

## Future feature ideas
- Improving the definition and conditions for being "On Battery" or "Plugged In" (e.g. how much power input?)
- Multiple settings profiles
- Ability to force a File Swapper state manually for individual apps
- A config database or a forum

## Donation
If you would like to show support for my work, you can do so at my [Ko-Fi](https://ko-fi.com/sharpjd)!

## Why this app?
Integrated graphics (iGPU) and CPU power efficiency have evolved and are becoming viable for gaming on battery — without the use of a dedicated graphics card (dGPU) or a charger. But despite the hardware being capable, there is unfortunately too much software friction to justify doing so on Windows gaming laptops (or any hybrid graphics system):

1. Many gaming laptops don't allow you to disable the dGPU manually or while on battery, so games automatically run on them and decimate your battery (100% -> 0% in less than an hour in some cases!). To prevent this, you must open the Graphics Settings and change the GPU preference to the iGPU — then when you plug your computer back in and want the game running on the dGPU, you need to remember to change the settings again. 

2. You then face having to turn down your game's graphics settings so that they don't overload your iGPU. But then when you're back on the charger and using your dGPU for prettier graphics again, you have to change the settings *another* time to restore them. This is on top of having to memorize two sets of settings!

3. Automatic frame limiter features like NVIDIA's BatteryBoost are also mostly ineffectual solutions. Due to dGPU boosting behavior, they can remain at unnecessarily high voltages and don't end up saving a lot of power, on top of capping your game to a dismal 30FPS by default.

These points of contention practically defeat half the purpose of a gaming laptop: *To be a **portable** gaming machine.* Ironically, non-gaming laptops *without* discrete GPUs may even give a better experience in this regard! Because of these reasons, it's completely understandable that gaming laptops still harbor the reputation of being impractical. 

This app aims to solve these problems by letting you configure apps to automatically target dGPU/iGPU, and also seamlessly switch between two sets of graphics settings for when you're plugged in and plugged out.

<details>
<summary>
 
## How does it work? (click to expand)
</summary>

#### GPU Preference Switcher:
Windows stores per-app GPU preferences as values in the Registry. All existing values will get added to the XML file. You will see these in the GUI, and you can enable automatic GPU preference switching for each of them. When the computer's power state changes, the app changes the necessary part of each value's data that controls which GPU the target app will run on. 

#### File Swapper:
Let's explain this one with an example. Assume our computer is currently plugged in. 
Let's say that we have a GPU Preference entry for `ShooterGame.exe` and it stores its settings in `C:\users\Bob\Documents\ShooterGame\settings.config`. Let's say we add a *file swap path* pointing to that config file.

The app will then copy and store this file internally. One copy will be stored for the Online (plugged in) state, and one for the Offline (on battery) state. Since this is our first time saving it, the same copy will be made for both. 

Now, let's close the game and plug our computer out. The app will now swap in the Offline version of the config file. We boot up the game and it runs on the iGPU, but we notice that the FPS is low — so we lower the resolution and turn down some settings. Now it's much better.

But after some time, we're done playing the game and go home and hit the charger. The app will then save the current config as the Offline state, and swap in the file for the Online state.

Now we boot up the game again, and the game is running on the dGPU. Normally, we'd have to restore all our settings to make the game look good again — but because the app restored the original config, we don't have to do any of this. We can start playing the game again with our original beautiful graphics.

The process was totally automatic, and required no intervention. 

Despite this, you may still encounter scenarios where this is not a seamless experience. The app has been designed with considerations and safeguards against some of these scenarios (e.g. file locked, unexpected crash, or the .exe saving the config when it shuts down). The way these mechanisms work take a lot to explain, and for now, you will need to study the source code yourself to understand them.

Many considerations have been put in place, but data loss can still result from the File Swapper system; do NOT manipulate important or sensitive data with it.

</details>

<details>
<summary>
 
## Build/develop (click to expand):
</summary>

- This app should only work if built for x64.
- The Visual Studio Installer Projects extension is needed to build the installer.
- **You may only need to follow these steps in case changes are made and/or the Setup project / installer build fails.**

### Step 1: Build the .EXEs and Assemblies:
 - For the `x64` configuration, the relevant output files are found in the following locations:
 	- Primary GPUPrefSwitcher components: `/Assemble/x64/<Debug or Release>/net8.0-windows`. A shortcut named `WORKING DIR` takes you directly there. 
 	- Intall.exe and Uninstall.exe (NOT the Setup or .msi file): `/Assemble/install`
 	- Setup.exe and Setup.msi: **These may have to be reconfigured and built manually when certain parts of the project changes**. See the next step (Step 2).
   - **This means that if you're simultaneously developing and running the project, Manual Installation (read even further below) is the more convenient method.**
     
### Step 2: Assemble the files for the installer/Setup project:
 Now it's time to update the Setup (.msi installer) project.
  1. Open the Directory view by right clicking the `Setup` project -> View -> File System
  2. Clear what's currently in the `Application Folder/install` folder. Some items must be selected and deleted before others.
  3. You might only ever have to do this step once, but you may have to undo and repeat this every time you change the `Install` or `Uninstall` projects. First, build the Installer and Uninstaller (you can just build the whole project). By default, they output to `/Assemble/install/`. Drag these contents into the `Application Folder`. It should look like this:
  4. Then, right click on the `Setup` project -> View -> Custom Actions. Right click `Install` and add `Install.exe` under the Application folder. You should end up with this:
  5. Now, right click `Uninstall` and add `Uninstall.exe`. Then, for each of them, select them and look at the `Properties` pane, and set `InstallerClass` to `False` and `Run64Bit` to `True`.
  6. This step is only applicable if you've changed what's in the `/Assemble/AppData` folder, in which you might need to update/register the changes by deleting the `AppData` folder inside the Setup project, and dragging it back into the Application Folder.
  7. Check that you end up with this. Mismatches may result in files not being found by the program:
  8. Select and build the Setup/Install (it does not automatically build with the whole project). You will find the output in the default directory (`/Setup/<Debug or Release>`).

### Manual installation and assembly + extra notes:
1. It is required that all EXEs and their related files* are placed in the same directory, because the app will look for them in `AppDomain.CurrentDomain.BaseDirectory`. This has already been done by default, and a shortcut named `WORKING DIR (Debug)` or `WORKING DIR (Release)` should take you directly there. If you un-merge the build paths, you'll need to manually merge the built files and folders.
 * It might not be necessary to drag in the Install/Uninstall .exe's or `install folder`. Nontheless — if you want them anyways (e.g. to uninstall/reinstall the service with a double click), you need to drag their parent `install` folder into the app directory. Both of them search for `..\GPUprefSwitcher.exe`. Don't forget to overwrite this folder every time you change `Install.exe` or `Uninstall.exe`
	
2. (Already done by default) Application data, user data, and settings currently go in the same folder as the program. Directly copy the `AppData` folder found under `/Assemble` into the app directory. 

3. In the end, you should end up with exactly this folder structure:
```
.
├── <Application Folder>/
|   ├── AppData/
|   |   ├── Settings files, user data, etc.
|   |   └── defaults/
|   |       └── relevant default settings files
|   ├── install/
|   |   ├── Install.exe + related files
|   |   └── Uninstall.exe + related files
|   |
|   ├── GPUPrefSwitcher.exe + related files
|   ├── GPUPrefSwitcherGUI.exe + related files
|   ├── GPUPrefSwitcherRepairer.exe + related files
|   ├── GPUPrefSwitcherSvcRestarter.exe + related files
|   └── GUIAdminFunctions.exe + related files
```

4. Double click and run `Install.exe` inside the `install` folder to install the service (and `Uninstall.exe` to uninstall). 
* (Or, just use `sc` to register the project; e.g. `sc.exe create GPUPrefSwitcher binpath= "C:/path/to/App Directory/GPUPrefSwitcher.exe" start= auto` (Yes, the spaces after binpath= and start= above *are* important — don't ask me why))

*The project was built in Visual Studio 2022. It was originally a .NET Framework 4.7.x project, but was migrated to .NET Core 8.x .

### Other potential development tips or pitfalls:
- **"Could not find metadata" or "could not find dll" errors:** fix compiler errors first, check the build order, and see this StackOverflow thread  https://stackoverflow.com/questions/44251030/vs-2017-metadata-file-dll-could-not-be-found
- **Cannot find .exe or Error MSB4094:** Sometimes if you change build paths, it freaks out because projects will somehow generate a double reference and also attempt to search for .exe's in nonexistent locations; check in build-related files for this
- **Unable to change the target architecture of builds:** try opening the .sln file and clearing everything between "GlobalSection(ProjectConfigurationPlatforms) = postSolution" and "EndGlobalSection"
- If you change the service name, you must change the service name constant in GPUPrefSwitcherSvcRestarter -> Program.cs, otherwise it will do nothing
- **Shortcuts to deployed executables:** https://stackoverflow.com/questions/3303962/visual-studio-deployment-project-create-shortcut-to-deployed-executable

</details>
---

## Attributions
* [CreateProcessAsUser](https://github.com/murrayju/CreateProcessAsUser)

