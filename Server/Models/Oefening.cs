using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    public class Oefening
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100), MinLength(3)]
        public string Naam { get; set; }

        [Required]
        public string Beschrijving { get; set; }

        [Required]
        public string InstructieVideoUrl { get; set; }
    }
}