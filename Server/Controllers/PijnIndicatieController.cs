using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevaliNow.Authorization;
using RevaliNow.Dtos;
using RevaliNow.Models;
using RevaliNow.Services;

namespace RevaliNow.Controllers
{
    [Route("patienten/{patientId:int}/pijnindicaties")]
    [ApiController]
    [Authorize(Roles = Rollen.Patient)]
    [AlleenEigenPatient]
    public class PijnIndicatieController(IPijnIndicatieService pijnIndicatieService) : ControllerBase
    {
        // GET patienten/5/pijnindicaties
        [HttpGet]
        public async Task<ActionResult<List<PijnIndicatieDto>>> Get(int patientId)
        {
            return Ok(await pijnIndicatieService.GetPijnIndicatiesAsync(patientId));
        }

        // GET patienten/5/pijnindicaties/per-week
        [HttpGet("per-week")]
        public async Task<ActionResult<List<PijnWeekDto>>> GetPerWeek(int patientId)
        {
            return Ok(await pijnIndicatieService.GetPerWeekAsync(patientId));
        }

        // GET patienten/5/pijnindicaties/12
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PijnIndicatieDto>> GetById(int patientId, int id)
        {
            var pijnIndicatie = await pijnIndicatieService.GetPijnIndicatieAsync(patientId, id);

            if (pijnIndicatie == null)
            {
                return NotFound();
            }

            return Ok(pijnIndicatie);
        }

        // POST patienten/5/pijnindicaties  { "pijnScore": 4, "toelichting": "..." }
        [HttpPost]
        public async Task<ActionResult<PijnIndicatieDto>> Create(int patientId, [FromBody] PijnIndicatieInvoerDto invoer)
        {
            var nieuw = await pijnIndicatieService.RegistreerAsync(patientId, invoer);
            return CreatedAtAction(nameof(GetById), new { patientId, id = nieuw.Id }, nieuw);
        }
    }
}
