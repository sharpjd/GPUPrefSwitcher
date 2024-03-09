using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using murrayju.ProcessExtensions;
using System;
using System.IO;

namespace GPUPrefSwitcher
{

    public static class Program
    {
        const string ServiceName = "GPUPrefSwitcher";
        public static readonly string SavedDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData/");

        /// <summary>
        /// Entry point of the entire app
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            //make this come first
            Logger.Initialize(
                SavedDataPath,
                "GPUPrefSwitcherErrorLog.txt",
                SavedDataPath,
                "GPUPrefSwitcherLog.txt",
                Path.Combine(Program.SavedDataPath, "oldLogs")
                );

            //create a file called "CRASHED" as a marker if the app crashes
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                //this file will get deleted upon a successful run from Switcher
                File.Create(Switcher.CRASHED_FILE_PATH);
                TryShowCrashedForm();//will activate if the crash marker exists
            };

            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = ServiceName;
            });

            LoggerProviderOptions.RegisterProviderOptions<
                EventLogSettings, EventLogLoggerProvider>(builder.Services);

            //builder.Services.AddSingleton<GPUPrefSwitcher>();
            builder.Services.AddHostedService<SwitcherService>();

            IHost host = builder.Build();
            host.Run();
        }

        /// <summary>
        /// Shows the crash form under these circumstances:
        /// - If a file named <see cref="Switcher.CRASHED_FILE_PATH"/> exists AND <see cref="SwitcherData.CurrentSwitcherData.DontShowErrorMessage"/> is false
        /// - Or if <see cref="SwitcherData"/> fails to initialize
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public static void TryShowCrashedForm()
        {
            var switcherData = SwitcherData.Initialize();
            if (switcherData != null && //this also handles the case where SwitcherData also crashes
                switcherData.CurrentSwitcherData.DontShowErrorMessage)
            {
                //do nothing
            }
            else
            {
                //ask user; the app cannot continue until errors are fixed
                if (File.Exists(Switcher.CRASHED_FILE_PATH))
                {
                    /*
                     * It is imperative that we run this as the user to avoid shatter attacks
                     */
                    ProcessExtensions.StartProcessAsCurrentUser(Path.Combine(SavedDataPath, "GPUPrefSwitcherRepairer.exe"));
                }
            }
        }
    }


}
