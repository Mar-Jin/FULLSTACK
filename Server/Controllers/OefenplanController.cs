using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevaliNow.Authorization;
using RevaliNow.Dtos;
using RevaliNow.Models;
using RevaliNow.Services;

namespace RevaliNow.Controllers
{
    [Route("patienten/{patientId:int}/oefenplannen")]
    [ApiController]
    [Authorize(Roles = Rollen.Patient)]
    [AlleenEigenPatient]
    public class OefenplanController(IOefenplanService oefenplanService) : ControllerBase
    {
        // GET patienten/5/oefenplannen
        [HttpGet]
        public async Task<ActionResult<List<OefenplanDto>>> Get(int patientId)
        {
            return Ok(await oefenplanService.GetOefenplannenAsync(patientId));
        }

        // GET patienten/5/oefenplannen/12
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OefenplanDto>> GetById(int patientId, int id)
        {
            var oefenplan = await oefenplanService.GetOefenplanAsync(patientId, id);

            if (oefenplan == null) {
                return NotFound();
            }

            return Ok(oefenplan);
        }

        // POST patienten/5/oefenplannen/12/afvinkingen  { "isAfgevinkt": true }
        // Legt een nieuwe afvinking vast (of het ongedaan maken ervan) en geeft de bijgewerkte voortgang terug.
        [HttpPost("{id:int}/afvinkingen")]
        public async Task<ActionResult<OefenplanDto>> Afvinken(int patientId, int id, [FromBody] AfvinkenDto afvinken)
        {
            return Ok(await oefenplanService.AfvinkenAsync(patientId, id, afvinken.IsAfgevinkt));
        }
    }
}
