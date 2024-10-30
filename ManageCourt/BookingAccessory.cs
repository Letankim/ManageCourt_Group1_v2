using System;
using System.Collections.Generic;

namespace Model;

public partial class BookingAccessory
{
    public int BookingAccessoryId { get; set; }

    public int? BookingId { get; set; }

    public int? AccessoryId { get; set; }

    public int Quantity { get; set; }

    public virtual Accessory? Accessory { get; set; }

    public virtual Booking? Booking { get; set; }
}
