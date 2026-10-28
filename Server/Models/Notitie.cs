using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    // Notitie van een zorgverlener over een patiënt
    public class Notitie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        [Required]
        public int AuteurGebruikerId { get; set; }

        public Gebruiker? Auteur { get; set; }

        [Required]
        public DateTime DatumTijdNotitie { get; set; }

        [Required, MaxLength(1000)]
        public string Inhoud { get; set; } = string.Empty;
    }
}
