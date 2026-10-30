using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RevaliNow.Migrations
{
    /// <summary>
    /// sp_DashboardSamenvatting geeft nu ook terug of de cliënt vandaag al pijn heeft doorgegeven en iets
    /// heeft gelogd, voor 'Vandaag te doen' op het dashboard. Nog steeds één aanroep.
    /// </summary>
    public partial class DashboardVandaag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(Procedure(metVandaag: true));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(Procedure(metVandaag: false));
        }

        private static string Procedure(bool metVandaag)
        {
            var vandaagKolommen = metVandaag
                ? """
                  ,
                           vandaag.Pijn                 AS PijnVandaag,
                           vandaag.Logboek              AS LogboekVandaag
                  """
                : "";
            // 'Vandaag' als datetime2-grenzen, zodat de indexes op (PatientId, datum) gebruikt worden
            var vandaagApply = metVandaag
                ? """
                  OUTER APPLY (
                        SELECT (SELECT COUNT(*) FROM PijnIndicaties
                                WHERE PatientId = @PatientId AND DatumTijdRegistratie >= @Vandaag AND DatumTijdRegistratie < @Morgen) AS Pijn,
                               (SELECT COUNT(*) FROM Logboek
                                WHERE PatientId = @PatientId AND DatumTijdActiviteit >= @Vandaag AND DatumTijdActiviteit < @Morgen) AS Logboek
                    ) AS vandaag
                  """
                : "";

            return $"""
                ALTER PROCEDURE sp_DashboardSamenvatting
                    @PatientId int
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DECLARE @Nu datetime2 = SYSDATETIME();
                    DECLARE @Vandaag datetime2 = CAST(CAST(@Nu AS date) AS datetime2);
                    DECLARE @Morgen datetime2 = DATEADD(DAY, 1, @Vandaag);

                    SELECT laatste.PijnScore            AS LaatstePijnScore,
                           laatste.DatumTijdRegistratie AS LaatstePijnDatum,
                           week.Gemiddelde              AS GemiddeldePijn7Dagen,
                           week.Aantal                  AS AantalMetingen7Dagen,
                           volgende.DatumTijdAfspraak   AS VolgendeAfspraakDatum,
                           volgende.DuurMinuten         AS VolgendeAfspraakDuur,
                           volgende.TypeAfspraak        AS VolgendeAfspraakType,
                           volgende.Status              AS VolgendeAfspraakStatus,
                           volgende.Revalidatiearts     AS VolgendeAfspraakArts{vandaagKolommen}
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
                    ) AS volgende
                    {vandaagApply};
                END
                """;
        }
    }
}
