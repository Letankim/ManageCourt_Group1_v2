using DataAccess.DAO;
using Model;
using Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class BookingAccessoryRepository : IBookingAccessoryRepository
    {
        private readonly BookingAccessoryDAO _bookingAccessoryDAO;

        public BookingAccessoryRepository(BookingAccessoryDAO bookingAccessoryDAO)
        {
            _bookingAccessoryDAO = bookingAccessoryDAO;
        }

        public async Task<List<BookingAccessory>> GetAllBookingAccessoriesAsync()
        {
            return await _bookingAccessoryDAO.GetAllBookingAccessoriesAsync();
        }

        public async Task<BookingAccessory> GetBookingAccessoryByIdAsync(int bookingAccessoryId)
        {
            return await _bookingAccessoryDAO.GetBookingAccessoryByIdAsync(bookingAccessoryId);
        }

        public async Task AddBookingAccessoryAsync(BookingAccessory bookingAccessory)
        {
            await _bookingAccessoryDAO.AddBookingAccessoryAsync(bookingAccessory);
        }

        public async Task UpdateBookingAccessoryAsync(BookingAccessory bookingAccessory)
        {
            await _bookingAccessoryDAO.UpdateBookingAccessoryAsync(bookingAccessory);
        }

        public async Task DeleteBookingAccessoryAsync(int bookingAccessoryId)
        {
            var bookingAccessory = await _bookingAccessoryDAO.GetBookingAccessoryByIdAsync(bookingAccessoryId);
            if (bookingAccessory != null)
            {
                await _bookingAccessoryDAO.DeleteBookingAccessoryAsync(bookingAccessoryId);
            }
        }
    }
}
