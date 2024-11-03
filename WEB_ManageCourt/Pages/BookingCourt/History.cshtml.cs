using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLogic.Interface;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WEB_ManageCourt.Pages.BookingCourt
{
    public class HistoryModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;

        public List<Booking> BookingHistory { get; set; }

        public HistoryModel(IBookingService bookingService, IUserService userService)
        {
            _bookingService = bookingService;
            _userService = userService;
        }
        public User CurrentUser { get; private set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem lịch sử đặt chỗ.";
                return RedirectToPage("/Auth/Login");
            }
            CurrentUser = await _userService.GetUserByUsernameAsync(username);
            if (CurrentUser == null) {
                TempData["ErrorMessage"] = "Tài khoản của bạn không hợp lệ.";
                return RedirectToPage("/Auth/Login");
            }
            BookingHistory = await _bookingService.GetBookingsByUserIdAsync(CurrentUser.UserId);

            return Page();
        }
    }
}
