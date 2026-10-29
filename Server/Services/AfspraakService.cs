using Microsoft.EntityFrameworkCore;
using RevaliNow.Data;
using RevaliNow.Dtos;

namespace RevaliNow.Services;

public interface IAfspraakService
{
    Task<List<AfspraakDto>> GetAfsprakenAsync(int patientId);
}

// Afspraken komen uit de view vw_AfspraakOverzicht (afspraak + naam van de arts in één query)
public class AfspraakService(RevaliNowDbContext context) : IAfspraakService
{
    // Alle afspraken op volgorde van datum; de frontend splitst ze in 'gepland' en 'eerder'
    public async Task<List<AfspraakDto>> GetAfsprakenAsync(int patientId) =>
        await context.Database.SqlQuery<AfspraakDto>($"""
                SELECT Id, DatumTijdAfspraak, DuurMinuten, TypeAfspraak, Status, Revalidatiearts, IsToekomstig
                FROM vw_AfspraakOverzicht
                WHERE PatientId = {patientId}
                ORDER BY DatumTijdAfspraak
                """)
            .ToListAsync();
}
