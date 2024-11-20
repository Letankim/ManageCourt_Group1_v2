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
        private readonly IUserService _userService;

        public DeleteModel(IBadmintonCourtService courtService, IUserService  userService)
        {
            _courtService = courtService;
            _userService = userService;
        }

        [BindProperty]
        public BadmintonCourt BadmintonCourt { get; set; } = default!;

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

            await _courtService.DeleteCourtAsync(id.Value);
            return RedirectToPage("./Index");
        }
    }
}
