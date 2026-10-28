using Microsoft.EntityFrameworkCore;
using RevaliNow.Data;
using RevaliNow.Dtos;

namespace RevaliNow.Services;

public interface IAfspraakService
{
    Task<List<AfspraakDto>> GetAfsprakenAsync(int patientId);
}

public class AfspraakService(RevaliNowDbContext context, TimeProvider tijd) : IAfspraakService
{
    // Alle afspraken op volgorde van datum; de frontend splitst ze in 'gepland' en 'eerder'
    public async Task<List<AfspraakDto>> GetAfsprakenAsync(int patientId)
    {
        var nu = tijd.GetLocalNow().DateTime;

        return await context.Afspraken
            .AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .OrderBy(a => a.DatumTijdAfspraak)
            .Select(a => new AfspraakDto
            {
                Id = a.Id,
                DatumTijdAfspraak = a.DatumTijdAfspraak,
                DuurMinuten = a.DuurMinuten,
                TypeAfspraak = a.TypeAfspraak,
                Status = a.Status,
                Revalidatiearts = a.Revalidatiearts!.Voornaam + " " + a.Revalidatiearts.Achternaam,
                IsToekomstig = a.DatumTijdAfspraak >= nu
            })
            .ToListAsync();
    }
}
