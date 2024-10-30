using Microsoft.EntityFrameworkCore;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class BookingAccessoryDAO : SingletonBase<BookingAccessory>
    {
        public async Task<List<BookingAccessory>> GetAllBookingAccessoriesAsync()
        {
            return await _context.BookingAccessories.ToListAsync();
        }

        public async Task<BookingAccessory> GetBookingAccessoryByIdAsync(int bookingAccessoryId)
        {
            return await _context.BookingAccessories.FindAsync(bookingAccessoryId);
        }

        public async Task AddBookingAccessoryAsync(BookingAccessory bookingAccessory)
        {
            await _context.BookingAccessories.AddAsync(bookingAccessory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookingAccessoryAsync(BookingAccessory bookingAccessory)
        {
            _context.BookingAccessories.Update(bookingAccessory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookingAccessoryAsync(int bookingAccessoryId)
        {
            var bookingAccessory = await _context.BookingAccessories.FindAsync(bookingAccessoryId);
            if (bookingAccessory != null)
            {
                _context.BookingAccessories.Remove(bookingAccessory);
                await _context.SaveChangesAsync();
            }
        }
    }
}
