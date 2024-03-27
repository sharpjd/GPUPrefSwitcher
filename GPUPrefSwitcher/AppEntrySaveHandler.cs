using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GPUPrefSwitcher
{

    /// <summary>
    /// Acts as an intermediary or interface with blocking for thread safety when using an <see cref="AppEntrySaveHandler"/>.
    /// Usage idea/tip: each function should be responsible for its own borrow and return. That is, if a Task/async function "A"
    /// borrows, and then calls another Task/async function "B" that *also* needs to borrow, then "A" should return it before 
    /// awaiting "B".
    /// </summary>
    public class AppEntryLibrarian //you could call this AppEntrySaveHandlerCoordinator or AppEntrySaveHandlerThreadCoordinator but that's ridiculous
    {

        private SemaphoreSlim semaphoreSlim = new(1);

        private AppEntrySaveHandler appEntrySaveHandler;

        /// <summary>
        /// You must call <see cref="Return(AppEntrySaveHandler)"/> after you're done with the <see cref="AppEntrySaveHandler"/> otherwise a deadlock will occur.
        /// Throws <see cref="TimeoutException"/> if it waits longer than (a default of) 10 seconds.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public async Task<AppEntrySaveHandler> Borrow(int timeout = 10000)
        {
            //Logger.inst.Log("Waiting to borrow.");
            bool inTime = await semaphoreSlim.WaitAsync(timeout);
            if(!inTime)
            {
                throw new TimeoutException($"Borrowing waited for too long. Check if every Borrow is followed by a Return");
            }

            //Logger.inst.Log("Borrowed.");
            return appEntrySaveHandler;
        }

        /// <summary>
        /// Similar to <see cref="Borrow(int)"/>, but instead of throwing an exception, simply returns null after the timeout.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<AppEntrySaveHandler> TryBorrow(int timeout = 10000)
        {
            bool inTime = await semaphoreSlim.WaitAsync(timeout);
            if (!inTime)
            {
                return null;
            }

            return appEntrySaveHandler;
        }

        public void Return(AppEntrySaveHandler appEntrySaveHandler_)
        {
            if(appEntrySaveHandler != appEntrySaveHandler_)
            {
                throw new ArgumentException("the same AppEntrySaveHandler instance wasn't returned");
            }
            
            semaphoreSlim.Release();

            //Logger.inst.Log("Returned.");
        }

        public AppEntryLibrarian() 
        {
            appEntrySaveHandler = new AppEntrySaveHandler();
        }
    }

    /// <summary>
    /// Used for reading/writing save data for app GPU preferences (<see cref="AppEntry"/>'s).
    /// 
    /// This is not thread safe. For such cases, use this with a borrow/return pattern by instantiating an <see cref="AppEntryLibrarian"/>
    /// </summary>
    public class AppEntrySaveHandler
    {
        /// <summary>
        /// !!! This should always contain deep copies !!!
        /// </summary>
        private List<AppEntry> prevAppEntries;

        public List<AppEntry> CurrentAppEntries
        {
            get
            {
                return currentAppEntries;
            }
        }
        private List<AppEntry> currentAppEntries;

        public void RevertAppEntriesToPrevious()
        {
            currentAppEntries = DeepCopyAppEntries(prevAppEntries);
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
            foreach (AppEntry a in appEntries)
            {
                newList.Add((AppEntry)a.Clone());
            }

            return newList;
        }

        internal readonly PreferencesXML PreferencesXML;

        public AppEntrySaveHandler()
        {

            PreferencesXML = new PreferencesXML();
            prevAppEntries = PreferencesXML.GetAppEntries();
            //currentAppEntries = PreferencesXML.GetAppEntries();
            currentAppEntries = DeepCopyAppEntries(prevAppEntries);

        }

        public void ChangeAppEntryByPath(string path, AppEntry updatedAppEntry)
        {

            int index = currentAppEntries.IndexOf(CurrentAppEntries.Single(x => x.AppPath == path));

            currentAppEntries[index] = updatedAppEntry;

            //Logger.inst.Log($"new: {CurrentAppEntries[CurrentAppEntries.IndexOf(CurrentAppEntries.Single(x => x.AppPath == path))]}");
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
            SaveAppEntryChanges_Internal();
        }

        private void SaveAppEntryChanges_Internal()
        {
            List<AppEntry> differences = new();
            //differences.AddRange(currentAppEntries.Where(entry => NotSameOrInPrevAppEntries(entry)));
            foreach(AppEntry a in currentAppEntries)
            {
                if(NotSameOrInPrevAppEntries(a)) differences.Add(a);
            }

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

            Logger.inst.Log($"Concluded saving. Differences: {differences.Count} Added: {needToAdd.Count} Removed: {needToRemoveFromXML.Count}");

            /*
            if (differences.Count > 0)
            {
                //Logger.inst.Log("Changed:");
                foreach (AppEntry appEntry in differences)
                {
                    Logger.inst.Log(appEntry.ToString());
                }
            }
            */

            bool NotSameOrInPrevAppEntries(AppEntry appEntry)
            {
                return !prevAppEntries.Contains(appEntry); //Contains uses Equals() which is implemented in AppEntry
            }
        }

        public bool AppEntriesHaveChangedFromLastSave()
        {
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
        }

    }

    public class AppEntrySaverException(string message) : Exception(message)
    {
    }
}
