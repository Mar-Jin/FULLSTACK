using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    // Beoordeling van een declaratie door een medewerker van de zorgverzekeraar
    public class Declaratieverwerking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DeclaratieId { get; set; }

        public Declaratie? Declaratie { get; set; }

        [Required]
        public int ZorgverzekeraarMedewerkerId { get; set; }

        public Gebruiker? ZorgverzekeraarMedewerker { get; set; }

        [Required]
        public DateTime DatumVerwerking { get; set; }

        [Required, MaxLength(30)]
        public string StatusVerwerking { get; set; } = string.Empty;

        // Bedragen in centen
        [Required, Range(0, int.MaxValue)]
        public int VergoedBedrag { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int AfgekeurdBedrag { get; set; }

        [MaxLength(200)]
        public string? Toelichting { get; set; }
    }
}
