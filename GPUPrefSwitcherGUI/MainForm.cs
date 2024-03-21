using GPUPrefSwitcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GPUPrefSwitcherGUI
{
    public partial class MainForm : Form
    {

        internal AppEntrySaveHandler appEntrySaver { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            
            Initialize();
        }

        private List<GPUPreference> GPUChoices = new();

        readonly AppOptions switcherOptions = new();
        private void Initialize()
        {
            Debug.WriteLine("Form initializing");

            GPUChoices = GetGPUPreferences();

            appEntrySaver = new AppEntrySaveHandler();

            dataGridView1.EditingControlShowing += DataGridView_EditingControlShowing;
            dataGridView1.CellValidating += dataGridView_CellValidating;
            dataGridView1.CellEndEdit += dataGridView_CellEndEdit;

            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            TipRichTextBox.Visible = switcherOptions.CurrentOptions.EnableTips;

            foreach(DataGridViewColumn dataGridViewColumn in dataGridView1.Columns)
            {
                dataGridViewColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            PickNewTip();
            UpdateGrid();

            UpdateResizableElements();
        }

        internal static List<GPUPreference> GetGPUPreferences()
        {
            /*
            * A google search says that the max amount of GPUs Windows supports is 13
            * This seems arbitrary but I will limit it to that for the following reasons:
            * - Handling an x amount of custom values makes the code a lot more confusing
            *  (more entries have to be added to GPUChoices because that's just how ComboBoxes handle data binds)
            * - Having a registry value set to something higher than the amount of GPUs in the system
            * doesn't seem to blow up the OS from my own basic testing, but still it might cause confusion
            */
            List<GPUPreference> gpuChoices = new()
            {
                new GPUPreference("-1 (Skip/Invalid)", -1),
                new GPUPreference("0 (Let Windows Decide)", 0),
                new GPUPreference("1 (Power Saving)", 1),
                new GPUPreference("2 (High Performance)", 2),
                new GPUPreference("3 (Other GPU)", 3),
                new GPUPreference("4 (Other GPU)", 4),
                new GPUPreference("5 (Other GPU)", 5),
                new GPUPreference("6 (Other GPU)", 6),
                new GPUPreference("7 (Other GPU)", 7),
                new GPUPreference("8 (Other GPU)", 8),
                new GPUPreference("9 (Other GPU)", 9),
                new GPUPreference("10 (Other GPU)", 10),
                new GPUPreference("11 (Other GPU)", 11),
                new GPUPreference("12 (Other GPU)", 12),
                new GPUPreference("13 (Other GPU)", 13)
            };

            return gpuChoices;
        }

        const int AppIconCol = 0;
        const int AppNameCol = 1;
        const int AppPathCol = 2;
        const int EnableSwitcherCol = 3;
        const int PluggedInCol = 4;
        const int OnBatteryCol = 5;
        const int EnableFileSwapperCol = 6;
        const int ConfigureCol = 7;

        /// <summary>
        /// Updates the main DataGridView with info from <see cref="AppEntrySaveHandler.CurrentAppEntries"/>
        /// Does not update the action buttons (save/revert/commit)
        /// </summary>
        internal void UpdateGrid()
        {
            dataGridView1.Rows.Clear();
            //for each row
            for (int i = 0; i < appEntrySaver.CurrentAppEntries.Count; i++)
            {
                AppEntry appEntry = appEntrySaver.CurrentAppEntries[i];

                //in case there is an entry with nothing in it (datagridviews do this by default)
                //if (appEntry.AppPath == null || appEntry.AppPath == string.Empty) continue;
                //Debug.WriteLine(appEntry.AppPath);

                dataGridView1.Rows.Add();

                System.Drawing.Image image = GetIconImageFromPath(appEntry.AppPath);
                dataGridView1.Rows[i].Cells[AppIconCol].Value = image;
                dataGridView1.Rows[i].Cells[AppPathCol].Value = appEntry.AppPath;
                dataGridView1.Rows[i].Cells[AppNameCol].Value = appEntry.AppName;
                dataGridView1.Rows[i].Cells[EnableSwitcherCol].Value = appEntry.EnableSwitcher;
                dataGridView1.Rows[i].Cells[EnableFileSwapperCol].Value = appEntry.EnableFileSwapper;

                DataGridViewComboBoxCell pluggedInCell = (DataGridViewComboBoxCell)dataGridView1.Rows[i].Cells[PluggedInCol];
                pluggedInCell.DataSource = GPUChoices;
                pluggedInCell.DisplayMember = nameof(GPUPreference.DisplayText);
                pluggedInCell.ValueMember = nameof(GPUPreference.ActualValue);
                pluggedInCell.Value = appEntry.GPUPrefPluggedIn;
                //pluggedInCell.FlatStyle = FlatStyle.Popup;//also allows coloring them

                DataGridViewComboBoxCell onBatteryCell = (DataGridViewComboBoxCell)dataGridView1.Rows[i].Cells[OnBatteryCol];
                onBatteryCell.DataSource = GPUChoices;
                onBatteryCell.DisplayMember = nameof(GPUPreference.DisplayText);
                onBatteryCell.ValueMember = nameof(GPUPreference.ActualValue);
                onBatteryCell.Value = appEntry.GPUPrefOnBattery;
                //onBatteryCell.FlatStyle = FlatStyle.Popup; //also allows coloring them

                //can resize rows
                dataGridView1.Rows[i].Resizable = DataGridViewTriState.True;
                //just the button text
                dataGridView1.Rows[i].Cells[ConfigureCol].Value = "More...";

                if (!appEntry.SeenInRegistry)
                {

                    //dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;

                    var newHeaderCell = new DataGridViewRowHeaderCell
                    {
                        Value = "ℹ",
                        ToolTipText = "This entry does not have a corresponding registry entry. The GPU Preference Number (On Battery/Plugged In) columns will not be reflected.\n" +
                        "You can add this entry via the Graphics Settings Panel, or you can remove this if it's no longer necessary.\n" +
                        "(Note: entries in the Graphics Settings Panel also may not have a registry entry; you can fix this by going into More -> checking \"Pend add to Registry\""
                    };
                    /* colors don't work
                    newHeaderCell.Style.ForeColor = Color.LightGray;
                    //dataGridView1.Rows[i].HeaderCell.DefaultCellStyle.BackColor = Color.LightGray;
                    */
                    newHeaderCell.Style.Font = new Font(FontFamily.GenericSansSerif, 15);
                    newHeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    /* neither can i set the dropdown button colors
                    onBatteryCell.Style.SelectionBackColor = Color.LightGray;
                    pluggedInCell.Style.SelectionBackColor = Color.LightGray;
                    */
                    //we could integrate this, but it looks hard https://stackoverflow.com/questions/65976232/how-to-change-the-combobox-dropdown-button-color

                    dataGridView1.Rows[i].HeaderCell = newHeaderCell;

                }

            }
        }

        //https://learn.microsoft.com/en-us/dotnet/desktop/winforms/advanced/how-to-extract-the-icon-associated-with-a-file-in-windows-forms?view=netframeworkdesktop-4.8
        public static System.Drawing.Image GetIconImageFromPath(string path)
        {

            Icon iconForFile = SystemIcons.Application; //default icon

            if (!File.Exists(path))
            {
                //Debug.WriteLine("Returning default image for " + path);
                return iconForFile.ToBitmap();
            }

            //FileInfo fileInfo = new FileInfo(path);

            try
            {
                iconForFile = System.Drawing.Icon.ExtractAssociatedIcon(path);
            }
            catch (FileNotFoundException) { }

            return iconForFile.ToBitmap();
            
        }
        //maybe get icons for UWP apps too
        //https://stackoverflow.com/questions/37686916/how-do-i-retrieve-a-windows-store-apps-icon-from-a-c-sharp-desktop-app

        //datagridview reference: https://www.youtube.com/watch?v=LzQSeCXYVKA

        /* example of variables I can access
         * this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.AppPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AppName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EnableSwitcher = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.GPUPrefPluggedIn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.GPUPrefOnBattery = new System.Windows.Forms.DataGridViewComboBoxColumn();
         */

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex < 0) return; //fixes null reference (row -1 is not an AppEntry)

            if(e.ColumnIndex == EnableSwitcherCol) 
            {
                var enableSwitcherCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[e.RowIndex].Cells[EnableSwitcherCol];
                bool enableSwitcher = bool.Parse(enableSwitcherCell.Value.ToString());

                if (!enableSwitcher) //if we're unchecking EnableSwitcher, we should uncheck EnableFileSwapper too
                {
                    var enableFileSwapperCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[e.RowIndex].Cells[EnableFileSwapperCol];
                    bool fileSwapperAlsoEnabled = bool.Parse(enableFileSwapperCell.Value.ToString());
                    if (fileSwapperAlsoEnabled)
                    {
                        enableFileSwapperCell.Value = false;
                    }
                }
            }

            AppEntry updatedAppEntry = appEntryFromRow(e.RowIndex);
            appEntrySaver.ChangeAppEntryByPathAndSave(updatedAppEntry.AppPath, updatedAppEntry);

            if (e.ColumnIndex == ConfigureCol)
            {
                var form = new EntryConfigForm(updatedAppEntry, this);
                form.ShowDialog();

                if (form.DialogResult == DialogResult.OK)
                {
                    AppEntry returned = form.CurrentAppEntry;
                    appEntrySaver.ChangeAppEntryByPathAndSave(updatedAppEntry.AppPath, returned);
                    UpdateGrid();
                }

            }
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            //should come after updating the list
            UpdateActionButtons();
        }

        #region desperate attempts at making DGV dropdowns more responsive

        //hacky fix https://stackoverflow.com/questions/11843488/how-to-detect-datagridview-checkbox-event-change#15011844
        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            UpdateActionButtons();
        }

        //FIRES WHEN POPULATING GRID, CAUSING POTENTIAL NULLREFERENCE
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            /* //this doesn't help 
            if ((e.ColumnIndex == PluggedInCol || e.ColumnIndex == OnBatteryCol) && e.RowIndex != -1)
            {
                //dataGridView1.EndEdit();
                UpdateActionButtons();
            }
            */
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            /* //this doesn't help
            if ((e.ColumnIndex == PluggedInCol || e.ColumnIndex == OnBatteryCol) && e.RowIndex != -1)
            {
                //https://stackoverflow.com/questions/32947475/datagridviewcomboboxcolumn-have-to-click-cell-twice-to-display-combo-box
                var editingControl = dataGridView1.EditingControl as DataGridViewComboBoxEditingControl;
                if (editingControl != null)
                    editingControl.DroppedDown = true;
            }
            */
        }

        private void DataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //TODO: not having the switcher enabled should grey these comboboxes out 
            if (e.Control is System.Windows.Forms.ComboBox comboBox)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                /* //make it typable 
                comboBox.DropDownStyle = ComboBoxStyle.DropDown;
                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
                */
            }
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            //object updatedValue = dataGridView1[e.ColumnIndex, e.RowIndex].Value;

            AppEntry updatedAppEntry = appEntryFromRow(e.RowIndex);
            appEntrySaver.ChangeAppEntryByPathAndSave(updatedAppEntry.AppPath, updatedAppEntry);

            //this should come after replacing the AppEntry
            UpdateActionButtons();
        }

        #endregion

        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == PluggedInCol || e.ColumnIndex == OnBatteryCol)
            {
                DataGridViewComboBoxCell cell = dataGridView1[e.ColumnIndex, e.RowIndex] as DataGridViewComboBoxCell;

                //Debug.WriteLine(e.FormattedValue.GetType());
                Debug.WriteLine((string)e.FormattedValue);
                //Debug.WriteLine(int.Parse("9"));
                //Debug.WriteLine(((string)e.FormattedValue).Length);

                //(if the value isn't a choice in GPUChoices; i.e. a custom value)
                if (!GPUChoices.Any(gpuChoice => gpuChoice.DisplayText == (string)e.FormattedValue))
                {
                    bool validInput = true;
                    int inputInteger = -2;
                    try
                    {
                        inputInteger = int.Parse((string)(e.FormattedValue));
                        //inputInteger = int.Parse(Regex.Match("[0-9]", (string)e.FormattedValue).ToString());
                    }
                    catch (FormatException)
                    {
                        validInput = false;
                    }

                    if (!validInput || inputInteger < -1 || inputInteger > 13)
                    {
                        e.Cancel = true;
                        MessageBox.Show("Please enter an integer greater than or equal to -1 and less than or equal to 13 with no other characters");
                    }
                    else //if new user input is valid
                    {
                        Debug.WriteLine(cell.Value.GetType());
                        cell.Value = inputInteger;
                    }
                }
            }
        }

        //TODO this seems dangerous, why are we retrieving data from the actual form?
        AppEntry appEntryFromRow(int rowIndex)
        {
            //TODO why can't we just take this from the appEntrySaver ?
            string path = dataGridView1[AppPathCol, rowIndex].Value.ToString();
            string name = dataGridView1[AppNameCol, rowIndex].Value.ToString();
            bool enabled = bool.Parse(dataGridView1[EnableSwitcherCol, rowIndex].Value.ToString());
            int pluggedIn = int.Parse(dataGridView1[PluggedInCol, rowIndex].Value.ToString());
            int onBattery = int.Parse(dataGridView1[OnBatteryCol, rowIndex].Value.ToString());
            bool enableFileSwapper = bool.Parse(dataGridView1[EnableFileSwapperCol, rowIndex].Value.ToString());

            //string[] fileSwapPaths = appEntrySaver.CurrentAppEntries.First(e => e.AppPath == path).FileSwapperPaths;
            var appEntryFromBackend = appEntrySaver.CurrentAppEntries.Single(e => e.AppPath == path);
            string[] fileSwapPaths = appEntryFromBackend.FileSwapperPaths;
            PowerLineStatus[] swapPathStates = appEntryFromBackend.SwapperStates;
            bool pendingAddToRegistry = appEntryFromBackend.PendingAddToRegistry;
            string runPathPluggedIn = appEntryFromBackend.RunPluggedInPath;
            string runOnBatteryPath = appEntryFromBackend.RunOnBatteryPath;
            bool seenInRegistry = appEntryFromBackend.SeenInRegistry;

            return new AppEntry()
            {
                AppPath = path,
                AppName = name,
                EnableSwitcher = enabled,
                GPUPrefOnBattery = onBattery,
                GPUPrefPluggedIn = pluggedIn,
                EnableFileSwapper = enableFileSwapper,
                FileSwapperPaths = fileSwapPaths,
                SwapperStates = swapPathStates,
                PendingAddToRegistry = pendingAddToRegistry,
                RunOnBatteryPath = runOnBatteryPath,
                RunPluggedInPath = runPathPluggedIn,
                SeenInRegistry = seenInRegistry,
            };
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            appEntrySaver.SaveAppEntryChanges();

            UpdateGrid();
            UpdateActionButtons();

            CommitButton.Enabled = true;
        }

        void UpdateActionButtons()
        {
            if (appEntrySaver.AppEntriesHaveChangedFromLastSave())
            {
                SaveButton.Enabled = true;
                RevertChangesButton.Enabled = true;
            }
            else
            {
                SaveButton.Enabled = false;
                RevertChangesButton.Enabled = false;
            }
        }

        private void RevertChangesButton_Click(object sender, EventArgs e)
        {
            appEntrySaver.RevertAppEntriesToPrevious();
            UpdateGrid();
            UpdateActionButtons();
        }

        private void CommitButton_Click(object sender, EventArgs e)
        {
            OptionsFormUtils.AskRestartService();
            CommitButton.Enabled = false;
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            var form = new OptionsForm(this);
            form.Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (appEntrySaver.AppEntriesHaveChangedFromLastSave())
                if (MessageBox.Show("You have unsaved changes. Are you sure you want to close the editor and lose them?", "Closing Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
        }

        private void AddEntryButton_Click(object sender, EventArgs e)
        {
            string newPath;

            OpenFileDialog ofd = new();
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                newPath = ofd.FileName;
            }
            else
            {
                newPath = string.Empty;
            }

            if (newPath != string.Empty)
            {
                FileInfo fileInfo = new(newPath);
                if (fileInfo.Extension == ".exe")
                {
                    if (appEntrySaver.CurrentAppEntries.Any(a => a.AppPath == newPath))
                    {
                        MessageBox.Show("The specified path already exists as an entry",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                            );
                    }
                    else
                    {


                        AppEntry newAppEntry = GetDefaultAppEntry(newPath) with { PendingAddToRegistry = true };
                        appEntrySaver.AddAppEntryAndSave(newAppEntry);

                        UpdateGrid();
                        UpdateActionButtons();

                        //focus on the new AppEntry
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            AppEntry a = appEntryFromRow(i);
                            if (a.AppPath == newPath)
                            {
                                dataGridView1.Rows[i].Selected = true;
                                dataGridView1.FirstDisplayedScrollingRowIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("The specified file is not a .exe",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                        );
                }

                UpdateResizableElements();//otherwise if the list is empty and we add something, the columns will appear squashed
            }

            AppEntry GetDefaultAppEntry(string path)
            {
                AppEntry newAppEntry = new()
                {
                    AppPath = path,
                    EnableSwitcher = false,
                    GPUPrefOnBattery = 0,
                    GPUPrefPluggedIn = 0,
                    EnableFileSwapper = false,
                    SwapperStates = Array.Empty<PowerLineStatus>(),
                    FileSwapperPaths = Array.Empty<string>(),
                    PendingAddToRegistry = false,
                    RunOnBatteryPath = string.Empty,
                    RunPluggedInPath = string.Empty,
                    SeenInRegistry = false,
                };
                return newAppEntry;
            }

        }

        private void RemoveAppsButton_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selected = dataGridView1.SelectedRows;

            if (selected.Count == 0)
            {
                MessageBox.Show("Select some entries (using the leftmost blank column) first",
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                        );
            }

            List<AppEntry> selectedAppEntries = new();
            foreach (DataGridViewRow row in selected)
            {
                selectedAppEntries.Add(appEntryFromRow(row.Index));
            }

            appEntrySaver.RemoveAll(a => selectedAppEntries.Contains(a));

            UpdateGrid();
            UpdateActionButtons();
        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                RemoveAppsButton.Text = $"Remove 1 App";
            }
            else
            {
                RemoveAppsButton.Text = $"Remove {dataGridView1.SelectedRows.Count} Apps";
            }
        }

        string[] tips = new string[]
        {
            "Tip: Removing an App Entry row from here only removes it from this app's config, meaning it may get added back on the next start-up (this app can *add* registry entries, but doesn't *remove* them). To prevent this, use the Windows Graphics options panel to truly remove it from the registry.",
            "Tip: Want to configure the OnBattery settings for an app while plugged in? Go to Options, check \"Spoof Power State\", and select \"Offline\", and commit the changes. Don't forget to uncheck \"Spoof Power State\".",
            "Tip: File Swapper functionality also freezes if the leftmost \"Enable\" checkbox for an app entry is not enabled.",
            "Tip: It is not reccomended to manipulate important data with this app.",
            "Tip: You can click on (most) of the column headers to sort the app entries by that column.",
            "Tip: Changes do not enact until you hit \"Commit Changes\", which restarts the service (this is the result of power saving design; the service doesn't have to periodically update and make I/O requests this way).",
            "Tip: You can turn off tips from Options.",
            //"Tip: This is my first ever proper public app, consider donating to <> to show appreciation for my work!",
            "Tip: The config locations of games often require searching the web. PCGamingWiki is a potential database for these locations. Games also often put their config files in the AppData folders, Documents folder, or even their own game directory.",
            "Tip: You can click on the tip text box area for a new tip",
            "Tip: In the More Options menu, you can enable this app to execute Task Scheduler entries (e.g. for running scripts you write) upon plugging in/out for even greater flexibility.",
            "Tip: Some buttons and labels display tooltips with additional info if you hover over them."
        };

        private void TipRichTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            PickNewTip();
        }

        private void PickNewTip()
        {
        Pick:
            Random random = new();
            string toShow = tips[random.Next(tips.Length)];

            if (toShow == TipRichTextBox.Text) goto Pick;

            TipRichTextBox.Text = toShow;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            UpdateResizableElements();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        void UpdateResizableElements()
        {

            dataGridView1.MaximumSize = new Size(Width - 185, dataGridView1.MaximumSize.Height);

            int requiredWidth = 0;
            requiredWidth += dataGridView1.Columns[AppIconCol].Width;
            requiredWidth += dataGridView1.Columns[EnableSwitcherCol].Width;
            requiredWidth += dataGridView1.Columns[PluggedInCol].Width;
            requiredWidth += dataGridView1.Columns[OnBatteryCol].Width;
            requiredWidth += dataGridView1.Columns[EnableFileSwapperCol].Width;
            requiredWidth += dataGridView1.Columns[ConfigureCol].Width;
            requiredWidth += dataGridView1.RowHeadersWidth;
            requiredWidth += 20; //can't figure out what makes up the rest of this space

            dataGridView1.MinimumSize = new Size(requiredWidth, dataGridView1.MaximumSize.Height);

            int remainingWidth = dataGridView1.Width - requiredWidth;

            int desiredColumnWidth_AppName = GetDesiredColumnWidth(dataGridView1, AppNameCol);
            int desiredColumnWidth_AppPath = GetDesiredColumnWidth(dataGridView1, AppPathCol);            

            if (remainingWidth >= desiredColumnWidth_AppName + desiredColumnWidth_AppPath)
            {
                dataGridView1.Columns[AppNameCol].Width = desiredColumnWidth_AppName;
                dataGridView1.Columns[AppPathCol].Width = desiredColumnWidth_AppPath;
            } else
            {
                int columnWidthCalculated_AppName = (int)Math.Round(remainingWidth * 0.2);
                dataGridView1.Columns[AppNameCol].Width = columnWidthCalculated_AppName;

                int columnWidthCalculated_AppPath = (int)Math.Round(remainingWidth * 0.8);
                dataGridView1.Columns[AppPathCol].Width = columnWidthCalculated_AppPath;
            }
            
        }

        int GetDesiredColumnWidth(DataGridView dataGridView, int columnIndex)
        {
            int max = 0;
            foreach(DataGridViewRow row in dataGridView.Rows)
            {
                string text = row.Cells[columnIndex].Value.ToString();
                Size textSize = TextRenderer.MeasureText(text, dataGridView.Font);

                if(textSize.Width > max) max = textSize.Width;
                
            }
            return max;
        }
    }

    class GPUPreference(string displayText, int actualValue)
    {
        public string DisplayText { get; init; } = displayText;
        public int ActualValue { get; init; } = actualValue;
    }
}
