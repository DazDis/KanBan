using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public interface ILabelService
    {
        Task<List<LabelModel>> GetLabelsAsync();
        Task<LabelModel?> CreateLabelAsync(LabelModel dto);
        Task DeleteLabelAsync(int id);
    }
}