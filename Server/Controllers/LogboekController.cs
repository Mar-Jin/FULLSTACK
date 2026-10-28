using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevaliNow.Authorization;
using RevaliNow.Dtos;
using RevaliNow.Models;
using RevaliNow.Services;

namespace RevaliNow.Controllers
{
    [Route("patienten/{patientId:int}/logboek")]
    [ApiController]
    [Authorize(Roles = Rollen.Patient)]
    [AlleenEigenPatient]
    public class LogboekController(ILogboekService logboekService) : ControllerBase
    {
        // GET patienten/5/logboek
        [HttpGet]
        public async Task<ActionResult<List<LogboekDto>>> Get(int patientId)
        {
            return Ok(await logboekService.GetLogboekAsync(patientId));
        }

        // GET patienten/5/logboek/12
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LogboekDto>> GetById(int patientId, int id)
        {
            var invoer = await logboekService.GetInvoerAsync(patientId, id);

            if (invoer == null)
            {
                return NotFound();
            }

            return Ok(invoer);
        }

        // POST patienten/5/logboek
        [HttpPost]
        public async Task<ActionResult<LogboekDto>> Create(int patientId, [FromBody] LogboekInvoerDto invoer)
        {
            var nieuw = await logboekService.ToevoegenAsync(patientId, invoer);
            return CreatedAtAction(nameof(GetById), new { patientId, id = nieuw.Id }, nieuw);
        }

        // PUT patienten/5/logboek/12
        [HttpPut("{id:int}")]
        public async Task<ActionResult<LogboekDto>> Update(int patientId, int id, [FromBody] LogboekInvoerDto invoer)
        {
            return Ok(await logboekService.BewerkenAsync(patientId, id, invoer));
        }

        // DELETE patienten/5/logboek/12
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int patientId, int id)
        {
            await logboekService.VerwijderenAsync(patientId, id);
            return NoContent();
        }
    }
}
