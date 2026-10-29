using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RevaliNow.Migrations
{
    /// <summary>
    /// Validatie-triggers: bedrijfsregels die een CHECK-constraint niet kan afdwingen, omdat ze over meerdere
    /// rijen of tabellen gaan. Ze gelden ook als iemand buiten de API om (bijv. via SSMS) data wijzigt.
    /// Een overtreding breekt de transactie af met THROW; de API vertaalt foutnummers 50000-50999 naar 400.
    /// </summary>
    public partial class ValidatieTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Afvinken van een oefening
            migrationBuilder.Sql("""
                CREATE TRIGGER TR_UitgevoerdeOefeningen_Validatie ON UitgevoerdeOefeningen
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- De dataset bevat historische afwijkingen (afvinkingen buiten de looptijd); die laten we staan
                    IF SESSION_CONTEXT(N'Seeding') = 1 RETURN;

                    -- Regel 1: alleen afvinken binnen de looptijd van het oefenplan
                    IF EXISTS (
                        SELECT 1 FROM inserted i
                        JOIN Oefenplannen o ON o.Id = i.OefenplanId
                        WHERE i.DatumTijdAfgevinkt < o.StartDatum OR i.DatumTijdAfgevinkt > o.EindDatum)
                        THROW 50001, 'Afvinken kan alleen binnen de looptijd van het oefenplan.', 1;

                    -- Regel 2: niet vooruit afvinken (5 minuten speling voor klokverschil)
                    IF EXISTS (SELECT 1 FROM inserted WHERE DatumTijdAfgevinkt > DATEADD(MINUTE, 5, SYSDATETIME()))
                        THROW 50002, 'Een oefening kan niet in de toekomst worden afgevinkt.', 1;

                    -- Regel 3: per dag niet vaker afvinken dan de frequentie, en niet vaker ongedaan maken dan afgevinkt.
                    -- Netto = aantal keer afgevinkt min aantal keer ongedaan gemaakt op die dag (inclusief de nieuwe rij).
                    IF EXISTS (
                        SELECT 1
                        FROM inserted i
                        JOIN Oefenplannen o ON o.Id = i.OefenplanId
                        CROSS APPLY (
                            SELECT SUM(CASE WHEN u.IsAfgevinkt = 1 THEN 1 ELSE -1 END) AS Netto
                            FROM UitgevoerdeOefeningen u
                            WHERE u.OefenplanId = i.OefenplanId
                              AND CAST(u.DatumTijdAfgevinkt AS date) = CAST(i.DatumTijdAfgevinkt AS date)
                        ) dag
                        WHERE (i.IsAfgevinkt = 1 AND dag.Netto > o.FrequentiePerDag)
                           OR (i.IsAfgevinkt = 0 AND dag.Netto < 0))
                        THROW 50003, 'Deze oefening is vandaag al vaak genoeg gedaan, of er is niets om ongedaan te maken.', 1;
                END
                """);

            // Pijnregistratie
            migrationBuilder.Sql("""
                CREATE TRIGGER TR_PijnIndicaties_Validatie ON PijnIndicaties
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- De dataset bevat ook registraties met een datum in de toekomst; die laten we staan
                    IF SESSION_CONTEXT(N'Seeding') = 1 RETURN;

                    -- Een pijnscore gaat over hoe je je nu voelt of voelde: niet vooruit registreren
                    IF EXISTS (SELECT 1 FROM inserted WHERE DatumTijdRegistratie > DATEADD(MINUTE, 5, SYSDATETIME()))
                        THROW 50010, 'Een pijnindicatie kan niet in de toekomst worden geregistreerd.', 1;
                END
                """);

            // Rolcontroles: een foreign key controleert alleen dát de gebruiker bestaat, niet welke rol hij heeft.
            // Deze triggers draaien ook tijdens het seeden, zodat de dataset zelf ook gecontroleerd wordt.
            migrationBuilder.Sql(RolcontroleTrigger("Patienten", "HuisartsId", "Huisarts", 50020,
                "De huisarts van een patiënt moet een gebruiker met de rol Huisarts zijn."));
            migrationBuilder.Sql(RolcontroleTrigger("Afspraken", "RevalidatieartsId", "Revalidatiearts", 50021,
                "Een afspraak moet bij een gebruiker met de rol Revalidatiearts zijn."));
            migrationBuilder.Sql(RolcontroleTrigger("Oefenplannen", "RevalidatieartsId", "Revalidatiearts", 50022,
                "Een oefenplan moet door een gebruiker met de rol Revalidatiearts zijn opgesteld."));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_UitgevoerdeOefeningen_Validatie;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_PijnIndicaties_Validatie;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_Patienten_Rolcontrole;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_Afspraken_Rolcontrole;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_Oefenplannen_Rolcontrole;");
        }

        private static string RolcontroleTrigger(string tabel, string kolom, string rol, int foutnummer, string melding) => $"""
            CREATE TRIGGER TR_{tabel}_Rolcontrole ON [{tabel}]
            AFTER INSERT, UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                -- Alleen controleren als de kolom echt (mogelijk) gewijzigd is
                IF NOT UPDATE([{kolom}]) RETURN;

                IF EXISTS (
                    SELECT 1 FROM inserted i
                    JOIN Gebruikers g ON g.Id = i.[{kolom}]
                    WHERE g.Rol <> '{rol}')
                    THROW {foutnummer}, N'{melding}', 1;
            END
            """;
    }
}
