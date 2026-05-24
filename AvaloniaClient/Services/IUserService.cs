using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public interface IUserService
    {
        Task<List<UserModel>> GetUsersAsync(CancellationToken token = default);
        Task<UserModel?> CreateUserAsync(UserModel dto, CancellationToken token = default);
        Task UpdateUserAsync(UserModel dto, CancellationToken token = default);
        Task DeleteUserAsync(int id, CancellationToken token = default);
    }
}