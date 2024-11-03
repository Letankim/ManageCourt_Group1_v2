using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using BusinessLogic.Interface;

namespace WEB_ManageCourt.Pages.Admin.Accessories
{
    public class CreateModel : PageModel
    {
        private readonly IAccessoryService _accessoryService;

        public CreateModel(IAccessoryService accessoryService)
        {
            _accessoryService = accessoryService;
        }

        [BindProperty]
        public Accessory Accessory { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _accessoryService.AddAccessoryAsync(Accessory);

            return RedirectToPage("./Index");
        }
    }
}
