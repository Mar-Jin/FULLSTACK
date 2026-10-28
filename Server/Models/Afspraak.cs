using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    public class Afspraak
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        [Required]
        public int RevalidatieartsId { get; set; }

        public Gebruiker? Revalidatiearts { get; set; }

        [Required]
        public DateTime DatumTijdAfspraak { get; set; }

        [Required, Range(1, 480)]
        public int DuurMinuten { get; set; }

        [Required, MaxLength(20)]
        public string TypeAfspraak { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}
