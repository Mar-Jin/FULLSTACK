using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using RevaliNow.Models;

namespace RevaliNow.Data;

// Vult een lege database met de dataset van de opdracht (Data/Seed/*.csv, 1-op-1 overgenomen uit Eindopdracht-data-v3.xlsx).
// De ID's uit de dataset blijven behouden, zodat alle verwijzingen tussen de tabellen kloppen.
public static class DbSeeder
{
    private static readonly string SeedMap = Path.Combine(AppContext.BaseDirectory, "Data", "Seed");

    public static void Seed(RevaliNowDbContext context, string patientWachtwoord)
    {
        if (context.Gebruikers.Any())
            return;

        // Alles in één transactie: of de hele dataset staat erin, of niets
        var strategy = context.Database.CreateExecutionStrategy();
        strategy.Execute(() =>
        {
            using var transactie = context.Database.BeginTransaction();
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            // Het inlezen van de dataset is geen gebruikersactie: de audit-triggers slaan dit over
            context.Database.ExecuteSqlRaw("EXEC sp_set_session_context @key = N'Seeding', @value = 1");

            VoegToe(context, Lees("Zorgverzekeraars.csv", r => new Zorgverzekeraar
            {
                Id = Int(r, "ZorgverzekeraarID"),
                Naam = Tekst(r, "Naam")
            }));

            VoegToe(context, Lees("Gebruikers.csv", r => new Gebruiker
            {
                Id = Int(r, "GebruikerID"),
                Gebruikersnaam = Tekst(r, "Gebruikersnaam"),
                WachtwoordHash = Tekst(r, "WachtwoordHash"),
                Rol = Tekst(r, "Rol"),
                Voornaam = Tekst(r, "Voornaam"),
                Achternaam = Tekst(r, "Achternaam"),
                OrganisatieId = IntOfNull(r, "OrganisatieID")
            }));

            var patienten = Lees("Patienten.csv", r => new Patient
            {
                Id = Int(r, "PatientID"),
                Voornaam = Tekst(r, "Voornaam"),
                Achternaam = Tekst(r, "Achternaam"),
                Geboortedatum = DateOnly.FromDateTime(Datum(r, "Geboortedatum")),
                Adres = Tekst(r, "Adres"),
                Telefoonnummer = Tekst(r, "Telefoonnummer"),
                Email = Tekst(r, "Email"),
                HuisartsId = Int(r, "HuisartsID"),
                ZorgverzekeraarId = Int(r, "ZorgverzekeraarID")
            }).ToList();
            VoegToe(context, patienten);

            VoegPatientAccountsToe(context, patienten, patientWachtwoord);

            VoegToe(context, Lees("Revalidatieartsen.csv", r => new PatientRevalidatiearts
            {
                Id = Int(r, "PatientRevalidatieartsID"),
                PatientId = Int(r, "PatientID"),
                RevalidatieartsId = Int(r, "RevalidatieartsID")
            }));

            VoegToe(context, Lees("Intakegesprekken.csv", r => new Intakegesprek
            {
                Id = Int(r, "IntakeID"),
                PatientId = Int(r, "PatientID"),
                RevalidatieartsId = Int(r, "RevalidatieartsID"),
                Diagnose = Tekst(r, "Diagnose"),
                ErnstBlessure = Tekst(r, "ErnstBlessure"),
                Behandeldoelen = Tekst(r, "Behandeldoelen"),
                DatumIntake = Datum(r, "DatumIntake")
            }));

            VoegToe(context, Lees("Afspraken.csv", r => new Afspraak
            {
                Id = Int(r, "AfspraakID"),
                PatientId = Int(r, "PatientID"),
                RevalidatieartsId = Int(r, "RevalidatieartsID"),
                DatumTijdAfspraak = Datum(r, "DatumTijdAfspraak"),
                DuurMinuten = Int(r, "DuurMinuten"),
                TypeAfspraak = Tekst(r, "TypeAfspraak"),
                Status = Tekst(r, "Status")
            }));

            VoegToe(context, Lees("Oefeningen.csv", r => new Oefening
            {
                Id = Int(r, "OefeningID"),
                Naam = Tekst(r, "Naam"),
                Beschrijving = Tekst(r, "Beschrijving"),
                InstructieVideoUrl = TekstOfNull(r, "InstructieVideoURL")
            }));

            VoegToe(context, Lees("Oefenplannen.csv", r => new Oefenplan
            {
                Id = Int(r, "PatientOefenplanID"),
                PatientId = Int(r, "PatientID"),
                OefeningId = Int(r, "OefeningID"),
                RevalidatieartsId = Int(r, "RevalidatieartsID"),
                StartDatum = Datum(r, "StartDatum"),
                EindDatum = Datum(r, "EindDatum"),
                Herhalingen = Int(r, "Herhalingen"),
                Sets = Int(r, "Sets"),
                FrequentiePerDag = Int(r, "FrequentiePerDag"),
                Opmerkingen = TekstOfNull(r, "Opmerkingen")
            }));

            VoegToe(context, Lees("Uitgevoerde-oefeningen.csv", r => new UitgevoerdeOefening
            {
                Id = Int(r, "UitgevoerdeOefeningID"),
                OefenplanId = Int(r, "PatientOefenplanID"),
                DatumTijdAfgevinkt = Datum(r, "DatumTijdAfgevinkt"),
                IsAfgevinkt = Int(r, "IsAfgevinkt") == 1
            }));

            VoegToe(context, Lees("Pijnindicaties.csv", r => new PijnIndicatie
            {
                Id = Int(r, "PijnIndicatieID"),
                PatientId = Int(r, "PatientID"),
                DatumTijdRegistratie = Datum(r, "DatumTijdRegistratie"),
                PijnScore = Int(r, "PijnScore"),
                Toelichting = TekstOfNull(r, "Toelichting")
            }));

            VoegToe(context, Lees("Activiteiten-logboek.csv", r => new Logboek
            {
                Id = Int(r, "LogboekID"),
                PatientId = Int(r, "PatientID"),
                DatumTijdActiviteit = Datum(r, "DatumTijdActiviteit"),
                Beschrijving = Tekst(r, "Beschrijving")
            }));

            VoegToe(context, Lees("Notities.csv", r => new Notitie
            {
                Id = Int(r, "NotitieID"),
                PatientId = Int(r, "PatientID"),
                AuteurGebruikerId = Int(r, "AuteurGebruikerID"),
                DatumTijdNotitie = Datum(r, "DatumTijdNotitie"),
                Inhoud = Tekst(r, "Inhoud")
            }));

            VoegToe(context, Lees("Medicatie.csv", r => new Medicatie
            {
                Id = Int(r, "MedicatieID"),
                PatientId = Int(r, "PatientID"),
                HuisartsId = Int(r, "HuisartsID"),
                Naam = Tekst(r, "Naam"),
                Dosering = Tekst(r, "Dosering"),
                Frequentie = Tekst(r, "Frequentie"),
                StartDatum = Datum(r, "StartDatum"),
                EindDatum = Datum(r, "EindDatum"),
                Status = Tekst(r, "Status")
            }));

            VoegToe(context, Lees("Accessoires.csv", r => new Accessoire
            {
                Id = Int(r, "AccessoireID"),
                PatientId = Int(r, "PatientID"),
                HuisartsId = Int(r, "HuisartsID"),
                Naam = Tekst(r, "Naam"),
                AdviesDatum = Datum(r, "AdviesDatum"),
                VerwachteGebruiksperiode = Tekst(r, "VerwachteGebruiksperiode"),
                Status = Tekst(r, "Status")
            }));

            VoegToe(context, Lees("Vergoedingsregels.csv", r => new Vergoedingsregel
            {
                Id = Int(r, "RegelID"),
                Naam = Tekst(r, "Naam"),
                MaxVergoedBedrag = Int(r, "MaxVergoedBedrag"),
                MaxVergoedPercentage = Int(r, "MaxVergoedPercentage"),
                Voorwaarden = Tekst(r, "Voorwaarden"),
                GeldigVanaf = Datum(r, "GeldigVanaf"),
                GeldigTot = DatumOfNull(r, "GeldigTot"),
                Actief = Int(r, "Actief") == 1
            }));

            VoegToe(context, Lees("Declaraties.csv", r => new Declaratie
            {
                Id = Int(r, "DeclaratieID"),
                PatientId = Int(r, "PatientID"),
                TypeDeclaratie = Tekst(r, "TypeDeclaratie"),
                GedeclareerdDoorGebruikerId = Int(r, "GedeclareerdDoorGebruikerID"),
                DatumHandeling = Datum(r, "DatumHandeling"),
                VergoedingsregelId = Int(r, "RegelID"),
                Omschrijving = Tekst(r, "Omschrijving"),
                GedeclareerdBedrag = Int(r, "GedeclareerdBedrag"),
                Status = Tekst(r, "Status")
            }));

            VoegToe(context, Lees("Declaratieverwerkingen.csv", r => new Declaratieverwerking
            {
                Id = Int(r, "VerwerkingID"),
                DeclaratieId = Int(r, "DeclaratieID"),
                ZorgverzekeraarMedewerkerId = Int(r, "ZorgverzekeraarMedewerkerID"),
                DatumVerwerking = Datum(r, "DatumVerwerking"),
                StatusVerwerking = Tekst(r, "StatusVerwerking"),
                VergoedBedrag = Int(r, "VergoedBedrag"),
                AfgekeurdBedrag = Int(r, "AfgekeurdBedrag"),
                Toelichting = TekstOfNull(r, "Toelichting")
            }));

            transactie.Commit();
            context.ChangeTracker.AutoDetectChangesEnabled = true;
        });
    }

    // De dataset bevat alleen accounts van medewerkers. Elke patiënt krijgt hier een eigen account
    // (gebruikersnaam = e-mailadres) met een testwachtwoord uit de configuratie.
    private static void VoegPatientAccountsToe(RevaliNowDbContext context, List<Patient> patienten, string wachtwoord)
    {
        // Eén keer hashen: bcrypt is bewust traag, en het is voor alle testaccounts hetzelfde wachtwoord
        var hash = BCrypt.Net.BCrypt.HashPassword(wachtwoord);

        context.Gebruikers.AddRange(patienten.Select(p => new Gebruiker
        {
            Gebruikersnaam = p.Email,
            WachtwoordHash = hash,
            Rol = Rollen.Patient,
            Voornaam = p.Voornaam,
            Achternaam = p.Achternaam,
            PatientId = p.Id
        }));
        context.SaveChanges();
        context.ChangeTracker.Clear();
    }

    // Schrijft rijen weg met hun eigen ID uit de dataset (daarvoor moet IDENTITY_INSERT tijdelijk aan)
    private static void VoegToe<T>(RevaliNowDbContext context, IEnumerable<T> rijen) where T : class
    {
        var tabel = context.Model.FindEntityType(typeof(T))!.GetTableName();

        context.Set<T>().AddRange(rijen);
        context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [" + tabel + "] ON");
        context.SaveChanges();
        context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [" + tabel + "] OFF");
        context.ChangeTracker.Clear();
    }

    private static IEnumerable<T> Lees<T>(string bestand, Func<CsvReader, T> maakRij)
    {
        using var reader = new StreamReader(Path.Combine(SeedMap, bestand));
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
            yield return maakRij(csv);
    }

    private static string Tekst(CsvReader r, string kolom) => r.GetField(kolom)!;

    private static string? TekstOfNull(CsvReader r, string kolom) =>
        string.IsNullOrEmpty(r.GetField(kolom)) ? null : r.GetField(kolom);

    private static int Int(CsvReader r, string kolom) => int.Parse(r.GetField(kolom)!, CultureInfo.InvariantCulture);

    private static int? IntOfNull(CsvReader r, string kolom) =>
        string.IsNullOrEmpty(r.GetField(kolom)) ? null : Int(r, kolom);

    private static DateTime Datum(CsvReader r, string kolom) =>
        DateTime.ParseExact(r.GetField(kolom)!, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

    private static DateTime? DatumOfNull(CsvReader r, string kolom) =>
        string.IsNullOrEmpty(r.GetField(kolom)) ? null : Datum(r, kolom);
}
