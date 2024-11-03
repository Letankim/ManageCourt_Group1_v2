using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interface;
using Model;
using WEB_ManageCourt.ViewModel;
using Newtonsoft.Json;
using WEB_ManageCourt.Services;
using WEB_ManageCourt.VNPAY;

namespace WEB_ManageCourt.Pages.BookingCourt
{
    public class IndexModel : PageModel
    {
        private readonly IBadmintonCourtService _courtService;
        private readonly IUserService _userService;
        private readonly IBookingService _bookingService;
        private readonly ICourtScheduleService _scheduleService;
        private readonly IAccessoryService _accessoryService;

        public IndexModel(IBadmintonCourtService courtService, IUserService userService, IBookingService bookingService, ICourtScheduleService scheduleService, IAccessoryService accessoryService)
        {
            _courtService = courtService;
            _userService = userService;
            _bookingService = bookingService;
            _scheduleService = scheduleService;
            _accessoryService = accessoryService;
        }

        [BindProperty]
        public int CourtId { get; set; }

        [BindProperty]
        public DateOnly BookingDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [BindProperty]
        public int SelectedTimeSlotId { get; set; }
        [BindProperty]
        public string ContactName { get; set; }

        [BindProperty]
        public string ContactEmail { get; set; }

        [BindProperty]
        public string ContactPhone { get; set; }
        [BindProperty]
        public string? Note { get; set; }

        [BindProperty]
        public string PaymentMethod { get; set; }
        [BindProperty]
        public decimal TotalPrice { get; set; }

        [BindProperty]
        public decimal? CourtPricePerHour { get; set; }
        [BindProperty]
        public string SelectedTimeSlots { get; set; }

        [BindProperty]
        public List<BookingAccessory> SelectedAccessories { get; set; }

        [BindProperty]
        public List<TimeSlotModel> AvailableTimeSlots { get; set; } = new List<TimeSlotModel>();
        public List<Accessory> Accessories { get; set; } = new List<Accessory>();

        public User CurrentUser { get; private set; }

        public async Task<IActionResult> OnGetAsync(int courtId)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để truy cập trang cá nhân.";
                return RedirectToPage("/Auth/Login");
            }

            CurrentUser = await _userService.GetUserByUsernameAsync(username);
            if (CurrentUser == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToPage("/Auth/Login");
            }
            CourtId = courtId;
            ContactEmail = CurrentUser.Email;
            ContactName = CurrentUser.FullName;
            ContactPhone = CurrentUser.Phone;
            var court = await  _courtService.GetCourtByIdAsync(courtId);
            if (court == null)
            {
                return NotFound();
            }
            CourtPricePerHour = court.PricePerHour;
            var schedules = await _scheduleService.GetAvailableSchedulesAsync(CourtId, BookingDate);
            AvailableTimeSlots = schedules.Select(schedule => new TimeSlotModel
            {
                TimeSlotId = schedule.ScheduleId,
                Time = schedule.TimeSlot,
                IsAvailable = schedule.IsAvailable ?? false
            }).ToList();

            Accessories = (await _accessoryService.GetListAllAccessoriesAsync()).ToList();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để truy cập trang cá nhân.";
                return RedirectToPage("/Auth/Login");
            }

            CurrentUser = await _userService.GetUserByUsernameAsync(username);
            if (CurrentUser == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToPage("/Auth/Login");
            }
            ContactEmail = CurrentUser.Email;
            ContactName = CurrentUser.FullName;
            ContactPhone = CurrentUser.Phone;
            Accessories = (await _accessoryService.GetListAllAccessoriesAsync()).ToList();
            var selectedAccJson = Request.Form["SelectedAccessories"];
            if (!string.IsNullOrEmpty(selectedAccJson))
            {
                SelectedAccessories = JsonConvert.DeserializeObject<List<BookingAccessory>>(selectedAccJson);
            }
            else
            {
                SelectedAccessories = new List<BookingAccessory>();
            }

            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var error in ModelState)
                {
                    foreach (var subError in error.Value.Errors)
                    {
                        errors.Add($"{error.Key}: {subError.ErrorMessage}");
                    }
                }
                TempData["ModelErrors"] = errors;
                return Page();
            }

            try
            {
                var selectedSlots = JsonConvert.DeserializeObject<List<SelectedSlot>>(SelectedTimeSlots);
                string timeSlotStr = string.Join(", ", selectedSlots.Select(s => s.SlotTime));

                if (SelectedAccessories != null && SelectedAccessories.Any())
                {
                    foreach (var accessory in SelectedAccessories)
                    {
                        accessory.BookingId = 0;
                        if (accessory.Quantity <= 0)
                        {
                            accessory.Quantity = 1;
                        }
                    }
                }

                var booking = new Booking
                {
                    UserId = CurrentUser.UserId,
                    CourtId = CourtId,
                    BookingDate = BookingDate,
                    TimeSlot = timeSlotStr,
                    TotalPrice = TotalPrice,
                    ContactName = ContactName,
                    ContactEmail = ContactEmail,
                    ContactPhone = ContactPhone,
                    PaymentMethod = PaymentMethod,
                    PaymentStatus = PaymentMethod != "Online" ? "Completed" : "Pending",
                    BookingAccessories = SelectedAccessories,
                    BookingStatus = "NoShow",
                    Note = Note
                };

                if (PaymentMethod == "Online")
                {
                    HttpContext.Session.SetString("PendingBooking", JsonConvert.SerializeObject(booking));
                    HttpContext.Session.SetString("SelectedSlots", JsonConvert.SerializeObject(selectedSlots));


                    string orderId;
                    string paymentUrl = VnPayHelper.CreatePaymentUrl(TotalPrice, $"Booking for court {CourtId} on {BookingDate:yyyy-MM-dd}", out orderId);
                    return Redirect(paymentUrl);
                }

                await _bookingService.AddBookingOrderAsync(booking);
                foreach(var slot in selectedSlots)
                {
                    await _scheduleService.MarkScheduleAsUnavailableAsync(slot.SlotId);
                } 
                EmailTemplateGenerator emailTemplate = new EmailTemplateGenerator();
                EmailService emailService = new EmailService();
                string templateSendConfirm = emailTemplate.GenerateBookingConfirmationEmail(ContactName, BookingDate.ToString("yyyy-MM-dd"), timeSlotStr, TotalPrice, ContactEmail, ContactPhone, Note, PaymentMethod);
                await emailService.SendEmailAsync(ContactEmail, "Xác nhận đặt sân cầu lông", templateSendConfirm);

                TempData["ConfirmationMessage"] = "Booking successfully created!";
                TempData["ContactName"] = ContactName;
                TempData["BookingDate"] = BookingDate.ToString("yyyy-MM-dd");
                TempData["TimeSlot"] = timeSlotStr;
                TempData["TotalPrice"] = TotalPrice.ToString("N0");

                return RedirectToPage("/BookingCourt/Confirmation");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your booking. Please try again. " + ex.Message;
                return RedirectToPage("/BookingCourt/ErrorBooking");
            }
        }

        public async Task<PartialViewResult> OnGetLoadTimeSlotsAsync(int courtId, DateOnly bookingDate)
        {
            var schedules = await _scheduleService.GetAvailableSchedulesAsync(courtId, bookingDate);
            var availableTimeSlots = schedules.Select(schedule => new TimeSlotModel
            {
                TimeSlotId = schedule.ScheduleId,
                Time = schedule.TimeSlot,
                IsAvailable = schedule.IsAvailable ?? false
            }).ToList();

            return Partial("_TimeSlotsPartial", availableTimeSlots);
        }
    }
}
