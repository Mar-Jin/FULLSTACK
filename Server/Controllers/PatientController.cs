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
    public class PatientController(RevaliNowDbContext context, TimeProvider tijd) : ControllerBase
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

        // De oefeningen van vandaag haalt de frontend apart op (GET oefenplannen), zodat het dashboard en de
        // oefenpagina dezelfde state delen.
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardResponseDto>> GetDashboard(int patientId)
        {
            var user = await _context.Patienten.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patientId);

            if (user == null)
            {
                return NotFound();
            }

            // Registraties 'in de toekomst' (komen voor in de dataset) horen niet in de grafiek of bij 'recent'
            var nu = tijd.GetLocalNow().DateTime;
            var painData = await _context.PijnIndicaties
                .AsNoTracking()
                .Where(p => p.PatientId == patientId && p.DatumTijdRegistratie <= nu)
                .OrderByDescending(p => p.DatumTijdRegistratie)
                .ToListAsync();

            // Laatste pijnscore, gemiddelde van 7 dagen en volgende afspraak in één aanroep
            var samenvatting = (await _context.Database
                    .SqlQuery<DashboardSamenvattingDto>($"EXEC sp_DashboardSamenvatting @PatientId = {patientId}")
                    .ToListAsync())
                .Single();

            var response = new DashboardResponseDto
            {
                User = user,
                RecentPains = painData.Take(2).ToList(),
                PainData = painData,
                Samenvatting = samenvatting
            };

            return Ok(response);
        }
    }
}
