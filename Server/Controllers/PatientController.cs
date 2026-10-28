using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliNow.Authorization;
using RevaliNow.Data;
using RevaliNow.Dtos;
using RevaliNow.Models;
using RevaliNow.Services;

namespace RevaliNow.Controllers
{
    [Route("patienten/{patientId:int}")]
    [ApiController]
    [Authorize(Roles = Rollen.Patient)]
    [AlleenEigenPatient]
    public class PatientController(RevaliNowDbContext context, IOefenplanService oefenplanService,
        IAfspraakService afspraakService) : ControllerBase
    {
        private readonly RevaliNowDbContext _context = context;

        [HttpGet]
        public ActionResult<Patient> Get(int patientId)
        {
            var patient = _context.Patienten
                .AsNoTracking()
                .Include(p => p.Huisarts)
                .FirstOrDefault(p => p.Id == patientId);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardResponseDto>> GetDashboard(int patientId)
        {
            var user = _context.Patienten.AsNoTracking().FirstOrDefault(p => p.Id == patientId);

            if (user == null)
            {
                return NotFound();
            }

            var painData = _context.PijnIndicaties
                .AsNoTracking()
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.DatumTijdRegistratie)
                .ToList();

            var recentPains = painData
                .Take(2)
                .ToList();

            // Zelfde berekening als op de oefenpagina, zodat de status van vandaag overal gelijk is
            var upcomingExercises = (await oefenplanService.GetOefenplannenAsync(patientId))
                .Where(op => op.IsActief)
                .ToList();

            var nextAppointment = (await afspraakService.GetAfsprakenAsync(patientId))
                .FirstOrDefault(a => a.IsToekomstig);

            var response = new DashboardResponseDto
            {
                User = user,
                RecentPains = recentPains,
                PainData = painData,
                UpcomingExercises = upcomingExercises,
                NextAppointment = nextAppointment
            };

            return Ok(response);
        }
    }
}
