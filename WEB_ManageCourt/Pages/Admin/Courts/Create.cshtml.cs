using System.Threading.Tasks;
using BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Model;
using System.Linq;
using WEB_ManageCourt.Services;

namespace WEB_ManageCourt.Pages.Admin.Courts
{
    public class CreateModel : PageModel
    {
        private readonly IBadmintonCourtService _courtService;
        private readonly ICourtImageService _courtImageService;
        private readonly IUserService _userService;

        public CreateModel(IBadmintonCourtService courtService, IUserService userService, ICourtImageService courtImageService)
        {
            _courtService = courtService;
            _userService = userService;
            _courtImageService = courtImageService;
        }

        [BindProperty]
        public BadmintonCourt BadmintonCourt { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
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
            var users = (await _userService.GetAllCourtOwnerAsync()).ToList();

            var selectListItems = new List<SelectListItem>();
            foreach (var u in users)
            {
                selectListItems.Add(new SelectListItem
                {
                    Value = u.UserId.ToString(), 
                    Text = $"{u.Username} - {u.FullName} - {u.Email}" 
                });
            }

            ViewData["OwnerId"] = selectListItems;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(List<IFormFile> courtImages)
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

            await _courtService.AddCourtAsync(BadmintonCourt);
            ImageService imageService = new ImageService();
            if (courtImages != null && courtImages.Count > 0)
            {
                foreach (var image in courtImages)
                {
                    if (image.Length > 0)
                    {
                        var courtImage = new CourtImage
                        {
                            CourtId = BadmintonCourt.CourtId, 
                            ImageUrl = await imageService.SaveImageAsync(image) 
                        };

                        await _courtImageService.AddImageAsync(courtImage);
                    }
                }
            }

            return RedirectToPage("./Index");
        }

    }
}
