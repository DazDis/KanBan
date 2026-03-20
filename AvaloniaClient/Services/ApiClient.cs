using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AvaloniaClient.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        var response = await _httpClient.PutAsJsonAsync(endpoint, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);
        response.EnsureSuccessStatusCode();
    }
}