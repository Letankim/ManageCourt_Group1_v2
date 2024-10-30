using DataAccess.DAO;
using Model;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserOtpRepository : IUserOtpRepository
    {
        private readonly UserOtpDAO _userOtpDao;

        public UserOtpRepository(UserOtpDAO userOtpDao)
        {
            _userOtpDao = userOtpDao;
        }

        public async Task SaveOtpAsync(int userId, string otpCode)
        {
            await _userOtpDao.SaveOtpAsync(userId, otpCode);
        }

        public async Task<UserOtp> GetOtpAsync(int userId)
        {
            return await _userOtpDao.GetOtpAsync(userId);
        }

        public async Task<bool> ValidateOtpAsync(int userId, string enteredOtp)
        {
            return await _userOtpDao.ValidateOtpAsync(userId, enteredOtp);
        }

        public async Task DeleteOtpAsync(int userId)
        {
            await _userOtpDao.DeleteOtpAsync(userId);
        }
    }
}
