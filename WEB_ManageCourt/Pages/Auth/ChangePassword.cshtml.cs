using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using BusinessLogic.Interface;
using Model;
using WEB_ManageCourt.Services;

namespace WEB_ManageCourt.Pages.Profile
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly EmailService _emailService;

        public ChangePasswordModel(IUserService userService, EmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        [BindProperty]
        public string? NewPassword { get; set; }

        [BindProperty]
        public string? ConfirmPassword { get; set; }

        [BindProperty]
        public string? EnteredOtp { get; set; }

        public bool IsOtpSent { get; private set; } = false;
        public bool IsOtpVerified { get; private set; } = false;
        public int RemainingTime { get; private set; } = -1; 

        public async Task<IActionResult> OnGet()
        {
            var otpSentTimeStr = HttpContext.Session.GetString("OtpSentTime");
            var isOtpVerified = HttpContext.Session.GetString("IsOtpVerified") == "true";
            IsOtpVerified = isOtpVerified;
            DateTime sentTime;
            DateTime currentTime;
            if (!string.IsNullOrEmpty(otpSentTimeStr)  && DateTime.TryParse(otpSentTimeStr, out sentTime))
            {
                sentTime = DateTime.SpecifyKind(sentTime, DateTimeKind.Utc);
                string currentString = DateTime.UtcNow.ToString("o");
                DateTime.TryParse(currentString, out currentTime);
                currentTime = DateTime.SpecifyKind(currentTime, DateTimeKind.Utc);
                RemainingTime = Math.Max(60 - (int)(currentTime - sentTime).TotalSeconds, 0);
                IsOtpSent = RemainingTime > 0;
            } 
            return Page();
        }

        public async Task<IActionResult> OnPostSendOtpAsync()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để đổi mật khẩu.";
                return RedirectToPage("/Auth/Login");
            }

            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng hoặc email không hợp lệ.";
                return RedirectToPage("/Auth/Login");
            }

            var otp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("OtpCode", otp);
            HttpContext.Session.SetString("OtpSentTime", DateTime.UtcNow.ToString("o"));

            await _emailService.SendEmailAsync(user.Email, "Mã OTP của bạn", $"Mã OTP của bạn là: {otp}");

            TempData["SuccessMessage"] = "OTP đã được gửi đến email của bạn.";
            IsOtpSent = true;
            RemainingTime = 60;
            return RedirectToPage();
        }

        public IActionResult OnPostVerifyOtp()
        {
            var otpSentTimeStr = HttpContext.Session.GetString("OtpSentTime");
            var storedOtp = HttpContext.Session.GetString("OtpCode");

            if (string.IsNullOrEmpty(otpSentTimeStr) || string.IsNullOrEmpty(storedOtp))
            {
                TempData["ErrorMessage"] = "OTP đã hết hạn. Vui lòng gửi lại OTP.";
                return RedirectToPage();
            }

            if (storedOtp != EnteredOtp)
            {
                TempData["ErrorMessage"] = "OTP không hợp lệ.";
            }

            DateTime sentTime;
            DateTime.TryParse(otpSentTimeStr, out sentTime);
            DateTime currentTime;
            sentTime = DateTime.SpecifyKind(sentTime, DateTimeKind.Utc);

            if (!string.IsNullOrEmpty(otpSentTimeStr) && DateTime.TryParse(otpSentTimeStr, out sentTime))
            {
                string currentString = DateTime.UtcNow.ToString("o");
                DateTime.TryParse(currentString, out currentTime);
                currentTime = DateTime.SpecifyKind(currentTime, DateTimeKind.Utc);
                if ((currentTime - sentTime).TotalSeconds <= 60 && storedOtp == EnteredOtp)
                {
                    HttpContext.Session.SetString("IsOtpVerified", "true");
                    IsOtpVerified = true;
                    TempData["SuccessMessage"] = "OTP đã được xác minh thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "OTP không hợp lệ.";
                    RemainingTime = Math.Max(60 - (int)(currentTime - sentTime).TotalSeconds, 0);
                    IsOtpSent = RemainingTime > 0;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "OTP đã hết hạn. Vui lòng gửi lại OTP.";
                IsOtpSent = false;
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            var isOtpVerified = HttpContext.Session.GetString("IsOtpVerified");
            if (isOtpVerified != "true")
            {
                TempData["ErrorMessage"] = "Vui lòng xác minh OTP trước khi đổi mật khẩu.";
                return RedirectToPage();
            }

            if (string.IsNullOrEmpty(NewPassword) || NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu xác nhận không khớp hoặc không hợp lệ.");
                return Page();
            }

            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToPage("/Auth/Login");
            }

            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToPage("/Auth/Login");
            }

            await _userService.UpdatePasswordAsync(user.UserId, NewPassword);

            HttpContext.Session.Remove("OtpCode");
            HttpContext.Session.Remove("OtpSentTime");
            HttpContext.Session.Remove("IsOtpVerified");

            TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công.";
            return RedirectToPage("/Auth/Profile");
        }
    }
}
