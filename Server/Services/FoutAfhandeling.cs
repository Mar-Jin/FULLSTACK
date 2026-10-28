using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace RevaliNow.Services;

// Centrale foutafhandeling: vertaalt fouten naar een nette HTTP-response (ProblemDetails) in plaats van een crash.
// Onverwachte fouten worden gelogd, maar de details gaan niet naar de client.
public class FoutAfhandeling(ILogger<FoutAfhandeling> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, melding) = exception switch
        {
            NietGevondenException e => (StatusCodes.Status404NotFound, e.Message),
            OngeldigeActieException e => (StatusCodes.Status400BadRequest, e.Message),
            // 547 = een CHECK- of FOREIGN KEY-constraint in de database weigert de wijziging
            DbUpdateException { InnerException: SqlException { Number: 547 } } =>
                (StatusCodes.Status400BadRequest, "De gegevens voldoen niet aan de regels van de database."),
            _ => (StatusCodes.Status500InternalServerError, "Er ging iets mis op de server.")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Onverwachte fout bij {Pad}", httpContext.Request.Path);

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = melding,
            Detail = melding
        }, cancellationToken);

        return true;
    }
}
