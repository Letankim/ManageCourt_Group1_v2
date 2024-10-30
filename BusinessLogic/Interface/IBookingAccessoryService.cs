using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface IBookingAccessoryService
    {
        Task<IEnumerable<BookingAccessory>> GetListAllBookingAccessoriesAsync();
        Task<BookingAccessory> GetBookingAccessoryByIdAsync(int id);
        Task AddBookingAccessoryAsync(BookingAccessory item);
        Task UpdateBookingAccessoryAsync(BookingAccessory item);
        Task DeleteBookingAccessoryAsync(int id);
    }
}
