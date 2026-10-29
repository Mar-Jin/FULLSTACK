using Microsoft.EntityFrameworkCore;
using RevaliNow.Data;
using RevaliNow.Dtos;
using RevaliNow.Models;

namespace RevaliNow.Services;

public interface IPijnIndicatieService
{
    Task<List<PijnIndicatieDto>> GetPijnIndicatiesAsync(int patientId);
    Task<PijnIndicatieDto?> GetPijnIndicatieAsync(int patientId, int id);
    Task<List<PijnWeekDto>> GetPerWeekAsync(int patientId);
    Task<PijnIndicatieDto> RegistreerAsync(int patientId, PijnIndicatieInvoerDto invoer);
}

public class PijnIndicatieService(RevaliNowDbContext context, TimeProvider tijd) : IPijnIndicatieService
{
    // Nieuwste eerst. Registraties 'in de toekomst' (komen voor in de dataset) laten we weg.
    public async Task<List<PijnIndicatieDto>> GetPijnIndicatiesAsync(int patientId)
    {
        var nu = tijd.GetLocalNow().DateTime;
        return await context.PijnIndicaties
            .AsNoTracking()
            .Where(p => p.PatientId == patientId && p.DatumTijdRegistratie <= nu)
            .OrderByDescending(p => p.DatumTijdRegistratie)
            .Select(p => NaarDto(p))
            .ToListAsync();
    }

    public async Task<PijnIndicatieDto?> GetPijnIndicatieAsync(int patientId, int id)
    {
        var pijn = await context.PijnIndicaties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.PatientId == patientId);

        return pijn == null ? null : NaarDto(pijn);
    }

    // Weekgemiddelde, berekend in de database (sp_PijnverloopPerWeek)
    public async Task<List<PijnWeekDto>> GetPerWeekAsync(int patientId) =>
        await context.Database
            .SqlQuery<PijnWeekDto>($"EXEC sp_PijnverloopPerWeek @PatientId = {patientId}")
            .ToListAsync();

    // De patiënt en het tijdstip komen niet uit de request, maar uit de route en de server,
    // zodat een cliënt nooit voor iemand anders of met een verzonnen datum kan registreren.
    public async Task<PijnIndicatieDto> RegistreerAsync(int patientId, PijnIndicatieInvoerDto invoer)
    {
        var pijn = new PijnIndicatie
        {
            PatientId = patientId,
            DatumTijdRegistratie = tijd.GetLocalNow().DateTime,
            PijnScore = invoer.PijnScore!.Value,
            Toelichting = string.IsNullOrWhiteSpace(invoer.Toelichting) ? null : invoer.Toelichting.Trim()
        };

        context.PijnIndicaties.Add(pijn);
        await context.SaveChangesAsync();
        return NaarDto(pijn);
    }

    private static PijnIndicatieDto NaarDto(PijnIndicatie p) => new()
    {
        Id = p.Id,
        DatumTijdRegistratie = p.DatumTijdRegistratie,
        PijnScore = p.PijnScore,
        Toelichting = p.Toelichting
    };
}
