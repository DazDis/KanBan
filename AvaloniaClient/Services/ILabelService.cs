using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public interface ILabelService
    {
        Task<List<LabelModel>> GetLabelsAsync(CancellationToken token = default);
        Task<LabelModel?> CreateLabelAsync(LabelModel dto, CancellationToken token = default);
        Task UpdateLabelAsync(LabelModel dto, CancellationToken token = default);
        Task DeleteLabelAsync(int id, CancellationToken token = default);
    }
}