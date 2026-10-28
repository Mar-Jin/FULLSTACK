using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Dtos
{
    public class LogboekDto
    {
        public int Id { get; set; }
        public DateTime DatumTijdActiviteit { get; set; }
        public string Beschrijving { get; set; } = string.Empty;
    }

    // Wat de cliënt zelf mag invullen. De datum/tijd en de patiënt bepaalt de server.
    public class LogboekInvoerDto
    {
        [Required(ErrorMessage = "Beschrijf de activiteit.")]
        [MaxLength(500, ErrorMessage = "De beschrijving mag maximaal 500 tekens zijn.")]
        public string Beschrijving { get; set; } = string.Empty;
    }
}
