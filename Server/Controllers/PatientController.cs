using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevaliNow.Authorization;
using RevaliNow.Dtos;
using RevaliNow.Models;
using RevaliNow.Services;

namespace RevaliNow.Controllers
{
    [Route("patienten/{patientId:int}")]
    [ApiController]
    [Authorize(Roles = Rollen.Patient)]
    [AlleenEigenPatient]
    public class PatientController(IPatientService patientService) : ControllerBase
    {
        // GET patienten/5
        [HttpGet]
        public async Task<ActionResult<PatientDto>> Get(int patientId)
        {
            var patient = await patientService.GetPatientAsync(patientId);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        // GET patienten/5/dashboard
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardResponseDto>> GetDashboard(int patientId)
        {
            var dashboard = await patientService.GetDashboardAsync(patientId);

            if (dashboard == null)
            {
                return NotFound();
            }

            return Ok(dashboard);
        }
    }
}
