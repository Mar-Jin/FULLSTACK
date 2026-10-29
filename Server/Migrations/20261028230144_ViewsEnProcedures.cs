using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RevaliNow.Migrations
{
    /// <summary>
    /// Views en stored procedures om het aantal round-trips naar de database te verminderen.
    /// Analyse en metingen: docs/database-analyse.md
    /// </summary>
    public partial class ViewsEnProcedures : Migration
    {
        // Begin en eind van 'vandaag' als datetime2, zodat de index op DatumTijdAfgevinkt gebruikt kan worden
        // (een vergelijking als CAST(kolom AS date) = ... zou de index niet kunnen gebruiken)
        private const string BeginVandaag = "CAST(CAST(SYSDATETIME() AS date) AS datetime2)";
        private const string BeginMorgen = "DATEADD(DAY, 1, CAST(CAST(SYSDATETIME() AS date) AS datetime2))";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Oefenplan + oefening + voortgang van vandaag in één query
            migrationBuilder.Sql($"""
                CREATE VIEW vw_OefenplanVoortgang
                AS
                SELECT o.Id, o.PatientId, o.OefeningId,
                       oe.Naam, oe.Beschrijving, oe.InstructieVideoUrl,
                       o.Herhalingen, o.Sets, o.FrequentiePerDag, o.StartDatum, o.EindDatum, o.Opmerkingen,
                       CAST(CASE WHEN SYSDATETIME() BETWEEN o.StartDatum AND o.EindDatum THEN 1 ELSE 0 END AS bit) AS IsActief,
                       -- Netto aantal keer vandaag: afgevinkt (+1) min ongedaan gemaakt (-1), nooit onder 0
                       CASE WHEN v.Netto > 0 THEN v.Netto ELSE 0 END AS KeerGedaanVandaag
                FROM Oefenplannen o
                JOIN Oefeningen oe ON oe.Id = o.OefeningId
                OUTER APPLY (
                    SELECT SUM(CASE WHEN u.IsAfgevinkt = 1 THEN 1 ELSE -1 END) AS Netto
                    FROM UitgevoerdeOefeningen u
                    WHERE u.OefenplanId = o.Id
                      AND u.DatumTijdAfgevinkt >= {BeginVandaag}
                      AND u.DatumTijdAfgevinkt < {BeginMorgen}
                ) v;
                """);

            // Afspraak met de naam van de revalidatiearts
            migrationBuilder.Sql("""
                CREATE VIEW vw_AfspraakOverzicht
                AS
                SELECT a.Id, a.PatientId, a.DatumTijdAfspraak, a.DuurMinuten, a.TypeAfspraak, a.Status,
                       g.Voornaam + ' ' + g.Achternaam AS Revalidatiearts,
                       CAST(CASE WHEN a.DatumTijdAfspraak >= SYSDATETIME() THEN 1 ELSE 0 END AS bit) AS IsToekomstig
                FROM Afspraken a
                JOIN Gebruikers g ON g.Id = a.RevalidatieartsId;
                """);

            // Afvinken of ongedaan maken: controleren + opslaan + nieuwe voortgang teruggeven in één aanroep
            migrationBuilder.Sql($"""
                CREATE PROCEDURE sp_OefeningAfvinken
                    @PatientId int,
                    @OefenplanId int,
                    @IsAfgevinkt bit
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SET XACT_ABORT ON; -- bij een fout altijd de hele transactie terugdraaien

                    DECLARE @Nu datetime2 = SYSDATETIME();
                    DECLARE @Frequentie int, @Start datetime2, @Eind datetime2, @KeerVandaag int;

                    BEGIN TRANSACTION;

                    -- UPDLOCK + HOLDLOCK: twee gelijktijdige klikken op hetzelfde oefenplan worden na elkaar verwerkt,
                    -- zodat ze niet allebei 'nog 1 keer te gaan' zien en de frequentie overschrijden
                    SELECT @Frequentie = FrequentiePerDag, @Start = StartDatum, @Eind = EindDatum
                    FROM Oefenplannen WITH (UPDLOCK, HOLDLOCK)
                    WHERE Id = @OefenplanId AND PatientId = @PatientId;

                    IF @Frequentie IS NULL
                        THROW 50404, N'Oefenplan niet gevonden.', 1;
                    IF @Nu < @Start OR @Nu > @Eind
                        THROW 50004, N'Dit oefenplan loopt vandaag niet.', 1;

                    SELECT @KeerVandaag = ISNULL(SUM(CASE WHEN IsAfgevinkt = 1 THEN 1 ELSE -1 END), 0)
                    FROM UitgevoerdeOefeningen
                    WHERE OefenplanId = @OefenplanId
                      AND DatumTijdAfgevinkt >= {BeginVandaag}
                      AND DatumTijdAfgevinkt < {BeginMorgen};

                    IF @IsAfgevinkt = 1 AND @KeerVandaag >= @Frequentie
                        THROW 50005, N'Deze oefening is vandaag al vaak genoeg gedaan.', 1;
                    IF @IsAfgevinkt = 0 AND @KeerVandaag <= 0
                        THROW 50006, N'Er is vandaag nog niets om ongedaan te maken.', 1;

                    INSERT INTO UitgevoerdeOefeningen (OefenplanId, DatumTijdAfgevinkt, IsAfgevinkt)
                    VALUES (@OefenplanId, @Nu, @IsAfgevinkt);

                    COMMIT TRANSACTION;

                    SELECT Id, PatientId, OefeningId, Naam, Beschrijving, InstructieVideoUrl, Herhalingen, Sets,
                           FrequentiePerDag, StartDatum, EindDatum, Opmerkingen, IsActief, KeerGedaanVandaag
                    FROM vw_OefenplanVoortgang
                    WHERE Id = @OefenplanId;
                END
                """);

            // Samenvatting voor het dashboard in één aanroep. Registraties 'in de toekomst' (komen voor in de dataset)
            // tellen niet mee als 'laatste' of 'afgelopen week'.
            migrationBuilder.Sql("""
                CREATE PROCEDURE sp_DashboardSamenvatting
                    @PatientId int
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DECLARE @Nu datetime2 = SYSDATETIME();

                    SELECT laatste.PijnScore            AS LaatstePijnScore,
                           laatste.DatumTijdRegistratie AS LaatstePijnDatum,
                           week.Gemiddelde              AS GemiddeldePijn7Dagen,
                           week.Aantal                  AS AantalMetingen7Dagen,
                           volgende.DatumTijdAfspraak   AS VolgendeAfspraakDatum,
                           volgende.DuurMinuten         AS VolgendeAfspraakDuur,
                           volgende.TypeAfspraak        AS VolgendeAfspraakType,
                           volgende.Status              AS VolgendeAfspraakStatus,
                           volgende.Revalidatiearts     AS VolgendeAfspraakArts
                    FROM (SELECT 1 AS Rij) AS basis
                    OUTER APPLY (
                        SELECT TOP 1 PijnScore, DatumTijdRegistratie
                        FROM PijnIndicaties
                        WHERE PatientId = @PatientId AND DatumTijdRegistratie <= @Nu
                        ORDER BY DatumTijdRegistratie DESC
                    ) AS laatste
                    OUTER APPLY (
                        SELECT CAST(AVG(CAST(PijnScore AS decimal(4, 1))) AS decimal(3, 1)) AS Gemiddelde, COUNT(*) AS Aantal
                        FROM PijnIndicaties
                        WHERE PatientId = @PatientId
                          AND DatumTijdRegistratie > DATEADD(DAY, -7, @Nu) AND DatumTijdRegistratie <= @Nu
                    ) AS week
                    OUTER APPLY (
                        SELECT TOP 1 DatumTijdAfspraak, DuurMinuten, TypeAfspraak, Status, Revalidatiearts
                        FROM vw_AfspraakOverzicht
                        WHERE PatientId = @PatientId AND DatumTijdAfspraak >= @Nu
                        ORDER BY DatumTijdAfspraak
                    ) AS volgende;
                END
                """);

            // Weekgemiddelde van de pijnscore (week begint op maandag; 1 januari 1900 was een maandag)
            migrationBuilder.Sql("""
                CREATE PROCEDURE sp_PijnverloopPerWeek
                    @PatientId int
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT w.WeekStart,
                           CAST(AVG(CAST(p.PijnScore AS decimal(4, 1))) AS decimal(3, 1)) AS GemiddeldePijn,
                           MIN(p.PijnScore) AS LaagstePijn,
                           MAX(p.PijnScore) AS HoogstePijn,
                           COUNT(*)         AS AantalMetingen
                    FROM PijnIndicaties p
                    CROSS APPLY (
                        SELECT DATEADD(DAY, -(DATEDIFF(DAY, '19000101', p.DatumTijdRegistratie) % 7),
                                       CAST(p.DatumTijdRegistratie AS date)) AS WeekStart
                    ) AS w
                    WHERE p.PatientId = @PatientId AND p.DatumTijdRegistratie <= SYSDATETIME()
                    GROUP BY w.WeekStart
                    ORDER BY w.WeekStart;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PijnverloopPerWeek;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DashboardSamenvatting;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_OefeningAfvinken;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_AfspraakOverzicht;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_OefenplanVoortgang;");
        }
    }
}
