using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WEB_ManageCourt.Pages.BookingCourt
{
    public class ErrorBookingModel : PageModel
    {
        public string? ErrornMessage { get; set; }
        public void OnGet()
        {
            ErrornMessage = TempData["ErrorMessage"] as string;
        }
    }
}
