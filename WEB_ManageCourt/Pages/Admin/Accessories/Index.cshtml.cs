using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace WEB_ManageCourt.Pages.Admin.Accessories
{
    public class IndexModel : PageModel
    {
        private readonly IAccessoryService _accessoryService;
        private readonly IUserService _userService;

        public IndexModel(IAccessoryService accessoryService, IUserService userService)
        {
            _accessoryService = accessoryService;
            _userService = userService;
        }

        public IList<Accessory> Accessory { get; set; } = default!;

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
            Accessory = (await _accessoryService.GetListAllAccessoriesAsync()).ToList();
            return Page();
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

            var accessories = new List<Accessory>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var accessory = new Accessory
                        {
                            Name = worksheet.Cells[row, 1].Value?.ToString(),
                            Price = decimal.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out var price) ? price : 0
                        };

                        accessories.Add(accessory);
                    }
                }
            }

            foreach (var accessory in accessories)
            {
                await _accessoryService.AddAccessoryAsync(accessory);
            }

            return RedirectToPage("./Index");
        }

        public IActionResult OnGetExportExcel()
        {
            var accessories = _accessoryService.GetListAllAccessoriesAsync().Result;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Accessories");
                worksheet.Cells[1, 1].Value = "Tên Phụ kiện";
                worksheet.Cells[1, 2].Value = "Giá";

                int row = 2;
                foreach (var accessory in accessories)
                {
                    worksheet.Cells[row, 1].Value = accessory.Name;
                    worksheet.Cells[row, 2].Value = accessory.Price;
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                var fileName = $"Accessories_Export_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}
