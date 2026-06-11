using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaClient.Services;

public class ApiClient : IApiClient
{
    private HttpClient _httpClient;
    private IConfigurationService _configurationService;

    public ApiClient(HttpClient httpClient, IConfigurationService configurationService)
    {
        _httpClient = httpClient;
        _configurationService = configurationService;
        _configurationService.UrlChanged += SetUrl;
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }
    public void SetUrl(string? http)
    {
        _httpClient = new();
        _httpClient.BaseAddress = new Uri(http);
    }
    public Uri GetUrl()
    {
        return _httpClient.BaseAddress;
    }
    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken token = default)
    {
        var response = await _httpClient.GetAsync(endpoint, token);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(token);
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data, CancellationToken token = default)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, data, token);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(token);
    }

    public async Task PutAsync(string endpoint, object data, CancellationToken token = default)
    {
        var response = await _httpClient.PutAsJsonAsync(endpoint, data, token);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(string endpoint, CancellationToken token = default)
    {
        var response = await _httpClient.DeleteAsync(endpoint, token);
        response.EnsureSuccessStatusCode();
    }
}