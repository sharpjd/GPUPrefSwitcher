using System.Diagnostics;

namespace ConsoleApp1
{

    internal class Program
    {
        static void Main(string[] args)
        {

            const string serviceName = "GPUPrefSwitcher"; //this needs to be updated if the service name is changed!!!

            StopWindowsService(serviceName);
            StartWindowsService(serviceName);
        }

        public static void StartWindowsService(string serviceName)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo("net.exe", "start " + serviceName)
                {
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = Environment.SystemDirectory
                };
                var start = Process.Start(processStartInfo);
                if (start == null)
                {
                    throw new Exception("Error using net.exe to start the service");
                }
                else
                {
                    start.WaitForExit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void StopWindowsService(string serviceName)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo("net.exe", "stop " + serviceName)
                {
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = Environment.SystemDirectory
                };
                var start = Process.Start(processStartInfo);
                if (start == null)
                {
                    throw new Exception("Error using net.exe to stop the service");
                } else
                {
                    start.WaitForExit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

}