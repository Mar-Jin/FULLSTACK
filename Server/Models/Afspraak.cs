using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    public class Afspraak
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int RevalidatieartsId { get; set; }

        [Required]
        public DateTime DatumTijdAfspraak { get; set; }

        [Required]
        public int DuurMinuten { get; set; }

        [Required]
        [MaxLength(50), MinLength(1)]
        public string TypeAfspraak { get; set; }

        [Required]
        [MaxLength(20), MinLength(1)]
        public string Status { get; set; }

        public Patient? Patient { get; set; }

        public Gebruiker? Revalidatiearts { get; set; }
    }
}