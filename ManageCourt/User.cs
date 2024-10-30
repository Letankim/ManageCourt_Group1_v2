using System;
using System.Collections.Generic;

namespace Model;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool? IsEnabled { get; set; }

    public virtual ICollection<BadmintonCourt> BadmintonCourts { get; set; } = new List<BadmintonCourt>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<UserOtp> UserOtps { get; set; } = new List<UserOtp>();
}
