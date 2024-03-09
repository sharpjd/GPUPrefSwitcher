
using GPUPrefSwitcher;
using System.Diagnostics;

namespace GPUPrefSwitcherRepairer
{
    /// <summary>
    /// This should be able to operate relatively independently
    /// </summary>
    public partial class RepairForm : Form
    {

        public static readonly string ErrorLogPath = Path.Combine(Program.SavedDataPath,
            "GPUPrefSwitcherErrorLog.txt");
        public static readonly string StandardLogPath = Path.Combine(Program.SavedDataPath,
            "GPUPrefSwitcherLog.txt");

        public RepairForm()
        {
            InitializeComponent();

            PopulateTextBox();

            richTextBox1.ReadOnly = true;

            //scroll to end
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void PopulateTextBox()
        {
            richTextBox1.Text = File.OpenText(ErrorLogPath).ReadToEnd();
        }

        private void OverwritePreferencesButton_Click(object sender, EventArgs e)
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

            string message = "ALL of your configurations for each of your app entries in the list will be erased. You can make a backup/copy of Preferences.xml before this. Are you sure you want to continue?";
            if (MessageBox.Show(message, "Reset Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                OptionsFormUtils.ResetPreferencesXmlFile(false);

                MessageBox.Show("Preferences file overwritten with default.");
            }
            else
            {
                return;
            }
        }

        private void OverwriteAppOptionsButton_Click(object sender, EventArgs e)
        {
            string message = "You are overwriting the App Options file. Changes will not take effect until you restart the service. Proceed?";
            if (MessageBox.Show(message, "Reset Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                var source = Path.Combine(Program.SavedDataPath, "defaults/AppOptions.xml");
                var destination = Path.Combine(Program.SavedDataPath, "AppOptions.xml");

                File.Copy(source, destination, true);

                MessageBox.Show("App Options file overwritten with default.");
            }
            else
            {
                return;
            }
        }

        private void OpenErrorLogButton_Click(object sender, EventArgs e)
        {
            OpenWithDefaultProgram(ErrorLogPath);
        }

        private void OpenStandardLogButton_Click(object sender, EventArgs e)
        {
            OpenWithDefaultProgram(StandardLogPath);
        }

        private void OpenAppDirectoryButton_Click(object sender, EventArgs e)
        {
            OpenWithDefaultProgram(Program.SavedDataPath);
        }

        //source: https://stackoverflow.com/questions/11365984/c-sharp-open-file-with-default-application-and-parameters
        public static void OpenWithDefaultProgram(string path)
        {
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }

        private void RestartServiceButton_Click(object sender, EventArgs e)
        {
            OptionsFormUtils.AskRestartService();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void RepairForm_Load(object sender, EventArgs e)
        {

        }

        private void RepairForm_Resize(object sender, EventArgs e)
        {
            richTextBox1.Size = new Size(Width - 15, Height - 230);
        }
    }
}
