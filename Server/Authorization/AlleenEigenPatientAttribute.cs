using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RevaliNow.Services;

namespace RevaliNow.Authorization;

// Controleert of de ingelogde patiënt eigenaar is van de opgevraagde data: het {patientId} in de route
// moet gelijk zijn aan het PatientId in het JWT-token. Zo niet, dan volgt direct 403 Forbidden,
// nog vóórdat de controller (en dus de database) wordt aangeroepen.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AlleenEigenPatientAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var routeWaarde = context.RouteData.Values["patientId"]?.ToString();
        var tokenWaarde = context.HttpContext.User.FindFirst(TokenService.PatientIdClaim)?.Value;

        if (routeWaarde is null || tokenWaarde is null || routeWaarde != tokenWaarde)
        {
            context.Result = new ObjectResult(new { message = "Je hebt geen toegang tot deze gegevens." })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
