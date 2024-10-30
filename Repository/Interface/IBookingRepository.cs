using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllBookingsAsync();
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task AddBookingAsync(Booking booking);
        Task UpdateBookingAsync(Booking booking);
        Task DeleteBookingAsync(int bookingId);
        Task<Booking> GetUserByBookingAsync(string contractName);
        Task<List<Booking>> GetAllBookingByOwnersAsync(int ownerId);
        // update status booking
        Task UpdateBookingPartialAsync(Booking selectedBooking);

    }
}
