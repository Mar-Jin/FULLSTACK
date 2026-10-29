using Microsoft.AspNetCore.Mvc;
using RevaliNow.Dtos;
using RevaliNow.Services;

namespace RevaliNow.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    // POST auth/login
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        // Validatie (Defensief programmeren)
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Vul alle velden in." });

        var resultaat = await authService.LoginAsync(request.Username, request.Password);

        if (resultaat == null)
            return Unauthorized(new { message = "Ongeldige inloggegevens." });

        return Ok(resultaat);
    }
}
