using BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using OfficeOpenXml;

namespace WEB_ManageCourt.Pages.Admin.Courts
{
    public class ExportExcelModel : PageModel
    {
        private readonly IBadmintonCourtService _courtService;

        public ExportExcelModel(IBadmintonCourtService courtService)
        {
            _courtService = courtService;
        }

        public IList<BadmintonCourt> BadmintonCourt { get; private set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var courts = await _courtService.GetListAllCourtsAsync(); 
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sân Cầu Lông");
                worksheet.Cells[1, 1].Value = "Tên Sân";
                worksheet.Cells[1, 2].Value = "Địa Điểm";
                worksheet.Cells[1, 3].Value = "Giá mỗi Giờ";
                worksheet.Cells[1, 4].Value = "Trạng Thái";
                worksheet.Cells[1, 5].Value = "Tên Chủ Sân";
                worksheet.Cells[1, 6].Value = "Email Chủ Sân";
                worksheet.Cells[1, 7].Value = "SĐT Chủ Sân";

                for (int i = 0; i < courts.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = courts[i].CourtName;
                    worksheet.Cells[i + 2, 2].Value = courts[i].Location;
                    worksheet.Cells[i + 2, 3].Value = courts[i].PricePerHour;
                    worksheet.Cells[i + 2, 4].Value = courts[i].IsEnabled == true ? "Kích hoạt" : "Không kích hoạt";
                    worksheet.Cells[i + 2, 5].Value = courts[i].Owner?.FullName; 
                    worksheet.Cells[i + 2, 6].Value = courts[i].Owner?.Email;
                    worksheet.Cells[i + 2, 7].Value = courts[i].Owner?.Phone;
                }
                package.Save();
            }

            stream.Position = 0;
            var excelName = $"Courts_Export_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);

        }
    }
}
