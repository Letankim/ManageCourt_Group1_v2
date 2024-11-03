using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using BusinessLogic.Interface;
using Microsoft.EntityFrameworkCore;

namespace WEB_ManageCourt.Pages.Admin.Accessories
{
    public class EditModel : PageModel
    {
        private readonly IAccessoryService _accessoryService;

        public EditModel(IAccessoryService accessoryService)
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

            // Sử dụng AsNoTracking để tránh theo dõi thực thể ban đầu
            var accessory = await _accessoryService.GetAccessoryByIdAsync(id.Value);
            if (accessory == null)
            {
                return NotFound();
            }

            Accessory = accessory;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var existingAccessory = await _accessoryService.GetAccessoryByIdAsync(Accessory.AccessoryId);
                if (existingAccessory != null)
                {
                    existingAccessory.Name = Accessory.Name;
                    existingAccessory.Price = Accessory.Price;

                    await _accessoryService.UpdateAccessoryAsync(existingAccessory);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _accessoryService.GetAccessoryByIdAsync(Accessory.AccessoryId) != null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
