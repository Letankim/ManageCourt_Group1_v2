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
        public async Task<List<AccessorySalesReport>> GetAccessorySalesReport(DateOnly startDate, DateOnly endDate)
        {
            return await _context.BookingAccessories
                .Include(ba => ba.Accessory)
                .Include(ba => ba.Booking)
                .Where(ba => (ba.Booking == null || (ba.Booking.BookingDate >= startDate && ba.Booking.BookingDate <= endDate)))
                .GroupBy(ba => new { ba.Accessory.AccessoryId, ba.Accessory.Name, ba.Accessory.Price })
                .Select(group => new AccessorySalesReport
                {
                    AccessoryId = group.Key.AccessoryId,
                    AccessoryName = group.Key.Name,
                    Price = group.Key.Price,
                    TotalQuantitySoldWithBookings = group.Where(ba => ba.Booking != null).Sum(ba => ba.Quantity),
                    TotalRevenueWithBookings = group.Key.Price * group.Where(ba => ba.Booking != null).Sum(ba => ba.Quantity),
                    TotalQuantitySoldWithoutBookings = group.Where(ba => ba.Booking == null).Sum(ba => ba.Quantity),
                    TotalRevenueWithoutBookings = group.Key.Price * group.Where(ba => ba.Booking == null).Sum(ba => ba.Quantity)
                })
                .ToListAsync();
        }

        public class AccessorySalesReport
        {
            public int AccessoryId { get; set; }
            public string AccessoryName { get; set; }
            public decimal Price { get; set; }
            public int TotalQuantitySoldWithBookings { get; set; }
            public decimal TotalRevenueWithBookings { get; set; }
            public int TotalQuantitySoldWithoutBookings { get; set; }
            public decimal TotalRevenueWithoutBookings { get; set; }
        }


    }
}
