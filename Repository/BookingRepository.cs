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

        public async Task<List<Booking>> GetAllBookingByOwnersAsync(int ownerId)
        {
            return await _bookingDAO.GetAllBookingByOwnersAsync(ownerId);
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
        public async Task<Booking> GetUserByBookingAsync(string contactName)
        {
            return await _bookingDAO.GetUserByBookingAsync(contactName);
        }
        // Update specific fields of booking status, payment status, and payment method
        public async Task UpdateBookingPartialAsync(Booking selectedBooking)
        {
            await _bookingDAO.UpdateBookingPartialAsync(selectedBooking);
        }
    }
}
