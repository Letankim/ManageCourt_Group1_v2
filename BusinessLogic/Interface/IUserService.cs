using DataAccess.DAO;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetListAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task AddUserAsync(User item);
        Task UpdateUserAsync(User item);
        Task DeleteUserAsync(int id);
        Task<List<User>> ImportFromJsonAsync(string filePath);
        Task<List<User>> ImportFromExcelAsync(string filePath);
        Task ExportToJsonAsync(List<User> users, string filePath);
        Task ExportToExcelAsync(List<User> users, string filePath);
        Task<User> GetUserByUsernameAsync(string username);

        Task<User> AuthenticateUserAsync(string username, string password);

        Task UpdatePasswordAsync(int userId, string newPassword);
        Task<User?> AuthenticateUserLoginAsync(string username, string password);

        Task<List<User>> GetAllCourtOwnerAsync();
    }
}
