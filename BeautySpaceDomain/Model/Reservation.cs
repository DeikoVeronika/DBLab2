using System;
using System.Collections.Generic;

namespace BeautySpaceDomain.Model;

public partial class Reservation : Entity
{
    public int ClientId { get; set; }

    public string Info { get; set; } = null!;

    public int TimeSlotId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual TimeSlot TimeSlot { get; set; } = null!;
}
