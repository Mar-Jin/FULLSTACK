using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevaliNow.Authorization;
using RevaliNow.Dtos;
using RevaliNow.Models;
using RevaliNow.Services;

namespace RevaliNow.Controllers
{
    [Route("patienten/{patientId:int}/afspraken")]
    [ApiController]
    [Authorize(Roles = Rollen.Patient)]
    [AlleenEigenPatient]
    public class AfspraakController(IAfspraakService afspraakService) : ControllerBase
    {
        // GET patienten/5/afspraken
        [HttpGet]
        public async Task<ActionResult<List<AfspraakDto>>> Get(int patientId)
        {
            return Ok(await afspraakService.GetAfsprakenAsync(patientId));
        }
    }
}
