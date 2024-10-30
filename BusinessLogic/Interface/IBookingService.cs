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
    }
}
