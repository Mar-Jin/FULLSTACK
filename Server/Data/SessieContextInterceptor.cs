using System.Data.Common;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RevaliNow.Services;

namespace RevaliNow.Data;

// Geeft bij elke databaseverbinding door wie er is ingelogd (SESSION_CONTEXT in SQL Server).
// De audit-triggers lezen dit uit om vast te leggen welke gebruiker een wijziging deed.
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
        command.CommandText = """
            EXEC sp_set_session_context @key = N'GebruikerId', @value = @gebruikerId;
            EXEC sp_set_session_context @key = N'PatientId', @value = @patientId;
            EXEC sp_set_session_context @key = N'Rol', @value = @rol;
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
