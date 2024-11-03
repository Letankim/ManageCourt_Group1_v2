﻿using Model;
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
        Task UpdateBookingPartialAsync(Booking selectedBooking);
        Task AddBookingOrderAsync(Booking booking);
        Task<List<Booking>> GetBookingsByUserIdAsync(int userId);
        Task<Booking> GetBookingDetailByUserIdAndBookingIdAsync(int userId, int bookingId);
        Task ChangeBookingStatusAsync(int bookingId, string status);
    }
}
