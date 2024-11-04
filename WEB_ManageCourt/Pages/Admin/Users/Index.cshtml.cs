using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLogic.Interface;
using Model;
using OfficeOpenXml;

namespace WEB_ManageCourt.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public IList<User> User { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? RoleFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }
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
            var allUsers = await _userService.GetListAllUsersAsync();

            if (!string.IsNullOrEmpty(RoleFilter))
            {
                allUsers = allUsers.Where(u => u.Role == RoleFilter).ToList();
            }

            if (!string.IsNullOrEmpty(StatusFilter))
            {
                bool isEnabled = StatusFilter == "Enabled";
                allUsers = allUsers.Where(u => u.IsEnabled == isEnabled).ToList();
            }

            User = allUsers.ToList();
            return Page();
        }

        public async Task<PartialViewResult> OnGetToggleStatusAsync(int? id)
        {
            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user != null)
            {
                user.IsEnabled = !user.IsEnabled;
                await _userService.UpdateUserAsync(user);
            }
            var allUsers = await _userService.GetListAllUsersAsync();
            User = allUsers.ToList();
            return Partial("_UserPartial", User);
        }

        public async Task<IActionResult> OnPostImportExcelAsync(IFormFile excelFile)
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
            if (excelFile == null || excelFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "File không hợp lệ.");
                return Page();
            }

            var users = new List<User>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var user = new User
                        {
                            Username = worksheet.Cells[row, 1].Value?.ToString(),
                            FullName = worksheet.Cells[row, 2].Value?.ToString(),
                            Email = worksheet.Cells[row, 3].Value?.ToString(),
                            Phone = worksheet.Cells[row, 4].Value?.ToString(),
                            Role = worksheet.Cells[row, 5].Value?.ToString(),
                            IsEnabled = worksheet.Cells[row, 6].Value?.ToString() == "True"
                        };

                        users.Add(user);
                    }
                }
            }

            foreach (var user in users)
            {
                await _userService.AddUserAsync(user);
            }

            return RedirectToPage("./Index");
        }

        public IActionResult OnGetExportExcel()
        {
            var users = _userService.GetListAllUsersAsync().Result;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Users");
                worksheet.Cells[1, 1].Value = "Username";
                worksheet.Cells[1, 2].Value = "Full Name";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Phone";
                worksheet.Cells[1, 5].Value = "Role";
                worksheet.Cells[1, 6].Value = "Is Enabled";

                int row = 2;
                foreach (var user in users)
                {
                    worksheet.Cells[row, 1].Value = user.Username;
                    worksheet.Cells[row, 2].Value = user.FullName;
                    worksheet.Cells[row, 3].Value = user.Email;
                    worksheet.Cells[row, 4].Value = user.Phone;
                    worksheet.Cells[row, 5].Value = user.Role;
                    worksheet.Cells[row, 6].Value = user.IsEnabled;
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                var fileName = $"Users_Export_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}
