using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DataAccess.DAO
{
    public class UserOtpDAO : SingletonBase<UserOtp>
    {

        public async Task SaveOtpAsync(int userId, string otpCode)
        {
            var existingOtp = await _context.UserOtps.FirstOrDefaultAsync(u => u.UserId == userId);

            if (existingOtp == null)
            {
                var userOtp = new UserOtp
                {
                    UserId = userId,
                    Otpcode = otpCode,
                    GeneratedAt = DateTime.Now
                };
                await _context.UserOtps.AddAsync(userOtp);
            }
            else
            {
                existingOtp.Otpcode = otpCode;
                existingOtp.GeneratedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<UserOtp> GetOtpAsync(int userId)
        {
            return await _context.UserOtps.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> ValidateOtpAsync(int userId, string enteredOtp)
        {
            var savedOtp = await GetOtpAsync(userId);

          
            if (savedOtp == null || savedOtp.GeneratedAt.AddMinutes(1) < DateTime.Now)
            {
                return false; 
            }

            return savedOtp.Otpcode == enteredOtp;
        }

        public async Task DeleteOtpAsync(int userId)
        {
            var UserOtps = await GetOtpAsync(userId);
            if (UserOtps != null)
            {
                _context.UserOtps.Remove(UserOtps);
                await _context.SaveChangesAsync();
            }
        }
    }
}
