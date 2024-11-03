using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WEB_ManageCourt.Pages.BookingCourt
{
    public class ConfirmationModel : PageModel
    {
        public string? ConfirmationMessage { get; set; }
        public string? ContactName { get; set; }
        public string? BookingDate { get; set; }
        public string? TimeSlot { get; set; }
        public string? TotalPrice { get; set; }

        public void OnGet()
        {
            ConfirmationMessage = TempData["ConfirmationMessage"] as string;
            ContactName = TempData["ContactName"] as string;
            BookingDate = TempData["BookingDate"] as string;
            TimeSlot = TempData["TimeSlot"] as string;
            TotalPrice = TempData["TotalPrice"] as string;
        }
    }
}
