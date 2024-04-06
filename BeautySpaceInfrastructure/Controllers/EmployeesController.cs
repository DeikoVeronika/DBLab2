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
    public class EmployeesController : Controller
    {
        private readonly DbbeautySpaceContext _context;

        public EmployeesController(DbbeautySpaceContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if(id == null) return RedirectToAction("Positions", "Index");
            ViewBag.PositionId = id;
            ViewBag.PositionName = name;
            var employeeByPosition = _context.Employees.Where(e => e.PositionId == id).Include(e => e.Position);
            return View(await employeeByPosition.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Position)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create(int? positionId)
        {
            var positions = _context.Positions.OrderBy(c => c.Name).ToList();
            ViewBag.PositionId = new SelectList(_context.Positions.OrderBy(p => p.Name), "Id", "Name", positionId);
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int positionId, [Bind("FirstName,LastName,PositionId,EmployeePortrait,PhoneNumber,Id")] Employee employee)
        {
            employee.PhoneNumber = "+" + new string(employee.PhoneNumber.Where(c => char.IsDigit(c)).ToArray());


            var existingEmployee = _context.Employees.FirstOrDefault(s => s.PhoneNumber == employee.PhoneNumber && s.Id != employee.Id);

            if (existingEmployee != null)
            {
                ModelState.AddModelError("PhoneNumber", "Працівник з таким номером телефону вже існує");
                var positions = _context.Positions.OrderBy(p => p.Name).ToList();
                ViewBag.PositionId = new SelectList(positions, "Id", "Name", employee.PositionId);
                return View(employee);
            }

            _context.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Positions", new { id = positionId, name = _context.Positions.Where(e => e.Id == positionId).FirstOrDefault().Name });
        }


        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Name", employee.PositionId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FirstName,LastName,PositionId,EmployeePortrait,PhoneNumber,Id")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            employee.PhoneNumber = "+" + new string(employee.PhoneNumber.Where(c => char.IsDigit(c)).ToArray());


            var existingEmployee = _context.Employees.FirstOrDefault(s => s.PhoneNumber == employee.PhoneNumber && s.Id != employee.Id);

            if (existingEmployee != null)
            {
                ModelState.AddModelError("PhoneNumber", "Працівник з таким номером телефону вже існує");
                var positions = _context.Positions.OrderBy(p => p.Name).ToList();
                ViewBag.PositionId = new SelectList(positions, "Id", "Name", employee.PositionId);
                return View(employee);
            }

            try
            {
                _context.Update(employee);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employee.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Details", "Positions", new { id = employee.PositionId });
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Position)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.Include(e => e.EmployeeServices).FirstOrDefaultAsync(e => e.Id == id);
            if (employee != null)
            {
                // Видалити всі EmployeeServices пов'язані з цим працівником
                _context.EmployeeServices.RemoveRange(employee.EmployeeServices);

                // Видалити працівника
                _context.Employees.Remove(employee);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Positions", new { id = employee.PositionId });
        }


        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
