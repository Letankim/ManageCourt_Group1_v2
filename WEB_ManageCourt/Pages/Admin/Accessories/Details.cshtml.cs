using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using BusinessLogic.Interface;

namespace WEB_ManageCourt.Pages.Admin.Accessories
{
    public class DetailsModel : PageModel
    {
        private readonly IAccessoryService _accessoryService;

        public DetailsModel(IAccessoryService accessoryService)
        {
            _accessoryService = accessoryService;
        }

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

            Accessory = accessory;
            return Page();
        }
    }
}
