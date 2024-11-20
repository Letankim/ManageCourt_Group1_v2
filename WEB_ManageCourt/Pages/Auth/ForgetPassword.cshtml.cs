using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using BusinessLogic.Interface;
using WEB_ManageCourt.Services;

namespace WEB_ManageCourt.Pages.Auth
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IUserOtpService _otpService;
        private readonly EmailService _emailService;

        [BindProperty]
        public string Username { get; set; }

        public bool IsSendOtpEnabled { get; private set; } = true;
        public string SendOtpMessage { get; set; } = "Gửi mã OTP";

        public ForgotPasswordModel(IUserService userService, IUserOtpService otpService, EmailService emailService)
        {
            _userService = userService;
            _otpService = otpService;
            _emailService = emailService;
        }

        public async Task<IActionResult> OnPostSendOtpAsync()
        {
            var user = await _userService.GetUserByUsernameAsync(Username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Người dùng không tồn tại.");
                return Page();
            }

            // Generate and save the OTP code
            var otpCode = new Random().Next(100000, 999999).ToString();
            await _otpService.SaveOtpAsync(user.UserId, otpCode);
            EmailTemplateGenerator emailServiceTemplate = new EmailTemplateGenerator();
            await _emailService.SendEmailAsync(user.Email, "Xác nhận OTP", emailServiceTemplate.GenerateOtpConfirmationEmail(user.FullName, otpCode));

            // Store the OTP sent time as UTC
            var sentTime = DateTime.UtcNow;
            HttpContext.Session.SetString("OtpUsername", Username);
            HttpContext.Session.SetString("OtpSentTime", sentTime.ToString("o")); // Store as UTC format
            HttpContext.Session.SetInt32("RemainingTime", 60);

            TempData["OtpMessage"] = "OTP đã được gửi đến email của bạn.";
            return RedirectToPage("VerifyOtp");
        }
    }
}
