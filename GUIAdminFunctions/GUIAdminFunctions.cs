using GPUPrefSwitcher;
using System.IO.Compression;
using System.Security.Principal;

namespace GUIAdminFunctions
{
    internal class GUIAdminFunctions
    {
        static void Main(string[] args)
        {
            bool hasRelevantArgs = false;
            

            if(args.Contains("-deleteOrphanedFiles")) hasRelevantArgs = true;
            if (args.Contains("-exportSettingsBank")) hasRelevantArgs = true;

            if (!hasRelevantArgs)
            {
                Console.WriteLine("Unrecognized arguments: " + args);
                Console.WriteLine("This app is intended for use by the GPUPrefSwitcher app, and not to be run from the CLI.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            if(!IsAdministrator())
            {
                Console.WriteLine("This app cannot be run as non-admin.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            if (args[0] == "-deleteOrphanedFiles")
            {
                Console.WriteLine("Deleting files...");

                var preferencesXML = new PreferencesXML();

                DeleteOrphanedFoldersAndFiles(preferencesXML.GetAppEntries(), preferencesXML);
                Console.WriteLine("Operation complete. Press any key to continue...");
                Console.ReadKey();
                return;
            }

            if (args[0] == "-exportSettingsBank")
            {
                SaveSettingsBankZippedTo(args[1]);
                Console.WriteLine("Operation complete. Press any key to continue...");
                Console.ReadKey();
                return;
            }
            
        }

        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                      .IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Requires Administrator permissions.
        /// </summary>
        /// <param name="appEntries"></param>
        /// <param name="forPreferencesXML"></param>
        /// <returns></returns>
        public static void DeleteOrphanedFoldersAndFiles(IEnumerable<AppEntry> appEntries, PreferencesXML forPreferencesXML)
        {
            string[] orphanedAppEntryFolders = FileSwapper.GetOrphanedAppEntryFolders(appEntries, forPreferencesXML);

            List<string> orphanedSwapPathFolders = new();

            foreach (AppEntry appEntry in appEntries)
            {
                FileSwapper fileSwapper = new FileSwapper(appEntry, forPreferencesXML);
                if (Directory.Exists(fileSwapper.SettingsBankFolderPath))
                    orphanedSwapPathFolders.AddRange(FileSwapper.GetOrphanedSwapPathFoldersFor(appEntry, forPreferencesXML));
            }

            foreach (string orphanedAppEntryFolder in orphanedAppEntryFolders)
            {
                DirectoryInfo dir = new DirectoryInfo(orphanedAppEntryFolder);
                dir.Delete(true);
            }

            foreach (string orphanedSwapPathFolder in orphanedSwapPathFolders)
            {
                DirectoryInfo dir = new DirectoryInfo(orphanedSwapPathFolder);
                dir.Delete(true);
            }
        }

        /// <summary>
        /// Requires Administrator permissions.
        /// </summary>
        /// <param name="path"></param>
        public static void SaveSettingsBankZippedTo(string path)
        {
            bool exists = false;
            if (File.Exists(path))
            {
                exists = true;
            }

            if (exists)
            {
                File.Delete(path);
            }

            ZipFile.CreateFromDirectory(FileSwapper.SwapPathFolder, path, CompressionLevel.Optimal, true);
        }
    }
}
