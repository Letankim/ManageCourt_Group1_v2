using System;
using System.Collections.Generic;

namespace Model;

public partial class Accessory
{
    public int AccessoryId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<BookingAccessory> BookingAccessories { get; set; } = new List<BookingAccessory>();
}
