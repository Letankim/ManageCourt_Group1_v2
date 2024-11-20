using Microsoft.EntityFrameworkCore;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class BookingDAO : SingletonBase<Booking>
    {
        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings.Include(x => x.User)
                .Include(x => x.Court)
                .ToListAsync();
        }
        public async Task<List<Booking>> GetAllBookingByOwnersAsync(int ownerId)
        {
            return await _context.Bookings
                .Include(x => x.User)
                .Include(x => x.Court)
                .Where(x => x.Court != null && x.Court.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                                 .Include(b => b.Court)
                                 .Include(b => b.User)
                                 .Include(b => b.BookingAccessories)
                                 .ThenInclude(ba => ba.Accessory)
                                 .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }
        public async Task<Booking> GetUserByBookingAsync(string contactName)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(u => u.ContactName == contactName);
        }

        public async Task AddBookingAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }
        // update status booking
        public async Task UpdateBookingPartialAsync(Booking selectedBooking)
        {
            var booking = await _context.Bookings.FindAsync(selectedBooking.BookingId);
            if (booking != null)
            {
                if (!string.IsNullOrWhiteSpace(selectedBooking.PaymentStatus))
                {
                    booking.PaymentStatus = selectedBooking.PaymentStatus;
                    _context.Entry(booking).Property(b => b.PaymentStatus).IsModified = true;
                }

                if (!string.IsNullOrWhiteSpace(selectedBooking.BookingStatus))
                {
                    booking.BookingStatus = selectedBooking.BookingStatus;
                    _context.Entry(booking).Property(b => b.BookingStatus).IsModified = true;
                }

                if (!string.IsNullOrWhiteSpace(selectedBooking.PaymentMethod))
                {
                    booking.PaymentMethod = selectedBooking.PaymentMethod;
                    _context.Entry(booking).Property(b => b.PaymentMethod).IsModified = true;
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("The booking with the specified ID was not found.");
            }
        }

        public async Task AddBookingOrderAsync(Booking booking)
        {
            try
            {
                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var innerException = dbEx.InnerException?.Message ?? dbEx.Message;
                throw new Exception($"An error occurred while saving the booking or accessories: {innerException}", dbEx);
            }

        }

        public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.Bookings
                                 .Include(b => b.User)
                                 .Include(b => b.Court)
                                 .Include(b => b.BookingAccessories)
                                 .Where(b => b.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<Booking> GetBookingDetailByUserIdAndBookingIdAsync(int userId, int bookingId)
        {
            return await _context.Bookings
                                 .Include(b => b.User)
                                 .Include(b => b.Court)
                                 .Include(b => b.BookingAccessories)
                                    .ThenInclude(ba => ba.Accessory) 
                                 .FirstOrDefaultAsync(b => b.UserId == userId && b.BookingId == bookingId);
        }

        public async Task ChangeBookingStatusAsync(int bookingId, string status)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.BookingStatus = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<DateOnly, decimal>> StatisticsAsync(DateOnly startDay, DateOnly endDay,  int? userId = null)
        {
            var filteredBookings = await _context.Bookings
                .Where(b => b.BookingDate >= startDay
                            && b.BookingDate <= endDay
                            && b.BookingStatus == "Confirmed"
                            && b.Court.OwnerId == userId) // Lọc theo UserId
                .Include(b => b.User)  // Include User for any related data if needed
                .Include(b => b.Court) // Include Court for any related data if needed
                .ToListAsync();

            var dailyRevenue = filteredBookings
                .GroupBy(b => b.BookingDate)
                .ToDictionary(g => g.Key, g => g.Sum(b => b.TotalPrice));

            return dailyRevenue;
        }

        public async Task<Dictionary<DateOnly, (int Confirmed, int NoShow, int Cancelled)>> StatisticStatus(DateOnly startDay, DateOnly endDay, int userId)
        {
            var statistics = new Dictionary<DateOnly, (int Confirmed, int NoShow, int Cancelled)>();

            var bookingsInRange = await _context.Bookings
                .Where(b => b.BookingDate >= startDay
                            && b.BookingDate <= endDay
                            && (b.BookingStatus == "Confirmed" || b.BookingStatus == "NoShow" || b.BookingStatus == "Cancelled")
                            && b.Court.OwnerId == userId) // Lọc theo UserId
                .Include(b => b.User)   // Include User for related data if needed
                .Include(b => b.Court)  // Include Court for related data if needed
                .ToListAsync();

            foreach (var booking in bookingsInRange)
            {
                var bookingDate = booking.BookingDate;

                if (!statistics.ContainsKey(bookingDate))
                {
                    statistics[bookingDate] = (Confirmed: 0, NoShow: 0, Cancelled: 0);
                }

                switch (booking.BookingStatus?.ToLower())
                {
                    case "confirmed":
                        statistics[bookingDate] = (statistics[bookingDate].Confirmed + 1, statistics[bookingDate].NoShow, statistics[bookingDate].Cancelled);
                        break;
                    case "noshow":
                        statistics[bookingDate] = (statistics[bookingDate].Confirmed, statistics[bookingDate].NoShow + 1, statistics[bookingDate].Cancelled);
                        break;
                    case "cancelled":
                        statistics[bookingDate] = (statistics[bookingDate].Confirmed, statistics[bookingDate].NoShow, statistics[bookingDate].Cancelled + 1);
                        break;
                }
            }

            return statistics;
        }

        public async Task<Dictionary<DateOnly, (int AfterPlay, int Online)>> StatisticPayment(DateOnly startDay, DateOnly endDay, int userId)
        {
            var statistics = new Dictionary<DateOnly, (int AfterPlay, int Online)>();

            var bookingsInRange = await _context.Bookings
                .Where(b => b.BookingDate >= startDay
                            && b.BookingDate <= endDay
                            && (b.PaymentMethod == "AfterPlay" || b.PaymentMethod == "Online")
                            && b.Court.OwnerId == userId) 
                .ToListAsync();

            foreach (var booking in bookingsInRange)
            {
                var bookingDate = booking.BookingDate;

                if (!statistics.ContainsKey(bookingDate))
                {
                    statistics[bookingDate] = (AfterPlay: 0, Online: 0);
                }

                switch (booking.PaymentMethod?.ToLower())
                {
                    case "afterplay":
                        statistics[bookingDate] = (statistics[bookingDate].AfterPlay + 1, statistics[bookingDate].Online);
                        break;
                    case "online":
                        statistics[bookingDate] = (statistics[bookingDate].AfterPlay, statistics[bookingDate].Online + 1);
                        break;
                }
            }

            return statistics;
        }

    }
}
