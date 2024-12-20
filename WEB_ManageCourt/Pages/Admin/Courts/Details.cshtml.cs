﻿using System.Threading.Tasks;
using BusinessLogic.Interface;
using BusinessLogic.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;

namespace WEB_ManageCourt.Pages.Admin.Courts
{
    public class DetailsModel : PageModel
    {
        private readonly IBadmintonCourtService _courtService;
        private readonly IUserService _userService;

        public DetailsModel(IBadmintonCourtService courtService, IUserService userService)
        {
            _courtService = courtService;
            _userService = userService;
        }

        public BadmintonCourt BadmintonCourt { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
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
            if (id == null)
            {
                return NotFound();
            }

            BadmintonCourt = await _courtService.GetCourtByIdAsync(id.Value);
            if (BadmintonCourt == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<JsonResult> OnGetToggleStatusAsync(int? id)
        {
            if (id == null)
            {
                return new JsonResult(new { success = false, message = "Invalid ID." });
            }

            var court = await _courtService.GetCourtByIdAsync(id.Value);
            if (court != null)
            {
                court.IsEnabled = !court.IsEnabled;
                await _courtService.UpdateCourtAsync(court);
                return new JsonResult(new { success = true, isEnabled = court.IsEnabled, message = "Status toggled successfully." });
            }
            else
            {
                return new JsonResult(new { success = false, message = "Court not found." });
            }
        }
    }
}
