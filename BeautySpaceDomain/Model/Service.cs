using System;
using System.Collections.Generic;

namespace BeautySpaceDomain.Model;

public partial class Service : Entity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<EmployeeService> EmployeeServices { get; set; } = new List<EmployeeService>();
}
