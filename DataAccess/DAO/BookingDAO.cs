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
            return await _context.Bookings.ToListAsync();
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
            return await _context.Bookings.FindAsync(bookingId);
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
                // Update fields if provided in the selectedBooking
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

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("The booking with the specified ID was not found.");
            }
        }
    }
}
