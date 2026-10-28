using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    public class Intakegesprek
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        [Required]
        public int RevalidatieartsId { get; set; }

        public Gebruiker? Revalidatiearts { get; set; }

        [Required, MaxLength(100)]
        public string Diagnose { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string ErnstBlessure { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Behandeldoelen { get; set; } = string.Empty;

        [Required]
        public DateTime DatumIntake { get; set; }
    }
}
