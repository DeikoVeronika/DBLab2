using System;
using System.Collections.Generic;

namespace BeautySpaceDomain.Model;

public partial class Category : Entity
{
    public string Name { get; set; } = null!;

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
