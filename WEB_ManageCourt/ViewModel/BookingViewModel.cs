using Model;
using System.ComponentModel.DataAnnotations;

namespace WEB_ManageCourt.ViewModel
{
    public class BookingViewModel
    {
        [Required]
        public int CourtId { get; set; }

        [Required]
        public DateOnly BookingDate { get; set; }

        [Required(ErrorMessage = "Please select at least one time slot.")]
        public List<int> SelectedTimeSlotIds { get; set; } = new List<int>();

        [Required]
        [StringLength(100, ErrorMessage = "Contact name cannot exceed 100 characters.")]
        public string ContactName { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string ContactEmail { get; set; } = null!;

        [Required]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string ContactPhone { get; set; } = null!;

        [Required]
        public string PaymentMethod { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public string? Note { get; set; }
        public List<BookingAccessory> SelectedAccessories { get; set; } = new List<BookingAccessory>();
    }
}
