namespace RevaliNow.Dtos
{
    // Gegevens van de ingelogde cliënt zoals de app ze toont
    public class PatientDto
    {
        public int Id { get; set; }
        public string Voornaam { get; set; } = string.Empty;
        public string Achternaam { get; set; } = string.Empty;
        public DateOnly Geboortedatum { get; set; }
        public string Adres { get; set; } = string.Empty;
        public string Telefoonnummer { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Huisarts { get; set; } = string.Empty;
    }
}
