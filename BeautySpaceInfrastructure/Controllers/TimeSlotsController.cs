using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeautySpaceDomain.Model;
using BeautySpaceInfrastructure;
using BeautySpaceInfrastructure.Models;

namespace BeautySpaceInfrastructure.Controllers
{
    public class TimeSlotsController : Controller
    {
        private readonly DbbeautySpaceContext _context;

        public TimeSlotsController(DbbeautySpaceContext context)
        {
            _context = context;
        }

        // GET: TimeSlots
        public async Task<IActionResult> Index()
        {
            var dbbeautySpaceContext = _context.TimeSlots
            .Include(t => t.EmployeeService)
            .OrderBy(t => t.Date)
            .ThenBy(t => t.StartTime);

            return View(await dbbeautySpaceContext.ToListAsync());

        }

        // GET: TimeSlots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeSlot = await _context.TimeSlots
                .Include(t => t.EmployeeService)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeSlot == null)
            {
                return NotFound();
            }

            return View(timeSlot);
        }

        // GET: TimeSlots/Create
        public IActionResult Create()
        {
            PopulateDropdownsForCreate();
            return View();
        }



        // POST: TimeSlots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int serviceId, int employeeId, [Bind("Date,StartTime,EndTime,IsBooked,Id")] TimeSlot timeSlot)
        {
            // *** виведення помиилки для окремого списку, скидання списку послуги коли не обрано працівника ***
            if (serviceId == 0 || employeeId == 0)
            {
                ModelState.AddModelError("", "Оберіть послугу та працівника.");
            }

            if (ModelState.IsValid)
            {

                
                // Визначення EmployeeServiceId на основі вибраних працівника та послуги
                var employeeService = await _context.EmployeeServices.FirstOrDefaultAsync(es => es.EmployeeId == employeeId && es.ServiceId == serviceId);

                // Якщо не вдалося знайти відповідний запис EmployeeService, повертається сторінка 404 
                if (employeeService == null)
                {
                    return RedirectToAction("Error", "Home");
                }

                // Встановлюємо правильне значення EmployeeServiceId для TimeSlot
                timeSlot.EmployeeServiceId = employeeService.Id;

                var existingTimeSlot = await _context.TimeSlots.FirstOrDefaultAsync(ts => ts.EmployeeServiceId == timeSlot.EmployeeServiceId && ts.Date == timeSlot.Date && ts.StartTime == timeSlot.StartTime && ts.EndTime == timeSlot.EndTime);
                if (existingTimeSlot != null)
                {
                    TempData["Message"] = "Часовий слот з такими датою, часом початку, часом закінчення та працівником вже існує.";
                    PopulateDropdownsForCreate();
                    return View(timeSlot);
                }

                _context.Add(timeSlot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdownsForCreate();
            return View(timeSlot);
        }

        // GET: TimeSlots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeSlot = await _context.TimeSlots.FindAsync(id);
            if (timeSlot == null)
            {
                return NotFound();
            }

            var employeeService = await _context.EmployeeServices
                .Include(es => es.Employee)
                .Include(es => es.Service)
                .FirstOrDefaultAsync(es => es.Id == timeSlot.EmployeeServiceId);

            if (employeeService == null)
            {
                return NotFound();
            }

            var employeeServices = GetAllEmployeeServices();

            var serviceList = GetServiceList(employeeServices);

            var employeeList = GetEmployeeList(employeeServices);

            var employeeListByService = GetEmployeeListByService(employeeServices, employeeService.ServiceId, employeeService.EmployeeId);

            ViewBag.ServiceIdList = new SelectList(serviceList, "Value", "Text");
            ViewBag.EmployeeIdList = new SelectList(employeeListByService, "Value", "Text");

            return View(timeSlot);
        }


        // POST: TimeSlots/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int serviceId, int employeeId, [Bind("Date,StartTime,EndTime,IsBooked,Id")] TimeSlot timeSlot)
        {
            if (id != timeSlot.Id)
            {
                return NotFound();
            }

            // Визначення EmployeeServiceId на основі вибраних працівника та послуги
            var employeeService = await _context.EmployeeServices.FirstOrDefaultAsync(es => es.EmployeeId == employeeId && es.ServiceId == serviceId);

            // Якщо не вдалося знайти відповідний запис EmployeeService, повертається сторінка 404 
            if (employeeService == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Встановлюємо правильне значення EmployeeServiceId для TimeSlot
            timeSlot.EmployeeServiceId = employeeService.Id;

            var existingTimeSlot = await _context.TimeSlots.FirstOrDefaultAsync(ts =>
        ts.Date == timeSlot.Date &&
        ts.StartTime == timeSlot.StartTime &&
        ts.EndTime == timeSlot.EndTime &&
        ts.EmployeeServiceId == timeSlot.EmployeeServiceId &&
        ts.Id != timeSlot.Id);

            if (existingTimeSlot != null)
            {
                TempData["Message"] = "Часовий слот з такими датою, часом початку, часом закінчення та працівником вже існує.";

                PopulateDropdownsForCreate();
                return View(timeSlot);
            }

            try
            {
                _context.Update(timeSlot);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimeSlotExists(timeSlot.Id))
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


        // GET: TimeSlots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeSlot = await _context.TimeSlots
                .Include(t => t.EmployeeService)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeSlot == null)
            {
                return NotFound();
            }

            return View(timeSlot);
        }

        // POST: TimeSlots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timeSlot = await _context.TimeSlots
                .Include(ts => ts.Reservations)
                .FirstOrDefaultAsync(ts => ts.Id == id);


            if (timeSlot != null)
            {
                _context.TimeSlots.Remove(timeSlot);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool TimeSlotExists(int id)
        {
            return _context.TimeSlots.Any(e => e.Id == id);
        }

        private HashSet<EmployeeService> GetAllEmployeeServices()
        {
            return _context.EmployeeServices
                .Include(es => es.Employee)
                .Include(es => es.Service)
                .ToHashSet();
        }
        private HashSet<SelectListItem> GetServiceList(HashSet<EmployeeService> employeeServices)
        {
            return employeeServices
                .GroupBy(es => es.ServiceId)
                .Select(group => group.First())
                .Select(es => new SelectListItem
                {
                    Value = es.ServiceId.ToString(),
                    Text = es.Service.Name
                })
                .ToHashSet();
        }
        private HashSet<SelectListItem> GetEmployeeList(HashSet<EmployeeService> employeeServices)
        {
            return employeeServices
                .GroupBy(es => es.EmployeeId)
                .Select(group => group.First())
                .Select(es => new SelectListItem
                {
                    Value = es.EmployeeId.ToString(),
                    Text = es.Employee.FirstName
                })
                .ToHashSet();
        }
        private List<SelectListItem> GetEmployeeListByService(HashSet<EmployeeService> employeeServices, int serviceId, int selectedEmployeeId)
        {
            var employeeListByService = employeeServices
                .Where(es => es.ServiceId == serviceId)
                .GroupBy(es => es.EmployeeId)
                .Select(group => group.First())
                .Select(es => new SelectListItem
                {
                    Value = es.EmployeeId.ToString(),
                    Text = es.Employee.FirstName,
                    Selected = es.EmployeeId == selectedEmployeeId
                })
                .ToList();

            return employeeListByService;
        }
        private void PopulateDropdownsForCreate()
        {
            var employeeServices = GetAllEmployeeServices();
            var serviceList = GetServiceList(employeeServices);
            var employeeList = GetEmployeeList(employeeServices);

            ViewBag.ServiceIdList = new SelectList(serviceList, "Value", "Text");
            ViewBag.EmployeeIdList = new SelectList(employeeList, "Value", "Text");
        }

        [HttpGet]
        //для створення
        public async Task<IActionResult> GetEmployeesByServiceId(int serviceId)
        {
            var employees = await _context.EmployeeServices
                .Where(es => es.ServiceId == serviceId)
                .Select(es => new SelectListItem
                {
                    Value = es.EmployeeId.ToString(), 
                    Text = es.Employee.FirstName
                })
                .Distinct()
                .ToListAsync();

            return Json(employees);
        }

        [HttpGet]
        //для редагування
        public JsonResult GetEmployeesByService(int serviceId)
        {
            var employees = _context.EmployeeServices
                .Where(es => es.ServiceId == serviceId)
                .Select(es => new { Value = es.EmployeeId.ToString(), Text = es.Employee.FirstName })
                .ToList();

            return Json(employees);
        }

        [HttpGet]
        public async Task<IActionResult> GetTimeSlotCount(int employeeServiceId)
        {
            var employeeService = await _context.EmployeeServices.Include(es => es.TimeSlots).FirstOrDefaultAsync(es => es.Id == employeeServiceId);
            if (employeeService == null)
            {
                return NotFound();
            }
            return Ok(employeeService.TimeSlots.Count);
        }

        [HttpGet]
        public async Task<bool> CheckReservations(int timeSlotId)
        {
            var timeSlot = await _context.TimeSlots.Include(ts => ts.Reservations).FirstOrDefaultAsync(ts => ts.Id == timeSlotId);
            if (timeSlot != null)
            {
                if (timeSlot.Reservations.Any())
                {
                    return true;
                }
            }

            return false;
        }

        [HttpGet]
        public async Task<IActionResult> GetTimeSlotsByEmployeeServiceId(int employeeId, int serviceId)
        {
            // Find the EmployeeServiceId based on the selected EmployeeId and ServiceId
            var employeeService = await _context.EmployeeServices.FirstOrDefaultAsync(es => es.EmployeeId == employeeId && es.ServiceId == serviceId);

            if (employeeService != null)
            {
                // Get time slots for the found EmployeeServiceId
                var timeSlots = await _context.TimeSlots
                    .Where(t => t.EmployeeServiceId == employeeService.Id && !t.IsBooked) // Filter out the booked time slots
                    .ToListAsync();

                var timeSlotSelectList = timeSlots.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = $"{t.Date.ToString("dd.MM")} {t.StartTime.ToString("HH:mm")}" // Додайте час до тексту
                }).ToList();


                return Json(timeSlotSelectList);
            }
            else
            {
                // No EmployeeService found for the selected EmployeeId and ServiceId
                return Json(new List<SelectListItem>());
            }
        }







    }
}
