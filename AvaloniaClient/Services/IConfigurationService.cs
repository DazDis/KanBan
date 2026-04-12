using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public interface IConfigurationService
    {
        string GetApiUrl();
        void SaveApiUrl(string url);
    }
}