using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interface;
using Model;

namespace WEB_ManageCourt.Pages.Admin.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly IBadmintonCourtService _courtService;
        private readonly IAccessoryService _accessoryService;
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;

        public int ActiveCourts { get; set; }
        public int InactiveCourts { get; set; }
        public int TotalAccessories { get; set; }
        public int TotalBookings { get; set; }
        public List<string> RoleLabels { get; set; } = new List<string>();
        public List<int> RoleCounts { get; set; } = new List<int>();

        public List<string> PaymentStatuses { get; set; } = new List<string>();
        public List<int> PaymentStatusCounts { get; set; } = new List<int>();

        public List<string> BookingStatuses { get; set; } = new List<string>();
        public List<int> BookingStatusCounts { get; set; } = new List<int>();

        public List<(string UserName, int BookingCount)> TopUsers { get; set; } = new List<(string, int)>();
        public List<(string CourtName, int BookingCount)> TopCourts { get; set; } = new List<(string, int)>();
        public Dictionary<string, decimal> CourtRevenue { get; set; } = new Dictionary<string, decimal>();


        public IndexModel(IBadmintonCourtService courtService, IAccessoryService accessoryService, IBookingService bookingService, IUserService userService)
        {
            _courtService = courtService;
            _accessoryService = accessoryService;
            _bookingService = bookingService;
            _userService = userService;
        }

        public async Task<IActionResult> OnGetAsync(DateTime? startDate, DateTime? endDate, string groupBy = "Day")
        {
            endDate ??= DateTime.Now;
            startDate ??= endDate.Value.AddDays(-7);

            var bookings = (await _bookingService.GetListAllBookingsAsync())
                .Where(b => b.BookingDate >= DateOnly.FromDateTime(startDate.Value) && b.BookingDate <= DateOnly.FromDateTime(endDate.Value))
                .ToList();

            TotalBookings = bookings.Count;

            if (groupBy == "Week")
            {
                bookings = bookings.GroupBy(b => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(b.BookingDate.ToDateTime(TimeOnly.MinValue), CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                    .SelectMany(g => g)
                    .ToList();
            }
            else if (groupBy == "Month")
            {
                bookings = bookings.GroupBy(b => new { b.BookingDate.Year, b.BookingDate.Month })
                    .SelectMany(g => g)
                    .ToList();
            }
            else if (groupBy == "Quarter")
            {
                bookings = bookings.GroupBy(b => new { b.BookingDate.Year, Quarter = (b.BookingDate.Month - 1) / 3 + 1 })
                    .SelectMany(g => g)
                    .ToList();
            }
            else if (groupBy == "Year")
            {
                bookings = bookings.GroupBy(b => b.BookingDate.Year)
                    .SelectMany(g => g)
                    .ToList();
            }


            TopUsers = bookings
        .Where(b => b.User != null)
        .GroupBy(b => b.User.Username)
        .Select(g => (UserName: g.Key, BookingCount: g.Count()))
        .OrderByDescending(g => g.BookingCount)
        .Take(5)
        .ToList();

            TopCourts = bookings
                .Where(b => b.Court != null)
                .GroupBy(b => b.Court.CourtName)
                .Select(g => (CourtName: g.Key, BookingCount: g.Count()))
                .OrderByDescending(g => g.BookingCount)
                .Take(5)
                .ToList();

            CourtRevenue = bookings
                .Where(b => b.Court != null)
                .GroupBy(b => b.Court.CourtName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(b => b.TotalPrice)
                );



            var paymentStatusGroups = bookings.GroupBy(b => b.PaymentStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() }).ToList();

            foreach (var group in paymentStatusGroups)
            {
                PaymentStatuses.Add(group.Status ?? "Unknown");
                PaymentStatusCounts.Add(group.Count);
            }

            var bookingStatusGroups = bookings.GroupBy(b => b.BookingStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() }).ToList();

            foreach (var group in bookingStatusGroups)
            {
                BookingStatuses.Add(group.Status ?? "Unknown");
                BookingStatusCounts.Add(group.Count);
            }

            var courts = await _courtService.GetListAllCourtsAsync();
            ActiveCourts = courts.Count(c => c.IsEnabled == true);
            InactiveCourts = courts.Count(c => c.IsEnabled == false);

            var accessories = (await _accessoryService.GetListAllAccessoriesAsync()).ToList();
            TotalAccessories = accessories.Count;

            var users = await _userService.GetListAllUsersAsync();
            var roleGroups = users.GroupBy(u => u.Role).Select(g => new { Role = g.Key, Count = g.Count() }).ToList();

            foreach (var group in roleGroups)
            {
                RoleLabels.Add(group.Role);
                RoleCounts.Add(group.Count);
            }

            return Page();
        }
    }
}
