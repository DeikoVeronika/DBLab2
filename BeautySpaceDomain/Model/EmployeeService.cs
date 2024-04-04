using System;
using System.Collections.Generic;

namespace BeautySpaceDomain.Model;

public partial class EmployeeService : Entity
{
    public int ServiceId { get; set; }

    public int EmployeeId { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;

    public virtual ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
}
