using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLogic.Interface;
using Model;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace WEB_ManageCourt.Pages.Admin.Users
{
    public class DetailsModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IBadmintonCourtService _courtService;
        private readonly IBookingService _bookingService;

        public DetailsModel(IUserService userService, IBadmintonCourtService courtService, IBookingService bookingService)
        {
            _userService = userService;
            _courtService = courtService;
            _bookingService = bookingService;
        }

        public User User { get; set; } = default!;
        public IList<BadmintonCourt> UserCourts { get; set; } = new List<BadmintonCourt>();
        public IList<Booking> UserBookings { get; set; } = new List<Booking>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            User = user;

            if (User.Role == "Owner")
            {
                UserCourts = await _courtService.GetCourtsByOwnerIdAsync(User.UserId);
            }
            else if (User.Role == "User")
            {
                UserBookings = await _bookingService.GetBookingsByUserIdAsync(User.UserId);
            }

            return Page();
        }
    }
}
