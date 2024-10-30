using System;
using System.Collections.Generic;

namespace Model;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int? CourtId { get; set; }

    public DateOnly BookingDate { get; set; }

    public string TimeSlot { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public string? Note { get; set; }

    public string? ContactName { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? PaymentStatus { get; set; }

    public string? BookingStatus { get; set; }

    public virtual ICollection<BookingAccessory> BookingAccessories { get; set; } = new List<BookingAccessory>();

    public virtual BadmintonCourt? Court { get; set; }

    public virtual User? User { get; set; }
}
