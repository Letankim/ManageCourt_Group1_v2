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
        private readonly IUserService _userService;

        public EditModel(IAccessoryService accessoryService, IUserService userService)
        {
            _accessoryService = accessoryService;
            _userService = userService;
        }

        [BindProperty]
        public Accessory Accessory { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var username = HttpContext.Session.GetString("Admin");
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để truy cập trang này.";
                return RedirectToPage("/Auth/Login");
            }

            var CurrentUser = await _userService.GetUserByUsernameAsync(username);
            if (CurrentUser == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToPage("/Auth/Login");
            }
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
            var username = HttpContext.Session.GetString("Admin");
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để truy cập trang này.";
                return RedirectToPage("/Auth/Login");
            }

            var CurrentUser = await _userService.GetUserByUsernameAsync(username);
            if (CurrentUser == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToPage("/Auth/Login");
            }
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
