using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using BusinessLogic.Interface;
using WEB_ManageCourt.ViewModels;
using Model;

namespace WEB_ManageCourt.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; }

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            if (TryValidateModel(RegisterViewModel, nameof(RegisterViewModel)) && ModelState.IsValid)
            {
                var existingUser = await _userService.GetUserByUsernameAsync(RegisterViewModel.Username);

                if (existingUser == null)
                {
                    var newUser = new User
                    {
                        Username = RegisterViewModel.Username,
                        Password = RegisterViewModel.Password,
                        FullName = RegisterViewModel.FullName,
                        Email = RegisterViewModel.Email,
                        Phone = RegisterViewModel.Phone,
                        Role = "User"
                    };

                    await _userService.AddUserAsync(newUser);
                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";

                    return RedirectToPage("/Auth/Login");
                }

                ModelState.AddModelError(string.Empty, "Tên đăng nhập đã tồn tại.");
            }

            return Page();
        }
    }
}
