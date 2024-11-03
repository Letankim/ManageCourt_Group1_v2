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
