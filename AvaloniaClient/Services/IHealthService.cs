using AvaloniaClient.Models;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public interface IHealthService
    {
        Task<HealthStatus> GetHealthStatusAsync(CancellationToken token = default);
        void ResetUrl(string url);
    }
}