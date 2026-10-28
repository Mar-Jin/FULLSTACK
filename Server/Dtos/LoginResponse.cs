namespace RevaliNow.Dtos;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Voornaam { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int? PatientId { get; set; }
}
