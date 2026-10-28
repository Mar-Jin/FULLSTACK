using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    public class Medicatie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        [Required]
        public int HuisartsId { get; set; }

        public Gebruiker? Huisarts { get; set; }

        [Required, MaxLength(100)]
        public string Naam { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Dosering { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Frequentie { get; set; } = string.Empty;

        [Required]
        public DateTime StartDatum { get; set; }

        [Required]
        public DateTime EindDatum { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}
