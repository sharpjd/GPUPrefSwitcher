using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;

namespace Install
{
    
    internal class Program
    {
        const string SERVICE_NAME = "GPUPrefSwitcher";
        const string EXE_NAME = "GPUPrefSwitcher.exe";

        static void Main(string[] args)
        {
            try
            {
                string pathToGPUPrefSwitcher = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\"+EXE_NAME);

                ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
                processStartInfo.UseShellExecute = true;
                //https://stackoverflow.com/questions/16174736/cannot-get-sc-exe-to-work-for-my-program
                //this fails if you don't put a space after the equal sign... ARE YOU SERIOUS??
                processStartInfo.Arguments = $"/c sc.exe create {SERVICE_NAME} binpath= \"{pathToGPUPrefSwitcher}\" start= auto";
                Process.Start(processStartInfo);

                string appDataPath_installer = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData"); //in the same folder as this .exe
                string appDataPath_app = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\AppData");

                if (Directory.Exists(appDataPath_app))
                {
                    DialogResult result = MessageBox.Show("An existing AppData folder has been detected in the install location. Would you like to delete and overwrite it?" +
                        "\n\nIf you select \"Yes\", the folder will be deleted, and all previous data in that folder will be lost and overwritten with installer defaults." +
                        "\n\nIf you select \"No\", the folder will not be touched, and the app will utilize the preexisting data in the folder.",
                        "Old AppData detected", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Information);

                    if(result == DialogResult.Yes)
                    {
                        Directory.Delete(appDataPath_app, true);
                        CopyDirectory(appDataPath_installer, appDataPath_app, true);
                    } else if (result == DialogResult.No)
                    {
                        //do nothing
                    }
                } else
                {
                    CopyDirectory(appDataPath_installer, appDataPath_app, true);
                }

                DirectorySecurity directorySecurity = Directory.GetAccessControl(appDataPath_app);

                //give everybody access to the AppData path otherwise ConfigGUI crashes if run from user context
                directorySecurity.AddAccessRule(new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                Directory.SetAccessControl(appDataPath_app, directorySecurity);

                //start the service AFTER doing everything
                ProcessStartInfo processStartInfo2 = new ProcessStartInfo("cmd.exe");
                processStartInfo2.UseShellExecute = true;
                processStartInfo2.Arguments = $"/c sc start {SERVICE_NAME}";
                Process.Start(processStartInfo2);

            } catch (Exception ex)
            {

                Console.WriteLine("An error was encountered during installation:");
                Console.WriteLine(ex);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            
        }

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
    }
}
