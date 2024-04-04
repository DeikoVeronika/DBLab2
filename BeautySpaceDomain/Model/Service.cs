using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeautySpaceDomain.Model;

public partial class Service : Entity
{
    [Required(ErrorMessage = "Введіть назву послуги")]
    [Display(Name = "Послуга")]
    public string Name { get; set; } = null!;

    [Display(Name = "Опис")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Введіть ціну послуги")]
    [RegularExpression(@"^.*[0-9].*$", ErrorMessage = "Ціна повинна містити лише цифри ")]
    [Range(0, 100000, ErrorMessage = "Ціна не може бути від'ємною та більшою за 100000")]
    [Display(Name = "Ціна")]
    public decimal Price { get; set; }

    [Display(Name = "Категорія")]
    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<EmployeeService> EmployeeServices { get; set; } = new List<EmployeeService>();
}







