using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeautySpaceDomain.Model;
using BeautySpaceInfrastructure;

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
            var dbbeautySpaceContext = _context.TimeSlots.Include(t => t.EmployeeService);
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
            // Отримуємо всі записи з таблиці EmployeeServices
            var employeeServices = _context.EmployeeServices
                .Include(es => es.Employee)
                .Include(es => es.Service)
                .ToHashSet();


            // Створюємо список для ServiceId
            var serviceList = employeeServices
                .GroupBy(es => es.ServiceId)
                .Select(group => group.First())
                .Select(es => new SelectListItem
                {
                    Value = es.ServiceId.ToString(),
                    Text = es.Service.Name
                })
                .ToHashSet();



            // Створюємо список для EmployeeId
            var employeeList = employeeServices
            .GroupBy(es => es.EmployeeId)
            .Select(group => group.First())
            .Select(es => new SelectListItem
            {
                Value = es.EmployeeId.ToString(),
                Text = es.Employee.FirstName
            })
            .ToHashSet();

            // Передаємо створені списки у ViewBag
            ViewBag.ServiceIdList = new SelectList(serviceList, "Value", "Text");
            ViewBag.EmployeeIdList = new SelectList(employeeList, "Value", "Text");

            return View();
        }


        // POST: TimeSlots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int serviceId, int employeeId, [Bind("Date,StartTime,EndTime,IsBooked,Id")] TimeSlot timeSlot)
        {
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

                _context.Add(timeSlot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EmployeeServiceId"] = new SelectList(_context.EmployeeServices, "Id", "Id", timeSlot.EmployeeServiceId);
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

            // Отримуємо всі записи з таблиці EmployeeServices
            var employeeServices = _context.EmployeeServices
                .Include(es => es.Employee)
                .Include(es => es.Service)
                .ToHashSet();

            // Створюємо список для ServiceId
            var serviceList = employeeServices
                .GroupBy(es => es.ServiceId)
                .Select(group => group.First())
                .Select(es => new SelectListItem
                {
                    Value = es.ServiceId.ToString(),
                    Text = es.Service.Name,
                    Selected = es.ServiceId == employeeService.ServiceId
                })
                .ToList();

            // Створюємо список для EmployeeId
            var employeeList = employeeServices
            .GroupBy(es => es.EmployeeId)
            .Select(group => group.First())
            .Select(es => new SelectListItem
            {
                Value = es.EmployeeId.ToString(),
                Text = es.Employee.FirstName
            })
            .ToHashSet();

            // Створюємо список для EmployeeId, відфільтрований за ServiceId
            var employeeListByService = employeeServices
                .Where(es => es.ServiceId == employeeService.ServiceId)
                .GroupBy(es => es.EmployeeId)
                .Select(group => group.First())
                .Select(es => new SelectListItem
                {
                    Value = es.EmployeeId.ToString(),
                    Text = es.Employee.FirstName,
                    Selected = es.EmployeeId == employeeService.EmployeeId
                })
                .ToList();

            // Передаємо створені списки у ViewBag
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
            var timeSlot = await _context.TimeSlots.FindAsync(id);
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



    }
}
