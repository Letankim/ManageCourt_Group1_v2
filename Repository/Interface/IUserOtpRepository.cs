using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IUserOtpRepository
    {
        Task SaveOtpAsync(int userId, string otpCode);
        Task<UserOtp> GetOtpAsync(int userId);
        Task<bool> ValidateOtpAsync(int userId, string enteredOtp);
        Task DeleteOtpAsync(int userId);
    }
}
