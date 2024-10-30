using System;
using System.Collections.Generic;

namespace Model;

public partial class CourtImage
{
    public int ImageId { get; set; }

    public int? CourtId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual BadmintonCourt? Court { get; set; }
}
