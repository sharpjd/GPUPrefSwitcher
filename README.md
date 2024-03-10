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

#### Install (Manual): [read more here]()

## Planned features (high priority)
- [] Option to preserve user data when uninstalling and updating with the installer
- [] Stalling file swapping if the target .exe is still running (in case the target app saves settings *when it closes*)
- [] Finish developer/detailed documentation

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

Despite this, you may still encounter scenarios where this is not a seamless experience. The app has been designed with considerations and safeguards against some of these scenarios (e.g. file locked, or the .exe saving the config when it shuts down). You can read about them more in depth <here>.

Many considerations have been put in place, but data loss can still result from the File Swapper system; do NOT manipulate important or sensitive data with it.

See <here> for more more in-depth explanations (that are intended more for programmers or developers).

</details>



