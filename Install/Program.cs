using System;
using System.Diagnostics;
using System.IO;

namespace Install
{
    internal class Program
    {
        const string SERVICE_NAME = "GPUPrefSwitcher";
        const string EXE_NAME = "GPUPrefSwitcher.exe";

        static void Main(string[] args)
        {
            try
            {
                string pathToGPUPrefSwitcher = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\"+EXE_NAME);

                ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
                processStartInfo.UseShellExecute = true;
                //https://stackoverflow.com/questions/16174736/cannot-get-sc-exe-to-work-for-my-program
                //this fails if you don't put a space after the equal sign... ARE YOU SERIOUS??
                processStartInfo.Arguments = $"/c sc.exe create {SERVICE_NAME} binpath= \"{pathToGPUPrefSwitcher}\" start= auto";
                Process.Start(processStartInfo);

                ProcessStartInfo processStartInfo2 = new ProcessStartInfo("cmd.exe");
                processStartInfo2.UseShellExecute = true;
                processStartInfo2.Arguments = $"/c sc start {SERVICE_NAME}";
                Process.Start(processStartInfo2);

            } catch (Exception ex)
            {

                Console.WriteLine("An error was encountered during installation:");
                Console.WriteLine(ex);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            
        }
    }
}
