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
    public class ClientsController : Controller
    {
        private readonly DbbeautySpaceContext _context;

        public ClientsController(DbbeautySpaceContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync();
            return View(clients);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
            .Include(c => c.Reservations)
                .ThenInclude(r => r.TimeSlot)
                    .ThenInclude(ts => ts.EmployeeService)
                        .ThenInclude(es => es.Service)
            .Include(c => c.Reservations)
                .ThenInclude(r => r.TimeSlot)
                    .ThenInclude(ts => ts.EmployeeService)
                        .ThenInclude(es => es.Employee)
            .FirstOrDefaultAsync(m => m.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }


        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,PhoneNumber,Birthday,Email,Id")] Client client)
        {
            client.PhoneNumber = "+" + new string(client.PhoneNumber.Where(c => char.IsDigit(c)).ToArray());

            //пеервірка на вік
            CheckAge(client);

            //перевірка на існування клієнта та пошти
            if (await ClientExists(client))
            {
                return View(client);
            }

            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }


        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FirstName,LastName,PhoneNumber,Birthday,Email,Id")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            client.PhoneNumber = "+" + new string(client.PhoneNumber.Where(c => char.IsDigit(c)).ToArray());

            //пеервірка на вік
            CheckAge(client);

            //перевірка на існування клієнта та пошти
            if (await ClientExists(client))
            {
                return View(client);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }

        private void CheckAge(Client client)
        {
            if (client.Birthday.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var birthday = client.Birthday.Value;
                var age = today.Year - birthday.Year;

                if (client.Birthday > today.AddYears(-age)) age--;

                if (age < 5 || age > 105)
                {
                    ModelState.AddModelError("Birthday", "Вік клієнта не може бути меншим за 5 років та більшим за 105");
                }
            }
        }

        private async Task<bool> ClientExists(Client client)
        {
            var existingClient = await _context.Clients
                .FirstOrDefaultAsync(c => c.LastName == client.LastName && c.FirstName == client.FirstName && c.Id != client.Id);

            if (existingClient != null)
            {
                ModelState.AddModelError("LastName", "Клієнт з таким іменем та прізвищем вже існує");
                return true;
            }

            var existingEmail = await _context.Clients
                .FirstOrDefaultAsync(c => c.Email == client.Email && c.Id != client.Id);

            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Ця електронна пошта вже використовується");
                return true;
            }

            return false;
        }
    }
}
