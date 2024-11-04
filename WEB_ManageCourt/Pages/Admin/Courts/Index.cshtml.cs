using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogic.Interface;
using BusinessLogic.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using OfficeOpenXml;

namespace WEB_ManageCourt.Pages.Admin.Courts
{
    public class IndexModel : PageModel
    {
        private readonly IBadmintonCourtService _courtService;
        private readonly IUserService _userService;
        public IndexModel(IBadmintonCourtService courtService, IUserService userService)
        {
            _courtService = courtService;
            _userService = userService;
        }

        public IList<BadmintonCourt> BadmintonCourt { get; set; } = default!;

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
            BadmintonCourt = (await _courtService.GetListAllCourtsAsync()).ToList();
            return Page();
        }

        public async Task<JsonResult> OnGetToggleStatusAsync(int? id)
        {
            if (id == null)
            {
                return new JsonResult(new { success = false, message = "Invalid ID." });
            }

            var court = await _courtService.GetCourtByIdAsync(id.Value);
            if (court != null)
            {
                court.IsEnabled = !court.IsEnabled;
                await _courtService.UpdateCourtAsync(court);
                return new JsonResult(new { success = true, isEnabled = court.IsEnabled, message = "Status toggled successfully." });
            }
            else
            {
                return new JsonResult(new { success = false, message = "Court not found." });
            }
        }

        public async Task<IActionResult> OnPostImportExcelAsync(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "File không hợp lệ.");
                return Page();
            }

            var courts = new List<BadmintonCourt>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    if (package.Workbook.Worksheets.Count == 0)
                    {
                        
                        ModelState.AddModelError(string.Empty, "Tệp Excel không chứa bất kỳ worksheet nào.");
                        return Page();
                    }

                    var worksheet = package.Workbook.Worksheets[0];

                    if (worksheet.Dimension == null)
                    {
                        
                        ModelState.AddModelError(string.Empty, "Worksheet không có dữ liệu.");
                        return Page();
                    }

                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        
                        var court = new BadmintonCourt
                        {
                            OwnerId = int.TryParse(worksheet.Cells[row, 1].Value?.ToString(), out var ownerId) ? ownerId : (int?)null,
                            CourtName = worksheet.Cells[row, 2].Value?.ToString(),
                            Location = worksheet.Cells[row, 3].Value?.ToString(),
                            PricePerHour = decimal.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out var price) ? price : 0,
                            Description = worksheet.Cells[row, 5].Value?.ToString(),
                            MapsLink = worksheet.Cells[row, 6].Value?.ToString(),
                            StartTime = TimeOnly.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out var startTime) ? startTime : TimeOnly.MinValue,
                            EndTime = TimeOnly.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out var endTime) ? endTime : TimeOnly.MaxValue,
                            IsEnabled = worksheet.Cells[row, 9].Value?.ToString() == "True"
                        };

                        courts.Add(court);
                    }
                }
            }

            foreach (var court in courts)
            {
                await _courtService.AddCourtAsync(court);
            }

            return RedirectToPage("./Index");
        }
    }
}
