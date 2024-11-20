using Microsoft.EntityFrameworkCore;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int userId);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> AuthenticateUserAsync(string username, string password);
        Task UpdatePasswordAsync(int userId, string newPassword);
        Task<User?> AuthenticateUserLoginAsync(string username, string password);

        Task<List<User>> GetAllCourtOwnerAsync();
    }
}
