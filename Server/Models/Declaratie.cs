using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    // Declaratie van een behandeling of consult bij de zorgverzekeraar
    public class Declaratie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        [Required, MaxLength(20)]
        public string TypeDeclaratie { get; set; } = string.Empty;

        [Required]
        public int GedeclareerdDoorGebruikerId { get; set; }

        public Gebruiker? GedeclareerdDoor { get; set; }

        [Required]
        public DateTime DatumHandeling { get; set; }

        [Required]
        public int VergoedingsregelId { get; set; }

        public Vergoedingsregel? Vergoedingsregel { get; set; }

        [Required, MaxLength(200)]
        public string Omschrijving { get; set; } = string.Empty;

        // Bedrag in centen
        [Required, Range(0, int.MaxValue)]
        public int GedeclareerdBedrag { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}
