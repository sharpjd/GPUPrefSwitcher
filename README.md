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
- Option to preserve user data when uninstalling and updating with the installer
- Stalling file swapping if the target .exe is still running (in case the target app saves settings *when it closes*)

## Future feature ideas
- Improving the definition and conditions for being "On Battery" or "Plugged In" (e.g. how much power input?)
- Multiple settings profiles
- Ability to force a File Swapper state manually for individual apps
- A config database or a forum

## Donation
If you would like to show support for my work, you can do so at my [Ko-Fi](https://ko-fi.com/sharpjd)!

## Why this app?
Integrated graphics (iGPU) and CPU power efficiency have evolved and are becoming viable for gaming on battery — without the use of a dedicated graphics card (dGPU) or a charger. But despite the hardware being capable, there is unfortunately too much software friction to justify doing so on Windows gaming laptops (or any hybrid graphics system):

1. Many gaming laptops don't allow you to disable the dGPU manually or while on battery, so games automatically run on them and decimate your battery (100% -> 0% in less than an hour in some cases!). To prevent this, you must open the Graphics Settings and change the GPU preference — then when you plug your computer back in and want the game running on the dGPU, you need to remember to change the settings again. 

2. You then face having to turn down your game's graphics settings so that they don't overload your iGPU. But then when you're back on the charger and using your dGPU for prettier graphics again, you have to change the settings *another* time to restore them. This is non top of having to memorize two sets of settings!

3. Automatic frame limiter features like NVIDIA's BatteryBoost are also mostly ineffectual solutions. Due to dGPU boosting behavior, they can remain at unnecessarily high voltages and don't end up saving a lot of power, on top of capping your game to a dismal 30FPS by default.

These points of contention practically defeat half the purpose of a gaming laptop: *To be a **portable** gaming machine.* Ironically, non-gaming laptops *without* discrete GPUs may even give a better experience in this regard! Because of these reasons, it's completely understandable that gaming laptops still harbor the reputation of being impractical. 

This app aims to solve these problems by letting you configure apps to automatically target dGPU/iGPU, and also seamlessly switch between two sets of graphics settings for when you're plugged in and plugged out.

## How does it work?
