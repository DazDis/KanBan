using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public interface IColumnService
    {
        Task<List<ColumnDTO>> GetColumnsAsync(CancellationToken token = default);
        Task<ColumnDTO?> CreateColumnAsync(ColumnDTO dto, CancellationToken token = default);
        Task UpdateColumnAsync(ColumnModel dto, CancellationToken token = default);
        Task DeleteColumnAsync(int id, CancellationToken token = default);

    }
}