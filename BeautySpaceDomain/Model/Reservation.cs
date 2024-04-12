using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeautySpaceDomain.Model;

public partial class Reservation : Entity
{
    [Display(Name = "Клієнт")]
    public int ClientId { get; set; }

    [Display(Name = "Інформація")]
    public string? Info { get; set; } = null!;

    public int TimeSlotId { get; set; }

    [Display(Name = "Клієнт")]
    public virtual Client? Client { get; set; } = null!;

    [Display(Name = "Час")]
    public virtual TimeSlot? TimeSlot { get; set; } = null!;
}
