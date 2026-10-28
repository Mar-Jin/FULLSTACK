using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    // Een oefening die een revalidatiearts aan een patiënt heeft toegewezen, met schema en looptijd
    public class Oefenplan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        [Required]
        public int OefeningId { get; set; }

        public Oefening? Oefening { get; set; }

        [Required]
        public int RevalidatieartsId { get; set; }

        public Gebruiker? Revalidatiearts { get; set; }

        [Required]
        public DateTime StartDatum { get; set; }

        [Required]
        public DateTime EindDatum { get; set; }

        [Required, Range(1, 100)]
        public int Herhalingen { get; set; }

        [Required, Range(1, 20)]
        public int Sets { get; set; }

        [Required, Range(1, 10)]
        public int FrequentiePerDag { get; set; }

        [MaxLength(200)]
        public string? Opmerkingen { get; set; }
    }
}
