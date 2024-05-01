using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
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
        //private static PreferencesXML preferencesXML;

        static AppOptions appOptions;
        private static SwitcherData switcherData;

        public static AppEntryLibrarian appEntryLibrarian;

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
        internal static async void Start(CancellationToken stoppingToken)
        {
            appOptions = new AppOptions();
            Logger.inst.EnableRealtimeStandardLogWrites = appOptions.CurrentOptions.EnableRealtimeLogging;
            Logger.inst.Log($"Initialized {nameof(AppOptions)}.");

            switcherData = SwitcherData.Initialize();
            Logger.inst.Log($"Initialized {nameof(SwitcherData)}.");

            appEntryLibrarian = new AppEntryLibrarian(); //exceptions get thrown from this too if there are problems with the XML (e.g. syntax)
            Logger.inst.Log($"Initialized {nameof(AppEntrySaveHandler)}.");

            //SystemEvents.PowerModeChanged += HandlePowerChangeEvent; **NOT the right event, this is for resume/suspend
            

            prevPowerLineStatus = switcherData.CurrentSwitcherData.PrevPowerStatus_enum;
            spoofPowerStateEnabled = appOptions.CurrentOptions.SpoofPowerStateEnabled;
            spoofPowerState = PowerLineStatusToOfflineOrOnline(appOptions.CurrentOptions.SpoofPowerState);

            lastShutdownWasClean = switcherData.CurrentSwitcherData.LastShutDownWasClean;
            //make sure static things initialize ^^^^^^^^ before running update 

            switcherData.CurrentSwitcherData.LastShutDownWasClean = false; //will be set to true when HandleStop() completes successfully
            switcherData.SaveToXML();

            updateInterval = appOptions.CurrentOptions.UpdateInterval;

            if (spoofPowerStateEnabled)
            {
                Logger.inst.Log($"Power State spoofing enabled: {spoofPowerState}");
            }

            File.Delete(CRASHED_FILE_PATH);//"successful" initialization yippee

            Task forever = BeginTimerForever(stoppingToken);

            /*
            forever.ContinueWith(
                (task) =>
                { throw task.Exception; }
                );
            */

            Logger.inst.Log($"Begin update logic timer.");

            if (!lastShutdownWasClean)
            {
                //doesn't actually serve a purpose for now
                Logger.inst.Log("Detected that service stop was not completed on the last run.");
            }

            await forever;
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
        //private static Task runningUpdateTask;

        private static List<Task> runningUpdateTasks = new();
        /// <summary>
        /// There should be only one of this running at any time.
        /// </summary>
        private static async Task BeginTimerForever(CancellationToken stoppingToken)
        {
            if (timerRunning) { throw new InvalidOperationException("This function cannot be called more than once"); }
            timerRunning = true;
            while (!stoppingToken.IsCancellationRequested)
            {

                for (int i = runningUpdateTasks.Count - 1; i >= 0; i--)
                {
                    Task task = runningUpdateTasks[i];
                    //TODO run a new task even if the previous one is going
                    if (task != null && task.IsFaulted)
                    {
                        Logger.inst.ErrorLog(task.Exception.ToString());
                        runningUpdateTasks.Remove(task);
                        throw task.Exception;
                    }

                    if (task == Task.CompletedTask)
                    {
                        runningUpdateTasks.Remove(task);
                    }

                }

                runningUpdateTasks.Add(RunUpdateLogic()); //no identified need to handle cancelling the update logic

                await Task.Delay(updateInterval, stoppingToken); //move this to the beginning of the loop to make attaching a debugger easier
            }
        }

        private static async Task RunUpdateLogic()
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
            currentPowerLineStatus = SystemInformation.PowerStatus.PowerLineStatus;
            if (spoofPowerStateEnabled)
            {
                //Logger.inst.Log($"Power State spoofing enabled: {spoofPowerState}");

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

            if (!runOnce)
            {
                Logger.inst.Log("Updating preferences regardless since this is the first update.");
                goto RunUpdateLogic;
            }

            //skip update if powerline status hasn't changed
            if (prevPowerLineStatus == currentPowerLineStatus)
            {
                Logger.inst.Log($"PowerLine status has NOT changed since last update. (Current state: {currentPowerLineStatus}; last state: {prevPowerLineStatus})", 2000);
                //return Task.CompletedTask;
                return;
            } else
            {
                Logger.inst.Log($"PowerLine status has changed since last update. Previous: {prevPowerLineStatus}; Current: {currentPowerLineStatus}");
                goto RunUpdateLogic;
            }
            
        RunUpdateLogic:

            runOnce = true;

            RegAndXMLMatchState regAndXMLMatchState = GetRegAndXmlMatchState();
            Logger.inst.Log($"Registry and XML match state: {regAndXMLMatchState}");

            //update XML file if there's an entry in the registry missing in the XML
            if (regAndXMLMatchState == RegAndXMLMatchState.RegMissingInXML ||
                regAndXMLMatchState == RegAndXMLMatchState.MissingInBoth)
            {
                WriteRegistryToXML();
            }

            //This NEEDS to come before (XML -> Registry) and after (Registry -> XML) (XML -> Registry depends on this)
            UpdateSeenInRegistryStatuses(); //sets the SeenInRegistry entry accordingly

            WriteXMLToRegistry(currentPowerLineStatus);

            AppEntrySaveHandler appEntrySaveHandler = await appEntryLibrarian.Borrow();
            Task swaps = BeginFileSwapLogic(currentPowerLineStatus, appEntrySaveHandler.CurrentAppEntries);
            appEntryLibrarian.Return(appEntrySaveHandler);

            BeginTaskSchedulerLogic(currentPowerLineStatus);

            prevPowerLineStatus = currentPowerLineStatus; //THIS MUST GO AT THE END

            await swaps; //we shouldn't, in case there is one that just keeps retrying forever

            //return Task.CompletedTask;
        }

        private static void BeginTaskSchedulerLogic(PowerLineStatus forPowerLineStatus)
        {
            if (forPowerLineStatus == PowerLineStatus.Online)
            {
                if(appOptions.CurrentOptions.RunTaskPluggedIn)
                {
                    TaskSchedulerUtils.RunPluggedInTask();
                    Logger.inst.Log("Ran Plugged In Task Scheduler entry");
                }
            } else if (forPowerLineStatus == PowerLineStatus.Offline)
            {
                if (appOptions.CurrentOptions.RunTaskOnBattery)
                {
                    TaskSchedulerUtils.RunOnBatteryTask();
                    Logger.inst.Log("Ran On Battery Task Scheduler entry");
                }
            } else
            {
                throw new NotImplementedException();
            }
        }

        private static void UpdateSeenInRegistryStatuses()
        {
            List<string> regPathValues = RegistryHelper.GetGpuPrefPathvalueNames().ToList();

            var hold = appEntryLibrarian.Borrow();
            hold.Wait();
            AppEntrySaveHandler appEntrySaveHandler = hold.Result;

            IReadOnlyList<AppEntry> appEntries = appEntrySaveHandler.CurrentAppEntries;

            for (int i = 0; i < appEntries.Count; i++)
            {
                AppEntry appEntry = appEntries[i];

                if (regPathValues.Contains(appEntry.AppPath)){
                    if (appEntry.SeenInRegistry == false)
                    {
                        appEntrySaveHandler.ChangeAppEntryByPath(appEntry.AppPath, appEntry with { SeenInRegistry = true });
                    }
                } else
                {
                    appEntrySaveHandler.ChangeAppEntryByPath(appEntry.AppPath, appEntry with { SeenInRegistry = false });
                }
            }
            appEntrySaveHandler.SaveAppEntryChanges();

            appEntryLibrarian.Return(appEntrySaveHandler);
        }

        private static async Task BeginFileSwapLogic(PowerLineStatus forPowerLineStatus, IEnumerable<AppEntry> forAppEntries)
        {
            Logger.inst.Log("Commence file swap logic.");

            //create the SettingsBank directory if it doesn't exist
            bool fileSwapperFolderExists = Directory.Exists(FileSwapper.SwapPathFolder);
            if (!fileSwapperFolderExists) { Directory.CreateDirectory(FileSwapper.SwapPathFolder); }

            AppEntrySaveHandler appEntrySaver = await appEntryLibrarian.Borrow();

            List<Task> fileSwapTasks = new();
            foreach (AppEntry appEntry in forAppEntries)
            {
                var fileSwap = new FileSwapper(appEntry, appEntryLibrarian);

                if (!appEntry.EnableFileSwapper) continue;
                if (appEntry.FileSwapperPaths.Length == 0) continue;

                Task fileSwapTask = fileSwap.InitiateFileSwaps(forPowerLineStatus, appEntrySaver);
                fileSwapTasks.Add(fileSwapTask);
            }

            appEntryLibrarian.Return(appEntrySaver);
            await Task.WhenAll(fileSwapTasks);

            Logger.inst.Log("File swap logic finished.");
        }

        static string[] ignoreList = RegistryHelper.ignoreValues;
        private static RegAndXMLMatchState GetRegAndXmlMatchState()
        {

            //TODO: get apps from UWP apps: https://stackoverflow.com/questions/50217328/get-icon-from-uwp-app

            var hold = appEntryLibrarian.Borrow();
            hold.Wait();
            AppEntrySaveHandler appEntrySaveHandler = hold.Result;

            IEnumerable<string> xmlPaths = from appEntry in appEntrySaveHandler.CurrentAppEntries select appEntry.AppPath;

            appEntryLibrarian.Return(appEntrySaveHandler);

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

            var hold = appEntryLibrarian.Borrow();
            hold.Wait();
            AppEntrySaveHandler appEntrySaver = hold.Result;

            Logger.inst.Log("FixRegXmlMismatch running: ");
            List<string> regPathValues = RegistryHelper.GetGpuPrefPathvalueNames().ToList();
            List<string> xmlPaths = (from appEntry in appEntrySaver.CurrentAppEntries select appEntry.AppPath).ToList();

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
                    appEntrySaver.CurrentAppEntries.Add(
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
                    appEntrySaver.SaveAppEntryChanges();
                    //appEntrySaveHandler.SaveAppEntryChanges();
                }
            }

            appEntryLibrarian.Return(appEntrySaver);

            Logger.inst.Log("Concluded Registry and XML mismatch fixing.");
        }

        internal static void WriteXMLToRegistry(PowerLineStatus powerLineStatus)
        {
            var hold = appEntryLibrarian.Borrow();
            hold.Wait();
            AppEntrySaveHandler appEntrySaveHandler = hold.Result;

            IEnumerable<AppEntry> appEntries = appEntrySaveHandler.CurrentAppEntries;

            bool systemIsOnbattery = powerLineStatus == PowerLineStatus.Offline;
            Logger.inst.Log($"System is on battery: {systemIsOnbattery}");

            foreach (AppEntry appEntry in appEntries)
            {

                if (appEntry.PendingAddToRegistry)
                {

                    Logger.inst.Log($"Pending registry add detected: {appEntry.AppPath}");

                    if (appEntry.SeenInRegistry)
                    {
                        Logger.inst.Log($"Skipping pended registry add because the value already exists in the registry: {appEntry.AppPath}", 2000);
                        appEntrySaveHandler.ChangeAppEntryByPath(appEntry.AppPath, appEntry with { PendingAddToRegistry = false });
                    } else
                    {
                        const int defaultPref = 0;
                        RegistryHelper.AddGpuPref(appEntry.AppPath, defaultPref);
                    }

                    AppEntry modifiedAppEntry = appEntry with { PendingAddToRegistry = false } ;

                    appEntrySaveHandler.ChangeAppEntryByPath(appEntry.AppPath, modifiedAppEntry);
                    //appEntrySaveHandler.SaveAppEntryChanges();
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
            appEntryLibrarian.Return(appEntrySaveHandler);

        }
    }
}
