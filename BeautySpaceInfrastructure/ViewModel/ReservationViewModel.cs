using BeautySpaceDomain.Model;
using Humanizer.Localisation;

namespace BeautySpaceInfrastructure.ViewModel
{
    public class ReservationViewModel
    {
        public Reservation Reservation { get; set; } = null!;
        public int CategoryId { get; set; }
        public int ServiceId { get; set; }
        public int EmployeeServiceId { get; set; }
        public int TimeSlotId { get; set; }
        public int ClientId { get; set; }

        public List<Category> Categories { get; set; }
        public List<Client> Clients { get; set; }
        public List<EmployeeService> EmployeeServices { get; set; }
        public List<TimeSlot> TimeSlots { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Service> Services { get; set; }

    }
}
