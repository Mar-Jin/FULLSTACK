using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliNow.Data;
using RevaliNow.Dtos;
using RevaliNow.Services;

namespace RevaliNow.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(RevaliNowDbContext context, ITokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        // 1. Validatie (Defensief programmeren)
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Vul alle velden in." });

        var user = await context.Gebruikers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Gebruikersnaam == request.Username);

        // 2. Wachtwoord controleren tegen de bcrypt-hash.
        // Bij een onbekende gebruiker en een fout wachtwoord dezelfde melding, zodat je niet kunt raden welke accounts bestaan.
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.WachtwoordHash))
            return Unauthorized(new { message = "Ongeldige inloggegevens." });

        return Ok(new LoginResponse
        {
            Token = tokenService.MaakToken(user),
            Id = user.Id,
            Voornaam = user.Voornaam,
            Rol = user.Rol,
            PatientId = user.PatientId
        });
    }
}
