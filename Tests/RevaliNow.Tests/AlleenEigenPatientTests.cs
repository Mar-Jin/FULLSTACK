using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using RevaliNow.Authorization;
using RevaliNow.Services;

namespace RevaliNow.Tests;

// Het filter dat een cliënt alleen bij zijn eigen gegevens laat (user story 7)
public class AlleenEigenPatientTests
{
    private static AuthorizationFilterContext MaakContext(string routePatientId, string? tokenPatientId)
    {
        var claims = tokenPatientId == null ? [] : new[] { new Claim(TokenService.PatientIdClaim, tokenPatientId) };
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test")) };
        var routeData = new RouteData();
        routeData.Values["patientId"] = routePatientId;

        return new AuthorizationFilterContext(
            new ActionContext(httpContext, routeData, new ActionDescriptor()),
            new List<IFilterMetadata>());
    }

    [Fact]
    public void EigenPatientId_WordtDoorgelaten()
    {
        var context = MaakContext(routePatientId: "7", tokenPatientId: "7");

        new AlleenEigenPatientAttribute().OnAuthorization(context);

        Assert.Null(context.Result); // geen resultaat = het verzoek gaat door naar de controller
    }

    [Fact]
    public void PatientIdVanEenAnder_Geeft403()
    {
        var context = MaakContext(routePatientId: "8", tokenPatientId: "7");

        new AlleenEigenPatientAttribute().OnAuthorization(context);

        var resultaat = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status403Forbidden, resultaat.StatusCode);
    }

    [Fact]
    public void TokenZonderPatientId_Geeft403()
    {
        // Bijvoorbeeld een medewerker: die heeft geen patientId-claim
        var context = MaakContext(routePatientId: "7", tokenPatientId: null);

        new AlleenEigenPatientAttribute().OnAuthorization(context);

        var resultaat = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status403Forbidden, resultaat.StatusCode);
    }
}
