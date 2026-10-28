using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    public class Vergoedingsregel
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Naam { get; set; } = string.Empty;

        // Bedrag in centen
        [Required, Range(0, int.MaxValue)]
        public int MaxVergoedBedrag { get; set; }

        [Required, Range(0, 100)]
        public int MaxVergoedPercentage { get; set; }

        [Required, MaxLength(100)]
        public string Voorwaarden { get; set; } = string.Empty;

        [Required]
        public DateTime GeldigVanaf { get; set; }

        public DateTime? GeldigTot { get; set; }

        [Required]
        public bool Actief { get; set; }
    }
}
