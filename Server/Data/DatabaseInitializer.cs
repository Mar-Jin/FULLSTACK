using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace RevaliNow.Data;

// Zet de database klaar bij het opstarten. Dit draait met de beheer-verbinding (db_owner), omdat migraties,
// seeden en het aanmaken van een login rechten nodig hebben die de API zelf bewust niet heeft.
public static class DatabaseInitializer
{
    public static void Initialiseer(string beheerConnectionString, string apiConnectionString, string patientWachtwoord)
    {
        var options = new DbContextOptionsBuilder<RevaliNowDbContext>()
            .UseSqlServer(beheerConnectionString, sql => sql.EnableRetryOnFailure())
            .Options;

        using var context = new RevaliNowDbContext(options);
        context.Database.Migrate();
        DbSeeder.Seed(context, patientWachtwoord);
        MaakApiLogin(context, apiConnectionString);
    }

    // Maakt (of werkt bij) de SQL-login waarmee de API verbindt, en maakt hem lid van rol_api.
    // Login en wachtwoord komen uit de connection string van de API, zodat ze maar op één plek staan.
    private static void MaakApiLogin(RevaliNowDbContext context, string apiConnectionString)
    {
        var api = new SqlConnectionStringBuilder(apiConnectionString);
        if (string.IsNullOrEmpty(api.UserID) || string.IsNullOrEmpty(api.Password))
            throw new InvalidOperationException("De API-connection string moet een User en Password bevatten.");

        // QUOTENAME zet de naam en het wachtwoord veilig tussen haakjes/aanhalingstekens (geen SQL-injectie)
        // CREATE LOGIN accepteert geen parameters, daarom wordt de opdracht als tekst opgebouwd
        context.Database.ExecuteSqlRaw("""
            DECLARE @sql nvarchar(max);

            IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = @login)
                SET @sql = N'CREATE LOGIN ' + QUOTENAME(@login) + N' WITH PASSWORD = ' + QUOTENAME(@wachtwoord, '''')
                         + N', DEFAULT_DATABASE = ' + QUOTENAME(DB_NAME());
            ELSE
                SET @sql = N'ALTER LOGIN ' + QUOTENAME(@login) + N' WITH PASSWORD = ' + QUOTENAME(@wachtwoord, '''');
            EXEC sp_executesql @sql;

            IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @login)
            BEGIN
                SET @sql = N'CREATE USER ' + QUOTENAME(@login) + N' FOR LOGIN ' + QUOTENAME(@login);
                EXEC sp_executesql @sql;
            END

            IF IS_ROLEMEMBER('rol_api', @login) = 0
            BEGIN
                SET @sql = N'ALTER ROLE rol_api ADD MEMBER ' + QUOTENAME(@login);
                EXEC sp_executesql @sql;
            END
            """,
            new SqlParameter("@login", api.UserID),
            new SqlParameter("@wachtwoord", api.Password));
    }
}
