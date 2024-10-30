using BusinessLogic.Interface;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Interface;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace BusinessLogic.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            this.bookingRepository = bookingRepository;
        }

        public async Task AddBookingAsync(Booking item)
        {
            await bookingRepository.AddBookingAsync(item);
        }

        public async Task DeleteBookingAsync(int id)
        {
            await bookingRepository.DeleteBookingAsync(id);
        }

        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            return await bookingRepository.GetBookingByIdAsync(id);
        }

        public async Task<IEnumerable<Booking>> GetListAllBookingsAsync()
        {
            return await bookingRepository.GetAllBookingsAsync();
        }

        public async Task UpdateBookingAsync(Booking item)
        {
            await bookingRepository.UpdateBookingAsync(item);
        }

        public async Task ExportToJsonAsync(List<Booking> bookings, string filePath)
        {
            string json = JsonConvert.SerializeObject(bookings, Newtonsoft.Json.Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<List<Booking>> ImportFromJsonAsync(string filePath)
        {
            string json = await File.ReadAllTextAsync(filePath);
            var bookings = JsonConvert.DeserializeObject<List<Booking>>(json);
            var importedBookings = new List<Booking>();

            foreach (var booking in bookings)
            {
                try
                {
                    // Check for existing user by contact name
                    var existingUser = await bookingRepository.GetUserByBookingAsync(booking.ContactName);
                    if (existingUser != null)
                    {
                        Console.WriteLine($"Duplicate username {booking.ContactName} found. Skipping...");
                        continue;
                    }

                    await bookingRepository.AddBookingAsync(booking);
                    importedBookings.Add(booking);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving booking for {booking.ContactName}: {ex.Message}");
                }
            }

            return importedBookings;
        }

        public async Task<List<Booking>> ImportFromExcelAsync(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var bookings = new List<Booking>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file was not found.", filePath);
            }

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    if (worksheet.Dimension == null)
                    {
                        throw new Exception("The Excel file appears to be empty.");
                    }

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var contactName = worksheet.Cells[row, 1].Value?.ToString(); // Contact Name
                        var contactEmail = worksheet.Cells[row, 2].Value?.ToString(); // Contact Email
                        var contactPhone = worksheet.Cells[row, 3].Value?.ToString(); // Contact Phone
                        var bookingDateString = worksheet.Cells[row, 4].Value?.ToString(); // Booking Date
                        var timeSlot = worksheet.Cells[row, 5].Value?.ToString(); // Time Slot
                        var totalPriceString = worksheet.Cells[row, 6].Value?.ToString(); // Total Price
                        var paymentMethod = worksheet.Cells[row, 7].Value?.ToString(); // Payment Method
                        var paymentStatus = worksheet.Cells[row, 8].Value?.ToString(); // Payment Status
                        var bookingStatus = worksheet.Cells[row, 9].Value?.ToString(); // Booking Status

                        // Validate contact name and booking date
                        if (string.IsNullOrWhiteSpace(contactName) || string.IsNullOrWhiteSpace(bookingDateString))
                        {
                            Console.WriteLine("Skipping row due to empty contact name or booking date.");
                            continue;
                        }

                        // Check for existing user by contact name
                        var existingUser = await bookingRepository.GetUserByBookingAsync(contactName);
                        if (existingUser != null)
                        {
                            Console.WriteLine($"Duplicate username {contactName} found. Skipping...");
                            continue;
                        }

                        // Parse the booking date
                        if (!DateOnly.TryParse(bookingDateString, out var bookingDate))
                        {
                            Console.WriteLine($"Invalid booking date format in row {row}. Skipping...");
                            continue; // Skip this row if the date is invalid
                        }

                        // Parse total price
                        if (!decimal.TryParse(totalPriceString, out var totalPrice))
                        {
                            Console.WriteLine($"Invalid total price format in row {row}. Skipping...");
                            continue; // Skip this row if the price is invalid
                        }

                        // Create a new Booking object
                        var booking = new Booking
                        {
                            ContactName = contactName,
                            ContactEmail = contactEmail,
                            ContactPhone = contactPhone,
                            BookingDate = bookingDate,
                            TimeSlot = timeSlot,
                            TotalPrice = totalPrice,
                            PaymentMethod = paymentMethod,
                            PaymentStatus = paymentStatus,
                            BookingStatus = bookingStatus
                        };

                        await bookingRepository.AddBookingAsync(booking);
                        bookings.Add(booking);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error importing bookings from Excel file: {ex.Message}", ex);
            }

            return bookings;
        }

        public async Task ExportToExcelAsync(List<Booking> bookings, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.Add("Bookings");

                worksheet.Cells[1, 1].Value = "Contact Name";
                worksheet.Cells[1, 2].Value = "Contact Email";
                worksheet.Cells[1, 3].Value = "Contact Phone";
                worksheet.Cells[1, 4].Value = "Booking Date";
                worksheet.Cells[1, 5].Value = "Time Slot";
                worksheet.Cells[1, 6].Value = "Total Price";
                worksheet.Cells[1, 7].Value = "Payment Method";
                worksheet.Cells[1, 8].Value = "Payment Status";
                worksheet.Cells[1, 9].Value = "Booking Status";

                for (int i = 0; i < bookings.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = bookings[i].ContactName;
                    worksheet.Cells[i + 2, 2].Value = bookings[i].ContactEmail;
                    worksheet.Cells[i + 2, 3].Value = bookings[i].ContactPhone;
                    worksheet.Cells[i + 2, 4].Value = bookings[i].BookingDate.ToString("yyyy-MM-dd"); // Format date if necessary
                    worksheet.Cells[i + 2, 5].Value = bookings[i].TimeSlot;
                    worksheet.Cells[i + 2, 6].Value = bookings[i].TotalPrice;
                    worksheet.Cells[i + 2, 7].Value = bookings[i].PaymentMethod;
                    worksheet.Cells[i + 2, 8].Value = bookings[i].PaymentStatus;
                    worksheet.Cells[i + 2, 9].Value = bookings[i].BookingStatus;
                }

                await package.SaveAsync();
            }
        }

        public async Task<Booking> GetUserByBookingAsync(string contactName)
        {
            return await bookingRepository.GetUserByBookingAsync(contactName);
        }
        public async Task<List<Booking>> GetAllBookingByOwnersAsync(int ownerId)
        {
            return await bookingRepository.GetAllBookingByOwnersAsync(ownerId);
        }

        // update status booking
        public async Task UpdateBookingPartialAsync(Booking selectedBooking)
        {
            await bookingRepository.UpdateBookingPartialAsync(selectedBooking);
        }
    }
}
