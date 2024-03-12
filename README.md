# GPUPrefSwitcher
Enables seamless switching of game settings and targeting the dGPU/iGPU for gaming laptops when they plug in or out, making portable gaming more convenient. 

## [Download/install (directly downloads the latest release)](https://github.com/sharpjd/GPUPrefSwitcher/releases/download/v0.1.0/GPUPrefSwitcher_Installer_v0.1.0.zip)

## Features
* When switching between plugged in / on battery, this app can:
  * Automatically change the GPU preference (Power Saving/High Performance — i.e. iGPU/dGPU) on a per-app basis
  * Automatically save and swap specified settings files, enabling 2 sets of settings
  * Automatically execute Task Scheduler triggers or scripts for extended functionality
* A GUI to configure all features
* Activity and error logging + notifying the user of errors
* Runs as a service

## Installation

#### Install (Wizard):
- Download the [latest installer Release](https://github.com/sharpjd/GPUPrefSwitcher/releases).
- Extract all .zip contents and run `setup.exe` (not the .msi file)

#### Install (Manual): 
- Download and extract to a location the [latest self-contained release](https://github.com/sharpjd/GPUPrefSwitcher/releases/download/v0.1.0/GPUPrefSwitcher_SelfContained_v0.1.0.zip)
- See step 3 of the Manual Installation section (expand the Build/Develop section below)

#### Prerequisites:
- A 64-bit system
- Windows 11 or newer versions of Windows 10
- .NET **Desktop** Runtime 8.0.1 
- .NET Framework 4.7.1
- (The installer should automatically download the latter two if they're not yet installed)

## Donation
If you would like to show support for my work, you can do so at my [Ko-Fi](https://ko-fi.com/sharpjd)!

## Why this app?

TL;DR: You want to game on your gaming laptop while on battery, but not have it kill your battery and have to change settings every time.

Integrated graphics (iGPU) and CPU power efficiency have evolved and are becoming viable for gaming on battery — without the use of a dedicated graphics card (dGPU) or a charger. But despite the hardware being capable, there is currently too much software friction to justify doing so on Windows gaming laptops (or any hybrid graphics system):

1. Many gaming laptops don't allow you to disable the dGPU manually or while on battery, so games automatically run on them and decimate your battery (100% -> 0% in less than an hour in some cases!). To prevent this, you must first open the Graphics Settings and change the GPU preference to the iGPU, and then run your game. Then, when you plug your computer back in and want the game running on the dGPU, you need to remember to go back and change the GPU preference setting *again*. 

2. Then, you face having to turn down your game's graphics settings so that they don't overload your iGPU. But when you're done, back on the charger and using your dGPU for prettier graphics again, you have to change the settings *another* time to restore those settings. This also means you need to memorize two distinct sets of settings!

3. Automatic frame limiter features like NVIDIA's BatteryBoost don't work as well due to their high base power draw in comparison to iGPU's, on top of capping your game to a dismal 30FPS by default.

These points of contention practically defeat half the purpose of a gaming laptop: *To be a **portable** gaming machine.* Ironically, non-gaming laptops *without* discrete GPUs may even give a better experience in this regard! Because of these reasons, it's completely understandable that gaming laptops still harbor the reputation of being impractical. 

This app aims to solve these problems by letting you configure apps to automatically target dGPU/iGPU, and also seamlessly switch between two sets of graphics settings for when you're plugged in and plugged out.

**Your laptop's iGPU may be faster than you realize.** If your laptop has Intel Iris Xe Graphics, an Intel Core Ultra Processor, or a Ryzen processor (all of which have/are strong iGPUs); and you have a game you play very often or an indie gamer, then you are a perfect candidate for this app! And even if you don't have a fast iGPU, it's unlikely you'll care much about graphics while gaming on the go anyway — so go ahead and turn those settings down.

<details>
<summary>
 
## How does it work? (click to expand)
</summary>

#### GPU Preference Switcher:
Windows stores per-app GPU preferences as values in the Registry. All existing values will get added to the XML file. You will see these in the GUI, and you can enable automatic GPU preference switching for each of them. When the computer's power state changes, the app changes the necessary part of each value's data that controls which GPU the target app will run on. 

#### File Swapper:
Let's explain this one with an example. Assume our computer is currently plugged in. 
Let's say that we have a GPU Preference entry for `ShooterGame.exe` and that the game stores its settings in `C:\users\Bob\Documents\ShooterGame\settings.config`. Let's say we add a *file swap path* pointing to that config file.

The app will then copy and store this file internally. One copy will be stored for the Online (plugged in) state, and one for the Offline (on battery) state. Since this is our first time saving it, the same copy will be made for both. 

Now, let's close the game and plug our computer out. The app will now swap in the Offline version of the config file. We boot up the game and it runs on the iGPU, but we notice that the FPS is low — so we lower the resolution and turn down some settings. Now it's much better.

But after some time, we're done playing the game and go home and hit the charger. The app will then save the current config as the Offline state, and swap in the file for the Online state.

Now we boot up the game again, and the game is running on the dGPU. Normally, we'd have to restore all our settings to make the game look good again — but because the app restored the original config, we don't have to do any of this. We can start playing the game again with our original beautiful graphics.

The process was totally automatic, requiring no intervention. 

Despite this, you may still encounter scenarios where this is not a seamless experience. The app has been designed with considerations and safeguards against some of these scenarios:
* **Target file locked:** The app will simply stall the file swap and retry repeatedly.
* **The target .exe saves the file *when it closes:*** The app is designed to delay swapping the file if the target .exe is still running. Data loss or an inappropriate overwrite can still occur if the target .exe spawns a separate process that modifies the file, but that seems like a very specific and unlikely scenario.
* **Unexpected crashes or ungraceful termination:** Is unlikely to cause inconsistent states or inappropriate overwrites. The state tracking string in the app data file for each corresponding file swapper state is immediately updated and saved after the each file swap occurs, and is not touched if the file swap doesn't occur.

Despite these safeguards, data loss can still result from the File Swapper system; **do NOT manipulate important or sensitive data with it.** 

</details>

## Future feature ideas
- Improving the definition and conditions for being "On Battery" or "Plugged In" (e.g. how much power input?)
- Multiple settings profiles
- A config database

<details>
 
<summary>
 
## Build/develop (click to expand):
</summary>

- This app should only work if built for x64.
- The Visual Studio Installer Projects extension is needed to build the installer.
- **You may only need to follow these steps in case changes are made and/or the Setup project / installer build fails.**

### Step 1: Build the .EXEs and Assemblies:
 - For the `x64` configuration, the relevant output files are found in the following locations:
 	- Primary GPUPrefSwitcher components: `/Assemble/x64/<Debug or Release>/net8.0-windows`. 
 	- Intall.exe and Uninstall.exe (NOT the Setup or .msi file): `/Assemble/install`
 	- The Setup.exe and .msi: `/Setup/<Debug or Release>`.
   - **This means that if you're simultaneously developing and running the project, Manual Installation (read even further below) is the more convenient method.**
     
### Step 2: The installer/Setup project:
 **You probably only need these steps if you have changed what goes into the AppData folder, or how the app interacts with its directories.** (Note: Some very specific settings or configurations may be necessary for this project to build successfully, so it's reccomended you modify only what you need. If at any point things get messed up, you can always delete, redownload, and add the Setup project again).
 * See the "Development Pitfalls" section for tips about the Setup project (which seems to have spotty online documentation).
  1. Open the Directory view by right clicking the `Setup` project -> View -> File System
![image](https://github.com/sharpjd/GPUPrefSwitcher/assets/59419827/b2432462-655d-47b2-b367-33be844c921a)
  2. This step is only applicable if you've changed what's in the `/Assemble/AppData` folder, in which you might need to update/register the changes by deleting the `AppData` folder inside the Setup project, and dragging it back into `Application Folder/install` (remember that you must clear folder contents before deleting the folder itself). The following screenshots are for reference:
![image](https://github.com/sharpjd/GPUPrefSwitcher/assets/59419827/5bb586f8-1b26-4828-b765-d9a7646c8b1e) ![image](https://github.com/sharpjd/GPUPrefSwitcher/assets/59419827/1c54f931-19f1-45bb-a187-1dac3526ef31)
  3. To build the Setup project / .msi file, right click the setup project and click `Build`. You will find the output in the default directory (`/Setup/<Debug or Release>`).

<a name="manual-installation-and-assembly"></a>
### Manual installation and assembly + extra notes:
1. It is required that all EXEs and their related files* are placed in a fixed and specified directory, because the app looks for them in specific locations. This should already be done by default. If you un-merge the build paths, you'll need to manually merge the built files and folders.
	
2. In the end, you should end up with exactly this folder structure (before running `Install.exe`):
```
.
├── <Application Folder>/
|   ├── install/
|   |   ├── Install.exe + related files
|   |   ├── Uninstall.exe + related files
|   |   └── AppData/
|   |       ├── Settings files, user data, etc.
|   |       └── defaults/
|   |           └── relevant default settings files
|   ├── GPUPrefSwitcher.exe + related files
|   ├── GPUPrefSwitcherGUI.exe + related files
|   ├── GPUPrefSwitcherRepairer.exe + related files
|   ├── GPUPrefSwitcherSvcRestarter.exe + related files
|   └── GUIAdminFunctions.exe + related files
```

3. Double click and run `Install.exe` inside the `install` folder to install the service (and `Uninstall.exe` to uninstall).

*The project was built in Visual Studio 2022. It was originally a .NET Framework 4.7.x project, but was migrated to .NET Core 8.x .

### Other potential development tips or pitfalls:
- **"Could not find metadata" or "could not find dll" errors:** fix compiler errors first, check the build order, and see this StackOverflow thread  https://stackoverflow.com/questions/44251030/vs-2017-metadata-file-dll-could-not-be-found
- **Cannot find .exe or Error MSB4094:** Sometimes if you change build paths, it freaks out because projects will somehow generate a double reference and also attempt to search for .exe's in nonexistent locations; check and fix build-related files (in my original case, the error pertained to an element/array containing two reference entries instead of just one).
- **Unable to change the target architecture of builds:** try opening the .sln file and clearing everything between "GlobalSection(ProjectConfigurationPlatforms) = postSolution" and "EndGlobalSection"
- If you change the service name, you must change the service name constant in certain places (e.g. GPUPrefSwitcherSvcRestarter -> Program.cs)
- **Setup Project:**
	- **Shortcuts to deployed executables:** https://stackoverflow.com/questions/3303962/visual-studio-deployment-project-create-shortcut-to-deployed-executable
	- **"Install" or "Uninstall" executables don't run:** Ensure that in the Setup project's Custom actions for Install and Uninstall, `InstallerClass` is set to `False` and `Run64Bit` is set to `True` (or accordingly otherwise)

</details>

## Attributions/disclaimers
* [CreateProcessAsUser](https://github.com/murrayju/CreateProcessAsUser)
* Despite many safeguards and considerations, data loss may result from using the app (e.g. the File Swapper). **Do not manipulate important data with this app.** The author is not responsible for such scenarios.

