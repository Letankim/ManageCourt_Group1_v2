using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using BusinessLogic.Interface;
using System.Threading.Tasks;

namespace WEB_ManageCourt.Pages.BookingCourt
{
    public class DetailsModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;

        public DetailsModel(IBookingService bookingService, IUserService userService)
        {
            _bookingService = bookingService;
            _userService = userService;
        }

        public Booking Booking { get; private set; }
        public User CurrentUser { get; private set; }

        public async Task<IActionResult> OnGetAsync(int bookingId)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem lịch sử đặt chỗ.";
                return RedirectToPage("/Auth/Login");
            }
            CurrentUser = await _userService.GetUserByUsernameAsync(username);
            if (CurrentUser == null)
            {
                TempData["ErrorMessage"] = "Tài khoản của bạn không hợp lệ.";
                return RedirectToPage("/Auth/Login");
            }
            Booking = await _bookingService.GetBookingDetailByUserIdAndBookingIdAsync(CurrentUser.UserId, bookingId);

            if (Booking == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy chi tiết đặt chỗ.";
                return RedirectToPage("/BookingCourt/History");
            }

            return Page();
        }
    }
}
