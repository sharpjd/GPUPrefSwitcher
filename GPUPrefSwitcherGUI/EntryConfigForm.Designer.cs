namespace GPUPrefSwitcherGUI
{
    partial class EntryConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntryConfigForm));
            dataGridView1 = new System.Windows.Forms.DataGridView();
            SwapPathColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            RemoveSwapPathColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            EnabledCheckbox = new System.Windows.Forms.CheckBox();
            EnableFileSwitcherCheckbox = new System.Windows.Forms.CheckBox();
            SaveButton = new System.Windows.Forms.Button();
            RevertButton = new System.Windows.Forms.Button();
            AddSwapPathButton = new System.Windows.Forms.Button();
            AppPathLabel = new System.Windows.Forms.Label();
            AppNameLabel = new System.Windows.Forms.Label();
            OnBatteryComboBox = new System.Windows.Forms.ComboBox();
            PluggedInComboBox = new System.Windows.Forms.ComboBox();
            OnBatteryLabel = new System.Windows.Forms.Label();
            PluggedInLabel = new System.Windows.Forms.Label();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            RunPluggingInTextBox = new System.Windows.Forms.TextBox();
            RunPluggingOutTextBox = new System.Windows.Forms.TextBox();
            BrowsePluggingOutButton = new System.Windows.Forms.Button();
            BrowsePluggingInButton = new System.Windows.Forms.Button();
            PendAddToRegistryCheckbox = new System.Windows.Forms.CheckBox();
            BrowseStorageButton = new System.Windows.Forms.Button();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { SwapPathColumn, RemoveSwapPathColumn });
            dataGridView1.Location = new System.Drawing.Point(10, 305);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 24;
            dataGridView1.Size = new System.Drawing.Size(491, 141);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // SwapPathColumn
            // 
            SwapPathColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            SwapPathColumn.HeaderText = "Path";
            SwapPathColumn.MinimumWidth = 6;
            SwapPathColumn.Name = "SwapPathColumn";
            SwapPathColumn.ReadOnly = true;
            SwapPathColumn.Width = 21;
            // 
            // RemoveSwapPathColumn
            // 
            RemoveSwapPathColumn.HeaderText = "";
            RemoveSwapPathColumn.MinimumWidth = 6;
            RemoveSwapPathColumn.Name = "RemoveSwapPathColumn";
            RemoveSwapPathColumn.Text = "";
            RemoveSwapPathColumn.Width = 70;
            // 
            // EnabledCheckbox
            // 
            EnabledCheckbox.AutoSize = true;
            EnabledCheckbox.Location = new System.Drawing.Point(13, 83);
            EnabledCheckbox.Name = "EnabledCheckbox";
            EnabledCheckbox.Size = new System.Drawing.Size(201, 19);
            EnabledCheckbox.TabIndex = 1;
            EnabledCheckbox.Text = "Enable GPU Preference Switching";
            toolTip1.SetToolTip(EnabledCheckbox, "Whether to change the GPUPreference, write this \r\nAppEntry to the Registry, or perform any \r\nFile Swaper operations. Disabling this will also disable\r\nthe File Swapper functionality.");
            EnabledCheckbox.UseVisualStyleBackColor = true;
            EnabledCheckbox.CheckedChanged += EnabledGPUPrefSwitch_CheckedChanged;
            // 
            // EnableFileSwitcherCheckbox
            // 
            EnableFileSwitcherCheckbox.AutoSize = true;
            EnableFileSwitcherCheckbox.Location = new System.Drawing.Point(10, 279);
            EnableFileSwitcherCheckbox.Name = "EnableFileSwitcherCheckbox";
            EnableFileSwitcherCheckbox.Size = new System.Drawing.Size(130, 19);
            EnableFileSwitcherCheckbox.TabIndex = 2;
            EnableFileSwitcherCheckbox.Text = "Enable File Swapper";
            toolTip1.SetToolTip(EnableFileSwitcherCheckbox, resources.GetString("EnableFileSwitcherCheckbox.ToolTip"));
            EnableFileSwitcherCheckbox.UseVisualStyleBackColor = true;
            EnableFileSwitcherCheckbox.CheckedChanged += EnableFileSwitcherCheckbox_CheckedChanged;
            // 
            // SaveButton
            // 
            SaveButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            SaveButton.Enabled = false;
            SaveButton.Location = new System.Drawing.Point(350, 466);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new System.Drawing.Size(66, 22);
            SaveButton.TabIndex = 3;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // RevertButton
            // 
            RevertButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            RevertButton.Enabled = false;
            RevertButton.Location = new System.Drawing.Point(421, 466);
            RevertButton.Name = "RevertButton";
            RevertButton.Size = new System.Drawing.Size(66, 22);
            RevertButton.TabIndex = 4;
            RevertButton.Text = "Revert";
            RevertButton.UseVisualStyleBackColor = true;
            RevertButton.Click += RevertButton_Click;
            // 
            // AddSwapPathButton
            // 
            AddSwapPathButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            AddSwapPathButton.Location = new System.Drawing.Point(10, 456);
            AddSwapPathButton.Name = "AddSwapPathButton";
            AddSwapPathButton.Size = new System.Drawing.Size(135, 32);
            AddSwapPathButton.TabIndex = 5;
            AddSwapPathButton.Text = "Add SwapPath";
            AddSwapPathButton.UseVisualStyleBackColor = true;
            AddSwapPathButton.Click += AddSwapPathButton_Click;
            // 
            // AppPathLabel
            // 
            AppPathLabel.AutoSize = true;
            AppPathLabel.Location = new System.Drawing.Point(10, 8);
            AppPathLabel.Name = "AppPathLabel";
            AppPathLabel.Size = new System.Drawing.Size(69, 15);
            AppPathLabel.TabIndex = 6;
            AppPathLabel.Text = "<AppPath>";
            // 
            // AppNameLabel
            // 
            AppNameLabel.AutoSize = true;
            AppNameLabel.Location = new System.Drawing.Point(60, 46);
            AppNameLabel.Name = "AppNameLabel";
            AppNameLabel.Size = new System.Drawing.Size(48, 15);
            AppNameLabel.TabIndex = 7;
            AppNameLabel.Text = "<Alias>";
            // 
            // OnBatteryComboBox
            // 
            OnBatteryComboBox.FormattingEnabled = true;
            OnBatteryComboBox.Location = new System.Drawing.Point(216, 136);
            OnBatteryComboBox.Name = "OnBatteryComboBox";
            OnBatteryComboBox.Size = new System.Drawing.Size(131, 23);
            OnBatteryComboBox.TabIndex = 8;
            OnBatteryComboBox.SelectedIndexChanged += OnBatteryComboBox_SelectedIndexChanged;
            // 
            // PluggedInComboBox
            // 
            PluggedInComboBox.FormattingEnabled = true;
            PluggedInComboBox.Location = new System.Drawing.Point(216, 108);
            PluggedInComboBox.Name = "PluggedInComboBox";
            PluggedInComboBox.Size = new System.Drawing.Size(131, 23);
            PluggedInComboBox.TabIndex = 9;
            PluggedInComboBox.SelectedIndexChanged += PluggedInComboBox_SelectedIndexChanged;
            // 
            // OnBatteryLabel
            // 
            OnBatteryLabel.AutoSize = true;
            OnBatteryLabel.Location = new System.Drawing.Point(10, 139);
            OnBatteryLabel.Name = "OnBatteryLabel";
            OnBatteryLabel.Size = new System.Drawing.Size(156, 15);
            OnBatteryLabel.TabIndex = 10;
            OnBatteryLabel.Text = "GPU Preference (On Battery)";
            // 
            // PluggedInLabel
            // 
            PluggedInLabel.AutoSize = true;
            PluggedInLabel.Location = new System.Drawing.Point(10, 111);
            PluggedInLabel.Name = "PluggedInLabel";
            PluggedInLabel.Size = new System.Drawing.Size(157, 15);
            PluggedInLabel.TabIndex = 11;
            PluggedInLabel.Text = "GPU Preference (Plugged In)";
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new System.Drawing.Point(10, 31);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(44, 46);
            pictureBox1.TabIndex = 12;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(13, 170);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(159, 15);
            label1.TabIndex = 13;
            label1.Text = "File to run when plugging in:";
            label1.Visible = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(13, 198);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(167, 15);
            label2.TabIndex = 14;
            label2.Text = "File to run when plugging out:";
            label2.Visible = false;
            // 
            // RunPluggingInTextBox
            // 
            RunPluggingInTextBox.Location = new System.Drawing.Point(216, 168);
            RunPluggingInTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            RunPluggingInTextBox.Name = "RunPluggingInTextBox";
            RunPluggingInTextBox.Size = new System.Drawing.Size(203, 23);
            RunPluggingInTextBox.TabIndex = 15;
            RunPluggingInTextBox.Visible = false;
            // 
            // RunPluggingOutTextBox
            // 
            RunPluggingOutTextBox.Location = new System.Drawing.Point(216, 196);
            RunPluggingOutTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            RunPluggingOutTextBox.Name = "RunPluggingOutTextBox";
            RunPluggingOutTextBox.Size = new System.Drawing.Size(203, 23);
            RunPluggingOutTextBox.TabIndex = 16;
            RunPluggingOutTextBox.Visible = false;
            // 
            // BrowsePluggingOutButton
            // 
            BrowsePluggingOutButton.Location = new System.Drawing.Point(424, 194);
            BrowsePluggingOutButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            BrowsePluggingOutButton.Name = "BrowsePluggingOutButton";
            BrowsePluggingOutButton.Size = new System.Drawing.Size(66, 22);
            BrowsePluggingOutButton.TabIndex = 17;
            BrowsePluggingOutButton.Text = "Browse";
            BrowsePluggingOutButton.UseVisualStyleBackColor = true;
            BrowsePluggingOutButton.Visible = false;
            BrowsePluggingOutButton.Click += BrowsePluggingOutButton_Click;
            // 
            // BrowsePluggingInButton
            // 
            BrowsePluggingInButton.Location = new System.Drawing.Point(424, 168);
            BrowsePluggingInButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            BrowsePluggingInButton.Name = "BrowsePluggingInButton";
            BrowsePluggingInButton.Size = new System.Drawing.Size(66, 22);
            BrowsePluggingInButton.TabIndex = 18;
            BrowsePluggingInButton.Text = "Browse";
            BrowsePluggingInButton.UseVisualStyleBackColor = true;
            BrowsePluggingInButton.Visible = false;
            BrowsePluggingInButton.Click += BrowsePluggingInButton_Click;
            // 
            // PendAddToRegistryCheckbox
            // 
            PendAddToRegistryCheckbox.AutoSize = true;
            PendAddToRegistryCheckbox.Location = new System.Drawing.Point(10, 254);
            PendAddToRegistryCheckbox.Name = "PendAddToRegistryCheckbox";
            PendAddToRegistryCheckbox.Size = new System.Drawing.Size(408, 19);
            PendAddToRegistryCheckbox.TabIndex = 19;
            PendAddToRegistryCheckbox.Text = "Pend add to Registry (e.g. if this entry's path is missing from the registry)";
            toolTip1.SetToolTip(PendAddToRegistryCheckbox, resources.GetString("PendAddToRegistryCheckbox.ToolTip"));
            PendAddToRegistryCheckbox.UseVisualStyleBackColor = true;
            PendAddToRegistryCheckbox.CheckedChanged += PendAddToRegistryCheckbox_CheckedChanged;
            // 
            // BrowseStorageButton
            // 
            BrowseStorageButton.Enabled = false;
            BrowseStorageButton.Location = new System.Drawing.Point(146, 276);
            BrowseStorageButton.Name = "BrowseStorageButton";
            BrowseStorageButton.Size = new System.Drawing.Size(175, 23);
            BrowseStorageButton.TabIndex = 20;
            BrowseStorageButton.Text = "Browse Stored File Directory";
            toolTip1.SetToolTip(BrowseStorageButton, "This is the directory that the app uses to internally\r\nstore the file states for plugged in/on battery for\r\nthis particular App Entry.");
            BrowseStorageButton.UseVisualStyleBackColor = true;
            BrowseStorageButton.Click += BrowseStorageButton_Click;
            // 
            // EntryConfigForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(510, 500);
            Controls.Add(dataGridView1);
            Controls.Add(BrowseStorageButton);
            Controls.Add(PendAddToRegistryCheckbox);
            Controls.Add(BrowsePluggingInButton);
            Controls.Add(BrowsePluggingOutButton);
            Controls.Add(RunPluggingOutTextBox);
            Controls.Add(RunPluggingInTextBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Controls.Add(PluggedInLabel);
            Controls.Add(OnBatteryLabel);
            Controls.Add(PluggedInComboBox);
            Controls.Add(OnBatteryComboBox);
            Controls.Add(AppNameLabel);
            Controls.Add(AppPathLabel);
            Controls.Add(AddSwapPathButton);
            Controls.Add(RevertButton);
            Controls.Add(SaveButton);
            Controls.Add(EnableFileSwitcherCheckbox);
            Controls.Add(EnabledCheckbox);
            MinimumSize = new System.Drawing.Size(369, 456);
            Name = "EntryConfigForm";
            Text = "EntryConfigForm";
            Load += EntryConfigForm_Load;
            Resize += EntryConfigForm_Resize;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.CheckBox EnabledCheckbox;
        private System.Windows.Forms.CheckBox EnableFileSwitcherCheckbox;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button RevertButton;
        private System.Windows.Forms.Button AddSwapPathButton;
        private System.Windows.Forms.Label AppPathLabel;
        private System.Windows.Forms.Label AppNameLabel;
        private System.Windows.Forms.ComboBox OnBatteryComboBox;
        private System.Windows.Forms.ComboBox PluggedInComboBox;
        private System.Windows.Forms.Label OnBatteryLabel;
        private System.Windows.Forms.Label PluggedInLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox RunPluggingInTextBox;
        private System.Windows.Forms.TextBox RunPluggingOutTextBox;
        private System.Windows.Forms.Button BrowsePluggingOutButton;
        private System.Windows.Forms.Button BrowsePluggingInButton;
        private System.Windows.Forms.CheckBox PendAddToRegistryCheckbox;
        private System.Windows.Forms.Button BrowseStorageButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn SwapPathColumn;
        private System.Windows.Forms.DataGridViewButtonColumn RemoveSwapPathColumn;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}