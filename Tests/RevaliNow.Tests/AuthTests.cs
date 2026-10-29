using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using RevaliNow.Models;
using RevaliNow.Services;

namespace RevaliNow.Tests;

public class AuthServiceTests
{
    private static async Task<AuthService> MaakServiceMetPatient()
    {
        var db = TestHulp.NieuweDatabase();
        db.Gebruikers.Add(TestHulp.PatientAccount(id: 30, patientId: 7, wachtwoord: "Welkom123!"));
        await db.SaveChangesAsync();
        return new AuthService(db, new TokenService(TestHulp.JwtConfiguratie()));
    }

    [Fact]
    public async Task Login_JuistWachtwoord_GeeftTokenEnPatientId()
    {
        var service = await MaakServiceMetPatient();

        var resultaat = await service.LoginAsync("patient7@example.com", "Welkom123!");

        Assert.NotNull(resultaat);
        Assert.False(string.IsNullOrEmpty(resultaat.Token));
        Assert.Equal(7, resultaat.PatientId);
        Assert.Equal(Rollen.Patient, resultaat.Rol);
    }

    [Fact]
    public async Task Login_FoutWachtwoord_GeeftNull()
    {
        var service = await MaakServiceMetPatient();

        Assert.Null(await service.LoginAsync("patient7@example.com", "fout"));
    }

    [Fact]
    public async Task Login_OnbekendeGebruiker_GeeftNull()
    {
        var service = await MaakServiceMetPatient();

        Assert.Null(await service.LoginAsync("bestaatniet@example.com", "Welkom123!"));
    }
}

public class TokenServiceTests
{
    private static JwtSecurityToken Lees(string token) => new JwtSecurityTokenHandler().ReadJwtToken(token);

    [Fact]
    public void MaakToken_Patient_BevatIdRolEnPatientId()
    {
        var service = new TokenService(TestHulp.JwtConfiguratie());

        var token = Lees(service.MaakToken(TestHulp.PatientAccount(id: 30, patientId: 7)));

        Assert.Equal("30", token.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal(Rollen.Patient, token.Claims.Single(c => c.Type == ClaimTypes.Role).Value);
        Assert.Equal("7", token.Claims.Single(c => c.Type == TokenService.PatientIdClaim).Value);
    }

    [Fact]
    public void MaakToken_Medewerker_HeeftGeenPatientId()
    {
        var service = new TokenService(TestHulp.JwtConfiguratie());
        var huisarts = new Gebruiker { Id = 1, Gebruikersnaam = "ha_test", Rol = Rollen.Huisarts, Voornaam = "A", Achternaam = "B" };

        var token = Lees(service.MaakToken(huisarts));

        Assert.DoesNotContain(token.Claims, c => c.Type == TokenService.PatientIdClaim);
    }

    [Fact]
    public void MaakToken_VerlooptNaIngesteldeTijd()
    {
        var service = new TokenService(TestHulp.JwtConfiguratie());

        var token = Lees(service.MaakToken(TestHulp.PatientAccount(id: 30, patientId: 7)));

        // ExpiresMinutes = 60 in de testconfiguratie
        Assert.InRange(token.ValidTo, DateTime.UtcNow.AddMinutes(59), DateTime.UtcNow.AddMinutes(61));
    }
}
