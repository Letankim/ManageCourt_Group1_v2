using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using BusinessLogic.Interface;

namespace WEB_ManageCourt.Pages.Admin.Bookings
{
    public class DetailsModel : PageModel
    {
        private readonly IBookingService _bookingService;

        public DetailsModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _bookingService.GetBookingByIdAsync(id.Value);
            if (booking == null)
            {
                return NotFound();
            }

            Booking = booking;
            return Page();
        }
    }
}
