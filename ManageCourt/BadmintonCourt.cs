using System;
using System.Collections.Generic;

namespace Model;

public partial class BadmintonCourt
{
    public int CourtId { get; set; }

    public int? OwnerId { get; set; }

    public string CourtName { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string? Description { get; set; }

    public string? MapsLink { get; set; }

    public decimal PricePerHour { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool? IsEnabled { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<CourtImage> CourtImages { get; set; } = new List<CourtImage>();

    public virtual ICollection<CourtSchedule> CourtSchedules { get; set; } = new List<CourtSchedule>();

    public virtual User? Owner { get; set; }
}
