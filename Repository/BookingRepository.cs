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

        public async Task AddBookingOrderAsync(Booking booking)
        {
            try
            {
                await _bookingDAO.AddBookingOrderAsync(booking);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _bookingDAO.GetBookingsByUserIdAsync(userId);
        }

        public async Task<Booking> GetBookingDetailByUserIdAndBookingIdAsync(int userId, int bookingId)
        {
            return await _bookingDAO.GetBookingDetailByUserIdAndBookingIdAsync(userId, bookingId);
        }
        public async Task ChangeBookingStatusAsync(int bookingId, string status)
        {
            await _bookingDAO.ChangeBookingStatusAsync(bookingId, status);
        }

        public async Task<Dictionary<DateOnly, decimal>> StatisticsAsync(DateOnly startDay, DateOnly endDay, int? userID = null)
        {
            return await _bookingDAO.StatisticsAsync(startDay, endDay, userID);
        }
        public async Task<Dictionary<DateOnly, (int Confirmed, int NoShow, int Cancelled)>> StatisticStatus(DateOnly startDay, DateOnly endDay, int userID)
        {
            return await _bookingDAO.StatisticStatus(startDay, endDay, userID);
        }

        public async Task<Dictionary<DateOnly, (int AfterPlay, int Online)>> StatisticPayment(DateOnly startDay, DateOnly endDay, int userID)
        {
            return await _bookingDAO.StatisticPayment(startDay, endDay, userID);
        }

    }
}
