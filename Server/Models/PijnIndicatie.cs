using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    public class PijnIndicatie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        [Required]
        public DateTime DatumTijdRegistratie { get; set; }

        [Required, Range(0, 10)]
        public int PijnScore { get; set; }

        [MaxLength(500)]
        public string? Toelichting { get; set; }
    }
}
