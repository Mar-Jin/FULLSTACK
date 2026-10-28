using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliNow.Authorization;
using RevaliNow.Data;
using RevaliNow.Models;

namespace RevaliNow.Controllers
{
    [Route("patienten/{patientId:int}/oefenplannen")]
    [ApiController]
    [Authorize(Roles = Rollen.Patient)]
    [AlleenEigenPatient]
    public class OefenplanController(RevaliNowDbContext context) : ControllerBase
    {
        private readonly RevaliNowDbContext _context = context;

        [HttpGet]
        public ActionResult<List<Oefenplan>> Get(int patientId)
        {
            var oefenplannen = _context.Oefenplannen
                .AsNoTracking()
                .Include(p => p.Oefening)
                .Where(p => p.PatientId == patientId)
                .OrderBy(p => p.StartDatum)
                .ToList();

            return Ok(oefenplannen);
        }

        [HttpGet("{id:int}")]
        public ActionResult<Oefenplan> GetById(int patientId, int id)
        {
            var oefenplan = _context.Oefenplannen
                .AsNoTracking()
                .Include(p => p.Oefening)
                .Include(p => p.Revalidatiearts)
                .FirstOrDefault(p => p.Id == id && p.PatientId == patientId);

            if (oefenplan == null) {
                return NotFound();
            }

            return Ok(oefenplan);
        }
    }
}
