using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliNow.Data;
using RevaliNow.Models;
using RevaliNow.Dtos;

namespace RevaliNow.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PatientController(RevaliNowDbContext context) : Controller
    {
        private readonly RevaliNowDbContext _context = context;

        [HttpGet("{id}")]
        public ActionResult<Patient> Get(int id)
        {
            var patient = _context.Patienten
                .Include(p => p.Huisarts)
                .FirstOrDefault(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [HttpGet("{id}/dashboard")]
        public ActionResult<DashboardResponseDto> GetDashboard(int id)
        {
            var user = _context.Patienten.FirstOrDefault(p => p.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var painData = _context.PijnIndicaties
                .Where(p => p.PatientId == id)
                .OrderByDescending(p => p.DatumTijdRegistratie)
                .ToList();

            var recentPains = painData
                .Take(2)
                .ToList();

            var upcomingExercises = _context.Oefenplannen
                .Include(op => op.Oefening)
                .Where(op => op.PatientId == id && op.EindDatum >= DateTime.Now && op.StartDatum <= DateTime.Now)
                .OrderBy(op => op.FrequentiePerDag)
                .ToList();
            
            var nextAppointment = _context.Afspraken
                .Where(a => a.PatientId == id && a.DatumTijdAfspraak >= DateTime.Now)
                .OrderBy(a => a.DatumTijdAfspraak)
                .FirstOrDefault();

            var exercises = _context.Oefenplannen
                .Include(op => op.Oefening)
                .Where(op => op.PatientId == id)
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
