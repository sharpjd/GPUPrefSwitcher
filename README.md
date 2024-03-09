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
