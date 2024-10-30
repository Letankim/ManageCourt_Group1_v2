using System;
using System.Collections.Generic;

namespace Model;

public partial class EnableLog
{
    public int LogId { get; set; }

    public string? EntityType { get; set; }

    public int EntityId { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime? ChangedDate { get; set; }
}
