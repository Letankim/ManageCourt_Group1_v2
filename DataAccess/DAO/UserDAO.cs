using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class UserDAO : SingletonBase<User>
    {
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddUserAsync(User user)
        {
            user.Password = EncryptPassword(user.Password);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .Include(u => u.Bookings)
                    .Include(u => u.BadmintonCourts)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user != null)
                {
                    _context.Bookings.RemoveRange(user.Bookings);
                    _context.BadmintonCourts.RemoveRange(user.BadmintonCourts);
                    _context.Users.Remove(user);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear();
                throw new Exception($"An error occurred while deleting the user with ID {userId}: {ex.Message}", ex);
            }
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            string encryptedPassword = EncryptPassword(password);
            var user = await _context.Users
                                 .SingleOrDefaultAsync(c => c.Username == username && c.Password == encryptedPassword);

            if (user != null)
            {
                return user;
            }
            return null;
        }

        private string EncryptPassword(string password)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = md5.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public async Task UpdatePasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception($"User with ID {userId} not found.");
            }

            user.Password = EncryptPassword(newPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
