using Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using static DataAccess.DAO.BookingAccessoryDAO;

namespace Repositories.Interface
{
    public interface IBookingAccessoryRepository
    {
        Task<List<BookingAccessory>> GetAllBookingAccessoriesAsync();
        Task<BookingAccessory> GetBookingAccessoryByIdAsync(int bookingAccessoryId);
        Task AddBookingAccessoryAsync(BookingAccessory bookingAccessory);
        Task UpdateBookingAccessoryAsync(BookingAccessory bookingAccessory);
        Task DeleteBookingAccessoryAsync(int bookingAccessoryId);
        Task<List<AccessorySalesReport>> GetAccessorySalesReport(DateOnly startDate, DateOnly endDate);
    }
}
