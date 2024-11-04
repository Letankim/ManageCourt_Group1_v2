using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model;

public partial class Accessory
{
    [Key]
    public int AccessoryId { get; set; }

    [Required(ErrorMessage = "Accessory name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = null!;

    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
    public decimal Price { get; set; }

    public virtual ICollection<BookingAccessory> BookingAccessories { get; set; } = new List<BookingAccessory>();
}
