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
        private readonly IUserService _userService;

        public DetailsModel(IAccessoryService accessoryService,  IUserService userService)
        {
            _accessoryService = accessoryService;
            _userService = userService;
        }

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
