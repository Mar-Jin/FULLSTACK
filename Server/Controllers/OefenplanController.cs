using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliNow.Data;
using RevaliNow.Models;

namespace RevaliNow.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OefenplanController(RevaliNowDbContext context) : Controller
    {
        private readonly RevaliNowDbContext _context = context;

        // GET: api/<OefenplanController>
        [HttpGet]
        public ActionResult<Oefenplan> Get()
        {
            var oefenplannen = _context.Oefenplannen.ToList();
            return Ok(oefenplannen);
        }

        // GET api/<OefenplanController>/5
        [HttpGet("{id}")]
        public ActionResult<Oefenplan> GetById(int id)
        {
            var oefenplan = _context.Oefenplannen
                .Include(p => p.Patient)
                .Include(p => p.Revalidatiearts)
                .FirstOrDefault(p => p.Id == id);

            if (oefenplan == null) {
                return NotFound();
            }

            return Ok(oefenplan);
        }
    }
}
