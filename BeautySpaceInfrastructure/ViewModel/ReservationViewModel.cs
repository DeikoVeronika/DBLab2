using BeautySpaceDomain.Model;
using Humanizer.Localisation;

namespace BeautySpaceInfrastructure.ViewModel;

public class ReservationViewModel
{
    //private readonly DbbeautySpaceContext _context;

    public Reservation? Reservation { get; set; } = null!;
    public int CategoryId { get; set; }
    public int ServiceId { get; set; }
    public int EmployeeServiceId { get; set; }
    public int TimeSlotId { get; set; }
    public int ClientId { get; set; }
    public int EmployeeId { get; set; }
    public virtual TimeSlot? TimeSlot { get; set; } = null!;
    public virtual Client? Client { get; set; } = null!;
    public string? Info { get; set; } = null!;

  
    public ReservationViewModel()
    {

    }
}
