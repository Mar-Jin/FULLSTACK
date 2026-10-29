using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Time.Testing;
using RevaliNow.Data;
using RevaliNow.Models;

namespace RevaliNow.Tests;

// Gedeelde hulpmiddelen voor de tests
public static class TestHulp
{
    // Vast 'nu' voor alle tests, zodat ze elke dag hetzelfde resultaat geven
    public static readonly DateTime Nu = new(2026, 10, 29, 12, 0, 0);

    // Elke test een eigen, lege database in het geheugen (geen SQL Server nodig)
    public static RevaliNowDbContext NieuweDatabase() =>
        new(new DbContextOptionsBuilder<RevaliNowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    public static FakeTimeProvider Tijd()
    {
        var tijd = new FakeTimeProvider(new DateTimeOffset(Nu, TimeSpan.Zero));
        tijd.SetLocalTimeZone(TimeZoneInfo.Utc);
        return tijd;
    }

    public static IConfiguration JwtConfiguratie() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "een-testsleutel-die-zeker-lang-genoeg-is-voor-hmac-sha256",
                ["Jwt:Issuer"] = "RevaliNow",
                ["Jwt:Audience"] = "RevaliNow",
                ["Jwt:ExpiresMinutes"] = "60"
            })
            .Build();

    public static Gebruiker PatientAccount(int id, int patientId, string wachtwoord = "Welkom123!") => new()
    {
        Id = id,
        Gebruikersnaam = $"patient{patientId}@example.com",
        // Lage workfactor: in tests hoeft bcrypt niet traag te zijn
        WachtwoordHash = BCrypt.Net.BCrypt.HashPassword(wachtwoord, workFactor: 4),
        Rol = Rollen.Patient,
        Voornaam = "Test",
        Achternaam = "Patient",
        PatientId = patientId
    };
}
