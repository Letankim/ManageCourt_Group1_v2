using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model;

public partial class BadmintonCourt
{
    [Key]
    public int CourtId { get; set; }

    public int? OwnerId { get; set; }

    [Required(ErrorMessage = "Court name is required")]
    [StringLength(100, ErrorMessage = "Court name cannot exceed 100 characters")]
    public string CourtName { get; set; } = null!;

    [Required(ErrorMessage = "Location is required")]
    [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
    public string Location { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Url(ErrorMessage = "Invalid URL format")]
    public string? MapsLink { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price per hour must be a positive value")]
    public decimal PricePerHour { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    public TimeOnly StartTime { get; set; }

    [Required(ErrorMessage = "End time is required")]
    public TimeOnly EndTime { get; set; }

    public bool? IsEnabled { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<CourtImage> CourtImages { get; set; } = new List<CourtImage>();

    public virtual ICollection<CourtSchedule> CourtSchedules { get; set; } = new List<CourtSchedule>();

    public virtual User? Owner { get; set; }
}
