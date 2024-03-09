using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            AppEntry[] appEntryCopies = appEntries.ToArray();
            List<AppEntry> newList = new();
            newList.AddRange(appEntryCopies);
            return newList;
        }


        public readonly PreferencesXML PreferencesXML;

        public AppEntrySaveHandler()
        {
            PreferencesXML = new PreferencesXML();
            prevAppEntries = PreferencesXML.GetAppEntries();
            currentAppEntries = PreferencesXML.GetAppEntries();
        }

        public void UpdateAppEntryByPath(string path, AppEntry updatedAppEntry)
        {

            int index = CurrentAppEntries.IndexOf(CurrentAppEntries.Single(x => x.AppPath == path));
            
            /* //for-loop alternative, but the above should throw an error with an obvious enough meaning
            int index = -1;
            for (int i = 0; i < CurrentAppEntries.Count; i++)
            {
                AppEntry appEntry = CurrentAppEntries[i];
                if (appEntry.AppPath == path)
                {
                    index = i; break;
                }
            }

            if(index<0)
                throw new AppEntrySaverException($"UpdateAppEntry: No AppEntry with path {path} was found");
            */

            CurrentAppEntries[index] = updatedAppEntry;
            
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
        public void SaveAppEntryChanges()
        {

            List<AppEntry> differences = new();
            differences.AddRange(currentAppEntries.Where( entry => NotSameOrInPrevAppEntries(entry) ));

            List<string> existingAppPaths = PreferencesXML.GetAppPaths().ToList();
            List<AppEntry> needToAdd = new();
            needToAdd.AddRange(currentAppEntries.Where( entry => !existingAppPaths.Contains(entry.AppPath) ));

            //remove appentries whose *paths* exist in the prev but not the current
            List<AppEntry> needToRemoveFromXML = new();
            foreach( AppEntry entry in prevAppEntries )
            {
                if(!currentAppEntries.Exists(a => a.AppPath == entry.AppPath))
                {
                    needToRemoveFromXML.Add(entry);
                }
            }

            //new AppEntries should be added first so that ModifyAppEntry can actually find the AppEntry with the path
            foreach (AppEntry appEntry in needToAdd)
            {
                PreferencesXML.AddAppEntryAndSave(appEntry);
            }

            foreach(AppEntry appEntry in differences)
            {
                PreferencesXML.ModifyAppEntryAndSave(appEntry.AppPath, appEntry);
            }

            foreach(AppEntry appEntry in needToRemoveFromXML)
            {
                bool success = PreferencesXML.TryDeleteAppEntryAndSave(appEntry.AppPath);
                if(!success) //this would probably only ever fail if AppEntries were deleted externally whilst the GUI or Service was still running 
                    throw new XMLHelperException($"DeleteAppEntry: AppEntry with the specified path not found in data store: {appEntry.AppPath}");
            }

            prevAppEntries = DeepCopyAppEntries(currentAppEntries); //update the saved entries

            Logger.inst.Log("Concluded saving");

            bool NotSameOrInPrevAppEntries (AppEntry appEntry)
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

                    Debug.WriteLine($"DIFFERENT?: {(e.Equals(prev)? "no" : "yes")}");
                    
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
