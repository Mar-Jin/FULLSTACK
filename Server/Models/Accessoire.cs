using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    // Hulpmiddel (bijv. krukken) dat een huisarts adviseert
    public class Accessoire
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

        [Required]
        public DateTime AdviesDatum { get; set; }

        [Required, MaxLength(20)]
        public string VerwachteGebruiksperiode { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}
