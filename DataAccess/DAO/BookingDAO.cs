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
    }
}
