using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public interface IColumnService
    {
        Task<List<ColumnModel>> GetColumnsAsync();
        Task<ColumnDTO?> CreateColumnAsync(ColumnDTO dto);
        Task UpdateColumnAsync(ColumnModel dto);
        Task DeleteColumnAsync(int id);

    }
}