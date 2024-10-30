using System;
using System.Collections.Generic;

namespace Model;

public partial class CourtSchedule
{
    public int ScheduleId { get; set; }

    public int? CourtId { get; set; }

    public DateOnly Date { get; set; }

    public string TimeSlot { get; set; } = null!;

    public bool? IsAvailable { get; set; }

    public virtual BadmintonCourt? Court { get; set; }
}
