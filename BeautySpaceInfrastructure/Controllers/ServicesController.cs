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
    public class ServicesController : Controller
    {
        private readonly DbbeautySpaceContext _context;

        public ServicesController(DbbeautySpaceContext context)
        {
            _context = context;
        }

        // GET: Services
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if(id == null) return RedirectToAction("Categories","Index");
            ViewBag.CategoryId = id;
            ViewBag.CategoryName = name;

            var serviceByCategory = _context.Services.Where(s => s.CategoryId == id).Include(s => s.Category);

            return View(await serviceByCategory.ToListAsync());
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // GET: Services/Create
        public IActionResult Create(int? categoryId)
        {
            //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            var categories = _context.Categories.OrderBy(c => c.Name).ToList();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", categoryId);
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int categoryId, [Bind("Name,Description,Price,CategoryId")] Service service)
        {

            if (_context.Services.Any(s => s.Name == service.Name))
            {
                ModelState.AddModelError("Name", "Це ім'я вже використовується. Виберіть інше.");
                var categories = _context.Categories.OrderBy(c => c.Name).ToList();
                ViewBag.CategoryId = new SelectList(categories, "Id", "Name", service.CategoryId);
                return View(service);
            }

            _context.Add(service);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Categories", new { id = categoryId, name = _context.Categories.FirstOrDefault(c => c.Id == categoryId)?.Name });
        }

        // GET: Services/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", service.CategoryId);
            return View(service);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,Price,CategoryId,Id")] Service service)
        {
            if (id != service.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", service.CategoryId);
            return View(service);
        }

        // GET: Services/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }



        [HttpGet]
        public async Task<int> GetServiceCount(int categoryId)
        {
            var category = await _context.Categories.Include(c => c.Services).FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category != null)
            {
                return category.Services.Count;
            }
            return 0;
        }

    }
}
