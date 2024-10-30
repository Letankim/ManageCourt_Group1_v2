using DataAccess.DAO;
using Microsoft.EntityFrameworkCore;
using Model;
using Repositories;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDAO _userDAO;

        public UserRepository(UserDAO userDAO)
        {
            _userDAO = userDAO;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userDAO.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userDAO.GetUserByIdAsync(userId);
        }

        public async Task AddUserAsync(User user)
        {
            await _userDAO.AddUserAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userDAO.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _userDAO.GetUserByIdAsync(userId);
            if (user != null)
            {
                await _userDAO.DeleteUserAsync(userId);
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userDAO.GetUserByUsernameAsync(username);
        }
        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            return await _userDAO.AuthenticateUserAsync(username, password);
        }

        public async Task UpdatePasswordAsync(int userId, string newPassword)
        {
            await _userDAO.UpdatePasswordAsync(userId, newPassword);
        }
    }
}
