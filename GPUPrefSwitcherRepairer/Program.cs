namespace GPUPrefSwitcherRepairer
{
    internal static class Program
    {
        public static readonly string SavedDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData\\");

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            AskToShowCrashForm();
        }

        public static void AskToShowCrashForm()
        {
            CrashedForm crashedForm = new CrashedForm();
            DialogResult dialogResult = crashedForm.ShowDialog();

            if (dialogResult == DialogResult.Yes)
            {
                RepairForm repair = new RepairForm();

                repair.ShowDialog();
            }
            else if (dialogResult == DialogResult.No)
            {
                //do nothing
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}