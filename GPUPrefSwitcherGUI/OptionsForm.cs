using GPUPrefSwitcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Windows.Shapes;

namespace GPUPrefSwitcherGUI
{
    public partial class OptionsForm : Form
    {

        private AppOptions switcherOptions;

        private MainForm parentMainForm { get; init; }

        public OptionsForm(MainForm mainForm)
        {
            InitializeComponent();

            parentMainForm = mainForm;

            switcherOptions = new AppOptions();

            SpoofPowerStateComboBox.DataSource = PowerStateChoices;
            SpoofPowerStateComboBox.SelectedIndex = PowerStateChoices.IndexOf(PowerLineStatusConversions.PowerLineStatusToOfflineOrOnline(switcherOptions.CurrentOptions.SpoofPowerState).ToLower());

            EnableTipsCheckBox.Checked = switcherOptions.CurrentOptions.EnableTips;

            //we can also hit enter to commit
            UpdateIntervalTextbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(CheckEnterKeyPress);

            UpdateFormComponents(switcherOptions.CurrentOptions);

        }

        void CheckEnterKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                Validate();
            }
        }

        private List<string> PowerStateChoices = new List<string>(
            new string[]
            {
                "offline",
                "online"
            }
        );

        //TODO this name is ambiguous with UpdateActionButtons()
        //this does NOT update the Save/Revert buttons
        private void UpdateFormComponents(AppOptionsDataStruct current)
        {
            UpdateIntervalTextbox.Text = current.UpdateInterval.ToString();
            SpoofPowerStateEnabledCheckbox.Checked = current.SpoofPowerStateEnabled;
            SpoofPowerStateComboBox.SelectedItem = current.SpoofPowerState;
            RunTaskPluggedInCheckbox.Checked = current.RunTaskPluggedIn;
            RunOnBatteryCheckBox.Checked = current.RunTaskOnBattery;
            RealTimeLoggingCheckbox.Checked = current.EnableRealtimeLogging;
            long orphanedSize = FileSwapperUtils.GetOrphanedSize(parentMainForm.appEntrySaver.CurrentAppEntries);
            FolderStatsLabel1.Text = $"Total size of orphaned files/folders: {orphanedSize / 1024d / 1024d:f4}MB";
            long settingsBankSize = FileSwapperUtils.GetSettingsBankDirectorySize();
            FolderStatsLabel2.Text = $"Size of entire SettingsBank directory: {settingsBankSize / 1024d / 1024d:f4}MB";

            UpdateSpoofPowerStateComboBox();
        }

        private void UpdateIntervalTextbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void UpdateIntervalTextbox_Validating(object sender, CancelEventArgs e)
        {
            try
            {

                int newInterval = int.Parse(UpdateIntervalTextbox.Text);
                if (newInterval >= 10 && newInterval <= 1000000000)
                {
                    switcherOptions.CurrentOptions.UpdateInterval = newInterval;//accept
                }
                else
                {
                    UpdateIntervalTextbox.Text = switcherOptions.CurrentOptions.UpdateInterval.ToString();
                    MessageBox.Show("Update Interval: Please enter a numeric value between 10 and 1000000000");
                }

            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                //restore
                UpdateIntervalTextbox.Text = switcherOptions.CurrentOptions.UpdateInterval.ToString();
                MessageBox.Show("Please enter a numeric value between 10 and 1000000000");
            }
            UpdateActionButtons();
        }

        //trigger value validation upon clicking anywhere in the form
        private void OptionsForm_MouseClick(object sender, MouseEventArgs e)
        {
            Validate();
        }

        //upon losing focus
        private void OptionsForm_Leave(object sender, EventArgs e)
        {
            Validate();
        }


        private void UpdateIntervalTextbox_Validated(object sender, EventArgs e)
        {

        }

        private void SpoofPowerStateEnabledCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            switcherOptions.CurrentOptions.SpoofPowerStateEnabled = SpoofPowerStateEnabledCheckbox.Checked;
            UpdateSpoofPowerStateComboBox();
            UpdateActionButtons();
        }
        private void UpdateSpoofPowerStateComboBox()
        {
            SpoofPowerStateComboBox.Enabled = SpoofPowerStateEnabledCheckbox.Checked;
        }


        private void SpoofPowerStateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = ((string)SpoofPowerStateComboBox.SelectedValue).ToLower();

            if (selected == "offline")
            {
                switcherOptions.CurrentOptions.SpoofPowerState = PowerLineStatus.Offline;
            }
            else if (selected == "online")
            {
                switcherOptions.CurrentOptions.SpoofPowerState = PowerLineStatus.Online;
            }
            else
            {
                throw new Exception();
            }
            UpdateActionButtons();
        }

        private void CommitChangesButton_Click(object sender, EventArgs e)
        {
            OptionsFormUtils.AskRestartService();
            CommitChangesButton.Enabled = false;
            
            /* //not sure this does anything because there's no gauruntee the serrice has updated the file; maybe we need some sort of file update listener?
            switcherOptions.Reload();
            UpdateFormComponents(switcherOptions.CurrentOptions);
            */
            parentMainForm.Close();
        }

        private void SaveButton_Click_1(object sender, EventArgs e)
        {

            bool requireRestart;
            switcherOptions.SaveToXML(out requireRestart);

            UpdateActionButtons();

            if (requireRestart)
            {
                CommitChangesButton.Enabled = true;
            }
        }

        private void RevertButton_Click_1(object sender, EventArgs e)
        {
            switcherOptions.RevertChanges();
            UpdateFormComponents(switcherOptions.CurrentOptions);
            UpdateActionButtons();
        }

        private void UpdateActionButtons()
        {
            if (switcherOptions.ChangesArePending())
            {
                SaveButton.Enabled = true;
                RevertButton.Enabled = true;
            }
            else
            {
                SaveButton.Enabled = false;
                RevertButton.Enabled = false;
            }
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {

        }

        private void EnableTipsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            switcherOptions.CurrentOptions.EnableTips = EnableTipsCheckBox.Checked;
            UpdateActionButtons();
        }

        private void ResetAppPreferencesListButton_Click(object sender, EventArgs e)
        {
            DialogResult cleanOrBackup = MessageBox.Show("Backup the configuration file to a location?", "Preferences Reset/Backup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (cleanOrBackup.Equals(DialogResult.Yes))
            {
                using SaveFileDialog folderDialog = new SaveFileDialog();
                folderDialog.Title = "Pick a location to save the file";
                folderDialog.CheckPathExists = true;
                folderDialog.CheckWriteAccess = true;
                folderDialog.FileName = "Preferences.xml";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(PreferencesXML.XML_PREFERENCES_PATH, folderDialog.FileName, true);
                }

            }

            string message = "Are you sure you want to reset the list of App Preferences? (Note that you are NOT resetting the options you see in the window you clicked this from; rather, you are resetting the list with all the rows of App Entries). You can also make a backup of Preferences.xml right now.";
            if (MessageBox.Show(message, "Reset Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                OptionsFormUtils.ResetPreferencesXmlFile();
                parentMainForm.Close();
            }
            else
            {
                return;
            }

        }

        private void ResetAppOptionsButton_Click(object sender, EventArgs e)
        {
            switcherOptions.RevertToDefault_TopSection();
            UpdateFormComponents(switcherOptions.CurrentOptions);
            UpdateActionButtons();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            //I can't find where this label is in the editor
        }

        private void RunOnBatteryCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            switcherOptions.CurrentOptions.RunTaskOnBattery = RunOnBatteryCheckBox.Checked;
            UpdateActionButtons();
        }

        private void RunTaskPluggedInCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            switcherOptions.CurrentOptions.RunTaskPluggedIn = RunTaskPluggedInCheckbox.Checked;
            UpdateActionButtons();
        }

        private void CleanSettingsBankButton_Click(object sender, EventArgs e)
        {
            /* 
             * 1. Detect folders that don't correspond to an appentry
             * 2. Detect folders that don't correspond to a swappath
             * 
             * "Detect/backup/clean orphaned folders/files"
             * Ask if you'd like to back these up somewhere
             * Show how many folders/files/size
             * - With the structure intact + directory compressed 
             * - Just the settings files (UNNECESSARY)
             */

            DialogResult cleanOrBackup = MessageBox.Show("Backup the SettingsBank directory (in a .zip) to a location?", "SettingsBank Clean/Backup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (cleanOrBackup.Equals(DialogResult.Yes))
            {
                using SaveFileDialog folderDialog = new SaveFileDialog();
                folderDialog.Title = "Pick a location to save the .zip";
                folderDialog.CheckPathExists = true;
                folderDialog.CheckWriteAccess = true;
                folderDialog.FileName = "SettingsBank_Backup.zip";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    OptionsFormUtils.AskZipSettingsBankTo(folderDialog.FileName);
                }

            }

            long orphanedSize = FileSwapperUtils.GetOrphanedSize(parentMainForm.appEntrySaver.CurrentAppEntries);

            DialogResult delete = MessageBox.Show(
                    "Would you like to delete all orphaned folders and files in the SettingsBank " +
                    $"(ones that don't belong to any App Entry or File Swap Path)? This will free up {orphanedSize / 1024d / 1024d:f4}MB.\n" +
                    "Warning: this might also delete unrelated files in the SettingsBank directory. You can cancel, go back, and backup the folder first.",
                    "SettingsBank Clean/Backup",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                    );

            if (delete == DialogResult.Yes)
            {
                OptionsFormUtils.AskDeleteFolders();
            }

            UpdateFormComponents(switcherOptions.CurrentOptions);
        }

        private void RealTimeLoggingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            switcherOptions.CurrentOptions.EnableRealtimeLogging = RealTimeLoggingCheckbox.Checked;
            UpdateActionButtons();
        }

        private void OpenTaskSchedulerButton_Click(object sender, EventArgs e)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C taskschd";
            process.StartInfo = startInfo;
            process.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var f = new GPUPrefSwitcherRepairer.RepairForm();
            f.Show();
        }

        private void OpenAppDirButton_Click(object sender, EventArgs e)
        {
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"AppData") + "\"";
            fileopener.Start();

        }
    }
}
