using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Dtos
{
    public class PijnIndicatieDto
    {
        public int Id { get; set; }
        public DateTime DatumTijdRegistratie { get; set; }
        public int PijnScore { get; set; }
        public string? Toelichting { get; set; }
    }

    // Wat de cliënt zelf invult. De patiënt en het tijdstip bepaalt de server.
    public class PijnIndicatieInvoerDto
    {
        [Required(ErrorMessage = "Kies een pijnscore.")]
        [Range(0, 10, ErrorMessage = "De pijnscore moet tussen 0 en 10 liggen.")]
        public int? PijnScore { get; set; }

        [MaxLength(500, ErrorMessage = "De toelichting mag maximaal 500 tekens zijn.")]
        public string? Toelichting { get; set; }
    }
}
