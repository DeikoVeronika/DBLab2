using System;
using System.Collections.Generic;

namespace BeautySpaceDomain.Model;

public partial class Position : Entity
{
    public string Name { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
