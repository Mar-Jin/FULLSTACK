using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    // Koppeltabel: welke revalidatiearts(en) een patiënt behandelen
    public class PatientRevalidatiearts
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        [Required]
        public int RevalidatieartsId { get; set; }

        public Gebruiker? Revalidatiearts { get; set; }
    }
}
