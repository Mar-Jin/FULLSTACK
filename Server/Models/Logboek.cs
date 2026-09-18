using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    public class Logboek
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public DateTime DatumTijdActiviteit { get; set; }

        [Required]
        public string Beschrijving { get; set; }

        public Patient? Patient { get; set; }
    }
}
