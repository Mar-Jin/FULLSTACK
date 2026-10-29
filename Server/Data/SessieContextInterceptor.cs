using System.Data.Common;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RevaliNow.Services;

namespace RevaliNow.Data;

// Geeft bij elke databaseverbinding door wie er is ingelogd (SESSION_CONTEXT in SQL Server).
// Row-level security gebruikt dit om rijen te filteren, en de audit-triggers om vast te leggen wie iets wijzigde.
// SQL Server wist de sessiecontext zelf zodra een verbinding terug in de pool gaat (sp_reset_connection),
// dus de gegevens van de ene gebruiker kunnen niet bij een volgend verzoek terechtkomen.
public class SessieContextInterceptor(IHttpContextAccessor httpContextAccessor) : DbConnectionInterceptor
{
    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        using var command = MaakCommand(connection);
        command?.ExecuteNonQuery();
    }

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        await using var command = MaakCommand(connection);
        if (command != null)
            await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private DbCommand? MaakCommand(DbConnection connection)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        var command = connection.CreateCommand();
        // @read_only = 1: binnen deze verbinding kan niemand de waarden daarna nog aanpassen
        // (ook niet via een SQL-injectie), dus row-level security kan er blind op vertrouwen.
        command.CommandText = """
            EXEC sp_set_session_context @key = N'GebruikerId', @value = @gebruikerId, @read_only = 1;
            EXEC sp_set_session_context @key = N'PatientId', @value = @patientId, @read_only = 1;
            EXEC sp_set_session_context @key = N'Rol', @value = @rol, @read_only = 1;
            """;
        VoegParameterToe(command, "@gebruikerId", ParseInt(user.FindFirstValue(ClaimTypes.NameIdentifier)));
        VoegParameterToe(command, "@patientId", ParseInt(user.FindFirstValue(TokenService.PatientIdClaim)));
        VoegParameterToe(command, "@rol", user.FindFirstValue(ClaimTypes.Role));
        return command;
    }

    private static void VoegParameterToe(DbCommand command, string naam, object? waarde)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = naam;
        parameter.Value = waarde ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }

    private static int? ParseInt(string? waarde) => int.TryParse(waarde, out var getal) ? getal : null;
}
