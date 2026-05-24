using System.Threading;
using System.Threading.Tasks;

public interface IApiClient
{
    void SetUrl(string http);
    Task<T?> GetAsync<T>(string endpoint, CancellationToken token = default);
    Task<T?> PostAsync<T>(string endpoint, object data, CancellationToken token = default);
    Task PutAsync(string endpoint, object data, CancellationToken token = default);
    Task DeleteAsync(string endpoint, CancellationToken token = default);
}