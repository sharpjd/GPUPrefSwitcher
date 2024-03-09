﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace GPUPrefSwitcher
{
    /// <summary>
    /// The main class that links all functionality together.
    /// </summary>
    public static class Switcher
    {
        /*
         * Registry always takes priority over the XML; if there's a mismatch, alert user
         * If there are registry entries not in the XML, add to XML
         */
        private static PreferencesXML preferencesXML;

        static AppOptions appOptions;
        private static SwitcherData switcherData;

        public static readonly string CRASHED_FILE_PATH = Path.Combine(Program.SavedDataPath + "CRASHED");

        static bool spoofPowerStateEnabled;
        static string spoofPowerState = "NO SPOOFING";
        static PowerLineStatus prevPowerLineStatus;
        static bool runOnce = false;
        static bool lastShutdownWasClean = false;

        static Func<PowerLineStatus, string> PowerLineStatusToOfflineOrOnline = PowerLineStatusConversions.PowerLineStatusToOfflineOrOnline;

        #region Initialization/End
        /// <summary>
        /// Call once upon service start. Initializes necessary components and starts the timer.
        /// </summary>
        internal static void Start()
        {
            switcherData = SwitcherData.Initialize();
            Logger.inst.Log($"Initialized {nameof(SwitcherData)}.");

            preferencesXML = new PreferencesXML(); //exceptions get thrown from this too if there are problems with the XML (e.g. syntax)
            Logger.inst.Log($"Initialized {nameof(PreferencesXML)}.");

            //SystemEvents.PowerModeChanged += HandlePowerChangeEvent; **NOT the right event, this is for resume/suspend
            appOptions = new AppOptions();
            Logger.inst.Log($"Initialized {nameof(AppOptions)}.");

            prevPowerLineStatus = switcherData.CurrentSwitcherData.PrevPowerStatus_enum;
            spoofPowerStateEnabled = appOptions.CurrentOptions.SpoofPowerStateEnabled;
            spoofPowerState = PowerLineStatusToOfflineOrOnline(appOptions.CurrentOptions.SpoofPowerState);

            lastShutdownWasClean = switcherData.CurrentSwitcherData.LastShutDownWasClean;
            //make sure static things initialize ^^^^^^^^ before running update 

            switcherData.CurrentSwitcherData.LastShutDownWasClean = false; //will be set to true when HandleStop() completes successfully
            switcherData.SaveToXML();

            updateInterval = appOptions.CurrentOptions.UpdateInterval;
            Logger.inst.EnableRealtimeStandardLogWrites = appOptions.CurrentOptions.EnableRealtimeLogging;

            File.Delete(CRASHED_FILE_PATH);//"successful" initialization yippee

            _ = BeginTimerForever();
            Logger.inst.Log($"Begin update logic timer.");
        }
        private static int updateInterval; 


        /// <summary>
        /// Call once upon service stop.
        /// </summary>
        internal static void Stop()
        {
            
            if (spoofPowerStateEnabled)
            {
                switcherData.CurrentSwitcherData.PrevPowerStatus_string = spoofPowerState.ToLower();
            } else
            {
                switcherData.CurrentSwitcherData.PrevPowerStatus_string = SystemInformation.PowerStatus.PowerLineStatus.ToString().ToLower();
            }

            switcherData.CurrentSwitcherData.DontShowErrorMessage = false;

            switcherData.CurrentSwitcherData.LastShutDownWasClean = true;

            switcherData.SaveToXML();
        }
        #endregion Initialization/End

        private static bool timerRunning = false;
        private static Task runningUpdateTask;
        /// <summary>
        /// There should be only one of this running at any time.
        /// </summary>
        private static async Task BeginTimerForever()
        {
            if (timerRunning) { throw new InvalidOperationException("This function cannot be called more than once"); }
            timerRunning = true;
            while (true)
            {
                await Task.Delay(updateInterval);

                /*
                 * Only ever run one of this task at once
                 */
                if(runningUpdateTask == Task.CompletedTask)
                {
                    runningUpdateTask = null;
                }

                if (runningUpdateTask == null)
                {
                    Task run = RunUpdateLogic();
                    runningUpdateTask = run;
                }
            }
        }

        private static Task RunUpdateLogic()
        {
            Logger.inst.Log("Begin update logic.", 2000);

            int retries = 0;
        WaitForUserLogin:
            try
            {
                _ = RegistryHelper.GetLoggedOnUserSID();
            }
            catch (InvalidOperationException)
            {
                retries++;
                Logger.inst.Log($"Could not get the currently logged on user (we may still be on the lock screen), delaying update logic ({retries} retries)");
                Thread.Sleep(updateInterval);
                goto WaitForUserLogin;
            }


            PowerLineStatus currentPowerLineStatus;
            if (spoofPowerStateEnabled)
            {
                Logger.inst.Log($"Power State spoofing enabled: {spoofPowerState}");

                string spoof = spoofPowerState.ToString().ToLower();
                if (spoof == "offline")
                {
                    currentPowerLineStatus = PowerLineStatus.Offline;
                }
                else if (spoof == "online")
                {
                    currentPowerLineStatus = PowerLineStatus.Online;
                }
                else
                {
                    throw new ArgumentException("Unknown SpoofPowerState");
                }
            }
            else
            {
                currentPowerLineStatus = SystemInformation.PowerStatus.PowerLineStatus;
            }

            if (!runOnce)
            {
                Logger.inst.Log("Updating preferences regardless since this is the first update.");
                goto CheckAndUpdate;
            }
            if (!lastShutdownWasClean)
            {
                //doesn't actually serve a purpose for now
                Logger.inst.Log("Detected that service stop was not completed on the last run.");
            }

            //skip update if powerline status hasn't changed
            if (prevPowerLineStatus == currentPowerLineStatus)
            {
                Logger.inst.Log($"PowerLine status has NOT changed since last update. (Current state: {currentPowerLineStatus}; last state: {prevPowerLineStatus})", 2000);
                return Task.CompletedTask;
            }
            else
            {
                Logger.inst.Log($"PowerLine status has changed since last update. Previous: {prevPowerLineStatus}; Current: {currentPowerLineStatus}");
                goto CheckAndUpdate;
            }

        CheckAndUpdate:
            runOnce = true;

            RegAndXMLMatchState regAndXMLMatchState = GetRegAndXmlMatchState();
            Logger.inst.Log($"Registry and XML match state: {regAndXMLMatchState}");

            //update XML file if there's an entry in the registry missing in the XML
            if (regAndXMLMatchState == RegAndXMLMatchState.RegMissingInXML ||
                regAndXMLMatchState == RegAndXMLMatchState.MissingInBoth)
            {
                WriteRegistryToXML();
            }

            WriteXMLToRegistry(currentPowerLineStatus);

            UpdateSeenInRegistryStatuses(); //sets the SeenInRegistry entry accordingly

            BeginFileSwapLogic(currentPowerLineStatus, preferencesXML.GetAppEntries());

            BeginTaskSchedulerLogic(currentPowerLineStatus);

            prevPowerLineStatus = currentPowerLineStatus; //THIS MUST GO AT THE END

            return Task.CompletedTask;
        }

        private static void BeginTaskSchedulerLogic(PowerLineStatus forPowerLineStatus)
        {
            if (forPowerLineStatus == PowerLineStatus.Online)
            {
                TaskSchedulerUtils.RunPluggedInTask();
            } else if (forPowerLineStatus == PowerLineStatus.Offline)
            {
                TaskSchedulerUtils.RunOnBatteryTask();
            } else
            {
                throw new NotImplementedException();
            }
        }

        private static void UpdateSeenInRegistryStatuses()
        {
            List<string> regPathValues = RegistryHelper.GetGpuPrefPathvalueNames().ToList();
            List<AppEntry> appEntries = preferencesXML.GetAppEntries();

            foreach(AppEntry appEntry in appEntries)
            {
                if (regPathValues.Contains(appEntry.AppPath)){
                    if(appEntry.SeenInRegistry==false)
                        preferencesXML.ModifyAppEntryAndSave(appEntry.AppPath, appEntry with { SeenInRegistry = true });
                } else
                {
                    preferencesXML.ModifyAppEntryAndSave(appEntry.AppPath, appEntry with { SeenInRegistry = false });
                }
            }
        }

        private static Task BeginFileSwapLogic(PowerLineStatus forPowerLineStatus, IEnumerable<AppEntry> forAppEntries)
        {
            Logger.inst.Log("Commence file swap logic.");
            //shorten these long names

            //create the SettingsBank directory if it doesn't exist
            bool fileSwapperFolderExists = Directory.Exists(FileSwapper.SwapPathFolder);
            if (!fileSwapperFolderExists) { Directory.CreateDirectory(FileSwapper.SwapPathFolder); }

            foreach (AppEntry appEntry in forAppEntries)
            {
                var fileSwap = new FileSwapper(appEntry, preferencesXML);

                if (!appEntry.EnableFileSwapper) continue;
                if (appEntry.FileSwapperPaths.Length == 0) continue;

                fileSwap.InitiateFileSwaps(forPowerLineStatus, preferencesXML);

            } //...repeat for every AppEntry

            Logger.inst.Log("File swap logic finished.");

            return Task.CompletedTask;
        }

        static string[] ignoreList = RegistryHelper.ignoreValues;
        private static RegAndXMLMatchState GetRegAndXmlMatchState()
        {
            
            //TODO: get apps from UWP apps: https://stackoverflow.com/questions/50217328/get-icon-from-uwp-app

            IEnumerable<string> xmlPaths = preferencesXML.GetAppPaths();
            IEnumerable<string> registryPathValues = RegistryHelper.GetGpuPrefPathvalueNames();

            //DEBUG
            /*
            Debug.WriteLine("--XMLPATHS--");
            foreach (string s in xmlPaths)
                Debug.WriteLine(s);
            Debug.WriteLine("--REGISTRYVALUES--");
            foreach (string s in registryPathValues)
                Debug.WriteLine(s);
            */

            bool missingInXML = false;
            bool missingInRegistry = false;

            foreach (string xmlPath in xmlPaths)
            {
                if (ignoreList.Contains(xmlPath)) { continue; }
                if (!registryPathValues.Contains(xmlPath))
                {
                    Debug.WriteLine("Missing in registry: " + xmlPath);
                    missingInRegistry = true;
                    break;
                }
                    
            }


            foreach (string registryPathValue in registryPathValues)
            {
                if(ignoreList.Contains(registryPathValue)) { continue; }
                if (!xmlPaths.Contains(registryPathValue))
                {
                    Debug.WriteLine("Missing in XML: " + registryPathValue);
                    missingInXML = true;
                    break;
                }
                    
            }

            if(missingInRegistry && !missingInXML)
                return RegAndXMLMatchState.XMLMissingInReg;

            if(!missingInRegistry && missingInXML)
                return RegAndXMLMatchState.RegMissingInXML;

            if(missingInRegistry && missingInXML)
                return RegAndXMLMatchState.MissingInBoth;

            return RegAndXMLMatchState.Matched;
        }

        /// <summary>
        /// Registry and XML mismatch state. RegMissingInXML means the Registry contains entries the XML doesn't, and vice versa for XMLMissingInReg.
        /// MissingInBoth means *both* sides don't contain entries the other has. Matched means it matches 1:1. MissingInRegistry shouldn't be uncommon
        /// because apps get installed and uninstalled all the time. 
        /// </summary>
        public enum RegAndXMLMatchState
        {
            RegMissingInXML,
            XMLMissingInReg,
            MissingInBoth,
            Matched,
        }

        private const string defaultPluggedin = "0";
        private const string defaultOnBattery = "0";
        private const string defaultEnableSwitcher = "false";
        private const string defaultEnableFileSwapper = "false";
        internal static void WriteRegistryToXML()
        {

            Logger.inst.Log("FixRegXmlMismatch running: ");
            List<string> regPathValues = RegistryHelper.GetGpuPrefPathvalueNames().ToList();
            List<string> xmlPaths = preferencesXML.GetAppPaths().ToList();

            for (int i = 0; i < regPathValues.Count(); i++)
            {
                string regPathValue = regPathValues[i];
                if (!xmlPaths.Contains(regPathValue))
                {
                    Logger.inst.Log("Reg entry missing in XML: " + regPathValue);

                    int existingRegPref;
                    try
                    {
                        existingRegPref = RegistryHelper.GetGpuPrefValue(regPathValue);
                    }
                    catch (RegistryHelper.NoGPUPreferenceDataException)
                    {
                        existingRegPref = -1;
                    }

                    /*
                    string status;
                    if(SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Offline)
                    {
                        status = "offline";
                    } else if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online)
                    {
                        status = "online";
                    } else
                    {
                        status = "unknown";
                    }

                    //TODO but there's no SwapPaths to use the above on lol
                    */
                    preferencesXML.AddAppEntryAndSave(
                            new AppEntry()
                            {
                                AppPath = regPathValue,
                                EnableSwitcher = Boolean.Parse(defaultEnableSwitcher),
                                GPUPrefOnBattery = existingRegPref,
                                GPUPrefPluggedIn = existingRegPref,
                                EnableFileSwapper = Boolean.Parse(defaultEnableFileSwapper),
                                FileSwapperPaths = new string[0],
                                SwapperStates = new PowerLineStatus[0],
                                PendingAddToRegistry = false,
                                RunOnBatteryPath = string.Empty,
                                RunPluggedInPath = string.Empty,
                                SeenInRegistry = true //if we're adding from the registry, shouldn't this be true?
                            }
                        ); 
                }
            }

            Logger.inst.Log("Concluded Registry and XML mismatch fixing.");
        }

        internal static void WriteXMLToRegistry(PowerLineStatus powerLineStatus)
        {
            IEnumerable<AppEntry> appEntries = preferencesXML.GetAppEntries();

            foreach (AppEntry appEntry in appEntries)
            {

                if (appEntry.PendingAddToRegistry)
                {

                    Logger.inst.Log($"Pending registry add detected: {appEntry.AppPath}");

                    const int defaultPref = 0;
                    
                    try
                    {
                        RegistryHelper.AddGpuPref(appEntry.AppPath, defaultPref);
                    } catch (InvalidOperationException)
                    {
                        Logger.inst.Log($"Skipped pending registry add because the value already exists in the registry: {appEntry.AppPath}", 2000);
                    }

                    AppEntry modifiedAppEntry = appEntry with { PendingAddToRegistry = false } ;

                    preferencesXML.ModifyAppEntryAndSave(appEntry.AppPath, modifiedAppEntry);
                }

                bool enabled = appEntry.EnableSwitcher;
                //Debug.WriteLine(enabled);

                if (enabled)
                {
                    string pathvalue = appEntry.AppPath;

                    if (appEntry.GPUPrefOnBattery < 0 || appEntry.GPUPrefPluggedIn < 0)
                    {
                        Logger.inst.Log("Skipping registry update for pathvalue due to negative preference value: " + pathvalue, 2000);
                        continue;
                    }
                    Logger.inst.Log("Updating registry for pathvalue: " + pathvalue, 2000);

                    bool systemIsOnbattery = powerLineStatus == PowerLineStatus.Offline;

                    try
                    {
                        if (systemIsOnbattery)
                        {
                            RegistryHelper.SetGpuPref(pathvalue, appEntry.GPUPrefOnBattery);
                        }
                        else
                        {
                            RegistryHelper.SetGpuPref(pathvalue, appEntry.GPUPrefPluggedIn);
                        }
                    }
                    catch (RegistryHelper.NoGPUPreferenceDataException)
                    {
                        Logger.inst.Log("Not updating registry for pathvalue due to missing GpuPreference data value in registry: " + pathvalue, 2000);
                        //TODO implement some way to fix 
                    }

                }
            }

        }
    }
}