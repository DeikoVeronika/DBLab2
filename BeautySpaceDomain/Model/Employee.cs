using System;
using System.Collections.Generic;

namespace BeautySpaceDomain.Model;

public partial class Employee : Entity
{
    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public int PositionId { get; set; }

    public byte[]? EmployeePortrait { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public virtual ICollection<EmployeeService> EmployeeServices { get; set; } = new List<EmployeeService>();

    public virtual Position Position { get; set; } = null!;
}
