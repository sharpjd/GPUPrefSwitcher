using System;
using System.IO;
using System.Windows.Forms;
using GPUPrefSwitcher;

namespace GPUPrefSwitcherGUI
{
    static class Program
    {
        public static readonly string SavedDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData/");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //this should go here first to initialize static things

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Logger.Initialize(
                SavedDataPath,
                "GUIErrorLog.txt",
                SavedDataPath,
                "GUILog.txt",
                Path.Combine(SavedDataPath, "oldGUILogs")
                );
            Application.Run(new MainForm());

        }
        
    }
}
