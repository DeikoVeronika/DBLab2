using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeautySpaceDomain.Model;
using BeautySpaceInfrastructure;
using BeautySpaceInfrastructure.Services;

namespace BeautySpaceInfrastructure.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DbbeautySpaceContext _context;
        private CategoryDataPortServiceFactory _categoryDataPortServiceFactory;

        public CategoriesController(DbbeautySpaceContext context)
        {
            _context = context;
            _categoryDataPortServiceFactory = new CategoryDataPortServiceFactory(context);
        }



        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Services", new { id = category.Id, name = category.Name});
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Id")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (_context.Categories.Any(ts => ts.Name == category.Name))
                {
                    ModelState.AddModelError("Name", "Категорія з такою назвою вже існує");
                    ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
                    return View(category);
                }

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (_context.Categories.Any(c => c.Name == category.Name && c.Id != category.Id))
                {
                    ModelState.AddModelError("Name", "Категорія з такою назвою вже існує");
                    ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
                    return View(category);
                }

                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.Include(c => c.Services).ThenInclude(s => s.EmployeeServices).FirstOrDefaultAsync(c => c.Id == id);
            if (category != null)
            {
                // Якщо категорія не має послуг, просто видалити категорію
                if (category.Services.Count == 0)
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                // Видалити всі EmployeeServices, пов'язані з послугами цієї категорії
                foreach (var service in category.Services)
                {
                    _context.EmployeeServices.RemoveRange(service.EmployeeServices);
                }

                // Видалити всі послуги пов'язані з категорією
                _context.Services.RemoveRange(category.Services);

                // Видалити категорію
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }




        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel, CancellationToken cancellationToken = default)
        {
            if (fileExcel == null || fileExcel.Length == 0)
            {
                TempData["ErrorMessage"] = "Файл для імпорту не обрано.";
                return RedirectToAction(nameof(Import));
            }

            try
            {
                var importService = _categoryDataPortServiceFactory.GetImportService(fileExcel.ContentType);
                using var stream = fileExcel.OpenReadStream();
                bool isNewServiceCreated = await importService.ImportFromStreamAsync(stream, cancellationToken);

                TempData["SuccessMessage"] = isNewServiceCreated ? "Імпорт завершено успішно! Нові дані були додані." : "Всі дані з файлу вже зберігаються в системі.";
            }
            catch (ArgumentException ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
            catch (NotImplementedException ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Export([FromQuery] string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    CancellationToken cancellationToken = default)
        {
            var exportService = _categoryDataPortServiceFactory.GetExportService(contentType);

            var memoryStream = new MemoryStream();

            await exportService.WriteToAsync(memoryStream, cancellationToken);

            await memoryStream.FlushAsync(cancellationToken);
            memoryStream.Position = 0;

            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName = $"categories_{DateTime.UtcNow.ToShortDateString()}.xlsx"
            };
        }
    }
}
