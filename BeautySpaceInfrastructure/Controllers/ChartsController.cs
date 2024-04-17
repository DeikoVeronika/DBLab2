using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeautySpaceInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly DbbeautySpaceContext _context;

        public ChartsController(DbbeautySpaceContext context)
        {
            _context = context;
        }

        [HttpGet("JsonDataCategory")]
        public JsonResult JsonDataCategory()
        {
            var categories = _context.Categories.Include(ts => ts.Services).ToList();
            List<object> catService = new List<object>();
            catService.Add(new[] { "Категорія", "Кількість послуг" });
            foreach (var c in categories)
            {
                catService.Add(new object[] { c.Name, c.Services.Count() });
            }

            return new JsonResult(catService);
        }

        [HttpGet("JsonDataReservation")]
        public JsonResult JsonDataReservation()
        {
            var clients = _context.Clients.Include(c => c.Reservations).ToList();
            List<object> cReservation = new List<object>();
            cReservation.Add(new[] { "Клієнт", "Кількість бронювань" });
            foreach (var c in clients)
            {
                cReservation.Add(new object[] { c.FirstName + " " + c.LastName, c.Reservations.Count() });
            }

            return new JsonResult(cReservation);
        }

    }
}
