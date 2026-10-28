using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RevaliNow.Migrations
{
    /// <inheritdoc />
    public partial class AuditTrail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Actie = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Tabelnaam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecordId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    GebruikerId = table.Column<int>(type: "int", nullable: true),
                    DatabaseGebruiker = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DatumTijd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.CheckConstraint("CK_AuditLogs_Actie", "[Actie] IN ('INSERT', 'UPDATE', 'DELETE')");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PatientId",
                table: "AuditLogs",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Tabelnaam_RecordId",
                table: "AuditLogs",
                columns: new[] { "Tabelnaam", "RecordId" });

            // Audit-triggers (user story 8). Een trigger legt elke wijziging vast, ook als die buiten de API om
            // gebeurt. De ingelogde API-gebruiker komt uit SESSION_CONTEXT (gezet door SessieContextInterceptor).
            migrationBuilder.Sql(AuditTrigger("PijnIndicaties", "PatientId", "r.PatientId"));
            migrationBuilder.Sql(AuditTrigger("Logboek", "PatientId", "r.PatientId"));
            // Een uitgevoerde oefening heeft zelf geen PatientId: die zoeken we op via het oefenplan
            migrationBuilder.Sql(AuditTrigger("UitgevoerdeOefeningen", "OefenplanId",
                "(SELECT o.PatientId FROM Oefenplannen o WHERE o.Id = r.OefenplanId)"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_PijnIndicaties_Audit;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_Logboek_Audit;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_UitgevoerdeOefeningen_Audit;");

            migrationBuilder.DropTable(
                name: "AuditLogs");
        }

        // Eén trigger per tabel voor INSERT, UPDATE en DELETE. 'inserted' en 'deleted' bevatten de nieuwe
        // en oude versie van de rijen; bij een UPDATE zijn ze allebei gevuld.
        // koppelKolom: de kolom waarmee de patiënt te vinden is; patientIdExpressie: hoe je daaruit het PatientId haalt.
        private static string AuditTrigger(string tabel, string koppelKolom, string patientIdExpressie)
        {
            return $"""
                CREATE TRIGGER TR_{tabel}_Audit ON [{tabel}]
                AFTER INSERT, UPDATE, DELETE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Het inlezen van de dataset bij de eerste start niet loggen
                    IF SESSION_CONTEXT(N'Seeding') = 1 RETURN;

                    DECLARE @actie nvarchar(10) =
                        CASE
                            WHEN EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted) THEN 'UPDATE'
                            WHEN EXISTS (SELECT 1 FROM inserted) THEN 'INSERT'
                            ELSE 'DELETE'
                        END;

                    INSERT INTO AuditLogs (Actie, Tabelnaam, RecordId, PatientId, GebruikerId, DatabaseGebruiker, DatumTijd)
                    SELECT @actie, '{tabel}', r.Id, {patientIdExpressie},
                           CAST(SESSION_CONTEXT(N'GebruikerId') AS int), SUSER_SNAME(), SYSDATETIME()
                    FROM (
                        SELECT Id, {koppelKolom} FROM inserted
                        UNION ALL
                        SELECT Id, {koppelKolom} FROM deleted WHERE NOT EXISTS (SELECT 1 FROM inserted)
                    ) AS r;
                END
                """;
        }
    }
}
