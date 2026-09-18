using Microsoft.AspNetCore.Mvc;
using RevaliNow.Data;
using RevaliNow.Models;


namespace RevaliNow.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PijnIndicatieController(RevaliNowDbContext context) : Controller
    {
        private readonly RevaliNowDbContext _context = context;

        [HttpGet]
        public ActionResult<PijnIndicatie> Get()
        {
            var pijnindicaties = _context.PijnIndicaties.ToList();
            return Ok(pijnindicaties);
        }

        [HttpGet("{id}")]
        public ActionResult<PijnIndicatie> GetById(int id)
        {
            var pijnIndicatie = _context.PijnIndicaties.Find(id);

            if (pijnIndicatie == null)
            {
                return NotFound();
            }

            return Ok(pijnIndicatie);
        }

        [HttpPost]
        public ActionResult<PijnIndicatie> Create([FromBody] PijnIndicatie pijnIndicatie)
        {
            _context.PijnIndicaties.Add(pijnIndicatie);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = pijnIndicatie.Id }, pijnIndicatie);
        }
    }
}
