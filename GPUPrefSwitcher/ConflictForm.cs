using System;
using System.Windows.Forms;

namespace GPUPrefSwitcher
{
    //these are to be kept in a list and hidden until shown
    [Obsolete("Not necessary due to atomic file swaps with a nonvolatile state variable; conflicts should never occur. Interactive elements from a service are also a security issue.")]
    public partial class ConflictForm : Form
    {

        public int choice = -1; //-1 = invalid, 0 = overwrite existing, 1 = keep existing

        //TODO Security issues? Do saved/copied files inherit permissions?
        public ConflictForm(ref AppEntry appEntry, ref string swapPath, ref string storedOnBatteryPath, ref string storedPluggedInPath, string forState)
        {
            InitializeComponent();

            InitializeFormComponents(ref appEntry, ref swapPath, ref storedOnBatteryPath, ref storedPluggedInPath, forState);
        }

        private void InitializeFormComponents(ref AppEntry appEntry, ref string swapPath, ref string storedOnBatteryPath, ref string storedPluggedInPath, string forState)
        {
            TargetFilePathRichTextbox.Text = swapPath;
            OnBatteryFilePathRichTextbox.Text = storedOnBatteryPath;
            PluggedInFilePathRichTextBox.Text = storedPluggedInPath;
            TargetStateLabel.Text = forState;
        }

        private void ConflictForm_Load(object sender, EventArgs e)
        {

        }

        private void OverwriteButton_Click(object sender, EventArgs e)
        {
            ConfirmButton.Enabled = true;

            PreserveButton.Enabled = true;
            OverwriteButton.Enabled = false;

            choice = 0;
        }

        private void PreserveButton_Click(object sender, EventArgs e)
        {
            ConfirmButton.Enabled = true;

            PreserveButton.Enabled = false;
            OverwriteButton.Enabled = true;

            choice = 1;
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            //TODO what to do with this after confirmation?
                //just keep it stored is the plan, this can't take that much RAM
        }

        private void CancelAndDelayButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void OpenTargetFileButton_Click(object sender, EventArgs e)
        {
            OpenFile(TargetFilePathRichTextbox.Text);
        }

        private void OpenExistingOnBatteryFileButton_Click(object sender, EventArgs e)
        {
            OpenFile(OpenExistingOnBatteryFileButton.Text);
        }

        private void OpenExistingPluggedInFileButton_Click(object sender, EventArgs e)
        {
            OpenFile(OpenExistingPluggedInFileButton.Text); 
        }

        private void DescriptionRichTextBox_TextChanged(object sender, EventArgs e)
        {
            
        }

        private OpenFileDialog openFileDialog;
        private void OpenFile(string filename)
        {
            openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = filename;
            openFileDialog.OpenFile();
        }
    }
}
