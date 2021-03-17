using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GFramework.Bases
{
    using Enums;
    using Interfaces;

    public class BaseUpdater
    {
        private Task updaterTask;
        private CancellationTokenSource cancelSource;

        private IUpdater updater;

        public BaseUpdater(IUpdater updater)
        {
            this.updater = updater;

        }

        public bool IsRunning => updaterTask != null && updaterTask.Status == TaskStatus.Running;

        public void Start()
        {
            if (updaterTask == null || updaterTask.Status != TaskStatus.Running)
            {
                cancelSource = new CancellationTokenSource();
                updaterTask = Task.Run(Work, cancelSource.Token);
            }
        }

        public void Stop()
        {
            if (updaterTask != null)
                cancelSource.Cancel();
        }

        private async void Work()
        {
            try
            {
                var token = cancelSource.Token;
                token.ThrowIfCancellationRequested();

                updater.Started();
                while(!token.IsCancellationRequested)
                {
                    if (updater.Mode == UpdaterMode.DelayBefore)
                        await Task.Delay(TimeSpan.FromMilliseconds(updater.Interval));

                    updater.Run();

                    if (updater.Mode == UpdaterMode.DelayAfter)
                        await Task.Delay(TimeSpan.FromMilliseconds(updater.Interval));

                    token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                updater.Stopped();
            }
        }
    }
}
