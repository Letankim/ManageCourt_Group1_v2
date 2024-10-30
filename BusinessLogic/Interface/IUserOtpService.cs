using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface IUserOtpService
    {
        Task SaveOtpAsync(int userId, string otpCode);
        Task<UserOtp> GetOtpAsync(int userId);
        Task<bool> ValidateOtpAsync(int userId, string enteredOtp);
        Task DeleteOtpAsync(int userId);
    }
}
