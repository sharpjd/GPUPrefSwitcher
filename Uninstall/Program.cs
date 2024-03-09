using System;
using System.Diagnostics;
using System.IO;

namespace Uninstall
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
                processStartInfo.Arguments = $"/c sc.exe delete {SERVICE_NAME}";
                Process.Start(processStartInfo);

            } catch (Exception ex)
            {

                Console.WriteLine("An error was encountered during uninstallation:");
                Console.WriteLine(ex);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            
        }
    }

    /*
    public class TaskSchedulerUtils
    {
        readonly static string TASK_NAME_ON_BATTERY = "Switched to Battery";
        readonly static string TASK_NAME_PLUGGED_IN = "Switched to Plugged In";

        readonly static string FOLDER_NAME = @"GPUPrefSwitcher";

        public static bool TaskExists(string taskName)
        {
            var taskService = new TaskService();
            
            var exists = taskService.FindTask(taskName, false);
            return exists != null;
            
        }

        public static void DeleteTaskByName(string taskName)
        {
            // Create a new instance of TaskService
            using (TaskService taskService = new TaskService())
            {
                // Get the task by name
                Microsoft.Win32.TaskScheduler.Task task = taskService.FindTask(taskName);

                // Check if the task exists
                if (task != null)
                {
                    // Delete the task
                    taskService.RootFolder.DeleteTask(taskName);
                    Console.WriteLine($"Task '{taskName}' deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"Task '{taskName}' not found.");
                }
            }
        }
    }
    */
}
