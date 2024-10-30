using System;
using System.Collections.Generic;

namespace Model;

public partial class UserOtp
{
    public int UserOtpid { get; set; }

    public int UserId { get; set; }

    public string Otpcode { get; set; } = null!;

    public DateTime GeneratedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
