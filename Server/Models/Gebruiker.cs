using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RevaliNow.Models
{
    // Inlogaccount. Medewerkers komen uit de dataset, patiëntaccounts worden door de seeder toegevoegd (Rol = Patient).
    public class Gebruiker
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Gebruikersnaam { get; set; } = string.Empty;

        // Nooit meesturen in een API-response
        [Required, MaxLength(100), JsonIgnore]
        public string WachtwoordHash { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Rol { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Voornaam { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Achternaam { get; set; } = string.Empty;

        // Alleen gevuld bij medewerkers van een zorgverzekeraar
        public int? OrganisatieId { get; set; }

        public Zorgverzekeraar? Organisatie { get; set; }

        // Alleen gevuld bij patiëntaccounts
        public int? PatientId { get; set; }

        public Patient? Patient { get; set; }
    }
}
