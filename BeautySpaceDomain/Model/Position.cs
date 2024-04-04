using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeautySpaceDomain.Model;

public partial class Position : Entity
{
    [Required(ErrorMessage = "Введіть назву посади")]
    [Display(Name = "Посада")]
    public string Name { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
