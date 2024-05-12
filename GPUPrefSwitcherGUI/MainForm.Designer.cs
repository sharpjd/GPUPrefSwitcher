namespace GPUPrefSwitcherGUI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            dataGridView1 = new System.Windows.Forms.DataGridView();
            AppIconColumn = new System.Windows.Forms.DataGridViewImageColumn();
            AppName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            AppPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            EnableSwitcher = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            GPUPrefPluggedIn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            GPUPrefOnBattery = new System.Windows.Forms.DataGridViewComboBoxColumn();
            EnableFileSwapper = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ConfigureColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            SaveButton = new System.Windows.Forms.Button();
            RevertChangesButton = new System.Windows.Forms.Button();
            CommitButton = new System.Windows.Forms.Button();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            AddEntryButton = new System.Windows.Forms.Button();
            RemoveAppsButton = new System.Windows.Forms.Button();
            TipRichTextBox = new System.Windows.Forms.RichTextBox();
            OptionsButton = new System.Windows.Forms.Button();
            richTextBox1 = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { AppIconColumn, AppName, AppPath, EnableSwitcher, GPUPrefPluggedIn, GPUPrefOnBattery, EnableFileSwapper, ConfigureColumn });
            dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridView1.Location = new System.Drawing.Point(0, 0);
            dataGridView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 28;
            dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new System.Drawing.Size(1189, 631);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;
            dataGridView1.RowStateChanged += dataGridView1_RowStateChanged;
            // 
            // AppIconColumn
            // 
            AppIconColumn.HeaderText = "Icon";
            AppIconColumn.MinimumWidth = 6;
            AppIconColumn.Name = "AppIconColumn";
            AppIconColumn.ReadOnly = true;
            AppIconColumn.Width = 36;
            // 
            // AppName
            // 
            AppName.FillWeight = 50F;
            AppName.HeaderText = "App Name";
            AppName.MinimumWidth = 6;
            AppName.Name = "AppName";
            AppName.ReadOnly = true;
            AppName.Width = 89;
            // 
            // AppPath
            // 
            AppPath.FillWeight = 50F;
            AppPath.HeaderText = "Path";
            AppPath.MinimumWidth = 6;
            AppPath.Name = "AppPath";
            AppPath.ReadOnly = true;
            AppPath.Width = 56;
            // 
            // EnableSwitcher
            // 
            EnableSwitcher.HeaderText = "Enabled";
            EnableSwitcher.MinimumWidth = 6;
            EnableSwitcher.Name = "EnableSwitcher";
            EnableSwitcher.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            EnableSwitcher.Width = 50;
            // 
            // GPUPrefPluggedIn
            // 
            GPUPrefPluggedIn.HeaderText = "Plugged In";
            GPUPrefPluggedIn.MinimumWidth = 6;
            GPUPrefPluggedIn.Name = "GPUPrefPluggedIn";
            GPUPrefPluggedIn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            GPUPrefPluggedIn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            GPUPrefPluggedIn.Width = 110;
            // 
            // GPUPrefOnBattery
            // 
            GPUPrefOnBattery.HeaderText = "On Battery";
            GPUPrefOnBattery.MinimumWidth = 6;
            GPUPrefOnBattery.Name = "GPUPrefOnBattery";
            GPUPrefOnBattery.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            GPUPrefOnBattery.Width = 110;
            // 
            // EnableFileSwapper
            // 
            EnableFileSwapper.HeaderText = "File Swapper";
            EnableFileSwapper.MinimumWidth = 6;
            EnableFileSwapper.Name = "EnableFileSwapper";
            EnableFileSwapper.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            EnableFileSwapper.Width = 55;
            // 
            // ConfigureColumn
            // 
            ConfigureColumn.HeaderText = "";
            ConfigureColumn.MinimumWidth = 50;
            ConfigureColumn.Name = "ConfigureColumn";
            ConfigureColumn.ReadOnly = true;
            ConfigureColumn.Width = 50;
            // 
            // SaveButton
            // 
            SaveButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            SaveButton.Enabled = false;
            SaveButton.Location = new System.Drawing.Point(1067, 9);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new System.Drawing.Size(66, 22);
            SaveButton.TabIndex = 1;
            SaveButton.Text = "Save";
            toolTip1.SetToolTip(SaveButton, "Writes the App Entry changes to the file (Preferences.xml). Changes will not take place until the service is restarted or \"Commit Changes\" is clicked.");
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // RevertChangesButton
            // 
            RevertChangesButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            RevertChangesButton.Enabled = false;
            RevertChangesButton.Location = new System.Drawing.Point(1067, 85);
            RevertChangesButton.Name = "RevertChangesButton";
            RevertChangesButton.Size = new System.Drawing.Size(66, 22);
            RevertChangesButton.TabIndex = 2;
            RevertChangesButton.Text = "Revert";
            toolTip1.SetToolTip(RevertChangesButton, "Undoes all the changes and restores the settings that\r\nwere loaded upon starting this GUI.\r\n");
            RevertChangesButton.UseVisualStyleBackColor = true;
            RevertChangesButton.Click += RevertChangesButton_Click;
            // 
            // CommitButton
            // 
            CommitButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            CommitButton.Enabled = false;
            CommitButton.Location = new System.Drawing.Point(1055, 37);
            CommitButton.Name = "CommitButton";
            CommitButton.Size = new System.Drawing.Size(92, 42);
            CommitButton.TabIndex = 3;
            CommitButton.Text = "Commit Changes";
            toolTip1.SetToolTip(CommitButton, "Update preferences based on saved settings (by the service).");
            CommitButton.UseVisualStyleBackColor = true;
            CommitButton.Click += CommitButton_Click;
            // 
            // AddEntryButton
            // 
            AddEntryButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            AddEntryButton.Location = new System.Drawing.Point(1045, 519);
            AddEntryButton.Name = "AddEntryButton";
            AddEntryButton.Size = new System.Drawing.Size(115, 26);
            AddEntryButton.TabIndex = 9;
            AddEntryButton.Text = "Add App";
            toolTip1.SetToolTip(AddEntryButton, "Browse for an executable to add as an App Entry to this list\r\nto configure. A corresponding Registry key will be added\r\nupon committing changes/restarting the service.");
            AddEntryButton.UseVisualStyleBackColor = true;
            AddEntryButton.Click += AddEntryButton_Click;
            // 
            // RemoveAppsButton
            // 
            RemoveAppsButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            RemoveAppsButton.Location = new System.Drawing.Point(1045, 550);
            RemoveAppsButton.Name = "RemoveAppsButton";
            RemoveAppsButton.Size = new System.Drawing.Size(115, 26);
            RemoveAppsButton.TabIndex = 10;
            RemoveAppsButton.Text = "Remove App(s)";
            toolTip1.SetToolTip(RemoveAppsButton, resources.GetString("RemoveAppsButton.ToolTip"));
            RemoveAppsButton.UseVisualStyleBackColor = true;
            RemoveAppsButton.Click += RemoveAppsButton_Click;
            // 
            // TipRichTextBox
            // 
            TipRichTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            TipRichTextBox.BackColor = System.Drawing.SystemColors.Control;
            TipRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            TipRichTextBox.Location = new System.Drawing.Point(1034, 183);
            TipRichTextBox.Name = "TipRichTextBox";
            TipRichTextBox.Size = new System.Drawing.Size(138, 262);
            TipRichTextBox.TabIndex = 11;
            TipRichTextBox.Text = "TipBox";
            toolTip1.SetToolTip(TipRichTextBox, "You can also click on this box for a new tip.");
            TipRichTextBox.MouseClick += TipRichTextBox_MouseClick;
            TipRichTextBox.TextChanged += TipRichTextBox_TextChanged;
            // 
            // OptionsButton
            // 
            OptionsButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            OptionsButton.Location = new System.Drawing.Point(1045, 582);
            OptionsButton.Name = "OptionsButton";
            OptionsButton.Size = new System.Drawing.Size(115, 30);
            OptionsButton.TabIndex = 4;
            OptionsButton.Text = "More Options";
            OptionsButton.UseVisualStyleBackColor = true;
            OptionsButton.Click += OptionsButton_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBox1.Location = new System.Drawing.Point(874, 79);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new System.Drawing.Size(138, 74);
            richTextBox1.TabIndex = 8;
            richTextBox1.Text = "Note: Changes will not apply until power state changes (you can spoof this Options)";
            richTextBox1.Visible = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1189, 631);
            Controls.Add(RemoveAppsButton);
            Controls.Add(AddEntryButton);
            Controls.Add(OptionsButton);
            Controls.Add(CommitButton);
            Controls.Add(RevertChangesButton);
            Controls.Add(SaveButton);
            Controls.Add(TipRichTextBox);
            Controls.Add(richTextBox1);
            Controls.Add(dataGridView1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Name = "MainForm";
            Text = "Config GUI - GPUPrefSwitcher";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            Resize += MainForm_Resize;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button RevertChangesButton;
        private System.Windows.Forms.Button CommitButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button OptionsButton;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button AddEntryButton;
        private System.Windows.Forms.Button RemoveAppsButton;
        private System.Windows.Forms.RichTextBox TipRichTextBox;
        private System.Windows.Forms.DataGridViewImageColumn AppIconColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn AppName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AppPath;
        private System.Windows.Forms.DataGridViewCheckBoxColumn EnableSwitcher;
        private System.Windows.Forms.DataGridViewComboBoxColumn GPUPrefPluggedIn;
        private System.Windows.Forms.DataGridViewComboBoxColumn GPUPrefOnBattery;
        private System.Windows.Forms.DataGridViewCheckBoxColumn EnableFileSwapper;
        private System.Windows.Forms.DataGridViewButtonColumn ConfigureColumn;
    }
}