using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using BusinessLogic.Interface;
using Model;
using WEB_ManageCourt.ViewModels;
using WEB_ManageCourt.ViewModel;

namespace WEB_ManageCourt.Pages.Profile
{
    public class EditProfileModel : PageModel
    {
        private readonly IUserService _userService;

        public EditProfileModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public UserProfileViewModel CurrentUser { get; set; } = new UserProfileViewModel();

        public async Task<IActionResult> OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để truy cập trang cá nhân.";
                return RedirectToPage("/Auth/Login");
            }

            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToPage("/Auth/Login");
            }

            // Map data to the view model for display
            CurrentUser.FullName = user.FullName;
            CurrentUser.Email = user.Email;
            CurrentUser.Phone = user.Phone;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var username = HttpContext.Session.GetString("Username");
            var user = await _userService.GetUserByUsernameAsync(username);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToPage("/Auth/Login");
            }
            user.FullName = CurrentUser.FullName;
            user.Email = CurrentUser.Email;
            user.Phone = CurrentUser.Phone;

            await _userService.UpdateUserAsync(user);
            TempData["SuccessMessage"] = "Thông tin cá nhân đã được cập nhật thành công.";
            return RedirectToPage("Profile");
        }
    }
}
