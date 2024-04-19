using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeautySpaceDomain.Model;
using BeautySpaceInfrastructure;
using DocumentFormat.OpenXml.EMMA;
using BeautySpaceInfrastructure.ViewModel;

namespace BeautySpaceInfrastructure.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly DbbeautySpaceContext _context;

        public ReservationsController(DbbeautySpaceContext context)
        {
            _context = context;
        }
        //private List<Client> GetClients()
        //{
        //    return _context.Clients.ToList();
        //}
        //private List<TimeSlot> GetTimeSlots()
        //{
        //    return _context.TimeSlots.ToList();
        //}

        //private List<Category> GetCategories()
        //{
        //    return _context.Categories.ToList();
        //}
        //private List<Service> GetServices()
        //{
        //    return _context.Services.ToList();
        //}
        //private List<EmployeeService> GetEmployeeServices()
        //{
        //    return _context.EmployeeServices.ToList();
        //}

        //private List<Employee> GetEmployees()
        //{
        //    return _context.Employees.ToList();
        //}



        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var dbbeautySpaceContext = _context.Reservations.Include(r => r.Client).Include(r => r.TimeSlot);
            return View(await dbbeautySpaceContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Client)
                .Include(r => r.TimeSlot)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        //public IActionResult Create()
        //{
        //    var viewModel = new ReservationViewModel
        //    {
        //        Clients = GetClients(),
        //        Categories = GetCategories(),
        //        Services = GetServices(),
        //        EmployeeServices = GetEmployeeServices(),
        //        Employees = GetEmployees(),
        //        TimeSlots = GetTimeSlots(), // Populate TimeSlots
        //        Reservation = new Reservation()
        //    };
        //    return View(viewModel);
        //}
        //public IActionResult Create()
        //{
        //    ViewData["ClientId"] = new SelectList(GetClientsSelectList(), "Value", "Text");
        //    ViewData["TimeSlotId"] = new SelectList(GetTimeSlotsSelectList(), "Value", "Text");
        //    return View();
        //}

        public IActionResult CreateReservation()
        {
            var reservationViewModel = new ReservationViewModel();

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "PhoneNumber");

            // Фільтруємо категорії, які мають послуги
            var categoriesWithServices = _context.Categories.Where(c => c.Services.Any()).ToList();
            ViewData["CategoryId"] = new SelectList(categoriesWithServices, "Id", "Name");

            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
            ViewData["EmployeeServiceId"] = new SelectList(_context.EmployeeServices, "Id", "Id");
            ViewData["EmployeeId"] = new SelectList(_context.Clients, "Id", "Name");
            ViewData["TimeSlotDateId"] = new SelectList(_context.TimeSlots, "Id", "Date");
            ViewData["TimeSlotTimeId"] = new SelectList(_context.TimeSlots, "Id", "StartTime");

            return View(reservationViewModel);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("ClientId,Info,TimeSlotId,Id")] ReservationViewModel reservationViewModel)
        {
            if (ModelState.IsValid)
            {
                // Перевірка на існування ідентичного бронювання
                var existingReservation = await _context.Reservations
                    .FirstOrDefaultAsync(r => r.ClientId == reservationViewModel.ClientId && r.TimeSlotId == reservationViewModel.TimeSlotId);

                if (existingReservation != null)
                {
                    ModelState.AddModelError("ClientId", "Таке бронювання для цього клієнта вже існує.");
                    ViewData["ClientId"] = new SelectList(GetClientsSelectList(), "Value", "Text", reservationViewModel.ClientId);
                    ViewData["TimeSlotId"] = new SelectList(GetTimeSlotsSelectList(), "Value", "Text", reservationViewModel.TimeSlotId);
                    return View(reservationViewModel);
                }

                var reservation = new Reservation
                {
                    ClientId = reservationViewModel.ClientId,
                    Info = reservationViewModel.Info,
                    TimeSlotId = reservationViewModel.TimeSlotId,
                    // Інші властивості резервації можуть бути встановлені тут
                };

                reservation.Info = DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy");
                _context.Add(reservation);
                await _context.SaveChangesAsync();

                // Отримати вибраний TimeSlot
                var selectedTimeSlot = await _context.TimeSlots.FindAsync(reservation.TimeSlotId);

                if (selectedTimeSlot != null)
                {
                    // Позначити TimeSlot як заброньований
                    selectedTimeSlot.IsBooked = true;
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(GetClientsSelectList(), "Value", "Text", reservationViewModel.ClientId);
            ViewData["TimeSlotId"] = new SelectList(GetTimeSlotsSelectList(), "Value", "Text", reservationViewModel.TimeSlotId);
            return View(reservationViewModel);
        }





        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            // Мапування Reservation на ReservationViewModel
            var reservationViewModel = new ReservationViewModel
            {
                Reservation = reservation,
                ClientId = reservation.ClientId,
                // Додайте інші властивості ReservationViewModel, які вам потрібні для редагування
            };

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Email", reservation.ClientId);
            ViewData["TimeSlotId"] = new SelectList(_context.TimeSlots, "Id", "Id", reservation.TimeSlotId);

            return View(reservationViewModel);
        }


        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,Info,TimeSlotId,Id")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Email", reservation.ClientId);
            ViewData["TimeSlotId"] = new SelectList(_context.TimeSlots, "Id", "Id", reservation.TimeSlotId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Client)
                .Include(r => r.TimeSlot)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await DeleteReservationAndUpdateTimeSlot(id))
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMultiple(List<int> selectedReservations)
        {
            if (selectedReservations == null || !selectedReservations.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var id in selectedReservations)
            {
                await DeleteReservationAndUpdateTimeSlot(id);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> DeleteReservationAndUpdateTimeSlot(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
            {
                return false;
            }

            var timeSlot = await _context.TimeSlots.FindAsync(reservation.TimeSlotId);
            if (timeSlot != null)
            {
                timeSlot.IsBooked = false;
            }

            _context.Reservations.Remove(reservation);
            return true;
        }




        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }

        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return phoneNumber;

            // Переконуємось, що номер починається з "+38" і має потрібну довжину
            if (phoneNumber.StartsWith("+38") && phoneNumber.Length == 13)
            {
                // Видаляємо "+38" для спрощення подальшого форматування
                string localPart = phoneNumber.Substring(3);
                return $"({localPart.Substring(0, 3)}) {localPart.Substring(3, 3)}-{localPart.Substring(6, 2)}-{localPart.Substring(8)}";
            }
            else
            {
                return phoneNumber; // Якщо формат невідомий або некоректний, повертаємо без змін
            }
        }

        public List<SelectListItem> GetClientsSelectList()
        {
            return _context.Clients
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = FormatPhoneNumber(c.PhoneNumber)
                })
                .ToList();
        }

        public List<SelectListItem> GetTimeSlotsSelectList()
        {
            // Отримати тільки ті TimeSlot, які позначені як false
            var timeSlots = _context.TimeSlots.Where(ts => !ts.IsBooked).ToList();

            // Створити список SelectListItem з цих TimeSlot
            var selectList = timeSlots.Select(ts => new SelectListItem
            {
                Value = ts.Id.ToString(),
                Text = ts.Id.ToString() // При необхідності ви можете використовувати форматування для відображення часових слотів
            }).ToList();

            return selectList;
        }

        [HttpGet]
        public IActionResult SearchClients(string term)
        {
            var clients = _context.Clients
                .Where(c => c.PhoneNumber.Contains(term))
                .Select(c => new {
                    id = c.Id,
                    text = FormatPhoneNumber(c.PhoneNumber)
                })
                .ToList();

            return Json(clients);
        }

    }
}
