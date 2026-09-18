using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RevaliNow.Models
{
    public class PijnIndicatie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public DateTime DatumTijdRegistratie { get; set; }

        [Required]
        [Range(0, 10)]
        public int PijnScore { get; set; }

        public string? Toelichting { get; set; }

        public Patient? Patient { get; set; }
    }
}