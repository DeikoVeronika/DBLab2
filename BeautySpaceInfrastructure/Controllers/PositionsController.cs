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
    public class PositionsController : Controller
    {
        private readonly DbbeautySpaceContext _context;

        public PositionsController(DbbeautySpaceContext context)
        {
            _context = context;
        }

        // GET: Positions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Positions.Include(p => p.Employees).OrderBy(p => p.Name).ToListAsync());
        }

        // GET: Positions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (position == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Employees", new {id = position.Id, name = position.Name});
        }

        // GET: Positions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Positions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Id")] Position position)
        {
            if (ModelState.IsValid)
            {
                // Перевірка на унікальність імені
                if (_context.Positions.Any(p => p.Name == position.Name))
                {
                    ModelState.AddModelError("Name", "Посада з таким ім'ям вже існує");
                    return View(position);
                }

                _context.Add(position);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(position);
        }

        // GET: Positions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }
            return View(position);
        }

        // POST: Positions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id")] Position position)
        {
            if (id != position.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Перевірка на унікальність імені
                if (_context.Positions.Any(p => p.Name == position.Name && p.Id != position.Id))
                {
                    ModelState.AddModelError("Name", "Посада з таким ім'ям вже існує");
                    return View(position);
                }

                try
                {
                    _context.Update(position);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PositionExists(position.Id))
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
            return View(position);
        }

        // GET: Positions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (position == null)
            {
                return NotFound();
            }

            // Перевірка чи посада не є посадою "Резерв"
            if (position.Name == "Резерв")
            {
                TempData["ErrorMessage"] = "Ви не можете видаляти посаду \"Резерв\". ";

                return RedirectToAction(nameof(Index));
            }

            return View(position);
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var position = await _context.Positions.Include(p => p.Employees).FirstOrDefaultAsync(p => p.Id == id);
            if (position == null)
            {
                return NotFound();
            }

            // Перенести працівників на посаду "Резерв"
            var reservePosition = await _context.Positions.FirstOrDefaultAsync(p => p.Name == "Резерв");
            if (reservePosition == null)
            {
                // Створити посаду "Резерв", якщо вона не існує
                reservePosition = new Position { Name = "Резерв" };
                _context.Positions.Add(reservePosition);
                await _context.SaveChangesAsync();
            }

            // Створити копію колекції працівників перед зміною її елементів
            var employeesToMove = position.Employees.ToList();

            foreach (var employee in employeesToMove)
            {
                employee.PositionId = reservePosition.Id;
                _context.Entry(employee).State = EntityState.Modified;
            }

            // Видалити вихідну посаду
            _context.Positions.Remove(position);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PositionExists(int id)
        {
            return _context.Positions.Any(e => e.Id == id);
        }

    }
}
