using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace GPUPrefSwitcher
{
    /// <summary>
    /// Used for reading/writing save data for app GPU preferences (<see cref="AppEntry"/>'s).
    /// </summary>
    public class AppEntrySaveHandler
    {
        /// <summary>
        /// !!! This should always contain deep copies !!!
        /// </summary>
        private List<AppEntry> prevAppEntries;
        /*
        private List<AppEntry> prevAppEntries
        {
            get
            {
                return prevAppEntries_;
            }
            set
            {
                Logger.inst.Log("Set PrevAppEntries");
                prevAppEntries_ = value;
            }
        }
        private List<AppEntry> prevAppEntries_;
        */

        public IReadOnlyList<AppEntry> CurrentAppEntries
        {
            get
            {
                return currentAppEntries.AsReadOnly();
            }
        }
        private List<AppEntry> currentAppEntries;

        public void RevertAppEntriesToPrevious()
        {
            try
            {
                semaphoreSlim.Wait();
                currentAppEntries = DeepCopyAppEntries(prevAppEntries);
            } finally
            {
                semaphoreSlim.Release();
            }
        }
        private static List<AppEntry> DeepCopyAppEntries(List<AppEntry> appEntries)
        {
            /*
            //AppEntry[] appEntryCopies = appEntries.ToArray();

            var appEntryCopies = new AppEntry[appEntries.Count()];
            for(int i = 0; i < appEntryCopies.Length; i++)
            {
                appEntryCopies[i] = appEntries[i]; //struct copy 
            }

            List<AppEntry> newList = new();
            newList.AddRange(appEntryCopies);
            */
            List<AppEntry> newList = new();
            foreach(AppEntry a in appEntries)
            {
                newList.Add((AppEntry)a.Clone());
            }

            return newList;
        }

        private SemaphoreSlim semaphoreSlim;

        internal readonly PreferencesXML PreferencesXML;

        public AppEntrySaveHandler()
        {
            try
            {
                semaphoreSlim = new SemaphoreSlim(1);

                semaphoreSlim.Wait();

                PreferencesXML = new PreferencesXML();
                prevAppEntries = PreferencesXML.GetAppEntries();
                //currentAppEntries = PreferencesXML.GetAppEntries();
                currentAppEntries = DeepCopyAppEntries(prevAppEntries);

            } finally { semaphoreSlim.Release(); }
            

            //Logger.inst.Log(currentAppEntries.Single(x => x.AppPath == "F:\\SteamLibrary\\steamapps\\common\\Apex Legends\\r5apex.exe").ToString());
            //Logger.inst.Log(prevAppEntries.Single(x => x.AppPath == "F:\\SteamLibrary\\steamapps\\common\\Apex Legends\\r5apex.exe").ToString());
        }

        public void ChangeAppEntryByPathAndSave(string path, AppEntry updatedAppEntry)
        {
            try
            {
                Logger.inst.Log($"ChangeAppEntryByPath waiting on a semaphore ({path})");
                semaphoreSlim.Wait();
                Logger.inst.Log($"ChangeAppEntryByPath entered a semaphore ({path}).");

                Logger.inst.Log($"Attempting to change \n {currentAppEntries.Single(x=>x.AppPath==path)}\n to \n {updatedAppEntry}");

                //Logger.inst.Log($"prev: {CurrentAppEntries[CurrentAppEntries.IndexOf(CurrentAppEntries.Single(x => x.AppPath == path))]}");

                int index = currentAppEntries.IndexOf(CurrentAppEntries.Single(x => x.AppPath == path));

                currentAppEntries[index] = updatedAppEntry;

                SaveAppEntryChanges_Internal();

                //Logger.inst.Log($"new: {CurrentAppEntries[CurrentAppEntries.IndexOf(CurrentAppEntries.Single(x => x.AppPath == path))]}");
            } finally
            {
                Logger.inst.Log($"ChangeAppEntryByPath released a semaphore ({path})");
                semaphoreSlim.Release();
            }
        }

        public int RemoveAll(Predicate<AppEntry> predicate)
        {
            return currentAppEntries.RemoveAll(predicate);
        }

        public void AddAppEntryAndSave(AppEntry appEntry)
        {
            try
            {
                semaphoreSlim.Wait();

                currentAppEntries.Add(appEntry);

                SaveAppEntryChanges_Internal();

            }
            finally { semaphoreSlim.Release(); }
        }
        /*
         * DECISION: Replacement or overwrite?
         * Per-Entry Replacement (SELECTED):
         * - Slightly more complicated
         * - Less likely to completely mess up file from interruptions
         * - Better performing
         * - More extensible
         * Overwrite:
         * - Less functions, easier to understand
         * - Need to make a backup while writing
         * 
         * UPDATE: Only per-entry eplacement is viable because the XML can still contain stuff that isn't in the registry!!
         */
        /// <summary>
        /// NOT THREAD SAFE
        /// </summary>
        public void SaveAppEntryChanges()
        {
            try
            {
                semaphoreSlim.Wait();

                SaveAppEntryChanges_Internal();
                
            } finally
            {
                semaphoreSlim.Release();
            }
        }

        private void SaveAppEntryChanges_Internal()
        {
            //Logger.inst.Log(currentAppEntries.Single(x => x.AppPath == "F:\\SteamLibrary\\steamapps\\common\\Apex Legends\\r5apex.exe").ToString());
            //Logger.inst.Log(prevAppEntries.Single(x => x.AppPath == "F:\\SteamLibrary\\steamapps\\common\\Apex Legends\\r5apex.exe").ToString());
            List<AppEntry> differences = new();
            differences.AddRange(currentAppEntries.Where(entry => NotSameOrInPrevAppEntries(entry)));

            List<string> existingAppPaths = new List<string>(from appEntry in PreferencesXML.GetAppEntries() select appEntry.AppPath);
            List<AppEntry> needToAdd = new();
            needToAdd.AddRange(currentAppEntries.Where(entry => !existingAppPaths.Contains(entry.AppPath)));

            //remove appentries whose *paths* exist in the prev but not the current
            List<AppEntry> needToRemoveFromXML = new();
            foreach (AppEntry entry in prevAppEntries)
            {
                if (!currentAppEntries.Exists(a => a.AppPath == entry.AppPath))
                {
                    needToRemoveFromXML.Add(entry);
                }
            }

            //new AppEntries should be added first so that ModifyAppEntry can actually find the AppEntry with the path
            foreach (AppEntry appEntry in needToAdd)
            {
                PreferencesXML.AddAppEntryAndSave(appEntry);
            }

            foreach (AppEntry appEntry in differences)
            {
                PreferencesXML.ModifyAppEntryAndSave(appEntry.AppPath, appEntry);
            }

            foreach (AppEntry appEntry in needToRemoveFromXML)
            {
                bool success = PreferencesXML.TryDeleteAppEntryAndSave(appEntry.AppPath);
                if (!success) //this would probably only ever fail if AppEntries were deleted externally whilst the GUI or Service was still running 
                    throw new XMLHelperException($"DeleteAppEntry: AppEntry with the specified path not found in data store: {appEntry.AppPath}");
            }

            prevAppEntries = DeepCopyAppEntries(currentAppEntries); //update the saved entries

            //Logger.inst.Log(currentAppEntries.Single(x => x.AppPath == "F:\\SteamLibrary\\steamapps\\common\\Apex Legends\\r5apex.exe").ToString());
            //Logger.inst.Log(prevAppEntries.Single(x => x.AppPath == "F:\\SteamLibrary\\steamapps\\common\\Apex Legends\\r5apex.exe").ToString());

            Logger.inst.Log($"Concluded saving. Differences: {differences.Count} Added: {needToAdd.Count} Removed: {needToRemoveFromXML.Count}");
            if(differences.Count > 0)
            {
                Logger.inst.Log("Changed:");
                foreach(AppEntry appEntry in differences)
                {
                    Logger.inst.Log(appEntry.ToString());
                }
            }

            bool NotSameOrInPrevAppEntries(AppEntry appEntry)
            {
                /*
                if (appEntry.AppPath == "F:\\SteamLibrary\\steamapps\\common\\Apex Legends\\r5apex.exe")
                {
                    Logger.inst.Log(prevAppEntries.Single(x => x.AppPath == "F:\\SteamLibrary\\steamapps\\common\\Apex Legends\\r5apex.exe").ToString());
                    Logger.inst.Log(CurrentAppEntries.Single(x => x.AppPath == "F:\\SteamLibrary\\steamapps\\common\\Apex Legends\\r5apex.exe").ToString());
                    Logger.inst.Log($"{appEntry} not in prev app entries: {!prevAppEntries.Contains(appEntry)}");
                }
                */
                return !prevAppEntries.Contains(appEntry); //Contains uses Equals() which is implemented in AppEntry
            }
        }

        public bool AppEntriesHaveChangedFromLastSave()
        {
            try {

                semaphoreSlim.Wait();

                if (prevAppEntries.SequenceEqual(CurrentAppEntries))
                {
                    return false;
                }
                else
                {

                    var diffs = currentAppEntries.Except(prevAppEntries);
                    return true;//move to end for debug info
                    foreach (AppEntry e in diffs)
                    {

                        Debug.WriteLine("CURRENT:");
                        Debug.WriteLine(e.ToString());

                        //WARNING: causes crash if Preferences.xml AppsList is empty
                        var prev = prevAppEntries.Single(x => x.AppPath == e.AppPath);
                        Debug.WriteLine("PREVIOUS:");
                        Debug.WriteLine(prev.ToString());

                        Debug.WriteLine(e.GetHashCode());
                        Debug.WriteLine(prev.GetHashCode());

                        Debug.WriteLine($"DIFFERENT?: {(e.Equals(prev) ? "no" : "yes")}");

                        /*
                        Debug.WriteLine(e.EnableFileSwapper.GetHashCode());
                        Debug.WriteLine(prev.EnableFileSwapper.GetHashCode());
                        */

                        /*
                        Debug.WriteLine(e.AppPath.GetHashCode());
                        Debug.WriteLine(prev.AppPath.GetHashCode());
                        */

                        /*
                        Debug.WriteLine(e.FileSwapperPaths.GetHashCode());
                        Debug.WriteLine(prev.FileSwapperPaths.GetHashCode());
                        Debug.WriteLine(e.SwapperStates.GetHashCode());
                        Debug.WriteLine(prev.SwapperStates.GetHashCode());
                        */

                        /*
                        Debug.WriteLine(e.getStringArrHash(e.FileSwapperPaths));
                        Debug.WriteLine(prev.getStringArrHash(prev.FileSwapperPaths));
                        Debug.WriteLine(e.getStringArrHash(e.SwapperStates));
                        Debug.WriteLine(prev.getStringArrHash(prev.SwapperStates));
                        */
                    }
                }
            } finally
            {
                semaphoreSlim.Release();
            }
        }

    }

    public class AppEntrySaverException(string message) : Exception(message)
    {
    }
}
