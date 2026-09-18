using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevaliNow.Data;
using RevaliNow.Models;
using RevaliNow.Dtos;

namespace RevaliNow.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly RevaliNowDbContext _context;
    private readonly PasswordHasher<Gebruiker> _passwordHasher = new();

    public AuthController(RevaliNowDbContext context) => _context = context;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // 1. Validatie (Defensief programmeren)
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            return BadRequest(new { message = "Vul alle velden in." });

        var user = await _context.Gebruikers
            .FirstOrDefaultAsync(u => u.Gebruikersnaam == request.Username);

        // 2. Wachtwoord controleren tegen de opgeslagen hash
        if (user == null || _passwordHasher.VerifyHashedPassword(user, user.WachtwoordHash, request.Password) == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Ongeldige inloggegevens." });

        return Ok(new {
            user.Id,
            user.Voornaam,
            user.Rol
        });
    }
}