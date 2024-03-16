using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPUPrefSwitcher
{

    public record FileSwapPathTask 
    {
        public required string FileSwapPath { get; init; }
        public required PowerLineStatus ForPowerLineStatus { get; init; }

        public int ID { get; }

        private static int id = 0;

        public FileSwapPathTask()
        {
            ID = id;
            id++;
        }
    }

    /// <summary>
    /// Represents the file swap data and behavior of an AppEntry for the detailed, mistake-prone file swap system.
    /// </summary>
    public class FileSwapper
    {

        /*
         * Everything that works with the SettingsBank directory should agree with this:
         * 
         * <Install directory>/SettingsBank/<AppEntry unique path ID>/<Unique SwapPath ID>/<Stored File>
         * 
         * e.g. <Install dir>\SettingsBank\0C972752749E68A1B716515E22E4885180965AA2\66ACF76A9D6DAEDA9F3DEDC0C1B5A5040F1331CB\OnBattery_Settings.txt
         * 
         * Writing a function to abstract each part of this path would cause as many problems as it solves for now
         */
        public static readonly string SwapPathFolder = Program.SavedDataPath + "SettingsBank";
        public static readonly string OnBatteryPrefix = "OnBattery_";
        public static readonly string PluggedInPrefix = "PluggedIn_";

        private AppEntry AppEntry { get; }

        public string SettingsBankFolderName
        {
            get => HashString(AppEntry.AppPath);
        }
        public string SettingsBankFolderPath
        {
            get => SwapPathFolder + "\\" + SettingsBankFolderName;
        }

        public PreferencesXML ForPreferencesXML { get; init; }

        public FileSwapper(AppEntry appEntry, PreferencesXML preferencesXML) 
        { 
            AppEntry = appEntry;
            ForPreferencesXML = preferencesXML;
        }

        
        public async Task InitiateFileSwaps(PowerLineStatus forPowerLineStatus, PreferencesXML preferencesXML)
        {
            Logger.inst.Log($"Initiating File Swaps for AppEntry with target path {AppEntry.AppPath}");

            //if the AppEntry doesn't yet have its own FileSwap folder, create it
            bool appEntryFolderExists = Directory.Exists(SettingsBankFolderPath);
            if (!appEntryFolderExists)
            {
                Directory.CreateDirectory(SettingsBankFolderPath);
                File.Create(Path.Combine(SettingsBankFolderPath, $"{AppEntry.AppName} settings are in {SettingsBankFolderPath}"));
            }

            List<Task> fileSwapTasks = new();
            //commence the swap for each FileSwapPath
            for (int i = 0; i < AppEntry.FileSwapperPaths.Length; i++)
            {

                string swapPath = AppEntry.FileSwapperPaths[i];
                if (!File.Exists(swapPath))
                {
                    Logger.inst.Log($"Skipping SwapPath {swapPath} (belonging to the AppEntry with path {AppEntry.AppPath}) because the target folder doesn't exist.");
                    continue;
                }

                FileSwapPathTask current = new FileSwapPathTask() { 
                    FileSwapPath = swapPath, 
                    ForPowerLineStatus = forPowerLineStatus, 
                };

                
                try
                {
                    fileSwapTasks.Add(InitiateSingleFileSwap(current, i));
                }
                catch (AggregateException)
                {
                    throw;
                }

            } //...repeat for every SwapPath

            Logger.inst.Log($"Finished firing single FileSwap tasks for AppEntry with target path {AppEntry.AppPath}");

            await Task.WhenAll(fileSwapTasks);

            Logger.inst.Log($"Completed all FileSwap tasks for AppEntry with target path {AppEntry.AppPath}");

        }


        /*
         * The FileSwapPath state (Offline/Online) in the XML file is changed as soon as the respective copy operation 
         * completes, and a file swap does not occur if it's already in its appropriate respective state.
         * 
         * This means that it at least shouldn't be possible for a race condition to mess up the Online/Offline state          <---(unless in the *extremely* unlikely event that the Online/Offline tracker change doesn't go through literally a single line after the file copy)
         * and its correspondence to which files are physically in the target path, and in the SettingsBank store. 
         * 
         * However, it's still possible for other undesirable behavior to occur. 
         * FOR EXAMPLE:
         * The user plugs their laptop out. A file swap task (Offline) is created. Assume this operation is unfortunately blocked. 
         * 
         * Assume that before it retries, the user plugs in their laptop, creating another file swap task (Online). 
         * 
         * Assume the file gets unlocked right before the creation of that (Online) task. The (Online) swap happens before the
         * (Offline) one, and THEN the (Offline) one executes, leaving us with undesired behavior: every other successful
         * FileSwap path has their correct (Online) state since we're plugged in, but this one has its (Offline) file in place.
         * 
         * 
         * Solution: 
         * We keep a list of FileSwapTasks, identified by their FileSwapPath and ID (+1 for each FileSwapTask created); 
         * When a task is created, if a FileSwapTask with the same target path already exists, it must
         * first wait for the old one (of a lower ID) to finish before executing itself. 
         */
        private static List<FileSwapPathTask> OngoingFileSwapTasks = new();
        /// <summary>
        /// Designed to be fire-and-forget. See adjacent comment for how race conditions/multiple calls are handled.
        /// </summary>
        private async Task InitiateSingleFileSwap(FileSwapPathTask fileSwapPathTask, int swapPathIndex) //yes the swapPathIndex parameter is a bit lazy, but otherwise I'd have to modify AppEntry to be a dictionary or 2d array
        {

            //track this task in a list (we will remove it from the list at the end when this task completes)
            OngoingFileSwapTasks.Add(fileSwapPathTask);

            string swapPath = fileSwapPathTask.FileSwapPath;
            PowerLineStatus forPowerLineStatus = fileSwapPathTask.ForPowerLineStatus;
            PreferencesXML preferencesXML = ForPreferencesXML;

        //use caution when modifying the sequence of logic here and note where this block is placed
        WaitForOlderFileSwaps:
            if (OngoingFileSwapTasks.Any(x => x.FileSwapPath == fileSwapPathTask.FileSwapPath && x.ID < fileSwapPathTask.ID))
            {
                Logger.inst.Log($"An older FileSwap task for {swapPath} already exists, delaying");
                await Task.Delay(10000);
                goto WaitForOlderFileSwaps;
            }

        WaitForAppToClose:
            if (await IsProcessRunning(AppEntry.AppPath)) //IsProcessRunning is apparently VERY expensive
            {
                Logger.inst.Log($"Target app {AppEntry.AppPath} is still running, delaying File Swap for {swapPath}");
                await Task.Delay(5000);
                goto WaitForAppToClose;
            }

            //check if this individual SwapPath has a folder
            string swapPathHash = GetSwapPathSettingsBankFolderName(swapPath);
            string swapPathBankFolderPath = SettingsBankFolderPath + "\\" + swapPathHash;
            bool swapPathFolderExists = Directory.Exists(swapPathBankFolderPath);
            if (!swapPathFolderExists) { Directory.CreateDirectory(swapPathBankFolderPath); }

            //then, create copies of the config file (one for OnBattery state, one for PluggedIn state) if they don't exist
            string nameOfFileToSwap = Path.GetFileName(swapPath); //TODO: Handle illegal characters (perhaps not here but somewhere else)

            string onBatteryStoredFileName = OnBatteryPrefix + nameOfFileToSwap;
            string onBatteryStoredFilePath = swapPathBankFolderPath + "\\" + onBatteryStoredFileName;
            bool onBatteryStoredFileExists = File.Exists(onBatteryStoredFilePath);
            if (!onBatteryStoredFileExists)
            {
            StoreFile_OnBattery:
                try
                {
                    await FileCopyConsiderAccessRules(swapPath, onBatteryStoredFilePath, true, false);
                    File.Create(Path.Combine(SettingsBankFolderPath, $"{Path.GetFileName(swapPath)} is in {swapPathHash}"));
                } 
                catch (IOException)
                {
                    Logger.inst.Log($"Could not copy and store file for OnBattery state for {swapPath}, trying again in a bit");
                    await Task.Delay(5000);
                    goto StoreFile_OnBattery;
                }    
            }

            string pluggedInStoredFileName = PluggedInPrefix + nameOfFileToSwap;
            string pluggedInStoredFilePath = swapPathBankFolderPath + "\\" + pluggedInStoredFileName;
            bool pluggedInStoredFileExists = File.Exists(pluggedInStoredFilePath);
            if (!pluggedInStoredFileExists)
            {
            StoreFile_PluggedIn:
                try
                {
                    await FileCopyConsiderAccessRules(swapPath, pluggedInStoredFilePath, true, false);
                    File.Create(Path.Combine(SettingsBankFolderPath, $"{Path.GetFileName(swapPath)} is in {swapPathHash}"));
                }
                catch (IOException)
                {
                    Logger.inst.Log($"Could not copy and store file for PluggedIn state for {swapPath}, trying again in a bit");
                    await Task.Delay(5000);
                    goto StoreFile_PluggedIn;
                }
            }

            //finally perform the swap
            if (forPowerLineStatus == PowerLineStatus.Offline)
            {
                if (AppEntry.SwapperStates[swapPathIndex] == PowerLineStatus.Online)
                {
                TryAgain:
                    int errorFlag = 0;
                    try
                    {
                        //no need to cancel the save if the substitution fails
                        await FileCopyConsiderAccessRules(swapPath, pluggedInStoredFilePath, true, false); //save current config to PluggedIn store
                        string s1 = $"Saved SwapPath {swapPath} for app {AppEntry.AppPath} as PluggedIn";
                        Logger.inst.Log(s1);

                        errorFlag = 1;

                        await FileCopyConsiderAccessRules(onBatteryStoredFilePath, swapPath, true, true); //then substitute with OnBattery config
                        string s2 = $"Substituted OnBattery config into SwapPath {swapPath} for app {AppEntry.AppPath}";
                        Logger.inst.Log(s2);

                    }
                    catch (IOException)
                    {

                        if (errorFlag == 0)
                        {
                            Logger.inst.Log($"Could not copy to destination {onBatteryStoredFilePath}, retrying in some time.");
                        }
                        else if (errorFlag == 1)
                        {
                            Logger.inst.Log($"Could not copy to destination {swapPath}, retrying in some time.");
                        }
                        await Task.Delay(5000);
                        goto TryAgain;
                    }

                    AppEntry modified = AppEntry; //struct copy
                    modified.SwapperStates[swapPathIndex] = PowerLineStatus.Offline;

                    preferencesXML.ModifyAppEntryAndSave(AppEntry.AppPath, modified);
                    string s3 = $"Saved SwapPath state for SwapPath {swapPath} for app {AppEntry.AppPath}";
                    Logger.inst.Log(s3);
                } else
                {
                    Logger.inst.Log($"Skipped operations for File Swap path {swapPath} because it's already in its correct state.");
                }
            }
            else if (forPowerLineStatus == PowerLineStatus.Online)
            {
                if (AppEntry.SwapperStates[swapPathIndex] == PowerLineStatus.Offline)
                {
                TryAgain:
                    int errorFlag = 0;
                    try
                    {

                        //no need to cancel the save if the substitution fails 
                        await FileCopyConsiderAccessRules(swapPath, onBatteryStoredFilePath, true, false); //save current config to OnBattery store
                        string s1 = $"Saved SwapPath {swapPath} for app {AppEntry.AppPath} as OnBattery";
                        Logger.inst.Log(s1);

                        errorFlag = 1;

                        await FileCopyConsiderAccessRules(pluggedInStoredFilePath, swapPath, true, true); //then substitute with PluggedIn config
                        string s2 = $"Substituted PluggedIn config into SwapPath {swapPath} for app {AppEntry.AppPath}";
                        Logger.inst.Log(s2);
                    }
                    catch (IOException)
                    {
                        if (errorFlag == 0)
                        {
                            Logger.inst.Log($"Could not copy to destination {onBatteryStoredFilePath}, retrying in some time.");
                        }
                        else if (errorFlag == 1)
                        {
                            Logger.inst.Log($"Could not copy to destination {swapPath}, retrying in some time.");
                        }
                        await Task.Delay(5000);
                        goto TryAgain;
                    }

                    AppEntry modified = AppEntry; //struct copy
                    modified.SwapperStates[swapPathIndex] = PowerLineStatus.Online;

                    preferencesXML.ModifyAppEntryAndSave(AppEntry.AppPath, modified);
                    string s3 = $"Saved SwapPath state for SwapPath {swapPath} for app {AppEntry.AppPath}";
                    Logger.inst.Log(s3);
                } else
                {
                    Logger.inst.Log($"Skipped operations for File Swap path {swapPath} because it's already in its correct state.");
                }
            }
            else
            {
                Debug.WriteLine($"Unknown power state: " + forPowerLineStatus.ToString());
            }

            Logger.inst.Log($"FileSwap Task {fileSwapPathTask.FileSwapPath} finished for {AppEntry.AppPath}");

            OngoingFileSwapTasks.Remove(fileSwapPathTask);

        }

        static async Task<bool> IsProcessRunning(string processPath) //this function is apparently VERY expensive to run (many mores times than this app itself) so await it!
        {
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                try
                {
                    // Compare the process's main module file name with the specified process path
                    if (await Task.Run(() => string.Equals(process.MainModule.FileName, processPath, StringComparison.OrdinalIgnoreCase))) //the expensive line in question
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    // Some processes may not have a valid MainModule property, so ignore any exceptions
                }
            }

            return false;
        }



        /// <summary>
        /// Get the folder name that corresponds to an app entry SwapPath. Maintains a standard format
        /// </summary>
        /// <param name="appEntry"></param>
        /// <returns></returns>
        //this takes a string instead of an AppEntry because it's more intuitive and helps distinguish from the similarly named function above
        public static string GetSwapPathSettingsBankFolderName(string swapPath)
        {
            return HashString(swapPath);
        }

        //source: https://stackoverflow.com/questions/3984138/hash-string-in-c-sharp
        //public static string GetHashString(string inputString)
        public static string HashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            static byte[] GetHash(string str)
            {
                //SHA1 is 40 characters vs. 64 characters with SHA256
                /*
                 * WARNING
                 * 
                 * Changing the hash algorithm will break ALL original corresponding file paths
                 * for the file swapping system
                 */
                using (HashAlgorithm algorithm = SHA1.Create())
                    return algorithm.ComputeHash(Encoding.UTF8.GetBytes(str));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Copies a file while maintaining access control (but not ownership, which is apparently irrelevant here because File.Copy(overwrite=true) does not seem to overwrite the target file's ownership).
        /// If the destination file's access rules don't match the source file's access rules, it cancels the copy and throws <see cref="PermissionMismatchException"/>.
        /// This is to prevent malicious apps from swapping malicious files into SettingsBank entries that point to files with different/higher access rights than the malicious app does.
        /// By default, it checks if the destination file exists; this can be used (and *is* used) to prevent Arbitrary File Write attacks.
        /// </summary>
        /// 
        /// And yes, I think it's clearer to throw exceptions instead of returning false, unless there is a better way (in which I have tried TWICE; this is perfectly functional)
        public static Task FileCopyConsiderAccessRules(string sourceFileName, string destinationFileName, bool overwrite, bool ensureDestinationExists = true)
        {

            FileInfo sourceInfo = new FileInfo(sourceFileName);
            FileInfo destinationInfo = new FileInfo(destinationFileName);

            FileSecurity sourceFileSecurity = sourceInfo.GetAccessControl();

            if (ensureDestinationExists)
            {
                if (!destinationInfo.Exists)
                    throw new ArgumentException($"Destination file {destinationFileName} does not exist ({nameof(ensureDestinationExists)} was set to true.)");
            }

            //this entire block ensures we don't overwrite if there is a security mismatch
            if (destinationInfo.Exists)
            {
                FileSecurity destinationFileSecurity = destinationInfo.GetAccessControl();

                var sourceAccessRules = sourceFileSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
                var destinationAccessRules = destinationFileSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));

                //make sure that each IdentityReference (e.g. user, admin)'s access rule set (i.e. rwx) from the source file matches the destination file
                foreach (FileSystemAccessRule sourceRule in sourceAccessRules)
                {
                    var sourceRuleIdentityReference = sourceRule.IdentityReference; //the IdentityReference that the access rule set belongs to

                    bool identityExistsInDestination = false;
                    //find the corresponding IdentityReference and check if the access rules match
                    foreach (FileSystemAccessRule destinationRule in destinationAccessRules)
                    {
                        if (destinationRule.IdentityReference == sourceRuleIdentityReference)
                        {
                            identityExistsInDestination = true;

                            //debug info for the event log
                            //throw new NotImplementedException($"{destinationInfo}, {sourceInfo} {destinationRule.IdentityReference}, {sourceRuleIdentityReference}, {destinationRule.FileSystemRights}, {sourceRule.FileSystemRights}, {sourceRule.FileSystemRights==destinationRule.FileSystemRights}");

                            //error if the corresponding identity's access rights are different
                            if (sourceRule.FileSystemRights != destinationRule.FileSystemRights)
                            {
                                throw new PermissionMismatchException($"Destination file {destinationInfo} access rules do not match source file {sourceInfo} access rules; in particular, {destinationRule.IdentityReference}:{destinationRule.FileSystemRights} != {sourceRule.IdentityReference}:{sourceRule.FileSystemRights}");
                            }
                        }
                    }
                    //error if we can't find the identity
                    if (!identityExistsInDestination)
                    {
                        throw new PermissionMismatchException($"Destination file {destinationInfo} access rules do not match source file {sourceInfo} access rules; the destination's permission identities do not match contain the source file's permission identities.");
                    }
                }

            }

            //copy source file Access Rules (but not ownership) to destination file; this MUST come after the security check block
            sourceFileSecurity.SetAccessRuleProtection(true, true); //supposedly this doesn't change the existing file's security settings?
            File.Copy(sourceFileName, destinationFileName, true);
            destinationInfo.SetAccessControl(sourceFileSecurity); //requires the file to exist, duh

            return Task.CompletedTask;
        }

        /*
        //Reference: https://stackoverflow.com/questions/882686/asynchronous-file-copy-move-in-c-sharp
        public static Task CopyToAsync(string source, string destination)
        {
            var openForReading = new FileStreamOptions { Mode = FileMode.Open };
            using FileStream _source = new FileStream(source, openForReading);

            var createForWriting = new FileStreamOptions
            {
                Mode = FileMode.CreateNew,
                Access = FileAccess.Write,
                Options = FileOptions.WriteThrough,
                BufferSize = 0, // disable FileStream buffering
                PreallocationSize = _source.Length // specify size up-front
            };
            using FileStream _destination = new FileStream(destination, createForWriting);
            return _source.CopyToAsync(_destination);
        }
        */

        public static string[] GetOrphanedSwapPathFoldersFor(AppEntry appEntry, PreferencesXML forPreferencesXML)
        {

            /*
             * go to this AppEntry FileSwapper SettingsBank folder and find all the folders
             * that don't correspond to a SwapPath
             */

            var fileSwap = new FileSwapper(appEntry, forPreferencesXML);

            string entryBankFolderName = fileSwap.SettingsBankFolderName;
            string pathToAppEntryBank = Path.Combine(SwapPathFolder, entryBankFolderName);

            string[] bankedFolderPaths = Directory.GetDirectories(pathToAppEntryBank);
            string[] bankedFolderNames = bankedFolderPaths.Select(Path.GetFileName).ToArray(); //get just the folder name (yes, Path.GetFileName works)

            //folders that correspond to an actual SwapPath
            List<string> nonOrphanedBankedFolderPaths = new();
            foreach (string swapPath in appEntry.FileSwapperPaths)
            {
                string swapPathBankFolderName = GetSwapPathSettingsBankFolderName(swapPath);

                if (bankedFolderNames.Contains(swapPathBankFolderName))
                    nonOrphanedBankedFolderPaths.Add(Path.Combine(pathToAppEntryBank, swapPathBankFolderName));
            }

            //the remaining folders that don't correspond to a SwapPath are "orphaned"
            List<string> orphanedBankedFolderPaths = new();
            foreach(string folderPath in bankedFolderPaths)
            {
                if(!nonOrphanedBankedFolderPaths.Contains(folderPath))
                    orphanedBankedFolderPaths.Add(folderPath);
            }

            return orphanedBankedFolderPaths.ToArray();
        }

        public static string[] GetOrphanedAppEntryFolders(IEnumerable<AppEntry> appEntries, PreferencesXML forPreferencesXML)
        {

            string[] bankedFolderPaths = Directory.GetDirectories(SwapPathFolder);
            string[] bankedFolderNames = bankedFolderPaths.Select(Path.GetFileName).ToArray(); //get just the folder name (yes, Path.GetFileName works)

            List<string> nonOrphanedFolderPaths = new();
            foreach (AppEntry appEntry in appEntries)
            {
                var fileSwap = new FileSwapper(appEntry, forPreferencesXML);

                string correspondingFolderName = fileSwap.SettingsBankFolderName;
                string correspondingFolderPath = Path.Combine(SwapPathFolder, correspondingFolderName);

                if (bankedFolderNames.Contains(correspondingFolderName))
                    nonOrphanedFolderPaths.Add(correspondingFolderPath);
            }

            IEnumerable<string> orphanedFolderPaths =
                from folderName in bankedFolderPaths
                where !nonOrphanedFolderPaths.Contains(folderName)
                select folderName;

            return orphanedFolderPaths.ToArray();
        }

        public static long GetOrphanedSize(IEnumerable<AppEntry> appEntries, PreferencesXML forPreferencesXML)
        {
            string[] orphanedAppEntryFolders = GetOrphanedAppEntryFolders(appEntries, forPreferencesXML);

            List<string> orphanedSwapPathFolders = new();

            foreach(AppEntry appEntry in appEntries)
            {
                FileSwapper fileSwapper = new FileSwapper(appEntry, forPreferencesXML);
                if(Directory.Exists(fileSwapper.SettingsBankFolderName))
                    orphanedSwapPathFolders.AddRange(GetOrphanedSwapPathFoldersFor(appEntry, forPreferencesXML));
            }

            long totalSizeOfOrphaned = 0;

            foreach(string orphanedAppEntryFolder in orphanedAppEntryFolders)
            {
                totalSizeOfOrphaned += DirSize(new DirectoryInfo(orphanedAppEntryFolder));
            }

            foreach (string orphanedSwapPathFolder in orphanedSwapPathFolders)
            {
                totalSizeOfOrphaned += DirSize(new DirectoryInfo(orphanedSwapPathFolder));
            }

            return totalSizeOfOrphaned;
        }

        


        class PermissionMismatchException(string message) : Exception(message)
        {
        }

        //https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public static long GetSettingsBankDirectorySize()
        {
            return DirSize(new DirectoryInfo(SwapPathFolder));
        }

        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }

        public static long DirSize_Orphaned(DirectoryInfo d, IEnumerable<AppEntry> appEntries, PreferencesXML preferencesXML)
        {
            



            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }
    }
}
