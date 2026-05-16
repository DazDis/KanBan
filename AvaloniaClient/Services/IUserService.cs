using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public interface IUserService
    {
        Task<List<UserModel>> GetUsersAsync();
        Task<UserModel?> CreateUserAsync(UserModel dto);
        Task UpdateUserAsync(UserModel dto);
        Task DeleteUserAsync(int id);
    }
}