using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace GPUPrefSwitcher
{
    public class OptionsFormUtils
    {
        public static void ResetPreferencesXmlFile(bool showRestartConfirmation = true)
        {
            var source = Path.Combine(Program.SavedDataPath, "defaults/Preferences.xml");
            var destination = Path.Combine(Program.SavedDataPath, "Preferences.xml");

            File.Copy(source, destination, true);

            if (showRestartConfirmation)
            {
                string message = "You will also need to restart the service, then re-open this app";
                MessageBox.Show(message, "Service Restart Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                AskRestartService();
            }
        }

        public static bool AskRestartService()
        {

            const int ERROR_CANCELLED = 1223; //The operation was canceled by the user.

            ProcessStartInfo info = new ProcessStartInfo("GPUPrefSwitcherSvcRestarter.exe");
            info.UseShellExecute = true;
            info.Verb = "runas";

            try
            {
                Process p = Process.Start(info);
                p.WaitForExit();
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == ERROR_CANCELLED)
                {
                    MessageBox.Show("Admin rights were denied, or error restarting service");
                    return false;
                }
                else
                    throw;
            }

            return true;
        }

        public static void AskDeleteFolders()
        {

            const int ERROR_CANCELLED = 1223; //The operation was canceled by the user.

            ProcessStartInfo info = new ProcessStartInfo("GUIAdminFunctions.exe", "-deleteOrphanedFiles");
            info.UseShellExecute = true;
            info.Verb = "runas";

            try
            {
                Process p = Process.Start(info);
                p.WaitForExit();
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == ERROR_CANCELLED)
                    MessageBox.Show("Admin rights were denied; they are necessary to delete some files in the SettingsBank");
                else
                    throw;
            }
        }

        public static void AskZipSettingsBankTo(string path)
        {

            const int ERROR_CANCELLED = 1223; //The operation was canceled by the user.

            string[] arguments = ["-exportSettingsBank", path];

            ProcessStartInfo info = new ProcessStartInfo("GUIAdminFunctions.exe", arguments);

            info.UseShellExecute = true;
            info.Verb = "runas";

            try
            {
                Process p = Process.Start(info);
                p.WaitForExit();
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == ERROR_CANCELLED)
                    MessageBox.Show("Admin rights were denied; they are necessary to export some files in the SettingsBank");
                else
                    throw;
            }
        }

    }
}
