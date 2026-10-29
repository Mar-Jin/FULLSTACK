using System.ComponentModel.DataAnnotations;
using RevaliNow.Dtos;
using RevaliNow.Models;
using RevaliNow.Services;

namespace RevaliNow.Tests;

public class PijnIndicatieServiceTests
{
    [Fact]
    public async Task Registreer_GebruiktPatientUitDeRouteEnTijdVanDeServer()
    {
        using var db = TestHulp.NieuweDatabase();
        var service = new PijnIndicatieService(db, TestHulp.Tijd());

        var resultaat = await service.RegistreerAsync(7, new PijnIndicatieInvoerDto { PijnScore = 4, Toelichting = "na traplopen" });

        var opgeslagen = db.PijnIndicaties.Single();
        Assert.Equal(7, opgeslagen.PatientId);
        Assert.Equal(TestHulp.Nu, opgeslagen.DatumTijdRegistratie);
        Assert.Equal(4, resultaat.PijnScore);
    }

    [Fact]
    public async Task Registreer_LegeToelichting_WordtNull()
    {
        using var db = TestHulp.NieuweDatabase();
        var service = new PijnIndicatieService(db, TestHulp.Tijd());

        await service.RegistreerAsync(7, new PijnIndicatieInvoerDto { PijnScore = 2, Toelichting = "   " });

        Assert.Null(db.PijnIndicaties.Single().Toelichting);
    }

    [Fact]
    public async Task GetPijnIndicaties_AlleenEigen_ZonderToekomst_NieuwsteEerst()
    {
        using var db = TestHulp.NieuweDatabase();
        db.PijnIndicaties.AddRange(
            new PijnIndicatie { PatientId = 7, DatumTijdRegistratie = TestHulp.Nu.AddDays(-5), PijnScore = 6 },
            new PijnIndicatie { PatientId = 7, DatumTijdRegistratie = TestHulp.Nu.AddDays(-1), PijnScore = 3 },
            new PijnIndicatie { PatientId = 7, DatumTijdRegistratie = TestHulp.Nu.AddDays(10), PijnScore = 9 }, // toekomst (dataset)
            new PijnIndicatie { PatientId = 8, DatumTijdRegistratie = TestHulp.Nu.AddDays(-1), PijnScore = 1 }); // ander
        await db.SaveChangesAsync();
        var service = new PijnIndicatieService(db, TestHulp.Tijd());

        var resultaat = await service.GetPijnIndicatiesAsync(7);

        Assert.Equal([3, 6], resultaat.Select(p => p.PijnScore));
    }

    // Validatie van de invoer gebeurt door [ApiController] op basis van de attributen op de DTO
    [Theory]
    [InlineData(0, true)]
    [InlineData(10, true)]
    [InlineData(-1, false)]
    [InlineData(11, false)]
    [InlineData(null, false)]
    public void Invoer_PijnScoreMoetTussen0En10Liggen(int? score, bool geldig)
    {
        var invoer = new PijnIndicatieInvoerDto { PijnScore = score };

        var isGeldig = Validator.TryValidateObject(invoer, new ValidationContext(invoer), null, validateAllProperties: true);

        Assert.Equal(geldig, isGeldig);
    }
}
