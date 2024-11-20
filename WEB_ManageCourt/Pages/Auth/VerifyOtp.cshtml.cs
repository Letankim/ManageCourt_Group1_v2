using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using BusinessLogic.Interface;
using WEB_ManageCourt.Services;

namespace WEB_ManageCourt.Pages.Auth
{
    public class VerifyOtpModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IUserOtpService _otpService;
        private readonly EmailService _emailService;

        [BindProperty]
        public string EnteredOtp { get; set; }

        public bool IsResendOtpEnabled { get; private set; } = false;
        public string OtpMessage { get; private set; } = "OTP đã được gửi đến email của bạn.";
        public int RemainingTime { get; private set; } = 60;
        public string DebugInfo { get; private set; } 

        public VerifyOtpModel(IUserService userService, IUserOtpService otpService, EmailService emailService)
        {
            _userService = userService;
            _otpService = otpService;
            _emailService = emailService;
        }

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("OtpUsername");
            if (string.IsNullOrEmpty(username))
            {
                TempData["OtpMessage"] = "Vui lòng nhập tên đăng nhập trước khi tiếp tục.";
                return RedirectToPage("/Auth/ForgotPassword");
            }

            var otpSentTimeStr = HttpContext.Session.GetString("OtpSentTime");
            DateTime sentTime;
            DateTime currentTime;
            if (!string.IsNullOrEmpty(otpSentTimeStr) && DateTime.TryParse(otpSentTimeStr, out sentTime))
            {
                sentTime = DateTime.SpecifyKind(sentTime, DateTimeKind.Utc);
                string currentString = DateTime.UtcNow.ToString("o");
                DateTime.TryParse(currentString, out currentTime);
                currentTime = DateTime.SpecifyKind(currentTime, DateTimeKind.Utc);
                RemainingTime = Math.Max(60 - (int)(currentTime - sentTime).TotalSeconds, 0);
                IsResendOtpEnabled = RemainingTime <= 0;

                if (RemainingTime <= 0)
                {
                    ModelState.AddModelError(string.Empty, "OTP đã hết hạn.");
                    IsResendOtpEnabled = true;
                }
            }
            else
            {
                sentTime = DateTime.UtcNow;
                HttpContext.Session.SetString("OtpSentTime", sentTime.ToString("o"));
                RemainingTime = 60;
                IsResendOtpEnabled = false;
            }

            HttpContext.Session.SetInt32("RemainingTime", RemainingTime);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var username = HttpContext.Session.GetString("OtpUsername");
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError(string.Empty, "Phiên OTP đã hết hạn. Vui lòng thử lại.");
                return RedirectToPage("/Auth/ForgotPassword");
            }

            var otpSentTime = HttpContext.Session.GetString("OtpSentTime");
            DateTime currentTime;
            if (string.IsNullOrEmpty(otpSentTime) || !DateTime.TryParse(otpSentTime, out DateTime sentTime))
            {
                ModelState.AddModelError(string.Empty, "OTP không hợp lệ hoặc đã hết hạn.");
                return Page();
            }


            sentTime = DateTime.SpecifyKind(sentTime, DateTimeKind.Utc);
            string currentString = DateTime.UtcNow.ToString("o");
            DateTime.TryParse(currentString, out currentTime);
            currentTime = DateTime.SpecifyKind(currentTime, DateTimeKind.Utc);
            RemainingTime = Math.Max(60 - (int)(currentTime - sentTime).TotalSeconds, 0);
            HttpContext.Session.SetInt32("RemainingTime", RemainingTime);
            if (RemainingTime <= 0)
            {
                ModelState.AddModelError(string.Empty, "OTP đã hết hạn.");
                return Page();
            }

            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Người dùng không tồn tại.");
                return Page();
            }

            var isValidOtp = await _otpService.ValidateOtpAsync(user.UserId, EnteredOtp);
            if (isValidOtp)
            {
                HttpContext.Session.SetString("OtpVerified", "true");
                HttpContext.Session.Remove("OtpSentTime");

                return RedirectToPage("NewPassword");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "OTP không hợp lệ. Vui lòng thử lại.");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostResendOtpAsync()
        {
            var username = HttpContext.Session.GetString("OtpUsername");
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError(string.Empty, "Phiên OTP đã hết hạn. Vui lòng thử lại.");
                return RedirectToPage("/Auth/ForgotPassword");
            }

            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Người dùng không tồn tại.");
                return Page();
            }

            var otpCode = new Random().Next(100000, 999999).ToString();
            await _otpService.SaveOtpAsync(user.UserId, otpCode);
            EmailTemplateGenerator emailServiceTemplate = new EmailTemplateGenerator();
            await _emailService.SendEmailAsync(user.Email, "Xác nhận OTP", emailServiceTemplate.GenerateOtpConfirmationEmail(user.FullName, otpCode));
            HttpContext.Session.SetString("OtpSentTime", DateTime.UtcNow.ToString("o"));
            HttpContext.Session.SetInt32("RemainingTime", 60);

            RemainingTime = 60;
            IsResendOtpEnabled = false;

            TempData["OtpMessage"] = "OTP đã được gửi lại đến email của bạn.";
            return RedirectToPage();
        }
    }
}
