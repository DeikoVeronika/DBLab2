using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeautySpaceDomain.Model;

public partial class Category : Entity
{
    private string _name;

    [Required(ErrorMessage = "Введіть назву категорії")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Назва категорії не може містити у собі менше двох символів")]
    [Display(Name="Категорія")]
    public string Name
    {
        get => _name;
        set => _name = value.Substring(0, 1).ToUpperInvariant() + value.Substring(1);
    }
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
