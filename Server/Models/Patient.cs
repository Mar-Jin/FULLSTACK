using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Voornaam { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Achternaam { get; set; } = string.Empty;

        [Required]
        public DateOnly Geboortedatum { get; set; }

        [Required, MaxLength(100)]
        public string Adres { get; set; } = string.Empty;

        [Required, MaxLength(15)]
        public string Telefoonnummer { get; set; } = string.Empty;

        [Required, MaxLength(100), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public int HuisartsId { get; set; }

        public Gebruiker? Huisarts { get; set; }

        [Required]
        public int ZorgverzekeraarId { get; set; }

        public Zorgverzekeraar? Zorgverzekeraar { get; set; }
    }
}
