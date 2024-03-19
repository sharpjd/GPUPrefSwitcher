using Microsoft.Win32.TaskScheduler;
using System;

namespace GPUPrefSwitcher
{
    public static class TaskSchedulerUtils
    {
        readonly static string TASK_NAME_ON_BATTERY = "Switched to Battery";
        readonly static string TASK_NAME_PLUGGED_IN = "Switched to Plugged In";

        readonly static string FOLDER_NAME = @"GPUPrefSwitcher";

        public static void RunOnBatteryTask()
        {
            if (!TaskAndFolderExists(TASK_NAME_ON_BATTERY))
            {
                string description = "The GPUPrefSwitcher service runs this task once when the system is considered on battery." +
                    "\n It is up to you to add actions (e.g. start a script).";
                CreateTask(TASK_NAME_ON_BATTERY, description, FOLDER_NAME);
                Logger.inst.Log("Created On Battery Task Scheduler entry because it doesn't exist yet");
            }

            RunTask(TASK_NAME_ON_BATTERY);
        }

        public static void RunPluggedInTask()
        {
            if (!TaskAndFolderExists(TASK_NAME_PLUGGED_IN))
            {
                string description = "The GPUPrefSwitcher service runs this task once when the system is considered plugged in." +
                    "\n It is up to you to add actions (e.g. start a script).";
                CreateTask(TASK_NAME_PLUGGED_IN, description, FOLDER_NAME);
                Logger.inst.Log("Created Plugged In Task Scheduler entry because it doesn't exist yet");
            }

            RunTask(TASK_NAME_PLUGGED_IN);
        }

        public static bool TaskAndFolderExists(string taskName)
        {
            using (TaskService taskService = new())
            {

                TaskFolder gpuPrefSwitcherFolder = taskService.GetFolder(FOLDER_NAME);
                if (gpuPrefSwitcherFolder == null) return false;

                // Get the folder with the specified name
                TaskFolder folder = taskService.RootFolder.SubFolders[FOLDER_NAME];

                // Iterate through the tasks in the folder
                foreach (Task task in folder.Tasks)
                {
                    // Check if the task has the specified name
                    if (task.Name.Equals(taskName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true; // Return the task if found
                    }
                }
                return false;
                //return taskService.FindTask(taskName, true) != null;
            }
        }

        public static void CreateTask(string taskName, string description, string folderPath)
        {
            using (TaskService taskService = new TaskService())
            {
                TaskDefinition taskDefinition = taskService.NewTask();
                taskDefinition.RegistrationInfo.Description = description;

                //we need to create at least one action otherwise it complains
                taskDefinition.Actions.Add(new ExecAction("cmd.exe", "/c echo Dummy task executed", null));

                taskDefinition.Settings.DisallowStartIfOnBatteries = false;
                taskDefinition.Settings.StopIfGoingOnBatteries = false;

                TaskFolder folder = taskService.GetFolder(folderPath);
                if (folder == null)
                {
                    taskService.RootFolder.CreateFolder(folderPath);
                    folder = taskService.GetFolder(folderPath);
                }
                    

                // Register the task
                folder.RegisterTaskDefinition(taskName, taskDefinition);
            }
        }

        public static void RunTask(string taskName)
        {
            using (TaskService taskService = new TaskService())
            {
                // Get the task
                Task task = taskService.FindTask(taskName);

                if (task != null)
                {
                    // Run the task
                    task.Run();
                }
                else
                {
                    Logger.inst.ErrorLog($"Task '{taskName}' not found.");
                    throw new NotImplementedException();
                }
            }
        }

    }
}
