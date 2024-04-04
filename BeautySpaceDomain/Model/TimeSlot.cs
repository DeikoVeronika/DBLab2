using System;
using System.Collections.Generic;

namespace BeautySpaceDomain.Model;

public partial class TimeSlot : Entity
{
    public int EmployeeServiceId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsBooked { get; set; }

    public virtual EmployeeService EmployeeService { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
