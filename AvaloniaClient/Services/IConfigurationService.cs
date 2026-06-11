using System;

namespace AvaloniaClient.Services
{
    public interface IConfigurationService
    {
        event Action<string>? UrlChanged;
        string GetApiUrl();
        void SaveApiUrl(string url);
    }
}