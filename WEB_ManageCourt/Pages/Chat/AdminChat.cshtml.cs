using BusinessLogic.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WEB_ManageCourt.Pages.Chat
{
    public class AdminChatModel : PageModel
    {
        private readonly IUserService _userService;
        public AdminChatModel(IUserService userService) { 
            _userService = userService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var username = HttpContext.Session.GetString("Admin");
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để truy cập trang này.";
                return RedirectToPage("/Auth/Login");
            }

            var CurrentUser = await _userService.GetUserByUsernameAsync(username);
            if (CurrentUser == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToPage("/Auth/Login");
            }
            return Page();
        }
    }

    
}
