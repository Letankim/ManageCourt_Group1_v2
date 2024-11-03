using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataAccess;
using Model;

namespace WEB_ManageCourt.Pages.Admin.Bookings
{
    public class CreateModel : PageModel
    {
        private readonly DataAccess.ManageCourtContext _context;

        public CreateModel(DataAccess.ManageCourtContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CourtId"] = new SelectList(_context.BadmintonCourts, "CourtId", "CourtName");
        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Password");
            return Page();
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Bookings.Add(Booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
