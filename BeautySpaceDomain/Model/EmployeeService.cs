﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeautySpaceDomain.Model;

public partial class EmployeeService : Entity
{
    public int ServiceId { get; set; }

    [Display(Name = "Працівник")]
    public int EmployeeId { get; set; }

    [Display(Name = "Працівник")]
    public virtual Employee Employee { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;

    public virtual ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
}
