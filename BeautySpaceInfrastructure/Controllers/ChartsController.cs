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

        [HttpGet("JsonData")]
        public JsonResult JsonData()
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
    }
}
