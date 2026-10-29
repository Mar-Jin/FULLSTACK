using Microsoft.EntityFrameworkCore;
using RevaliNow.Data;
using RevaliNow.Dtos;

namespace RevaliNow.Services;

public interface IAuthService
{
    // Geeft null terug bij ongeldige inloggegevens
    Task<LoginResponse?> LoginAsync(string gebruikersnaam, string wachtwoord);
}

public class AuthService(RevaliNowDbContext context, ITokenService tokenService) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(string gebruikersnaam, string wachtwoord)
    {
        var gebruiker = await context.Gebruikers
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Gebruikersnaam == gebruikersnaam);

        // Bij een onbekende gebruiker en een fout wachtwoord hetzelfde resultaat,
        // zodat je niet kunt raden welke accounts bestaan.
        if (gebruiker == null || !BCrypt.Net.BCrypt.Verify(wachtwoord, gebruiker.WachtwoordHash))
            return null;

        return new LoginResponse
        {
            Token = tokenService.MaakToken(gebruiker),
            Id = gebruiker.Id,
            Voornaam = gebruiker.Voornaam,
            Rol = gebruiker.Rol,
            PatientId = gebruiker.PatientId
        };
    }
}
