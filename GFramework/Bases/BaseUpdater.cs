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

        public bool IsRunning => updaterTask != null && !cancelSource.IsCancellationRequested;

        public void Start()
        {
            if (updaterTask == null || updaterTask.Status != TaskStatus.Running)
            {
                cancelSource = new CancellationTokenSource();
                updaterTask = new Task(Work, cancelSource.Token, TaskCreationOptions.RunContinuationsAsynchronously | TaskCreationOptions.LongRunning);
                updaterTask.Start();
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

                await updater.Started();
                while (!token.IsCancellationRequested)
                {
                    if (updater.Mode == UpdaterMode.DelayBefore)
                        await Task.Delay(TimeSpan.FromMilliseconds(updater.Interval));

                    await updater.Run();

                    if (updater.Mode == UpdaterMode.DelayAfter)
                        await Task.Delay(TimeSpan.FromMilliseconds(updater.Interval));

                    token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                await updater.Stopped();
            }
        }
    }
}
