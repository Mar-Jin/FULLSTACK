using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using RevaliNow.Services;

namespace RevaliNow.Tests;

public class FoutAfhandelingTests
{
    private static async Task<(int Status, string Titel)> Afhandelen(Exception fout)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        await new FoutAfhandeling(NullLogger<FoutAfhandeling>.Instance).TryHandleAsync(httpContext, fout, CancellationToken.None);

        httpContext.Response.Body.Position = 0;
        var json = await JsonDocument.ParseAsync(httpContext.Response.Body);
        return (httpContext.Response.StatusCode, json.RootElement.GetProperty("title").GetString()!);
    }

    [Fact]
    public async Task NietGevonden_Geeft404MetMelding()
    {
        var (status, titel) = await Afhandelen(new NietGevondenException("Logboekregel niet gevonden."));

        Assert.Equal(StatusCodes.Status404NotFound, status);
        Assert.Equal("Logboekregel niet gevonden.", titel);
    }

    [Fact]
    public async Task OngeldigeActie_Geeft400MetMelding()
    {
        var (status, titel) = await Afhandelen(new OngeldigeActieException("Beschrijf de activiteit."));

        Assert.Equal(StatusCodes.Status400BadRequest, status);
        Assert.Equal("Beschrijf de activiteit.", titel);
    }

    [Fact]
    public async Task OnverwachteFout_Geeft500_ZonderTechnischeDetails()
    {
        var (status, titel) = await Afhandelen(new InvalidOperationException("Geheime stacktrace-informatie"));

        Assert.Equal(StatusCodes.Status500InternalServerError, status);
        Assert.Equal("Er ging iets mis op de server.", titel);
        Assert.DoesNotContain("Geheime", titel);
    }
}
