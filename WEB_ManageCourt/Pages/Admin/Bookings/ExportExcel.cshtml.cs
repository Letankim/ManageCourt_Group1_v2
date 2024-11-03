using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Service;
using OfficeOpenXml;

namespace WEB_ManageCourt.Pages.Admin.Bookings
{
    public class ExportExcelModel : PageModel
    {
        private readonly IBookingService _bookingService;

        public ExportExcelModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public IList<Booking> Booking { get; private set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var bookings = (await _bookingService.GetListAllBookingsAsync()).ToList();
            var stream = new MemoryStream();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Bookings");
                worksheet.Cells[1, 1].Value = "Ngày Đặt";
                worksheet.Cells[1, 2].Value = "Thời gian";
                worksheet.Cells[1, 3].Value = "Tổng Giá";
                worksheet.Cells[1, 4].Value = "Tên người đặt";
                worksheet.Cells[1, 5].Value = "Phương thức Thanh toán";
                worksheet.Cells[1, 6].Value = "Trạng thái đặt chỗ";
                worksheet.Cells[1, 7].Value = "Tên Sân";
                worksheet.Cells[1, 8].Value = "Người dùng";

                for (int i = 0; i < bookings.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = bookings[i].BookingDate.ToString("dd/MM/yyyy");
                    worksheet.Cells[i + 2, 2].Value = bookings[i].TimeSlot;
                    worksheet.Cells[i + 2, 3].Value = bookings[i].TotalPrice;
                    worksheet.Cells[i + 2, 4].Value = bookings[i].ContactName;
                    worksheet.Cells[i + 2, 5].Value = bookings[i].PaymentMethod;
                    worksheet.Cells[i + 2, 6].Value = bookings[i].BookingStatus;
                    worksheet.Cells[i + 2, 7].Value = bookings[i].Court.CourtName;
                    worksheet.Cells[i + 2, 8].Value = bookings[i].User.Username;
                }

                package.SaveAs(stream);
            }

            var fileName = $"Booking_Export_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }




    }
}
