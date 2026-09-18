using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RevaliNow.Models
{
    public class Gebruiker
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Gebruikersnaam { get; set; }

        [Required]
        public string WachtwoordHash { get; set; }

        [Required]
        public string Rol { get; set; }

        [Required]
        public string Voornaam { get; set; }

        [Required]
        public string Achternaam { get; set; }

        [AllowNull]
        public int? OrganisatieId { get; set; }

        [AllowNull]
        public Zorgverzekeraar? Organisatie { get; set; }
    }
}