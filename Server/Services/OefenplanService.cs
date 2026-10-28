using Microsoft.EntityFrameworkCore;
using RevaliNow.Data;
using RevaliNow.Dtos;
using RevaliNow.Models;

namespace RevaliNow.Services;

public interface IOefenplanService
{
    Task<List<OefenplanDto>> GetOefenplannenAsync(int patientId);
    Task<OefenplanDto?> GetOefenplanAsync(int patientId, int oefenplanId);
    Task<OefenplanDto> AfvinkenAsync(int patientId, int oefenplanId, bool isAfgevinkt);
}

// Afvinken wordt opgeslagen als gebeurtenis in UitgevoerdeOefeningen (1 = uitgevoerd, 0 = ongedaan gemaakt),
// net als in de dataset. Er wordt dus nooit iets overschreven: de volledige geschiedenis blijft bewaard.
public class OefenplanService(RevaliNowDbContext context, TimeProvider tijd) : IOefenplanService
{
    public async Task<List<OefenplanDto>> GetOefenplannenAsync(int patientId)
    {
        var plannen = await context.Oefenplannen
            .AsNoTracking()
            .Include(o => o.Oefening)
            .Where(o => o.PatientId == patientId)
            .ToListAsync();

        var keerVandaag = await TelKeerVandaagAsync(plannen.Select(p => p.Id).ToList());

        // Lopende oefenplannen eerst, daarna op startdatum (volgorde van het programma)
        return plannen
            .Select(p => NaarDto(p, keerVandaag.GetValueOrDefault(p.Id)))
            .OrderByDescending(p => p.IsActief)
            .ThenBy(p => p.StartDatum)
            .ToList();
    }

    public async Task<OefenplanDto?> GetOefenplanAsync(int patientId, int oefenplanId)
    {
        var plan = await context.Oefenplannen
            .AsNoTracking()
            .Include(o => o.Oefening)
            .FirstOrDefaultAsync(o => o.Id == oefenplanId && o.PatientId == patientId);

        if (plan == null)
            return null;

        var keerVandaag = await TelKeerVandaagAsync([plan.Id]);
        return NaarDto(plan, keerVandaag.GetValueOrDefault(plan.Id));
    }

    public async Task<OefenplanDto> AfvinkenAsync(int patientId, int oefenplanId, bool isAfgevinkt)
    {
        var plan = await GetOefenplanAsync(patientId, oefenplanId)
            ?? throw new NietGevondenException("Oefenplan niet gevonden.");

        if (!plan.IsActief)
            throw new OngeldigeActieException("Dit oefenplan loopt vandaag niet.");
        if (isAfgevinkt && plan.IsAfgerondVandaag)
            throw new OngeldigeActieException("Deze oefening is vandaag al vaak genoeg gedaan.");
        if (!isAfgevinkt && plan.KeerGedaanVandaag == 0)
            throw new OngeldigeActieException("Er is vandaag nog niets om ongedaan te maken.");

        context.UitgevoerdeOefeningen.Add(new UitgevoerdeOefening
        {
            OefenplanId = oefenplanId,
            DatumTijdAfgevinkt = tijd.GetLocalNow().DateTime,
            IsAfgevinkt = isAfgevinkt
        });
        await context.SaveChangesAsync();

        plan.KeerGedaanVandaag += isAfgevinkt ? 1 : -1;
        return plan;
    }

    // Per oefenplan: aantal keer afgevinkt min aantal keer ongedaan gemaakt, alleen voor vandaag.
    // Eén query met GROUP BY in de database, in plaats van alle (28.000+) rijen op te halen.
    private async Task<Dictionary<int, int>> TelKeerVandaagAsync(List<int> oefenplanIds)
    {
        var vandaag = tijd.GetLocalNow().Date;
        var morgen = vandaag.AddDays(1);

        var tellingen = await context.UitgevoerdeOefeningen
            .Where(u => oefenplanIds.Contains(u.OefenplanId)
                        && u.DatumTijdAfgevinkt >= vandaag && u.DatumTijdAfgevinkt < morgen)
            .GroupBy(u => u.OefenplanId)
            .Select(g => new { OefenplanId = g.Key, Netto = g.Sum(u => u.IsAfgevinkt ? 1 : -1) })
            .ToListAsync();

        return tellingen.ToDictionary(t => t.OefenplanId, t => Math.Max(0, t.Netto));
    }

    private OefenplanDto NaarDto(Oefenplan plan, int keerVandaag)
    {
        var nu = tijd.GetLocalNow().DateTime;
        return new OefenplanDto
        {
            Id = plan.Id,
            OefeningId = plan.OefeningId,
            Naam = plan.Oefening!.Naam,
            Beschrijving = plan.Oefening.Beschrijving,
            InstructieVideoUrl = plan.Oefening.InstructieVideoUrl,
            Herhalingen = plan.Herhalingen,
            Sets = plan.Sets,
            FrequentiePerDag = plan.FrequentiePerDag,
            StartDatum = plan.StartDatum,
            EindDatum = plan.EindDatum,
            Opmerkingen = plan.Opmerkingen,
            IsActief = plan.StartDatum <= nu && plan.EindDatum >= nu,
            KeerGedaanVandaag = keerVandaag
        };
    }
}
