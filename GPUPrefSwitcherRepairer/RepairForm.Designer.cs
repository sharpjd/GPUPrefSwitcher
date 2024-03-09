namespace GPUPrefSwitcherRepairer
{
    partial class RepairForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepairForm));
            OverwritePreferencesXMLButton = new Button();
            OverwriteAppOptionsButton = new Button();
            richTextBox1 = new RichTextBox();
            OpenErrorLogButton = new Button();
            OpenStandardLogButton = new Button();
            OpenAppDirectoryButton = new Button();
            RestartServiceButton = new Button();
            toolTip1 = new ToolTip(components);
            SuspendLayout();
            // 
            // OverwritePreferencesXMLButton
            // 
            OverwritePreferencesXMLButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            OverwritePreferencesXMLButton.Location = new Point(21, 567);
            OverwritePreferencesXMLButton.Margin = new Padding(3, 2, 3, 2);
            OverwritePreferencesXMLButton.Name = "OverwritePreferencesXMLButton";
            OverwritePreferencesXMLButton.Size = new Size(493, 22);
            OverwritePreferencesXMLButton.TabIndex = 0;
            OverwritePreferencesXMLButton.Text = "⚠Backup/Overwrite AppEntry Preferences file (Preferences.xml) with default⚠";
            toolTip1.SetToolTip(OverwritePreferencesXMLButton, resources.GetString("OverwritePreferencesXMLButton.ToolTip"));
            OverwritePreferencesXMLButton.UseVisualStyleBackColor = true;
            OverwritePreferencesXMLButton.Click += OverwritePreferencesButton_Click;
            // 
            // OverwriteAppOptionsButton
            // 
            OverwriteAppOptionsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            OverwriteAppOptionsButton.Location = new Point(21, 541);
            OverwriteAppOptionsButton.Margin = new Padding(3, 2, 3, 2);
            OverwriteAppOptionsButton.Name = "OverwriteAppOptionsButton";
            OverwriteAppOptionsButton.Size = new Size(493, 22);
            OverwriteAppOptionsButton.TabIndex = 1;
            OverwriteAppOptionsButton.Text = "Overwrite service/app options file (AppOptions.xml) with default ";
            toolTip1.SetToolTip(OverwriteAppOptionsButton, resources.GetString("OverwriteAppOptionsButton.ToolTip"));
            OverwriteAppOptionsButton.UseVisualStyleBackColor = true;
            OverwriteAppOptionsButton.Click += OverwriteAppOptionsButton_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(10, 9);
            richTextBox1.Margin = new Padding(3, 2, 3, 2);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(1024, 418);
            richTextBox1.TabIndex = 2;
            richTextBox1.Text = "";
            richTextBox1.TextChanged += richTextBox1_TextChanged;
            // 
            // OpenErrorLogButton
            // 
            OpenErrorLogButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            OpenErrorLogButton.Location = new Point(21, 431);
            OpenErrorLogButton.Margin = new Padding(3, 2, 3, 2);
            OpenErrorLogButton.Name = "OpenErrorLogButton";
            OpenErrorLogButton.Size = new Size(248, 22);
            OpenErrorLogButton.TabIndex = 3;
            OpenErrorLogButton.Text = "Open Error Log File (displayed above)";
            OpenErrorLogButton.UseVisualStyleBackColor = true;
            OpenErrorLogButton.Click += OpenErrorLogButton_Click;
            // 
            // OpenStandardLogButton
            // 
            OpenStandardLogButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            OpenStandardLogButton.Location = new Point(274, 431);
            OpenStandardLogButton.Margin = new Padding(3, 2, 3, 2);
            OpenStandardLogButton.Name = "OpenStandardLogButton";
            OpenStandardLogButton.Size = new Size(240, 22);
            OpenStandardLogButton.TabIndex = 4;
            OpenStandardLogButton.Text = "Open Standard Log (with text editor)";
            OpenStandardLogButton.UseVisualStyleBackColor = true;
            OpenStandardLogButton.Click += OpenStandardLogButton_Click;
            // 
            // OpenAppDirectoryButton
            // 
            OpenAppDirectoryButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            OpenAppDirectoryButton.Location = new Point(21, 458);
            OpenAppDirectoryButton.Margin = new Padding(3, 2, 3, 2);
            OpenAppDirectoryButton.Name = "OpenAppDirectoryButton";
            OpenAppDirectoryButton.Size = new Size(152, 22);
            OpenAppDirectoryButton.TabIndex = 5;
            OpenAppDirectoryButton.Text = "Open App Directory";
            OpenAppDirectoryButton.UseVisualStyleBackColor = true;
            OpenAppDirectoryButton.Click += OpenAppDirectoryButton_Click;
            // 
            // RestartServiceButton
            // 
            RestartServiceButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            RestartServiceButton.Location = new Point(21, 484);
            RestartServiceButton.Margin = new Padding(3, 2, 3, 2);
            RestartServiceButton.Name = "RestartServiceButton";
            RestartServiceButton.Size = new Size(152, 22);
            RestartServiceButton.TabIndex = 6;
            RestartServiceButton.Text = "Restart Service";
            RestartServiceButton.UseVisualStyleBackColor = true;
            RestartServiceButton.Click += RestartServiceButton_Click;
            // 
            // RepairForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1046, 598);
            Controls.Add(RestartServiceButton);
            Controls.Add(OpenAppDirectoryButton);
            Controls.Add(OpenStandardLogButton);
            Controls.Add(OpenErrorLogButton);
            Controls.Add(richTextBox1);
            Controls.Add(OverwriteAppOptionsButton);
            Controls.Add(OverwritePreferencesXMLButton);
            Margin = new Padding(3, 2, 3, 2);
            MinimumSize = new Size(556, 486);
            Name = "RepairForm";
            Text = "Repair";
            Load += RepairForm_Load;
            Resize += RepairForm_Resize;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button OverwritePreferencesXMLButton;
        private System.Windows.Forms.Button OverwriteAppOptionsButton;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button OpenErrorLogButton;
        private System.Windows.Forms.Button OpenStandardLogButton;
        private System.Windows.Forms.Button OpenAppDirectoryButton;
        private System.Windows.Forms.Button RestartServiceButton;
        private ToolTip toolTip1;
    }
}