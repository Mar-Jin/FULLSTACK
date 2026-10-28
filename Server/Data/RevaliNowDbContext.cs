using Microsoft.EntityFrameworkCore;
using RevaliNow.Models;

namespace RevaliNow.Data
{
    public class RevaliNowDbContext : DbContext
    {
        public RevaliNowDbContext(DbContextOptions<RevaliNowDbContext> options)
            : base(options)
        {
        }

        public DbSet<Zorgverzekeraar> Zorgverzekeraars { get; set; }
        public DbSet<Gebruiker> Gebruikers { get; set; }
        public DbSet<Patient> Patienten { get; set; }
        public DbSet<PatientRevalidatiearts> PatientRevalidatieartsen { get; set; }
        public DbSet<Intakegesprek> Intakegesprekken { get; set; }
        public DbSet<Afspraak> Afspraken { get; set; }
        public DbSet<Oefening> Oefeningen { get; set; }
        public DbSet<Oefenplan> Oefenplannen { get; set; }
        public DbSet<UitgevoerdeOefening> UitgevoerdeOefeningen { get; set; }
        public DbSet<PijnIndicatie> PijnIndicaties { get; set; }
        public DbSet<Logboek> Logboek { get; set; }
        public DbSet<Notitie> Notities { get; set; }
        public DbSet<Medicatie> Medicaties { get; set; }
        public DbSet<Accessoire> Accessoires { get; set; }
        public DbSet<Vergoedingsregel> Vergoedingsregels { get; set; }
        public DbSet<Declaratie> Declaraties { get; set; }
        public DbSet<Declaratieverwerking> Declaratieverwerkingen { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Constraints op databaseniveau: de database bewaakt de bedrijfsregels ook als iemand
            // buiten de API om (bijv. via SSMS) data wijzigt.

            modelBuilder.Entity<Zorgverzekeraar>()
                .HasIndex(z => z.Naam).IsUnique();

            modelBuilder.Entity<Gebruiker>(e =>
            {
                e.HasIndex(g => g.Gebruikersnaam).IsUnique();
                // Een patiënt heeft hooguit één account
                e.HasIndex(g => g.PatientId).IsUnique().HasFilter("[PatientId] IS NOT NULL");
                e.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Gebruikers_Rol",
                        $"[Rol] IN ('{Rollen.Patient}', '{Rollen.Huisarts}', '{Rollen.Revalidatiearts}', '{Rollen.Zorgverzekeraar}')");
                    // Alleen patiëntaccounts zijn aan een patiënt gekoppeld
                    t.HasCheckConstraint("CK_Gebruikers_PatientId",
                        $"([Rol] = '{Rollen.Patient}' AND [PatientId] IS NOT NULL) OR ([Rol] <> '{Rollen.Patient}' AND [PatientId] IS NULL)");
                    // Alleen medewerkers van een zorgverzekeraar horen bij een organisatie
                    t.HasCheckConstraint("CK_Gebruikers_OrganisatieId",
                        $"([Rol] = '{Rollen.Zorgverzekeraar}' AND [OrganisatieId] IS NOT NULL) OR ([Rol] <> '{Rollen.Zorgverzekeraar}' AND [OrganisatieId] IS NULL)");
                });
            });

            modelBuilder.Entity<Patient>(e =>
            {
                e.HasIndex(p => p.Email).IsUnique();
                e.HasOne(p => p.Huisarts).WithMany().HasForeignKey(p => p.HuisartsId);
                e.ToTable(t => t.HasCheckConstraint("CK_Patienten_Email", "[Email] LIKE '%_@_%._%'"));
            });

            modelBuilder.Entity<PatientRevalidatiearts>(e =>
            {
                e.ToTable("PatientRevalidatieartsen");
                e.HasIndex(pr => new { pr.PatientId, pr.RevalidatieartsId }).IsUnique();
                e.HasOne(pr => pr.Revalidatiearts).WithMany().HasForeignKey(pr => pr.RevalidatieartsId);
            });

            modelBuilder.Entity<Intakegesprek>(e =>
            {
                e.HasOne(i => i.Revalidatiearts).WithMany().HasForeignKey(i => i.RevalidatieartsId);
                e.ToTable(t => t.HasCheckConstraint("CK_Intakegesprekken_ErnstBlessure",
                    "[ErnstBlessure] IN ('Licht', 'Licht tot matig', 'Matig', 'Ernstig', 'Chronisch')"));
            });

            modelBuilder.Entity<Afspraak>(e =>
            {
                e.HasOne(a => a.Revalidatiearts).WithMany().HasForeignKey(a => a.RevalidatieartsId);
                e.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Afspraken_DuurMinuten", "[DuurMinuten] BETWEEN 1 AND 480");
                    t.HasCheckConstraint("CK_Afspraken_TypeAfspraak", "[TypeAfspraak] IN ('Intake', 'Controle')");
                    t.HasCheckConstraint("CK_Afspraken_Status", "[Status] IN ('Gepland', 'Afgerond')");
                });
            });

            modelBuilder.Entity<Oefening>()
                .HasIndex(o => o.Naam).IsUnique();

            modelBuilder.Entity<Oefenplan>(e =>
            {
                e.HasOne(o => o.Revalidatiearts).WithMany().HasForeignKey(o => o.RevalidatieartsId);
                e.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Oefenplannen_Periode", "[EindDatum] >= [StartDatum]");
                    t.HasCheckConstraint("CK_Oefenplannen_Herhalingen", "[Herhalingen] BETWEEN 1 AND 100");
                    t.HasCheckConstraint("CK_Oefenplannen_Sets", "[Sets] BETWEEN 1 AND 20");
                    t.HasCheckConstraint("CK_Oefenplannen_FrequentiePerDag", "[FrequentiePerDag] BETWEEN 1 AND 10");
                });
            });

            modelBuilder.Entity<PijnIndicatie>()
                .ToTable(t => t.HasCheckConstraint("CK_PijnIndicaties_PijnScore", "[PijnScore] BETWEEN 0 AND 10"));

            modelBuilder.Entity<Notitie>()
                .HasOne(n => n.Auteur).WithMany().HasForeignKey(n => n.AuteurGebruikerId);

            modelBuilder.Entity<Medicatie>(e =>
            {
                e.ToTable("Medicaties", t =>
                {
                    t.HasCheckConstraint("CK_Medicaties_Periode", "[EindDatum] >= [StartDatum]");
                    t.HasCheckConstraint("CK_Medicaties_Status", "[Status] IN ('Actief', 'Afgerond')");
                });
                e.HasOne(m => m.Huisarts).WithMany().HasForeignKey(m => m.HuisartsId);
            });

            modelBuilder.Entity<Accessoire>(e =>
            {
                e.HasOne(a => a.Huisarts).WithMany().HasForeignKey(a => a.HuisartsId);
                e.ToTable(t => t.HasCheckConstraint("CK_Accessoires_Status", "[Status] IN ('Actief', 'Afgerond')"));
            });

            modelBuilder.Entity<Vergoedingsregel>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Vergoedingsregels_MaxVergoedBedrag", "[MaxVergoedBedrag] >= 0");
                    t.HasCheckConstraint("CK_Vergoedingsregels_MaxVergoedPercentage", "[MaxVergoedPercentage] BETWEEN 0 AND 100");
                    t.HasCheckConstraint("CK_Vergoedingsregels_Periode", "[GeldigTot] IS NULL OR [GeldigTot] >= [GeldigVanaf]");
                });

            modelBuilder.Entity<Declaratie>(e =>
            {
                e.HasOne(d => d.GedeclareerdDoor).WithMany().HasForeignKey(d => d.GedeclareerdDoorGebruikerId);
                e.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Declaraties_TypeDeclaratie",
                        $"[TypeDeclaratie] IN ('{Rollen.Huisarts}', '{Rollen.Revalidatiearts}')");
                    t.HasCheckConstraint("CK_Declaraties_Status", "[Status] IN ('Nieuw', 'Verwerkt')");
                    t.HasCheckConstraint("CK_Declaraties_GedeclareerdBedrag", "[GedeclareerdBedrag] >= 0");
                });
            });

            modelBuilder.Entity<Declaratieverwerking>(e =>
            {
                // Een declaratie wordt maar één keer verwerkt
                e.HasIndex(v => v.DeclaratieId).IsUnique();
                e.HasOne(v => v.ZorgverzekeraarMedewerker).WithMany().HasForeignKey(v => v.ZorgverzekeraarMedewerkerId);
                e.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Declaratieverwerkingen_StatusVerwerking",
                        "[StatusVerwerking] IN ('Goedgekeurd', 'Gedeeltelijk goedgekeurd', 'Afgekeurd')");
                    t.HasCheckConstraint("CK_Declaratieverwerkingen_Bedragen", "[VergoedBedrag] >= 0 AND [AfgekeurdBedrag] >= 0");
                });
            });

            // Zet alle relaties in de hele DbContext op Restrict: medische gegevens worden nooit
            // automatisch mee verwijderd (en het voorkomt 'multiple cascade paths' in SQL Server).
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
