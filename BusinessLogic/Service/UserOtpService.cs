using BusinessLogic.Interface;
using DataAccess.DAO;
using Model;
using Repositories;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Service
{
    public class UserOtpService : IUserOtpService
    {
        private readonly IUserOtpRepository _userOtpRepository;

        public UserOtpService(IUserOtpRepository userOtpRepository)
        {
            _userOtpRepository = userOtpRepository;
        }

        public async Task SaveOtpAsync(int userId, string otpCode)
        {
            await _userOtpRepository.SaveOtpAsync(userId, otpCode);
        }

        public async Task<UserOtp> GetOtpAsync(int userId)
        {
            return await _userOtpRepository.GetOtpAsync(userId);
        }

        public async Task<bool> ValidateOtpAsync(int userId, string enteredOtp)
        {
            return await _userOtpRepository.ValidateOtpAsync(userId, enteredOtp);
        }

        public async Task DeleteOtpAsync(int userId)
        {
            await _userOtpRepository.DeleteOtpAsync(userId);
        }

    }
}
