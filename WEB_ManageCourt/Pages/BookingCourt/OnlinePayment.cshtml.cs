using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using Newtonsoft.Json;
using WEB_ManageCourt.VNPAY;
using WEB_ManageCourt.Services;
using System.Threading.Tasks;
using BusinessLogic.Interface;
using System.Collections.Generic;
using WEB_ManageCourt.ViewModel;

namespace WEB_ManageCourt.Pages.BookingCourt
{
    public class OnlinePaymentModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly EmailService _emailService;
        private readonly ICourtScheduleService _scheduleService;
        private readonly string vnp_HashSecret = "YOUR_HASH_SECRET_KEY"; // Replace with your actual secret key

        public string PaymentStatusMessage { get; set; }

        public OnlinePaymentModel(IBookingService bookingService, EmailService emailService, ICourtScheduleService scheduleService)
        {
            _bookingService = bookingService;
            _emailService = emailService;
            _scheduleService = scheduleService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var queryParameters = HttpContext.Request.Query;
            VnPayLibrary vnpay = new VnPayLibrary();

            foreach (var param in queryParameters)
            {
                if (param.Key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(param.Key, param.Value);
                }
            }
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");

            if (vnp_ResponseCode == "00")
            {
                var pendingBookingJson = HttpContext.Session.GetString("PendingBooking");
                var selectedSlotsJson = HttpContext.Session.GetString("SelectedSlots");

                if (string.IsNullOrEmpty(pendingBookingJson) || string.IsNullOrEmpty(selectedSlotsJson))
                {
                    PaymentStatusMessage = "Không tìm thấy thông tin đặt chỗ hoặc thông tin thời gian.";
                    TempData["ErrorMessage"] = PaymentStatusMessage;
                    return Page();
                }

                var pendingBooking = JsonConvert.DeserializeObject<Booking>(pendingBookingJson);
                var selectedSlots = JsonConvert.DeserializeObject<List<SelectedSlot>>(selectedSlotsJson);

                pendingBooking.PaymentStatus = "Completed";
                await _bookingService.AddBookingOrderAsync(pendingBooking);

                EmailTemplateGenerator emailTemplate = new EmailTemplateGenerator();
                string templateSendConfirm = emailTemplate.GenerateBookingConfirmationEmail(
                    pendingBooking.ContactName,
                    pendingBooking.BookingDate.ToString("yyyy-MM-dd"),
                    pendingBooking.TimeSlot,
                    pendingBooking.TotalPrice,
                    pendingBooking.ContactEmail,
                    pendingBooking.ContactPhone,
                    pendingBooking.Note,
                    pendingBooking.PaymentMethod
                );
                await _emailService.SendEmailAsync(pendingBooking.ContactEmail, "Xác nhận đặt sân cầu lông", templateSendConfirm);

                foreach (var slot in selectedSlots)
                {
                    await _scheduleService.MarkScheduleAsUnavailableAsync(slot.SlotId);
                }

                TempData["ConfirmationMessage"] = "Booking successfully created!";
                TempData["ContactName"] = pendingBooking.ContactName;
                TempData["BookingDate"] = pendingBooking.BookingDate.ToString("yyyy-MM-dd");
                TempData["TimeSlot"] = pendingBooking.TimeSlot;
                TempData["TotalPrice"] = pendingBooking.TotalPrice.ToString("N0");

                HttpContext.Session.Remove("PendingBooking");
                HttpContext.Session.Remove("SelectedSlots");

                PaymentStatusMessage = $"Thanh toán cho đơn đặt chỗ thành công!";
                TempData["SuccessMessage"] = PaymentStatusMessage;
                return RedirectToPage("/BookingCourt/Confirmation");
            }
            else
            {
                PaymentStatusMessage = $"Thanh toán cho đơn đặt chỗ thất bại hoặc không hợp lệ. Mã lỗi: {vnp_ResponseCode}";
                TempData["ErrorMessage"] = PaymentStatusMessage;
                return Page();
            }
        }
    }
}
