using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RevaliNow.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50), MinLength(1)]
        public string Voornaam { get; set; }

        [Required]
        [MaxLength(50), MinLength(1)]
        public string Achternaam { get; set; }

        [Required]
        public DateTime Geboortedatum { get; set; }

        [Required]
        [MaxLength(50), MinLength(1)]
        public string Straatnaam { get; set; }

        [Required]
        [MaxLength(10), MinLength(1)]
        public string Huisnummer { get; set; }

        [Required]
        [StringLength(9)]
        public string Telefoonnummer { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public int HuisartsId { get; set; }

        public Gebruiker? Huisarts { get; set; }
    }
}


