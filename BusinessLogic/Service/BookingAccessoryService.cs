using BusinessLogic.Interface;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Interface;
using static DataAccess.DAO.BookingAccessoryDAO;

namespace BusinessLogic.Service
{
    public class BookingAccessoryService : IBookingAccessoryService
    {
        private readonly IBookingAccessoryRepository bookingAccessoryRepository;

        public BookingAccessoryService(IBookingAccessoryRepository bookingAccessoryRepository)
        {
            this.bookingAccessoryRepository = bookingAccessoryRepository;
        }

        public async Task AddBookingAccessoryAsync(BookingAccessory item)
        {
            await bookingAccessoryRepository.AddBookingAccessoryAsync(item);
        }

        public async Task DeleteBookingAccessoryAsync(int id)
        {
            await bookingAccessoryRepository.DeleteBookingAccessoryAsync(id);
        }

        public async Task<BookingAccessory> GetBookingAccessoryByIdAsync(int id)
        {
            return await bookingAccessoryRepository.GetBookingAccessoryByIdAsync(id);
        }

        public async Task<IEnumerable<BookingAccessory>> GetListAllBookingAccessoriesAsync()
        {
            return await bookingAccessoryRepository.GetAllBookingAccessoriesAsync();
        }

        public async Task UpdateBookingAccessoryAsync(BookingAccessory item)
        {
            await bookingAccessoryRepository.UpdateBookingAccessoryAsync(item);
        }

        public async Task<List<AccessorySalesReport>> GetAccessorySalesReport(DateOnly startDate, DateOnly endDate)
        {
            return await bookingAccessoryRepository.GetAccessorySalesReport(startDate, endDate);
        }

    }
}
