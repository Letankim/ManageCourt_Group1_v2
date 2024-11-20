using BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Model;
using WEB_ManageCourt.Services;
namespace WEB_ManageCourt.Pages.Admin.Courts
{
    public class EditModel : PageModel
    {
        private readonly IBadmintonCourtService _courtService;
        private readonly IUserService _userService;

        public EditModel(IBadmintonCourtService courtService, IUserService userService)
        {
            _courtService = courtService;
            _userService = userService;
        }

        [BindProperty]
        public BadmintonCourt BadmintonCourt { get; set; } = default!;

        public SelectList OwnerList { get; set; } = default!;

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

            var badmintonCourt = await _courtService.GetCourtByIdAsync(id.Value);
            if (badmintonCourt == null)
            {
                return NotFound();
            }
            BadmintonCourt = badmintonCourt;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(List<IFormFile> images)
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
                var existingCourt = await _courtService.GetCourtByIdAsync(BadmintonCourt.CourtId);
                if (existingCourt == null)
                {
                    return NotFound();
                }

                existingCourt.CourtName = BadmintonCourt.CourtName;
                existingCourt.Location = BadmintonCourt.Location;
                existingCourt.Description = BadmintonCourt.Description;
                existingCourt.MapsLink = BadmintonCourt.MapsLink;
                existingCourt.PricePerHour = BadmintonCourt.PricePerHour;
                existingCourt.StartTime = BadmintonCourt.StartTime;
                existingCourt.EndTime = BadmintonCourt.EndTime;
                existingCourt.IsEnabled = BadmintonCourt.IsEnabled;

                if (images != null && images.Count > 0)
                {
                    existingCourt.CourtImages.Clear();
                    foreach (var image in images)
                    {
                        if (image.Length > 0)
                        {
                            var imageService = new ImageService();
                            var imageUrl = await imageService.SaveImageAsync(image);
                            var courtImage = new CourtImage
                            {
                                ImageUrl = imageUrl,
                                CourtId = existingCourt.CourtId
                            };
                            existingCourt.CourtImages.Add(courtImage);
                        }
                    }
                }

                await _courtService.UpdateCourtAsync(existingCourt);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while updating the court: {ex.Message}");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}