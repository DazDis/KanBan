using AvaloniaClient.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public class HealthService : IHealthService
    {
        private readonly IApiClient _apiClient;
        public HealthService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        public void ResetUrl(string? url)
        {
            _apiClient.SetUrl(url);
        }
        public async Task<HealthStatus> GetHealthStatusAsync(CancellationToken token = default)
        {
            try
            {
                var response = await _apiClient.GetAsync<object>("api/health", token);
                return new HealthStatus
                {
                    IsAvailable = true,
                    Message = "Сервер доступен"
                };
            }
            catch (HttpRequestException ex)
            {
                return new HealthStatus
                {
                    IsAvailable = false,
                    Message = $"Сервер недоступен ({_apiClient.GetUrl()}): {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new HealthStatus
                {
                    IsAvailable = false,
                    Message = $"Не удалось подключиться к серверу ({_apiClient.GetUrl()}): {ex.Message}"
                };
            }
        }

    }
}