using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RevaliNow.Models;

namespace RevaliNow.Services;

public interface ITokenService
{
    string MaakToken(Gebruiker gebruiker);
}

// Maakt een JWT met daarin wie de gebruiker is, welke rol hij heeft en (bij een patiënt) welk PatientId bij hem hoort.
// De API gebruikt die claims om te controleren of iemand bij de opgevraagde data mag.
public class TokenService(IConfiguration configuration) : ITokenService
{
    public const string PatientIdClaim = "patientId";

    public string MaakToken(Gebruiker gebruiker)
    {
        var jwt = configuration.GetSection("Jwt");
        var sleutel = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, gebruiker.Id.ToString()),
            new(ClaimTypes.Name, gebruiker.Gebruikersnaam),
            new(ClaimTypes.Role, gebruiker.Rol)
        };

        if (gebruiker.PatientId is not null)
            claims.Add(new Claim(PatientIdClaim, gebruiker.PatientId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwt.GetValue("ExpiresMinutes", 60)),
            signingCredentials: new SigningCredentials(sleutel, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
