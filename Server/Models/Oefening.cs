using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    public class Oefening
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Naam { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Beschrijving { get; set; } = string.Empty;

        [MaxLength(500), Url]
        public string? InstructieVideoUrl { get; set; }
    }
}
