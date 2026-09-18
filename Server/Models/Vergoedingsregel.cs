using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    public class Vergoedingsregel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100), MinLength(3)]
        public string Naam { get; set; }

        [Required]

        public int MaxVergoedBedrag { get; set; }

        [Required]
        [Range(0, 100)]
        public int MaxVergoedPercentage { get; set; }

        [Required]
        public string Voorwaarden { get; set; }

        [Required]
        public DateTime GeldigVanaf { get; set; }

        [Required]
        public bool Actief { get; set; }
    }
}