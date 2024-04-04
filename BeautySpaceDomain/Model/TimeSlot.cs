using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeautySpaceDomain.Model;

public partial class TimeSlot : Entity
{
    public int EmployeeServiceId { get; set; }

    [Required(ErrorMessage = "Введіть дату послуги")]
    [Display(Name = "Дата")]
    public DateOnly Date { get; set; }

    [Required(ErrorMessage = "Введіть час початку послуги")]
    [Display(Name = "Початок")]
    public TimeOnly StartTime { get; set; }

    [Required(ErrorMessage = "Введіть час кінця послуги")]
    [Display(Name = "Кінець")]
    public TimeOnly EndTime { get; set; }

    [Display(Name = "Доступність")]
    public bool IsBooked { get; set; }

    public virtual EmployeeService EmployeeService { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
