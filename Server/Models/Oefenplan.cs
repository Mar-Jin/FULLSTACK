using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    public class Oefenplan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int OefeningId { get; set; }

        [Required]
        public int RevalidatieartsId { get; set; }

        [Required]
        public DateTime StartDatum { get; set; }

        [Required]
        public DateTime EindDatum { get; set; }

        [Required]
        public int Herhalingen { get; set; }

        [Required]
        public int Sets { get; set; }

        [Required]
        public int FrequentiePerDag { get; set; }

        [Required]
        public string Opmerkingen { get; set; }

        public Patient? Patient { get; set; }

        public Oefening? Oefening { get; set; }

        public Gebruiker? Revalidatiearts { get; set; }
    }
}
