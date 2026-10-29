using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliNow.Authorization;
using RevaliNow.Data;
using RevaliNow.Dtos;
using RevaliNow.Models;

namespace RevaliNow.Controllers
{
    [Route("patienten/{patientId:int}/pijnindicaties")]
    [ApiController]
    [Authorize(Roles = Rollen.Patient)]
    [AlleenEigenPatient]
    public class PijnIndicatieController(RevaliNowDbContext context, TimeProvider tijd) : ControllerBase
    {
        private readonly RevaliNowDbContext _context = context;

        [HttpGet]
        public ActionResult<List<PijnIndicatie>> Get(int patientId)
        {
            var pijnindicaties = _context.PijnIndicaties
                .AsNoTracking()
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.DatumTijdRegistratie)
                .ToList();

            return Ok(pijnindicaties);
        }

        // GET patienten/5/pijnindicaties/per-week: weekgemiddelde, berekend in de database (sp_PijnverloopPerWeek)
        [HttpGet("per-week")]
        public async Task<ActionResult<List<PijnWeekDto>>> GetPerWeek(int patientId)
        {
            return Ok(await _context.Database
                .SqlQuery<PijnWeekDto>($"EXEC sp_PijnverloopPerWeek @PatientId = {patientId}")
                .ToListAsync());
        }

        [HttpGet("{id:int}")]
        public ActionResult<PijnIndicatie> GetById(int patientId, int id)
        {
            var pijnIndicatie = _context.PijnIndicaties
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id && p.PatientId == patientId);

            if (pijnIndicatie == null)
            {
                return NotFound();
            }

            return Ok(pijnIndicatie);
        }

        [HttpPost]
        public ActionResult<PijnIndicatie> Create(int patientId, [FromBody] PijnIndicatie pijnIndicatie)
        {
            // De patiënt en het tijdstip komen niet uit de request, maar uit de route en de server,
            // zodat een cliënt nooit voor iemand anders of met een verzonnen datum kan registreren.
            pijnIndicatie.Id = 0;
            pijnIndicatie.PatientId = patientId;
            pijnIndicatie.DatumTijdRegistratie = tijd.GetLocalNow().DateTime;

            _context.PijnIndicaties.Add(pijnIndicatie);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { patientId, id = pijnIndicatie.Id }, pijnIndicatie);
        }
    }
}
