using RevaliNow.Dtos;
using RevaliNow.Models;
using RevaliNow.Services;

namespace RevaliNow.Tests;

public class LogboekServiceTests
{
    [Fact]
    public async Task Toevoegen_ZetPatientEnTijdstipVanDeServer()
    {
        // Arrange
        using var db = TestHulp.NieuweDatabase();
        var service = new LogboekService(db, TestHulp.Tijd());

        // Act
        var resultaat = await service.ToevoegenAsync(7, new LogboekInvoerDto { Beschrijving = "  Boodschappen gedaan  " });

        // Assert
        var opgeslagen = db.Logboek.Single();
        Assert.Equal(7, opgeslagen.PatientId);
        Assert.Equal(TestHulp.Nu, opgeslagen.DatumTijdActiviteit);
        Assert.Equal("Boodschappen gedaan", resultaat.Beschrijving); // spaties eromheen weggehaald
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Toevoegen_LegeBeschrijving_WordtGeweigerd(string beschrijving)
    {
        using var db = TestHulp.NieuweDatabase();
        var service = new LogboekService(db, TestHulp.Tijd());

        await Assert.ThrowsAsync<OngeldigeActieException>(
            () => service.ToevoegenAsync(7, new LogboekInvoerDto { Beschrijving = beschrijving }));
        Assert.Empty(db.Logboek);
    }

    [Fact]
    public async Task GetLogboek_GeeftAlleenEigenRegels_NieuwsteEerst()
    {
        using var db = TestHulp.NieuweDatabase();
        db.Logboek.AddRange(
            new Logboek { PatientId = 7, DatumTijdActiviteit = TestHulp.Nu.AddDays(-2), Beschrijving = "oud" },
            new Logboek { PatientId = 7, DatumTijdActiviteit = TestHulp.Nu.AddDays(-1), Beschrijving = "nieuw" },
            new Logboek { PatientId = 8, DatumTijdActiviteit = TestHulp.Nu, Beschrijving = "van een ander" });
        await db.SaveChangesAsync();
        var service = new LogboekService(db, TestHulp.Tijd());

        var resultaat = await service.GetLogboekAsync(7);

        Assert.Equal(["nieuw", "oud"], resultaat.Select(l => l.Beschrijving));
    }

    [Fact]
    public async Task Bewerken_WijzigtAlleenDeBeschrijving()
    {
        using var db = TestHulp.NieuweDatabase();
        var origineel = new Logboek { PatientId = 7, DatumTijdActiviteit = TestHulp.Nu.AddDays(-3), Beschrijving = "wandeling" };
        db.Logboek.Add(origineel);
        await db.SaveChangesAsync();
        var service = new LogboekService(db, TestHulp.Tijd());

        await service.BewerkenAsync(7, origineel.Id, new LogboekInvoerDto { Beschrijving = "wandeling van 30 minuten" });

        var opgeslagen = db.Logboek.Single();
        Assert.Equal("wandeling van 30 minuten", opgeslagen.Beschrijving);
        Assert.Equal(TestHulp.Nu.AddDays(-3), opgeslagen.DatumTijdActiviteit); // tijdstip blijft staan
    }

    [Fact]
    public async Task Bewerken_RegelVanEenAnderePatient_GeeftNietGevonden()
    {
        using var db = TestHulp.NieuweDatabase();
        var vanAnder = new Logboek { PatientId = 8, DatumTijdActiviteit = TestHulp.Nu, Beschrijving = "van patiënt 8" };
        db.Logboek.Add(vanAnder);
        await db.SaveChangesAsync();
        var service = new LogboekService(db, TestHulp.Tijd());

        await Assert.ThrowsAsync<NietGevondenException>(
            () => service.BewerkenAsync(7, vanAnder.Id, new LogboekInvoerDto { Beschrijving = "gehackt" }));
        Assert.Equal("van patiënt 8", db.Logboek.Single().Beschrijving);
    }

    [Fact]
    public async Task Verwijderen_RegelVanEenAnderePatient_GeeftNietGevondenEnBlijftBestaan()
    {
        using var db = TestHulp.NieuweDatabase();
        var vanAnder = new Logboek { PatientId = 8, DatumTijdActiviteit = TestHulp.Nu, Beschrijving = "van patiënt 8" };
        db.Logboek.Add(vanAnder);
        await db.SaveChangesAsync();
        var service = new LogboekService(db, TestHulp.Tijd());

        await Assert.ThrowsAsync<NietGevondenException>(() => service.VerwijderenAsync(7, vanAnder.Id));
        Assert.Single(db.Logboek);
    }

    [Fact]
    public async Task Verwijderen_EigenRegel_IsWeg()
    {
        using var db = TestHulp.NieuweDatabase();
        var eigen = new Logboek { PatientId = 7, DatumTijdActiviteit = TestHulp.Nu, Beschrijving = "eigen" };
        db.Logboek.Add(eigen);
        await db.SaveChangesAsync();
        var service = new LogboekService(db, TestHulp.Tijd());

        await service.VerwijderenAsync(7, eigen.Id);

        Assert.Empty(db.Logboek);
    }
}
