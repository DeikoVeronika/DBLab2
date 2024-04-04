using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeautySpaceDomain.Model;

public partial class Employee : Entity
{
    [Required(ErrorMessage = "Введіть ім'я працівника")]
    [Display(Name = "Ім'я")]

    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Введіть прізвище працівника")]
    [Display(Name = "Прізвище")]
    public string? LastName { get; set; }

    public int PositionId { get; set; }

    public byte[]? EmployeePortrait { get; set; }

    [Required(ErrorMessage = "Введіть мобільний номер працівника")]
    [Display(Name = "Мобільний номер")]
    public string PhoneNumber { get; set; } = null!;

    public virtual ICollection<EmployeeService> EmployeeServices { get; set; } = new List<EmployeeService>();

    public virtual Position Position { get; set; } = null!;
}
