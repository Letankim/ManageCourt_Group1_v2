using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using BusinessLogic.Interface;

namespace WEB_ManageCourt.Pages.Auth
{
    public class NewPasswordModel : PageModel
    {
        private readonly IUserService _userService;

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public NewPasswordModel(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult OnGet()
        {
            var otpVerified = HttpContext.Session.GetString("OtpVerified");
            if (otpVerified == null || otpVerified != "true")
            {
                TempData["OtpMessage"] = "Bạn cần xác thực OTP trước khi đặt lại mật khẩu.";
                return RedirectToPage("ForgotPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var otpVerified = HttpContext.Session.GetString("OtpVerified");
            if (otpVerified == null || otpVerified != "true")
            {
                TempData["OtpMessage"] = "Bạn cần xác thực OTP trước khi đặt lại mật khẩu.";
                return RedirectToPage("ForgotPassword");
            }

            if (string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập cả hai trường mật khẩu.");
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu không khớp. Vui lòng thử lại.");
                return Page();
            }
            string username = HttpContext.Session.GetString("OtpUsername");
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError(string.Empty, "Tên người dùng không hợp lệ.");
                return RedirectToPage("ForgotPassword");
            }

            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Người dùng không tồn tại.");
                return RedirectToPage("ForgotPassword");
            }

            await _userService.UpdatePasswordAsync(user.UserId, NewPassword);
            HttpContext.Session.Remove("OtpVerified");
            HttpContext.Session.Remove("OtpUsername");

            TempData["SuccessMessage"] = "Mật khẩu đã được đặt lại thành công! Vui lòng đăng nhập.";
            return RedirectToPage("/Auth/Login");
        }
    }
}
