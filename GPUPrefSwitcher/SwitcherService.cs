using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Threading;
using System.Threading.Tasks;

namespace GPUPrefSwitcher
{
    //TODO: optimization, make this async (scoped processing service...?)
    internal class SwitcherService : IHostedService
    {

        private readonly ILogger<SwitcherService> _logger;

        public SwitcherService(ILogger<SwitcherService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            Switcher.Start();

            SystemEvents.SessionEnding += OnSessionEnd;

            return Task.CompletedTask;
        }

        private void OnSessionEnd(object sender, SessionEndingEventArgs e)
        {
            e.Cancel = true;
            Logger.inst.Log($"OnSessionEnd received (reason: {e.Reason})");
            if (e.Reason == SessionEndReasons.SystemShutdown)
            {
                CleanUpService();
            }
            e.Cancel = false;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            Logger.inst.Log("Service StopAsync called");

            CleanUpService();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            CleanUpService();
        }

        private void CleanUpService()
        {
            Logger.inst.Log("Service ending cleanup called.");

            Logger.inst.DumpStandardLogBufferToStandardLog().Wait();

            Switcher.Stop();

            Logger.inst.WaitForFinishAndRelease();
            Logger.inst.Dispose();
        }

    }
}
