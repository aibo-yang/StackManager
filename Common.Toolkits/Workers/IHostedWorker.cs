using System.Threading;
using System.Threading.Tasks;

namespace Common.Toolkits.Workers
{
    public interface IHostedWorker
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
