namespace GPUPrefSwitcher
{
    partial class ConflictForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConflictForm));
            this.DescriptionRichTextBox = new System.Windows.Forms.RichTextBox();
            this.TargetFilePathRichTextbox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.TargetStateLabel = new System.Windows.Forms.Label();
            this.OnBatteryFilePathRichTextbox = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.PluggedInFilePathRichTextBox = new System.Windows.Forms.RichTextBox();
            this.OverwriteButton = new System.Windows.Forms.Button();
            this.PreserveButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.ConfirmButton = new System.Windows.Forms.Button();
            this.OpenTargetFileButton = new System.Windows.Forms.Button();
            this.OpenExistingOnBatteryFileButton = new System.Windows.Forms.Button();
            this.OpenExistingPluggedInFileButton = new System.Windows.Forms.Button();
            this.CancelAndDelayButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // DescriptionRichTextBox
            // 
            this.DescriptionRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DescriptionRichTextBox.Location = new System.Drawing.Point(12, 12);
            this.DescriptionRichTextBox.Name = "DescriptionRichTextBox";
            this.DescriptionRichTextBox.Size = new System.Drawing.Size(486, 165);
            this.DescriptionRichTextBox.TabIndex = 0;
            this.DescriptionRichTextBox.Text = resources.GetString("DescriptionRichTextBox.Text");
            this.DescriptionRichTextBox.TextChanged += new System.EventHandler(this.DescriptionRichTextBox_TextChanged);
            // 
            // TargetFilePathRichTextbox
            // 
            this.TargetFilePathRichTextbox.Location = new System.Drawing.Point(158, 224);
            this.TargetFilePathRichTextbox.Name = "TargetFilePathRichTextbox";
            this.TargetFilePathRichTextbox.Size = new System.Drawing.Size(243, 25);
            this.TargetFilePathRichTextbox.TabIndex = 1;
            this.TargetFilePathRichTextbox.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 227);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Target file:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 262);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Existing OnBattery file:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 199);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(252, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Currently trying to swap in file for this state:";
            // 
            // TargetStateLabel
            // 
            this.TargetStateLabel.AutoSize = true;
            this.TargetStateLabel.Location = new System.Drawing.Point(270, 199);
            this.TargetStateLabel.Name = "TargetStateLabel";
            this.TargetStateLabel.Size = new System.Drawing.Size(50, 16);
            this.TargetStateLabel.TabIndex = 5;
            this.TargetStateLabel.Text = "<state>";
            // 
            // OnBatteryFilePathRichTextbox
            // 
            this.OnBatteryFilePathRichTextbox.Location = new System.Drawing.Point(158, 255);
            this.OnBatteryFilePathRichTextbox.Name = "OnBatteryFilePathRichTextbox";
            this.OnBatteryFilePathRichTextbox.Size = new System.Drawing.Size(243, 25);
            this.OnBatteryFilePathRichTextbox.TabIndex = 6;
            this.OnBatteryFilePathRichTextbox.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 292);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(140, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Existing PluggedIn file:";
            // 
            // PluggedInFilePathRichTextBox
            // 
            this.PluggedInFilePathRichTextBox.Location = new System.Drawing.Point(158, 286);
            this.PluggedInFilePathRichTextBox.Name = "PluggedInFilePathRichTextBox";
            this.PluggedInFilePathRichTextBox.Size = new System.Drawing.Size(243, 25);
            this.PluggedInFilePathRichTextBox.TabIndex = 8;
            this.PluggedInFilePathRichTextBox.Text = "";
            // 
            // OverwriteButton
            // 
            this.OverwriteButton.Location = new System.Drawing.Point(11, 353);
            this.OverwriteButton.Name = "OverwriteButton";
            this.OverwriteButton.Size = new System.Drawing.Size(221, 49);
            this.OverwriteButton.TabIndex = 9;
            this.OverwriteButton.Text = "Overwrite target file with stored file";
            this.OverwriteButton.UseVisualStyleBackColor = true;
            this.OverwriteButton.Click += new System.EventHandler(this.OverwriteButton_Click);
            // 
            // PreserveButton
            // 
            this.PreserveButton.Location = new System.Drawing.Point(277, 353);
            this.PreserveButton.Name = "PreserveButton";
            this.PreserveButton.Size = new System.Drawing.Size(221, 49);
            this.PreserveButton.TabIndex = 10;
            this.PreserveButton.Text = "Keep current file and overwrite stored file";
            this.PreserveButton.UseVisualStyleBackColor = true;
            this.PreserveButton.Click += new System.EventHandler(this.PreserveButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(200, 334);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 16);
            this.label5.TabIndex = 11;
            this.label5.Text = "Select an option:";
            // 
            // ConfirmButton
            // 
            this.ConfirmButton.Enabled = false;
            this.ConfirmButton.Location = new System.Drawing.Point(423, 415);
            this.ConfirmButton.Name = "ConfirmButton";
            this.ConfirmButton.Size = new System.Drawing.Size(75, 28);
            this.ConfirmButton.TabIndex = 12;
            this.ConfirmButton.Text = "Confirm";
            this.ConfirmButton.UseVisualStyleBackColor = true;
            this.ConfirmButton.Click += new System.EventHandler(this.ConfirmButton_Click);
            // 
            // OpenTargetFileButton
            // 
            this.OpenTargetFileButton.Location = new System.Drawing.Point(407, 224);
            this.OpenTargetFileButton.Name = "OpenTargetFileButton";
            this.OpenTargetFileButton.Size = new System.Drawing.Size(75, 23);
            this.OpenTargetFileButton.TabIndex = 13;
            this.OpenTargetFileButton.Text = "Open";
            this.OpenTargetFileButton.UseVisualStyleBackColor = true;
            this.OpenTargetFileButton.Click += new System.EventHandler(this.OpenTargetFileButton_Click);
            // 
            // OpenExistingOnBatteryFileButton
            // 
            this.OpenExistingOnBatteryFileButton.Location = new System.Drawing.Point(407, 255);
            this.OpenExistingOnBatteryFileButton.Name = "OpenExistingOnBatteryFileButton";
            this.OpenExistingOnBatteryFileButton.Size = new System.Drawing.Size(75, 23);
            this.OpenExistingOnBatteryFileButton.TabIndex = 14;
            this.OpenExistingOnBatteryFileButton.Text = "Open";
            this.OpenExistingOnBatteryFileButton.UseVisualStyleBackColor = true;
            this.OpenExistingOnBatteryFileButton.Click += new System.EventHandler(this.OpenExistingOnBatteryFileButton_Click);
            // 
            // OpenExistingPluggedInFileButton
            // 
            this.OpenExistingPluggedInFileButton.Location = new System.Drawing.Point(407, 286);
            this.OpenExistingPluggedInFileButton.Name = "OpenExistingPluggedInFileButton";
            this.OpenExistingPluggedInFileButton.Size = new System.Drawing.Size(75, 23);
            this.OpenExistingPluggedInFileButton.TabIndex = 15;
            this.OpenExistingPluggedInFileButton.Text = "Open";
            this.OpenExistingPluggedInFileButton.UseVisualStyleBackColor = true;
            this.OpenExistingPluggedInFileButton.Click += new System.EventHandler(this.OpenExistingPluggedInFileButton_Click);
            // 
            // CancelAndDelayButton
            // 
            this.CancelAndDelayButton.Location = new System.Drawing.Point(12, 415);
            this.CancelAndDelayButton.Name = "CancelAndDelayButton";
            this.CancelAndDelayButton.Size = new System.Drawing.Size(165, 28);
            this.CancelAndDelayButton.TabIndex = 16;
            this.CancelAndDelayButton.Text = "Cancel and delay";
            this.CancelAndDelayButton.UseVisualStyleBackColor = true;
            this.CancelAndDelayButton.Click += new System.EventHandler(this.CancelAndDelayButton_Click);
            // 
            // ConflictForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 450);
            this.Controls.Add(this.CancelAndDelayButton);
            this.Controls.Add(this.OpenExistingPluggedInFileButton);
            this.Controls.Add(this.OpenExistingOnBatteryFileButton);
            this.Controls.Add(this.OpenTargetFileButton);
            this.Controls.Add(this.ConfirmButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.PreserveButton);
            this.Controls.Add(this.OverwriteButton);
            this.Controls.Add(this.PluggedInFilePathRichTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.OnBatteryFilePathRichTextbox);
            this.Controls.Add(this.TargetStateLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TargetFilePathRichTextbox);
            this.Controls.Add(this.DescriptionRichTextBox);
            this.Name = "ConflictForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ConflictForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox DescriptionRichTextBox;
        private System.Windows.Forms.RichTextBox TargetFilePathRichTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label TargetStateLabel;
        private System.Windows.Forms.RichTextBox OnBatteryFilePathRichTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox PluggedInFilePathRichTextBox;
        private System.Windows.Forms.Button OverwriteButton;
        private System.Windows.Forms.Button PreserveButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button ConfirmButton;
        private System.Windows.Forms.Button OpenTargetFileButton;
        private System.Windows.Forms.Button OpenExistingOnBatteryFileButton;
        private System.Windows.Forms.Button OpenExistingPluggedInFileButton;
        private System.Windows.Forms.Button CancelAndDelayButton;
    }
}