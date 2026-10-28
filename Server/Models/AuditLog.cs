using System.ComponentModel.DataAnnotations;

namespace RevaliNow.Models
{
    // Audit trail: wie heeft wat, wanneer gewijzigd. Wordt gevuld door database-triggers (zie migratie AuditTrail),
    // zodat ook wijzigingen buiten de API om (bijv. via SSMS) worden vastgelegd.
    public class AuditLog
    {
        [Key]
        public long Id { get; set; }

        // INSERT, UPDATE of DELETE
        [Required, MaxLength(10)]
        public string Actie { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Tabelnaam { get; set; } = string.Empty;

        [Required]
        public int RecordId { get; set; }

        // De cliënt bij wie de gewijzigde data hoort
        public int? PatientId { get; set; }

        // De ingelogde gebruiker van de API (leeg bij een wijziging buiten de API om)
        public int? GebruikerId { get; set; }

        // De SQL Server-login die de wijziging uitvoerde
        [Required, MaxLength(128)]
        public string DatabaseGebruiker { get; set; } = string.Empty;

        [Required]
        public DateTime DatumTijd { get; set; }
    }
}
