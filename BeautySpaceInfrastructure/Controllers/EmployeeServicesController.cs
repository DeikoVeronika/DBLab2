using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeautySpaceDomain.Model;
using BeautySpaceInfrastructure;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace BeautySpaceInfrastructure.Controllers
{
    public class EmployeeServicesController : Controller
    {
        private readonly DbbeautySpaceContext _context;

        public EmployeeServicesController(DbbeautySpaceContext context)
        {
            _context = context;
        }

        // GET: EmployeeServices
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null) return RedirectToAction("Services", "Index");

            ViewBag.ServiceId = id;
            ViewBag.ServiceName = name;

            var employeeByService = _context.EmployeeServices.Where(es => es.ServiceId == id).Include(es => es.Employee).Include(es => es.Service);
            return View(await employeeByService.ToListAsync());
        }

        // GET: EmployeeServices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeService = await _context.EmployeeServices
                .Include(e => e.Employee)
                .Include(e => e.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeService == null)
            {
                return NotFound();
            }

            return RedirectToAction("Details", "Employees", new { id = employeeService.EmployeeId });
        }

        // GET: EmployeeServices/Create
        public IActionResult Create(int? serviceId)
        {
            var services = _context.Services.OrderBy(c => c.Name).ToList();
            ViewBag.ServiceId = new SelectList(_context.Services.OrderBy(p => p.Name), "Id", "Name", serviceId);
            ViewBag.ServiceName = _context.Services.FirstOrDefault(s => s.Id == serviceId)?.Name;


            ViewData["EmployeeId"] = new SelectList(_context.Employees.Select(e => new {
                Id = e.Id,
                FullName = e.FirstName + " " + e.LastName
            }), "Id", "FullName");
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
            return View();
        }

        // POST: EmployeeServices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int serviceId, [Bind("ServiceId,EmployeeId")] EmployeeService employeeService)
        {
            // Перевірка, чи працівник вже доданий до цієї послуги
            var existingEmployeeService = await _context.EmployeeServices
                .FirstOrDefaultAsync(es => es.ServiceId == serviceId && es.EmployeeId == employeeService.EmployeeId);

            if (existingEmployeeService != null)
            {
                TempData["Message"] = "Цей працівник вже доданий до цієї послуги.";
                // Перенаправлення на сторінку створення
                return RedirectToAction("Create", "EmployeeServices", new { serviceId });
            }

            _context.Add(employeeService);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Services", new { id = serviceId, name = _context.Services.FirstOrDefault(s => s.Id == serviceId)?.Name });
        }


        // GET: EmployeeServices/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var employeeService = await _context.EmployeeServices.FindAsync(id);
        //    if (employeeService == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", employeeService.EmployeeId);
        //    ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", employeeService.ServiceId);
        //    return View(employeeService);
        //}

        // POST: EmployeeServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ServiceId,EmployeeId,Id")] EmployeeService employeeService)
        //{
        //    if (id != employeeService.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(employeeService);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!EmployeeServiceExists(employeeService.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", employeeService.EmployeeId);
        //    ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", employeeService.ServiceId);
        //    return View(employeeService);
        //}

        // GET: EmployeeServices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeService = await _context.EmployeeServices
                .Include(e => e.Employee)
                .Include(e => e.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeService == null)
            {
                return NotFound();
            }

            return View(employeeService);
        }

        // POST: EmployeeServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeService = await _context.EmployeeServices.Include(es => es.TimeSlots).FirstOrDefaultAsync(es => es.Id == id);
            int serviceId = employeeService.ServiceId;

            if (employeeService != null)
            {
                // Видалити всі TimeSlots, пов'язані з цим EmployeeService
                _context.TimeSlots.RemoveRange(employeeService.TimeSlots);

                // Видалити EmployeeService
                _context.EmployeeServices.Remove(employeeService);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Services", new { id = serviceId, name = _context.Services.FirstOrDefault(s => s.Id == serviceId)?.Name });
        }


        [HttpGet]
        public async Task<IActionResult> GetEmployeeServiceCount(int serviceId)
        {
            var service = await _context.Services.Include(s => s.EmployeeServices).FirstOrDefaultAsync(s => s.Id == serviceId);
            if (service == null)
            {
                return NotFound();
            }
            return Ok(service.EmployeeServices.Count);
        }

        private bool EmployeeServiceExists(int id)
        {
            return _context.EmployeeServices.Any(e => e.Id == id);
        }
    }
}
