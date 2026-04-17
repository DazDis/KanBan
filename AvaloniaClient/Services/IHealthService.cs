using AvaloniaClient.DataBase;
using AvaloniaClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public interface IHealthService
    {
        Task<HealthStatus> GetHealthStatusAsync();
        void ResetUrl(string url);
    }
}