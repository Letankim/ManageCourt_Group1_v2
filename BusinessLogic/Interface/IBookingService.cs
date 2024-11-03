using DataAccess.DAO;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetListAllBookingsAsync();
        Task<Booking> GetBookingByIdAsync(int id);
        Task AddBookingAsync(Booking item);
        Task UpdateBookingAsync(Booking item);
        Task DeleteBookingAsync(int id);
        Task<List<Booking>> ImportFromJsonAsync(string filePath);
        Task<List<Booking>> ImportFromExcelAsync(string filePath);
        Task ExportToJsonAsync(List<Booking> bookings, string filePath);
        Task ExportToExcelAsync(List<Booking> bookings, string filePath);
        Task<Booking> GetUserByBookingAsync(string contractName);
        Task<List<Booking>> GetAllBookingByOwnersAsync(int ownerId);
        //update status booking 
        Task UpdateBookingPartialAsync(Booking selectedBooking);

        Task AddBookingOrderAsync(Booking booking);

        Task<List<Booking>> GetBookingsByUserIdAsync(int userId);

        Task<Booking> GetBookingDetailByUserIdAndBookingIdAsync(int userId, int bookingId);

        Task ChangeBookingStatusAsync(int bookingId, string status);
    }
}
