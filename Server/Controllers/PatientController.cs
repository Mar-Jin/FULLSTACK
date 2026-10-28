using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliNow.Authorization;
using RevaliNow.Data;
using RevaliNow.Dtos;
using RevaliNow.Models;

namespace RevaliNow.Controllers
{
    [Route("patienten/{patientId:int}")]
    [ApiController]
    [Authorize(Roles = Rollen.Patient)]
    [AlleenEigenPatient]
    public class PatientController(RevaliNowDbContext context) : ControllerBase
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
        public ActionResult<DashboardResponseDto> GetDashboard(int patientId)
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

            var upcomingExercises = _context.Oefenplannen
                .AsNoTracking()
                .Include(op => op.Oefening)
                .Where(op => op.PatientId == patientId && op.EindDatum >= DateTime.Now && op.StartDatum <= DateTime.Now)
                .OrderBy(op => op.FrequentiePerDag)
                .ToList();

            var nextAppointment = _context.Afspraken
                .AsNoTracking()
                .Include(a => a.Revalidatiearts)
                .Where(a => a.PatientId == patientId && a.DatumTijdAfspraak >= DateTime.Now)
                .OrderBy(a => a.DatumTijdAfspraak)
                .FirstOrDefault();

            var exercises = _context.Oefenplannen
                .AsNoTracking()
                .Include(op => op.Oefening)
                .Where(op => op.PatientId == patientId)
                .ToList();

            var response = new DashboardResponseDto
            {
                User = user,
                RecentPains = recentPains,
                PainData = painData,
                UpcomingExercises = upcomingExercises,
                NextAppointment = nextAppointment,
                Exercises = exercises
            };

            return Ok(response);
        }
    }
}
