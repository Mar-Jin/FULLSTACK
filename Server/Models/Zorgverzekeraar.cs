using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    public class Zorgverzekeraar
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Naam { get; set; } = string.Empty;
    }
}
