using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    // Een regel in het activiteitenlogboek van een patiënt
    public class Logboek
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        [Required]
        public DateTime DatumTijdActiviteit { get; set; }

        [Required, MaxLength(500)]
        public string Beschrijving { get; set; } = string.Empty;
    }
}
