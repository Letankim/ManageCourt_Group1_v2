using System.Threading.Tasks;
using BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;

namespace WEB_ManageCourt.Pages.Admin.Courts
{
    public class DeleteModel : PageModel
    {
        private readonly IBadmintonCourtService _courtService;

        public DeleteModel(IBadmintonCourtService courtService)
        {
            _courtService = courtService;
        }

        [BindProperty]
        public BadmintonCourt BadmintonCourt { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var badmintoncourt = await _courtService.GetCourtByIdAsync(id.Value);

            if (badmintoncourt == null)
            {
                return NotFound();
            }
            else
            {
                BadmintonCourt = badmintoncourt;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _courtService.DeleteCourtAsync(id.Value);
            return RedirectToPage("./Index");
        }
    }
}
