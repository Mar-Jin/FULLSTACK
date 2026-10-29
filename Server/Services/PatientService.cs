using Microsoft.EntityFrameworkCore;
using RevaliNow.Data;
using RevaliNow.Dtos;

namespace RevaliNow.Services;

public interface IPatientService
{
    Task<PatientDto?> GetPatientAsync(int patientId);
    Task<DashboardResponseDto?> GetDashboardAsync(int patientId);
}

public class PatientService(RevaliNowDbContext context, IPijnIndicatieService pijnIndicatieService) : IPatientService
{
    public async Task<PatientDto?> GetPatientAsync(int patientId) =>
        await context.Patienten
            .AsNoTracking()
            .Where(p => p.Id == patientId)
            .Select(p => new PatientDto
            {
                Id = p.Id,
                Voornaam = p.Voornaam,
                Achternaam = p.Achternaam,
                Geboortedatum = p.Geboortedatum,
                Adres = p.Adres,
                Telefoonnummer = p.Telefoonnummer,
                Email = p.Email,
                Huisarts = p.Huisarts!.Voornaam + " " + p.Huisarts.Achternaam
            })
            .FirstOrDefaultAsync();

    // De oefeningen van vandaag haalt de frontend apart op (GET oefenplannen),
    // zodat het dashboard en de oefenpagina dezelfde state delen.
    public async Task<DashboardResponseDto?> GetDashboardAsync(int patientId)
    {
        var patient = await GetPatientAsync(patientId);
        if (patient == null)
            return null;

        var pijn = await pijnIndicatieService.GetPijnIndicatiesAsync(patientId);

        // Laatste pijnscore, gemiddelde van 7 dagen en volgende afspraak in één aanroep
        var samenvatting = (await context.Database
                .SqlQuery<DashboardSamenvattingDto>($"EXEC sp_DashboardSamenvatting @PatientId = {patientId}")
                .ToListAsync())
            .Single();

        return new DashboardResponseDto
        {
            User = patient,
            RecentPains = pijn.Take(2).ToList(),
            PainData = pijn,
            Samenvatting = samenvatting
        };
    }
}
