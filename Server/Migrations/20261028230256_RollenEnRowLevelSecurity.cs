using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RevaliNow.Migrations
{
    /// <summary>
    /// Databaserollen (least privilege) en row-level security (RLS).
    ///
    /// De API logt in met een eigen login (lid van rol_api) en zet bij elke verbinding wie er is ingelogd in
    /// SESSION_CONTEXT (zie SessieContextInterceptor). De security policy hieronder filtert op basis daarvan elke
    /// query: ook als er in de API een fout zou zitten, geeft de database geen rijen van een andere cliënt terug.
    /// Beheerders (db_owner: migraties, seeden, SSMS) en de auditor (alleen lezen) worden niet gefilterd.
    /// </summary>
    public partial class RollenEnRowLevelSecurity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---------- Rollen ----------

            // rol_api: precies wat RevaliNow nodig heeft, niet meer
            migrationBuilder.Sql("CREATE ROLE rol_api;");
            migrationBuilder.Sql("""
                GRANT SELECT ON dbo.Gebruikers TO rol_api;             -- inloggen, naam van de arts
                GRANT SELECT ON dbo.Patienten TO rol_api;
                GRANT SELECT ON dbo.Oefeningen TO rol_api;
                GRANT SELECT ON dbo.Oefenplannen TO rol_api;
                GRANT SELECT ON dbo.Afspraken TO rol_api;
                GRANT SELECT, INSERT ON dbo.PijnIndicaties TO rol_api;
                GRANT SELECT, INSERT ON dbo.UitgevoerdeOefeningen TO rol_api;
                GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.Logboek TO rol_api;
                GRANT SELECT ON dbo.vw_OefenplanVoortgang TO rol_api;
                GRANT SELECT ON dbo.vw_AfspraakOverzicht TO rol_api;
                GRANT EXECUTE ON dbo.sp_OefeningAfvinken TO rol_api;
                GRANT EXECUTE ON dbo.sp_DashboardSamenvatting TO rol_api;
                GRANT EXECUTE ON dbo.sp_PijnverloopPerWeek TO rol_api;
                """);
            // Geen rechten op AuditLogs: de audit-triggers schrijven daar wel in (ownership chaining: trigger en
            // tabel hebben dezelfde eigenaar), maar de API zelf kan de audit trail niet lezen, wijzigen of wissen.

            // rol_auditor: mag de audit trail en de historie inzien, maar niets wijzigen
            migrationBuilder.Sql("CREATE ROLE rol_auditor;");
            migrationBuilder.Sql("""
                GRANT SELECT ON dbo.AuditLogs TO rol_auditor;
                GRANT SELECT ON dbo.PijnIndicatiesHistorie TO rol_auditor;
                GRANT SELECT ON dbo.LogboekHistorie TO rol_auditor;
                GRANT SELECT ON dbo.PijnIndicaties TO rol_auditor;
                GRANT SELECT ON dbo.Logboek TO rol_auditor;
                """);

            // ---------- Row-level security ----------

            // Eigen schema, zodat de beveiligingsfuncties los staan van de data
            migrationBuilder.Sql("CREATE SCHEMA Beveiliging;");

            // Mag de huidige gebruiker de gegevens van deze patiënt zien? (voor alle tabellen met een PatientId)
            migrationBuilder.Sql("""
                CREATE FUNCTION Beveiliging.fn_Toegang(@PatientId int)
                RETURNS TABLE
                WITH SCHEMABINDING
                AS
                RETURN
                    SELECT 1 AS Toegang
                    WHERE IS_MEMBER('db_owner') = 1
                       OR IS_MEMBER('rol_auditor') = 1
                       -- Patiënt: alleen zijn eigen gegevens
                       OR (SESSION_CONTEXT(N'Rol') = N'Patient'
                           AND CAST(SESSION_CONTEXT(N'PatientId') AS int) = @PatientId)
                       -- Huisarts: alleen zijn eigen patiënten (RLS geldt niet binnen deze functie, dus de
                       -- huisarts wordt hier expliciet gecontroleerd)
                       OR (SESSION_CONTEXT(N'Rol') = N'Huisarts'
                           AND EXISTS (SELECT 1 FROM dbo.Patienten p
                                       WHERE p.Id = @PatientId
                                         AND p.HuisartsId = CAST(SESSION_CONTEXT(N'GebruikerId') AS int)))
                       -- Revalidatiearts: alleen patiënten die aan hem gekoppeld zijn
                       OR (SESSION_CONTEXT(N'Rol') = N'Revalidatiearts'
                           AND EXISTS (SELECT 1 FROM dbo.PatientRevalidatieartsen pr
                                       WHERE pr.PatientId = @PatientId
                                         AND pr.RevalidatieartsId = CAST(SESSION_CONTEXT(N'GebruikerId') AS int)));
                """);

            // Voor de tabel Patienten zelf: de huisarts staat in de rij, dus geen opzoeking in Patienten nodig
            migrationBuilder.Sql("""
                CREATE FUNCTION Beveiliging.fn_PatientToegang(@PatientId int, @HuisartsId int)
                RETURNS TABLE
                WITH SCHEMABINDING
                AS
                RETURN
                    SELECT 1 AS Toegang
                    WHERE IS_MEMBER('db_owner') = 1
                       OR IS_MEMBER('rol_auditor') = 1
                       OR (SESSION_CONTEXT(N'Rol') = N'Patient'
                           AND CAST(SESSION_CONTEXT(N'PatientId') AS int) = @PatientId)
                       OR (SESSION_CONTEXT(N'Rol') = N'Huisarts'
                           AND CAST(SESSION_CONTEXT(N'GebruikerId') AS int) = @HuisartsId)
                       OR (SESSION_CONTEXT(N'Rol') = N'Revalidatiearts'
                           AND EXISTS (SELECT 1 FROM dbo.PatientRevalidatieartsen pr
                                       WHERE pr.PatientId = @PatientId
                                         AND pr.RevalidatieartsId = CAST(SESSION_CONTEXT(N'GebruikerId') AS int)));
                """);

            // Voor de koppeltabel: de arts staat in de rij. Bewust zonder verwijzing naar Patienten, anders zouden
            // de filters van Patienten en deze tabel naar elkaar verwijzen (oneindige lus).
            migrationBuilder.Sql("""
                CREATE FUNCTION Beveiliging.fn_KoppelingToegang(@PatientId int, @RevalidatieartsId int)
                RETURNS TABLE
                WITH SCHEMABINDING
                AS
                RETURN
                    SELECT 1 AS Toegang
                    WHERE IS_MEMBER('db_owner') = 1
                       OR IS_MEMBER('rol_auditor') = 1
                       OR (SESSION_CONTEXT(N'Rol') = N'Patient'
                           AND CAST(SESSION_CONTEXT(N'PatientId') AS int) = @PatientId)
                       OR (SESSION_CONTEXT(N'Rol') = N'Revalidatiearts'
                           AND CAST(SESSION_CONTEXT(N'GebruikerId') AS int) = @RevalidatieartsId);
                """);

            // Uitgevoerde oefeningen hebben geen PatientId: toegang als je bij de patiënt van het oefenplan mag.
            // Let op: SQL Server past RLS níet toe op tabellen die je binnen een predicaatfunctie opvraagt, dus de
            // controle moet hier expliciet via fn_Toegang (alleen 'het oefenplan bestaat' zou alles doorlaten).
            migrationBuilder.Sql("""
                CREATE FUNCTION Beveiliging.fn_OefenplanToegang(@OefenplanId int)
                RETURNS TABLE
                WITH SCHEMABINDING
                AS
                RETURN
                    SELECT 1 AS Toegang
                    FROM dbo.Oefenplannen o
                    CROSS APPLY Beveiliging.fn_Toegang(o.PatientId) t
                    WHERE o.Id = @OefenplanId;
                """);

            // FILTER = welke rijen je ziet (SELECT, UPDATE, DELETE).
            // BLOCK AFTER INSERT/UPDATE = je kunt geen rij toevoegen of zo wijzigen dat hij bij een ander hoort.
            migrationBuilder.Sql("""
                CREATE SECURITY POLICY Beveiliging.PatientgegevensBeleid
                    ADD FILTER PREDICATE Beveiliging.fn_PatientToegang(Id, HuisartsId) ON dbo.Patienten,
                    ADD FILTER PREDICATE Beveiliging.fn_KoppelingToegang(PatientId, RevalidatieartsId) ON dbo.PatientRevalidatieartsen,
                    ADD FILTER PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.Oefenplannen,
                    ADD FILTER PREDICATE Beveiliging.fn_OefenplanToegang(OefenplanId) ON dbo.UitgevoerdeOefeningen,
                    ADD BLOCK PREDICATE Beveiliging.fn_OefenplanToegang(OefenplanId) ON dbo.UitgevoerdeOefeningen AFTER INSERT,
                    ADD FILTER PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.PijnIndicaties,
                    ADD BLOCK PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.PijnIndicaties AFTER INSERT,
                    ADD BLOCK PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.PijnIndicaties AFTER UPDATE,
                    ADD FILTER PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.PijnIndicatiesHistorie,
                    ADD FILTER PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.Logboek,
                    ADD BLOCK PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.Logboek AFTER INSERT,
                    ADD BLOCK PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.Logboek AFTER UPDATE,
                    ADD FILTER PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.LogboekHistorie,
                    ADD FILTER PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.Afspraken,
                    ADD FILTER PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.Intakegesprekken,
                    ADD FILTER PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.Notities,
                    ADD FILTER PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.Medicaties,
                    ADD FILTER PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.Accessoires,
                    ADD FILTER PREDICATE Beveiliging.fn_Toegang(PatientId) ON dbo.Declaraties
                WITH (STATE = ON);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP SECURITY POLICY IF EXISTS Beveiliging.PatientgegevensBeleid;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS Beveiliging.fn_OefenplanToegang;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS Beveiliging.fn_KoppelingToegang;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS Beveiliging.fn_PatientToegang;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS Beveiliging.fn_Toegang;");
            migrationBuilder.Sql("DROP SCHEMA IF EXISTS Beveiliging;");
            migrationBuilder.Sql("DROP ROLE IF EXISTS rol_auditor;");
            migrationBuilder.Sql("DROP ROLE IF EXISTS rol_api;");
        }
    }
}
