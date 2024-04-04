using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeautySpaceDomain.Model;

public partial class Client : Entity
{
    [Required(ErrorMessage = "Введіть ім'я клієнта")]
    [Display(Name = "Ім'я")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Введіть прізвище клієнта")]
    [Display(Name = "Прізвище")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Введіть мобільний номер клієнта")]
    [Display(Name = "Мобільний номер")]
    public string PhoneNumber { get; set; } = null!;

    [Display(Name = "Дата народження")]
    public DateOnly? Birthday { get; set; }

    [Required(ErrorMessage = "Введіть електронну пошту клієнта")]
    [Display(Name = "Електронна пошта")]
    public string Email { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
