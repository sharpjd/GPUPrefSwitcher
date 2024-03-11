using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration.Install;
using System.ComponentModel;
using System.Collections;

namespace InstallActions
{

    [RunInstaller(true)]
    public class InstallerActions : Installer
    {
        //https://learn.microsoft.com/en-us/dotnet/api/system.configuration.install.installer?view=netframework-4.8.1
        public InstallerActions() : base()
        {
            // Attach the 'Committed' event.
            this.Committed += new InstallEventHandler(MyInstaller_Committed);
            // Attach the 'Committing' event.
            this.Committing += new InstallEventHandler(MyInstaller_Committing);
        }
        // Event handler for 'Committing' event.
        private void MyInstaller_Committing(object sender, InstallEventArgs e)
        {
            Console.WriteLine("");
            Console.WriteLine("Committing Event occurred.");
            Console.WriteLine("");
        }
        // Event handler for 'Committed' event.
        private void MyInstaller_Committed(object sender, InstallEventArgs e)
        {
            Console.WriteLine("");
            Console.WriteLine("Committed Event occurred.");
            Console.WriteLine("");
        }
        // Override the 'Install' method.
        public override void Install(IDictionary savedState)
        {
            base.Install(savedState);
        }
        // Override the 'Commit' method.
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }
        // Override the 'Rollback' method.
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }
        public static void Main()
        {
            Console.WriteLine("Usage : installutil.exe Installer.exe ");
        }
    }
}
