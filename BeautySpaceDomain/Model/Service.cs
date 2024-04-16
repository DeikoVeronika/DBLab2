using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeautySpaceDomain.Model;

public partial class Service : Entity
{
    private string _name;

    [Required(ErrorMessage = "Введіть назву послуги")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Назва послуги не може містити у собі менше двох символів")]
    [RegularExpression(@"^.*[а-яА-Яa-zA-Z].*[а-яА-Яa-zA-Z].*$", ErrorMessage = "Назва послуги повинна містити принаймні дві букви")]
    [Display(Name = "Послуга")]
    public string Name
    {
        get => _name;
        set => _name = value.Substring(0, 1).ToUpperInvariant() + value.Substring(1);
    }
    [Display(Name = "Опис")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Введіть ціну послуги")]
    [RegularExpression(@"^.*[0-9].*$", ErrorMessage = "Ціна повинна містити лише цифри ")]
    [Range(0, 100000, ErrorMessage = "Ціна не може бути від'ємною та більшою за 100000")]
    [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
    [Display(Name = "Ціна ₴")]
    public decimal Price { get; set; }

    [Display(Name = "Категорія")]
    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<EmployeeService> EmployeeServices { get; set; } = new List<EmployeeService>();
}







