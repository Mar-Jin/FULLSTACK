using Microsoft.EntityFrameworkCore;
using RevaliNow.Data;
using RevaliNow.Dtos;
using RevaliNow.Models;

namespace RevaliNow.Services;

public interface ILogboekService
{
    Task<List<LogboekDto>> GetLogboekAsync(int patientId);
    Task<LogboekDto?> GetInvoerAsync(int patientId, int id);
    Task<LogboekDto> ToevoegenAsync(int patientId, LogboekInvoerDto invoer);
    Task<LogboekDto> BewerkenAsync(int patientId, int id, LogboekInvoerDto invoer);
    Task VerwijderenAsync(int patientId, int id);
}

public class LogboekService(RevaliNowDbContext context, TimeProvider tijd) : ILogboekService
{
    // Nieuwste activiteit bovenaan
    public async Task<List<LogboekDto>> GetLogboekAsync(int patientId) =>
        await context.Logboek
            .AsNoTracking()
            .Where(l => l.PatientId == patientId)
            .OrderByDescending(l => l.DatumTijdActiviteit)
            .Select(l => NaarDto(l))
            .ToListAsync();

    public async Task<LogboekDto?> GetInvoerAsync(int patientId, int id)
    {
        var invoer = await context.Logboek
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id && l.PatientId == patientId);

        return invoer == null ? null : NaarDto(invoer);
    }

    public async Task<LogboekDto> ToevoegenAsync(int patientId, LogboekInvoerDto invoer)
    {
        var logboek = new Logboek
        {
            PatientId = patientId,
            DatumTijdActiviteit = tijd.GetLocalNow().DateTime,
            Beschrijving = SchoonOp(invoer.Beschrijving)
        };

        context.Logboek.Add(logboek);
        await context.SaveChangesAsync();
        return NaarDto(logboek);
    }

    // Alleen de beschrijving is te wijzigen; het tijdstip van de activiteit blijft staan
    public async Task<LogboekDto> BewerkenAsync(int patientId, int id, LogboekInvoerDto invoer)
    {
        var logboek = await ZoekAsync(patientId, id);
        logboek.Beschrijving = SchoonOp(invoer.Beschrijving);
        await context.SaveChangesAsync();
        return NaarDto(logboek);
    }

    public async Task VerwijderenAsync(int patientId, int id)
    {
        var logboek = await ZoekAsync(patientId, id);
        context.Logboek.Remove(logboek);
        await context.SaveChangesAsync();
    }

    // Altijd ook op PatientId filteren: een ID van iemand anders geeft 'niet gevonden'
    private async Task<Logboek> ZoekAsync(int patientId, int id) =>
        await context.Logboek.FirstOrDefaultAsync(l => l.Id == id && l.PatientId == patientId)
        ?? throw new NietGevondenException("Logboekregel niet gevonden.");

    private static string SchoonOp(string beschrijving)
    {
        var tekst = beschrijving.Trim();
        if (tekst.Length == 0)
            throw new OngeldigeActieException("Beschrijf de activiteit.");
        return tekst;
    }

    private static LogboekDto NaarDto(Logboek l) => new()
    {
        Id = l.Id,
        DatumTijdActiviteit = l.DatumTijdActiviteit,
        Beschrijving = l.Beschrijving
    };
}
