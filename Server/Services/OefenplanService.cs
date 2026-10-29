using Microsoft.EntityFrameworkCore;
using RevaliNow.Data;
using RevaliNow.Dtos;

namespace RevaliNow.Services;

public interface IOefenplanService
{
    Task<List<OefenplanDto>> GetOefenplannenAsync(int patientId);
    Task<OefenplanDto?> GetOefenplanAsync(int patientId, int oefenplanId);
    Task<OefenplanDto> AfvinkenAsync(int patientId, int oefenplanId, bool isAfgevinkt);
}

// Afvinken wordt opgeslagen als gebeurtenis in UitgevoerdeOefeningen (1 = uitgevoerd, 0 = ongedaan gemaakt),
// net als in de dataset. Er wordt dus nooit iets overschreven: de volledige geschiedenis blijft bewaard.
//
// De voortgang van vandaag komt uit de view vw_OefenplanVoortgang (één query in plaats van twee), en afvinken
// gaat via de stored procedure sp_OefeningAfvinken (één aanroep in plaats van drie). Zie docs/database-analyse.md.
// SqlQuery met een interpolated string maakt van elke {waarde} een SQL-parameter, dus geen SQL-injectie.
public class OefenplanService(RevaliNowDbContext context) : IOefenplanService
{
    public async Task<List<OefenplanDto>> GetOefenplannenAsync(int patientId)
    {
        var plannen = await context.Database.SqlQuery<OefenplanDto>($"""
                SELECT Id, OefeningId, Naam, Beschrijving, InstructieVideoUrl, Herhalingen, Sets, FrequentiePerDag,
                       StartDatum, EindDatum, Opmerkingen, IsActief, KeerGedaanVandaag
                FROM vw_OefenplanVoortgang
                WHERE PatientId = {patientId}
                """)
            .ToListAsync();

        // Lopende oefenplannen eerst, daarna op startdatum (volgorde van het programma)
        return plannen
            .OrderByDescending(p => p.IsActief)
            .ThenBy(p => p.StartDatum)
            .ToList();
    }

    public async Task<OefenplanDto?> GetOefenplanAsync(int patientId, int oefenplanId)
    {
        var plannen = await context.Database.SqlQuery<OefenplanDto>($"""
                SELECT Id, OefeningId, Naam, Beschrijving, InstructieVideoUrl, Herhalingen, Sets, FrequentiePerDag,
                       StartDatum, EindDatum, Opmerkingen, IsActief, KeerGedaanVandaag
                FROM vw_OefenplanVoortgang
                WHERE PatientId = {patientId} AND Id = {oefenplanId}
                """)
            .ToListAsync();

        return plannen.FirstOrDefault();
    }

    // De procedure controleert de regels (looptijd, frequentie, eigenaar) en geeft fouten met THROW;
    // FoutAfhandeling vertaalt die naar 400 of 404.
    public async Task<OefenplanDto> AfvinkenAsync(int patientId, int oefenplanId, bool isAfgevinkt)
    {
        var resultaat = await context.Database.SqlQuery<OefenplanDto>(
                $"EXEC sp_OefeningAfvinken @PatientId = {patientId}, @OefenplanId = {oefenplanId}, @IsAfgevinkt = {isAfgevinkt}")
            .ToListAsync();

        return resultaat.Single();
    }
}
