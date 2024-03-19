using GPUPrefSwitcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GPUPrefSwitcherGUI
{
    public partial class EntryConfigForm : Form
    {

        private readonly List<GPUPreference> GPUChoicesOnBattery = new();
        private readonly List<GPUPreference> GPUChoicesPluggedIn = new();


        private AppEntry PrevSavedAppEntry;
        public AppEntry CurrentAppEntry;

        private FileSwapper fileSwap { get; }

        string SettingsBankPath
        {
            get
            {
                return fileSwap.SettingsBankFolderPath;
            }
        }

        private MainForm parentMainForm;

        public EntryConfigForm(AppEntry appEntry, MainForm mainForm)
        {
            InitializeComponent();

            parentMainForm = mainForm;

            PrevSavedAppEntry = appEntry;
            CurrentAppEntry = appEntry;

            fileSwap = new FileSwapper(appEntry, mainForm.appEntrySaver);

            GPUChoicesPluggedIn = MainForm.GetGPUPreferences();
            GPUChoicesOnBattery = MainForm.GetGPUPreferences();

            UpdateElements(ref appEntry);

            dataGridView1.FirstDisplayedScrollingColumnIndex = RemoveSwapPathColumn.Index;

            UpdateActionButtons();
        }

        void UpdateElements(ref AppEntry appEntry)
        {

            pictureBox1.Image = MainForm.GetIconImageFromPath(appEntry.AppPath);

            AppPathLabel.Text = appEntry.AppPath;
            AppNameLabel.Text = appEntry.AppName;
            EnabledCheckbox.Checked = appEntry.EnableSwitcher;

            int pluggedIn = appEntry.GPUPrefPluggedIn;
            PluggedInComboBox.DataSource = GPUChoicesPluggedIn;
            PluggedInComboBox.DisplayMember = nameof(GPUPreference.DisplayText);
            PluggedInComboBox.ValueMember = nameof(GPUPreference.ActualValue);
            PluggedInComboBox.SelectedIndex = GPUChoicesPluggedIn.IndexOf(GPUChoicesPluggedIn.Single(x => x.ActualValue == pluggedIn));

            int onBattery = appEntry.GPUPrefOnBattery;
            OnBatteryComboBox.DataSource = GPUChoicesOnBattery;
            OnBatteryComboBox.DisplayMember = nameof(GPUPreference.DisplayText);
            OnBatteryComboBox.ValueMember = nameof(GPUPreference.ActualValue);
            OnBatteryComboBox.SelectedIndex = GPUChoicesOnBattery.IndexOf(GPUChoicesOnBattery.Single(x => x.ActualValue == onBattery));

            EnableFileSwitcherCheckbox.Checked = appEntry.EnableFileSwapper;
            PendAddToRegistryCheckbox.Checked = appEntry.PendingAddToRegistry;

            if (Directory.Exists(SettingsBankPath))
            {
                BrowseStorageButton.Enabled = true;
            }
            else BrowseStorageButton.Enabled = false;

            UpdateDataGridView(ref appEntry);
        }

        void UpdateDataGridView(ref AppEntry appEntry)
        {
            dataGridView1.Rows.Clear();

            for (int i = 0; i < appEntry.FileSwapperPaths.Length; i++)
            {
                string swapPath = appEntry.FileSwapperPaths[i];
                int row = dataGridView1.Rows.Add();
                dataGridView1.Rows[row].Cells[0].Value = swapPath;
                ((DataGridViewTextBoxCell)dataGridView1.Rows[row].Cells[0]).Style.Alignment = DataGridViewContentAlignment.TopRight;

                //just the button text
                dataGridView1.Rows[row].Cells[1].Value = "Remove";
            }
        }

        private void EnabledGPUPrefSwitch_CheckedChanged(object sender, EventArgs e)
        {
            CurrentAppEntry = CurrentAppEntry with { EnableSwitcher = EnabledCheckbox.Checked };

            if (!CurrentAppEntry.EnableSwitcher) //disabling Switcher means disabling File swapper too 
            {
                CurrentAppEntry = CurrentAppEntry with { EnableFileSwapper = false };
                EnableFileSwitcherCheckbox.Checked = false;
            }

            UpdateActionButtons();
        }

        private void OnBatteryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //CurrentAppEntry.GPUPrefOnBattery = gpuPrefComboBoxToSelectedValue(OnBatteryComboBox);//bruh how do you get the value as an int
            CurrentAppEntry = CurrentAppEntry with { GPUPrefOnBattery = OnBatteryComboBox.SelectedIndex - 1 };//jank temporary fix but I could not find a cleaner way
            UpdateActionButtons();
        }
        private void PluggedInComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //CurrentAppEntry.GPUPrefPluggedIn = gpuPrefComboBoxToSelectedValue(PluggedInComboBox);
            CurrentAppEntry = CurrentAppEntry with { GPUPrefPluggedIn = PluggedInComboBox.SelectedIndex - 1 };//jank temporary fix but I could not find a cleaner way
            UpdateActionButtons();
        }
        private int GpuPrefComboBoxToSelectedValue(ComboBox c)
        {
            Debug.WriteLine(c.GetItemText(c.SelectedValue));
            Debug.WriteLine(c.GetItemText(c.SelectedValue));
            return GPUChoicesPluggedIn.Single(x => x.DisplayText == c.SelectedText).ActualValue;
            //^^^selection of GPUChoicesPluggedIn is arbitrary, it could've been GPUChoicesOnBattery too
        }



        private void RevertButton_Click(object sender, EventArgs e)
        {
            this.CurrentAppEntry = PrevSavedAppEntry;
            UpdateElements(ref CurrentAppEntry);
            UpdateActionButtons();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void EnableFileSwitcherCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            CurrentAppEntry = CurrentAppEntry with { EnableFileSwapper = EnableFileSwitcherCheckbox.Checked };
            UpdateActionButtons();
        }

        private void AddSwapPathButton_Click(object sender, EventArgs e)
        {

            string newSwapPath;

            OpenFileDialog ofd = new()
            {
                RestoreDirectory = true
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                newSwapPath = ofd.FileName;

                //ensure something else doesn't already have this SwapPath, otherwise it'd be undefined behavior
                if (CurrentAppEntry.FileSwapperPaths.Contains(newSwapPath))
                {
                    MessageBox.Show("The specified File Swap path has already been added for this entry.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                foreach (AppEntry a in parentMainForm.appEntrySaver.CurrentAppEntries)//TODO this can be refactored out
                {
                    foreach (string swapPath in a.FileSwapperPaths)
                    {
                        if (swapPath == newSwapPath)
                        {
                            MessageBox.Show($"This SwapPath already exists in the App Entry pointing to {a.AppPath} " +
                                $"— having more than one App Entry with File Swapper paths pointing to the same file is undefined behavior.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Information
                                );
                            return;
                        }
                    }
                }
            }
            else
            {
                return;
            }

            if (newSwapPath != string.Empty)
            {
                Debug.WriteLine(newSwapPath);
                string[] swapPathsNew = CurrentAppEntry.FileSwapperPaths.Append(newSwapPath).ToArray();
                PowerLineStatus[] swapperStatesNew = CurrentAppEntry.SwapperStates.Append(SystemInformation.PowerStatus.PowerLineStatus).ToArray();
                CurrentAppEntry = CurrentAppEntry with { SwapperStates = swapperStatesNew, FileSwapperPaths = swapPathsNew };

                UpdateDataGridView(ref CurrentAppEntry);
                UpdateActionButtons();
            }
        }

        void UpdateActionButtons()
        {
            if (PrevSavedAppEntry.GetHashCode() != CurrentAppEntry.GetHashCode())
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (CurrentAppEntry.FileSwapperPaths.Length == 0) return;
            if (e.RowIndex > CurrentAppEntry.FileSwapperPaths.Length - 1) return;

            if (e.ColumnIndex == 1)//remove button
            {
                var paths = new List<string>(CurrentAppEntry.FileSwapperPaths);
                paths.RemoveAt(e.RowIndex);
                CurrentAppEntry = CurrentAppEntry with { FileSwapperPaths = paths.ToArray() };
            }
            UpdateDataGridView(ref CurrentAppEntry);
            UpdateActionButtons();
        }

        private void EntryConfigForm_Load(object sender, EventArgs e)
        {

        }

        private void PendAddToRegistryCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            CurrentAppEntry = CurrentAppEntry with { PendingAddToRegistry = PendAddToRegistryCheckbox.Checked };
            UpdateActionButtons();
        }

        private void BrowseStorageButton_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(SettingsBankPath))
            {
                Process.Start("explorer.exe", SettingsBankPath);
            }
            else
            {
                MessageBox.Show("This App Entry doesn't have a corresponding storage folder yet. You might need to restart the service to create the folders after enabling the File Swapper");
            }

        }

        private void BrowsePluggingInButton_Click(object sender, EventArgs e)
        {

        }

        private void BrowsePluggingOutButton_Click(object sender, EventArgs e)
        {

        }


        private void EntryConfigForm_Resize(object sender, EventArgs e)
        {
            UpdateResizableElements();
        }

        private void UpdateResizableElements()
        {
            dataGridView1.Width = Width - 40;
            dataGridView1.Height = Height - dataGridView1.Location.Y - 100;
        }
    }
}
