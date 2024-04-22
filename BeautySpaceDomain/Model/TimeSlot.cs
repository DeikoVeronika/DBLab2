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
    [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Невірний формат часу")]

    public TimeOnly StartTime { get; set; }

    [Required(ErrorMessage = "Введіть час кінця послуги")]
    [Display(Name = "Кінець")]
    [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Невірний формат часу")]
    public TimeOnly EndTime { get; set; }

    [Display(Name = "Недоступний для бронювання")]
    public bool IsBooked { get; set; }

    public virtual EmployeeService? EmployeeService { get; set; } = null!;
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
