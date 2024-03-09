using System.Diagnostics;

namespace GPUPrefSwitcherRepairer
{
    public partial class CrashedForm : Form
    {

        public CrashedForm()
        {
            InitializeComponent();
            DialogResult = DialogResult.No;
        }

        private void CrashedForm_Load(object sender, EventArgs e)
        {
        }

        private void YesButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void NoButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }

        private void OpenServicesButton_Click(object sender, EventArgs e)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C services.msc"; //you need to explicitly type .msc for services apparently but not for taskschd
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
