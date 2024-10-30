using DataAccess.DAO;
using Model;
using Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDAO _bookingDAO;

        public BookingRepository(BookingDAO bookingDAO)
        {
            _bookingDAO = bookingDAO;
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _bookingDAO.GetAllBookingsAsync();
        }

        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            return await _bookingDAO.GetBookingByIdAsync(bookingId);
        }

        public async Task AddBookingAsync(Booking booking)
        {
            await _bookingDAO.AddBookingAsync(booking);
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            await _bookingDAO.UpdateBookingAsync(booking);
        }

        public async Task DeleteBookingAsync(int bookingId)
        {
            var booking = await _bookingDAO.GetBookingByIdAsync(bookingId);
            if (booking != null)
            {
                await _bookingDAO.DeleteBookingAsync(bookingId);
            }
        }
    }
}
