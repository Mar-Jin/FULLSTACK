using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    public class UitgevoerdeOefening
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientOefenplanId { get; set; }

        [Required]
        public DateTime DatumTijdAfgevinkt { get; set; }

        [Required]
        public bool IsAfgevinkt { get; set; }

        public Oefenplan? PatientOefenplan { get; set; }
    }
}