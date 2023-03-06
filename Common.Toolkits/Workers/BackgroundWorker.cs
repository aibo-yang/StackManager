using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Toolkits.Workers
{
    public abstract class BackgroundWorker :IHostedWorker, IDisposable
    {
        private Task executingTask;
        private CancellationTokenSource stoppingCts;

        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken[1] { cancellationToken });
            executingTask = ExecuteAsync(stoppingCts.Token);
          
            if (executingTask.IsCompleted)
            {
                return executingTask;
            }

            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (executingTask != null)
            {
                try
                {
                    stoppingCts.Cancel();
                }
                finally
                {
                    await Task.WhenAny(new Task[2] { executingTask, Task.Delay(-1, cancellationToken)}).ConfigureAwait(continueOnCapturedContext: false);
                }
            }
        }

        public virtual void Dispose()
        {
            stoppingCts?.Cancel();
        }
    }
}
