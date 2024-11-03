using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using BusinessLogic.Interface;

namespace WEB_ManageCourt.Pages.Admin.Accessories
{
    public class DeleteModel : PageModel
    {
        private readonly IAccessoryService _accessoryService;

        public DeleteModel(IAccessoryService accessoryService)
        {
            _accessoryService = accessoryService;
        }

        [BindProperty]
        public Accessory Accessory { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accessory = await _accessoryService.GetAccessoryByIdAsync(id.Value);

            if (accessory == null)
            {
                return NotFound();
            }
            else
            {
                Accessory = accessory;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _accessoryService.DeleteAccessoryAsync(id.Value);

            return RedirectToPage("./Index");
        }
    }
}
