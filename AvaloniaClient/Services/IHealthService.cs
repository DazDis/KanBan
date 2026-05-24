using AvaloniaClient.DataBase;
using AvaloniaClient.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public interface IHealthService
    {
        Task<HealthStatus> DropDBAsync(CancellationToken token = default);
        Task<HealthStatus> GetHealthStatusAsync(CancellationToken token = default);
        void ResetUrl(string url);
    }
}