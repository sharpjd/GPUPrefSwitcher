namespace GPUPrefSwitcherRepairer
{
    partial class CrashedForm
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
            richTextBox1 = new RichTextBox();
            YesButton = new Button();
            NoButton = new Button();
            OpenServicesButton = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = SystemColors.MenuBar;
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.Location = new Point(10, 9);
            richTextBox1.Margin = new Padding(3, 2, 3, 2);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(368, 90);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "The GPUPrefSwitcher service has encountered irrecoverable errors and cannot continue until they are fixed.\n\nOpen the repair form to diagnose the issue now?";
            // 
            // YesButton
            // 
            YesButton.Anchor = AnchorStyles.Bottom;
            YesButton.Location = new Point(10, 136);
            YesButton.Margin = new Padding(3, 2, 3, 2);
            YesButton.Name = "YesButton";
            YesButton.Size = new Size(82, 22);
            YesButton.TabIndex = 1;
            YesButton.Text = "Yes";
            YesButton.UseVisualStyleBackColor = true;
            YesButton.Click += YesButton_Click;
            // 
            // NoButton
            // 
            NoButton.Anchor = AnchorStyles.Bottom;
            NoButton.Location = new Point(98, 136);
            NoButton.Margin = new Padding(3, 2, 3, 2);
            NoButton.Name = "NoButton";
            NoButton.Size = new Size(82, 22);
            NoButton.TabIndex = 2;
            NoButton.Text = "No";
            NoButton.UseVisualStyleBackColor = true;
            NoButton.Click += NoButton_Click;
            // 
            // OpenServicesButton
            // 
            OpenServicesButton.Location = new Point(273, 136);
            OpenServicesButton.Name = "OpenServicesButton";
            OpenServicesButton.Size = new Size(105, 23);
            OpenServicesButton.TabIndex = 3;
            OpenServicesButton.Text = "Open Services";
            OpenServicesButton.UseVisualStyleBackColor = true;
            OpenServicesButton.Click += OpenServicesButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(10, 111);
            label1.Name = "label1";
            label1.Size = new Size(307, 13);
            label1.TabIndex = 4;
            label1.Text = "(You can stop this from showing if you disable the service.)";
            // 
            // CrashedForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(388, 167);
            Controls.Add(label1);
            Controls.Add(OpenServicesButton);
            Controls.Add(NoButton);
            Controls.Add(YesButton);
            Controls.Add(richTextBox1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "CrashedForm";
            Text = "GPUPrefSwitcherService Crashed";
            Load += CrashedForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button YesButton;
        private System.Windows.Forms.Button NoButton;
        private Button OpenServicesButton;
        private Label label1;
    }
}