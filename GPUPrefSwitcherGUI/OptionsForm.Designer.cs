namespace GPUPrefSwitcherGUI
{
    partial class OptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            UpdateIntervalTextbox = new System.Windows.Forms.TextBox();
            SpoofPowerStateEnabledCheckbox = new System.Windows.Forms.CheckBox();
            SpoofPowerStateComboBox = new System.Windows.Forms.ComboBox();
            CommitChangesButton = new System.Windows.Forms.Button();
            SaveButton = new System.Windows.Forms.Button();
            RevertButton = new System.Windows.Forms.Button();
            EnableTipsCheckBox = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            ResetAppPreferencesListButton = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            ResetAppOptionsButton = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            RunOnBatteryCheckBox = new System.Windows.Forms.CheckBox();
            RunTaskPluggedInCheckbox = new System.Windows.Forms.CheckBox();
            CleanSettingsBankButton = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            RealTimeLoggingCheckbox = new System.Windows.Forms.CheckBox();
            FolderStatsLabel1 = new System.Windows.Forms.Label();
            FolderStatsLabel2 = new System.Windows.Forms.Label();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            OpenTaskSchedulerButton = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // UpdateIntervalTextbox
            // 
            UpdateIntervalTextbox.Location = new System.Drawing.Point(138, 10);
            UpdateIntervalTextbox.Name = "UpdateIntervalTextbox";
            UpdateIntervalTextbox.Size = new System.Drawing.Size(106, 23);
            UpdateIntervalTextbox.TabIndex = 0;
            toolTip1.SetToolTip(UpdateIntervalTextbox, "How often the Service checks whether the computer");
            UpdateIntervalTextbox.TextChanged += UpdateIntervalTextbox_TextChanged;
            UpdateIntervalTextbox.Validating += UpdateIntervalTextbox_Validating;
            UpdateIntervalTextbox.Validated += UpdateIntervalTextbox_Validated;
            // 
            // SpoofPowerStateEnabledCheckbox
            // 
            SpoofPowerStateEnabledCheckbox.AutoSize = true;
            SpoofPowerStateEnabledCheckbox.Location = new System.Drawing.Point(10, 38);
            SpoofPowerStateEnabledCheckbox.Name = "SpoofPowerStateEnabledCheckbox";
            SpoofPowerStateEnabledCheckbox.Size = new System.Drawing.Size(122, 19);
            SpoofPowerStateEnabledCheckbox.TabIndex = 3;
            SpoofPowerStateEnabledCheckbox.Text = "Spoof Power State";
            toolTip1.SetToolTip(SpoofPowerStateEnabledCheckbox, resources.GetString("SpoofPowerStateEnabledCheckbox.ToolTip"));
            SpoofPowerStateEnabledCheckbox.UseVisualStyleBackColor = true;
            SpoofPowerStateEnabledCheckbox.CheckedChanged += SpoofPowerStateEnabledCheckbox_CheckedChanged;
            // 
            // SpoofPowerStateComboBox
            // 
            SpoofPowerStateComboBox.Enabled = false;
            SpoofPowerStateComboBox.FormattingEnabled = true;
            SpoofPowerStateComboBox.Location = new System.Drawing.Point(138, 36);
            SpoofPowerStateComboBox.Name = "SpoofPowerStateComboBox";
            SpoofPowerStateComboBox.Size = new System.Drawing.Size(106, 23);
            SpoofPowerStateComboBox.TabIndex = 4;
            SpoofPowerStateComboBox.SelectedIndexChanged += SpoofPowerStateComboBox_SelectedIndexChanged;
            // 
            // CommitChangesButton
            // 
            CommitChangesButton.Enabled = false;
            CommitChangesButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            CommitChangesButton.Location = new System.Drawing.Point(10, 290);
            CommitChangesButton.Name = "CommitChangesButton";
            CommitChangesButton.Size = new System.Drawing.Size(290, 28);
            CommitChangesButton.TabIndex = 5;
            CommitChangesButton.Text = "Restart service to enact changes";
            CommitChangesButton.UseVisualStyleBackColor = true;
            CommitChangesButton.Click += CommitChangesButton_Click;
            // 
            // SaveButton
            // 
            SaveButton.Enabled = false;
            SaveButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            SaveButton.Location = new System.Drawing.Point(233, 234);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new System.Drawing.Size(66, 22);
            SaveButton.TabIndex = 6;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click_1;
            // 
            // RevertButton
            // 
            RevertButton.Enabled = false;
            RevertButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            RevertButton.Location = new System.Drawing.Point(233, 262);
            RevertButton.Name = "RevertButton";
            RevertButton.Size = new System.Drawing.Size(66, 22);
            RevertButton.TabIndex = 7;
            RevertButton.Text = "Revert";
            RevertButton.UseVisualStyleBackColor = true;
            RevertButton.Click += RevertButton_Click_1;
            // 
            // EnableTipsCheckBox
            // 
            EnableTipsCheckBox.AutoSize = true;
            EnableTipsCheckBox.Location = new System.Drawing.Point(10, 62);
            EnableTipsCheckBox.Name = "EnableTipsCheckBox";
            EnableTipsCheckBox.Size = new System.Drawing.Size(79, 19);
            EnableTipsCheckBox.TabIndex = 8;
            EnableTipsCheckBox.Text = "Show Tips";
            toolTip1.SetToolTip(EnableTipsCheckBox, "Whether to display the tips box on the main \r\nconfiguration screen. This may require you to close\r\nand reopen the GUI.");
            EnableTipsCheckBox.UseVisualStyleBackColor = true;
            EnableTipsCheckBox.CheckedChanged += EnableTipsCheckBox_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 12);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(114, 15);
            label1.TabIndex = 9;
            label1.Text = "Update Interval (ms)";
            toolTip1.SetToolTip(label1, resources.GetString("label1.ToolTip"));
            // 
            // ResetAppPreferencesListButton
            // 
            ResetAppPreferencesListButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            ResetAppPreferencesListButton.Location = new System.Drawing.Point(10, 437);
            ResetAppPreferencesListButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            ResetAppPreferencesListButton.Name = "ResetAppPreferencesListButton";
            ResetAppPreferencesListButton.Size = new System.Drawing.Size(289, 22);
            ResetAppPreferencesListButton.TabIndex = 10;
            ResetAppPreferencesListButton.Text = "⚠Backup/Reset App Preferences List⚠";
            toolTip1.SetToolTip(ResetAppPreferencesListButton, resources.GetString("ResetAppPreferencesListButton.ToolTip"));
            ResetAppPreferencesListButton.UseVisualStyleBackColor = true;
            ResetAppPreferencesListButton.Click += ResetAppPreferencesListButton_Click;
            // 
            // label2
            // 
            label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            label2.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            label2.Location = new System.Drawing.Point(10, 330);
            label2.Name = "label2";
            label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            label2.Size = new System.Drawing.Size(290, 2);
            label2.TabIndex = 11;
            // 
            // ResetAppOptionsButton
            // 
            ResetAppOptionsButton.Location = new System.Drawing.Point(110, 133);
            ResetAppOptionsButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            ResetAppOptionsButton.Name = "ResetAppOptionsButton";
            ResetAppOptionsButton.Size = new System.Drawing.Size(90, 22);
            ResetAppOptionsButton.TabIndex = 12;
            ResetAppOptionsButton.Text = "Use Defaults";
            ResetAppOptionsButton.UseVisualStyleBackColor = true;
            ResetAppOptionsButton.Click += ResetAppOptionsButton_Click;
            // 
            // label3
            // 
            label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            label3.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            label3.Location = new System.Drawing.Point(10, 171);
            label3.Name = "label3";
            label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            label3.Size = new System.Drawing.Size(290, 2);
            label3.TabIndex = 13;
            label3.Click += label3_Click;
            // 
            // RunOnBatteryCheckBox
            // 
            RunOnBatteryCheckBox.AutoSize = true;
            RunOnBatteryCheckBox.Location = new System.Drawing.Point(10, 176);
            RunOnBatteryCheckBox.Name = "RunOnBatteryCheckBox";
            RunOnBatteryCheckBox.Size = new System.Drawing.Size(253, 19);
            RunOnBatteryCheckBox.TabIndex = 14;
            RunOnBatteryCheckBox.Text = "Run Task Scheduler for switching to battery";
            toolTip1.SetToolTip(RunOnBatteryCheckBox, resources.GetString("RunOnBatteryCheckBox.ToolTip"));
            RunOnBatteryCheckBox.UseVisualStyleBackColor = true;
            RunOnBatteryCheckBox.CheckedChanged += RunOnBatteryCheckBox_CheckedChanged;
            // 
            // RunTaskPluggedInCheckbox
            // 
            RunTaskPluggedInCheckbox.AutoSize = true;
            RunTaskPluggedInCheckbox.Location = new System.Drawing.Point(10, 201);
            RunTaskPluggedInCheckbox.Name = "RunTaskPluggedInCheckbox";
            RunTaskPluggedInCheckbox.Size = new System.Drawing.Size(303, 19);
            RunTaskPluggedInCheckbox.TabIndex = 15;
            RunTaskPluggedInCheckbox.Text = "Run Task Scheduler entry for switching to plugged in";
            toolTip1.SetToolTip(RunTaskPluggedInCheckbox, resources.GetString("RunTaskPluggedInCheckbox.ToolTip"));
            RunTaskPluggedInCheckbox.UseVisualStyleBackColor = true;
            RunTaskPluggedInCheckbox.CheckedChanged += RunTaskPluggedInCheckbox_CheckedChanged;
            // 
            // CleanSettingsBankButton
            // 
            CleanSettingsBankButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            CleanSettingsBankButton.Location = new System.Drawing.Point(10, 343);
            CleanSettingsBankButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            CleanSettingsBankButton.Name = "CleanSettingsBankButton";
            CleanSettingsBankButton.Size = new System.Drawing.Size(289, 26);
            CleanSettingsBankButton.TabIndex = 16;
            CleanSettingsBankButton.Text = "Clean/Backup FileSwapper storage folder";
            toolTip1.SetToolTip(CleanSettingsBankButton, resources.GetString("CleanSettingsBankButton.ToolTip"));
            CleanSettingsBankButton.UseVisualStyleBackColor = true;
            CleanSettingsBankButton.Click += CleanSettingsBankButton_Click;
            // 
            // label4
            // 
            label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            label4.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            label4.Location = new System.Drawing.Point(10, 422);
            label4.Name = "label4";
            label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            label4.Size = new System.Drawing.Size(290, 2);
            label4.TabIndex = 17;
            // 
            // RealTimeLoggingCheckbox
            // 
            RealTimeLoggingCheckbox.AutoSize = true;
            RealTimeLoggingCheckbox.Location = new System.Drawing.Point(10, 87);
            RealTimeLoggingCheckbox.Name = "RealTimeLoggingCheckbox";
            RealTimeLoggingCheckbox.Size = new System.Drawing.Size(200, 19);
            RealTimeLoggingCheckbox.TabIndex = 18;
            RealTimeLoggingCheckbox.Text = "Enable realtime standard logging";
            toolTip1.SetToolTip(RealTimeLoggingCheckbox, resources.GetString("RealTimeLoggingCheckbox.ToolTip"));
            RealTimeLoggingCheckbox.UseVisualStyleBackColor = true;
            RealTimeLoggingCheckbox.CheckedChanged += RealTimeLoggingCheckbox_CheckedChanged;
            // 
            // FolderStatsLabel1
            // 
            FolderStatsLabel1.AutoSize = true;
            FolderStatsLabel1.Location = new System.Drawing.Point(7, 397);
            FolderStatsLabel1.Name = "FolderStatsLabel1";
            FolderStatsLabel1.Size = new System.Drawing.Size(38, 15);
            FolderStatsLabel1.TabIndex = 19;
            FolderStatsLabel1.Text = "label5";
            // 
            // FolderStatsLabel2
            // 
            FolderStatsLabel2.AutoSize = true;
            FolderStatsLabel2.Location = new System.Drawing.Point(7, 377);
            FolderStatsLabel2.Name = "FolderStatsLabel2";
            FolderStatsLabel2.Size = new System.Drawing.Size(38, 15);
            FolderStatsLabel2.TabIndex = 20;
            FolderStatsLabel2.Text = "label6";
            // 
            // OpenTaskSchedulerButton
            // 
            OpenTaskSchedulerButton.Location = new System.Drawing.Point(7, 226);
            OpenTaskSchedulerButton.Name = "OpenTaskSchedulerButton";
            OpenTaskSchedulerButton.Size = new System.Drawing.Size(153, 23);
            OpenTaskSchedulerButton.TabIndex = 21;
            OpenTaskSchedulerButton.Text = "Open Task Scheduler";
            OpenTaskSchedulerButton.UseVisualStyleBackColor = true;
            OpenTaskSchedulerButton.Click += OpenTaskSchedulerButton_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(110, 255);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(75, 23);
            button1.TabIndex = 22;
            button1.Text = "DEBUG";
            button1.UseVisualStyleBackColor = true;
            button1.Visible = false;
            button1.Click += button1_Click;
            // 
            // OptionsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(311, 477);
            Controls.Add(button1);
            Controls.Add(OpenTaskSchedulerButton);
            Controls.Add(FolderStatsLabel2);
            Controls.Add(FolderStatsLabel1);
            Controls.Add(RealTimeLoggingCheckbox);
            Controls.Add(label4);
            Controls.Add(CleanSettingsBankButton);
            Controls.Add(RunTaskPluggedInCheckbox);
            Controls.Add(RunOnBatteryCheckBox);
            Controls.Add(label3);
            Controls.Add(ResetAppOptionsButton);
            Controls.Add(label2);
            Controls.Add(ResetAppPreferencesListButton);
            Controls.Add(label1);
            Controls.Add(EnableTipsCheckBox);
            Controls.Add(RevertButton);
            Controls.Add(SaveButton);
            Controls.Add(CommitChangesButton);
            Controls.Add(SpoofPowerStateComboBox);
            Controls.Add(SpoofPowerStateEnabledCheckbox);
            Controls.Add(UpdateIntervalTextbox);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Name = "OptionsForm";
            Text = "Options";
            Load += OptionsForm_Load;
            Leave += OptionsForm_Leave;
            MouseClick += OptionsForm_MouseClick;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox UpdateIntervalTextbox;
        private System.Windows.Forms.CheckBox SpoofPowerStateEnabledCheckbox;
        private System.Windows.Forms.ComboBox SpoofPowerStateComboBox;
        private System.Windows.Forms.Button CommitChangesButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button RevertButton;
        private System.Windows.Forms.CheckBox EnableTipsCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ResetAppPreferencesListButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ResetAppOptionsButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox RunOnBatteryCheckBox;
        private System.Windows.Forms.CheckBox RunTaskPluggedInCheckbox;
        private System.Windows.Forms.Button CleanSettingsBankButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox RealTimeLoggingCheckbox;
        private System.Windows.Forms.Label FolderStatsLabel1;
        private System.Windows.Forms.Label FolderStatsLabel2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button OpenTaskSchedulerButton;
        private System.Windows.Forms.Button button1;
    }
}