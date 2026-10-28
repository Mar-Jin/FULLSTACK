using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    // Elke keer dat een patiënt een oefening uit zijn oefenplan afvinkt (of weer ongedaan maakt)
    public class UitgevoerdeOefening
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OefenplanId { get; set; }

        public Oefenplan? Oefenplan { get; set; }

        [Required]
        public DateTime DatumTijdAfgevinkt { get; set; }

        [Required]
        public bool IsAfgevinkt { get; set; }
    }
}
