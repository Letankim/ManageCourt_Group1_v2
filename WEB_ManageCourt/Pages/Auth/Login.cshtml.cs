using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using BusinessLogic.Interface;
using WEB_ManageCourt.ViewModels;
using Model;

namespace WEB_ManageCourt.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        [BindProperty]
        public LoginViewModel LoginViewModel { get; set; }

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostLoginAsync()
        {

            if (TryValidateModel(LoginViewModel, nameof(LoginViewModel)) && ModelState.IsValid)
            {
                var user = await _userService.AuthenticateUserLoginAsync(LoginViewModel.Username, LoginViewModel.Password);

                if (user != null)
                {
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("FullName", user.FullName ?? "");
                    return RedirectToPage("/Index");
                }

                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            return Page();
        }
    }
}
