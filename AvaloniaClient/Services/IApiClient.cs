using System.Threading.Tasks;

public interface IApiClient
{
    void SetUrl(string http);
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task PutAsync(string endpoint, object data);
    Task DeleteAsync(string endpoint);
}