using Microsoft.EntityFrameworkCore;
using RevaliNow.Models;

namespace RevaliNow.Data
{
    public class RevaliNowDbContext : DbContext
    {
        // Constructor
        public RevaliNowDbContext(DbContextOptions<RevaliNowDbContext> options)
            : base(options)
        {
        }

        // DbSets voor je entiteiten
        public DbSet<Zorgverzekeraar> Zorgverzekeraars { get; set; }
        public DbSet<Gebruiker> Gebruikers { get; set; }
        public DbSet<Patient> Patienten { get; set; }
        public DbSet<Afspraak> Afspraken { get; set; }
        public DbSet<Logboek> Logboek { get; set; }
        public DbSet<Oefening> Oefeningen { get; set; }
        public DbSet<Oefenplan> Oefenplannen { get; set; }
        public DbSet<PijnIndicatie> PijnIndicaties { get; set; }
        public DbSet<UitgevoerdeOefening> UitgevoerdeOefeningen { get; set; }
        public DbSet<Vergoedingsregel> Vergoedingsregels { get; set; }

        // Hash van "password123" (gemaakt met PasswordHasher), gebruikt voor alle testgebruikers
        private const string TestWachtwoordHash = "AQAAAAIAAYagAAAAEF39z5hGdv0L4PLYjHFU9rjcIJejcuqCzW3jhAIMaS5Fl7heXGkdSK76evpFMp+V6w==";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Zorgverzekeraar>().HasData(
                new Zorgverzekeraar { Id = 1, Naam = "ZorgDirect" },
                new Zorgverzekeraar { Id = 2, Naam = "GezondVooruit" }
            );

            modelBuilder.Entity<Gebruiker>().HasData(
                new Gebruiker { Id = 1, Gebruikersnaam = "zvm_jansen", WachtwoordHash = TestWachtwoordHash, Rol = "Zorgverzekeraar", Voornaam = "Sophie", Achternaam = "Jansen", OrganisatieId = 1 },
                new Gebruiker { Id = 2, Gebruikersnaam = "zvm_devries", WachtwoordHash = TestWachtwoordHash, Rol = "Zorgverzekeraar", Voornaam = "Mark", Achternaam = "de Vries", OrganisatieId = 2 },
                new Gebruiker { Id = 3, Gebruikersnaam = "ha_bakker", WachtwoordHash = TestWachtwoordHash, Rol = "Huisarts", Voornaam = "Lisa", Achternaam = "Bakker", OrganisatieId = null},
                new Gebruiker { Id = 4, Gebruikersnaam = "ha_janssen", WachtwoordHash = TestWachtwoordHash, Rol = "Huisarts", Voornaam = "Thomas", Achternaam = "Janssen", OrganisatieId = null },
                new Gebruiker { Id = 5, Gebruikersnaam = "ra_smit", WachtwoordHash = TestWachtwoordHash, Rol = "Revalidatiearts", Voornaam = "Emma", Achternaam = "Smit", OrganisatieId = null },
                new Gebruiker { Id = 6, Gebruikersnaam = "ra_groen", WachtwoordHash = TestWachtwoordHash, Rol = "Revalidatiearts", Voornaam = "David", Achternaam = "Groen", OrganisatieId = null },
                new Gebruiker { Id = 7, Gebruikersnaam = "ra_visser", WachtwoordHash = TestWachtwoordHash, Rol = "Revalidatiearts", Voornaam = "Linda", Achternaam = "Visser", OrganisatieId = null }
            );


            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = 1, Voornaam = "Freddy", Achternaam = "Voetballer", Geboortedatum = DateTime.Parse("1995-03-15"), Straatnaam = "Kruisbandstraat", Huisnummer = "10", Telefoonnummer = "612345678", Email = "freddy.v@example.com", HuisartsId = 3 },
                new Patient { Id = 2, Voornaam = "Anna", Achternaam = "Kamer", Geboortedatum = DateTime.Parse("1988-11-22"), Straatnaam = "Therapiestraat", Huisnummer = "5", Telefoonnummer = "623456789", Email = "anna.k@example.com", HuisartsId = 4 },
                new Patient { Id = 3, Voornaam = "Bas", Achternaam = "Verkade", Geboortedatum = DateTime.Parse("1976-07-01"), Straatnaam = "Oefenlaan", Huisnummer = "12", Telefoonnummer = "634567890", Email = "bas.v@example.com", HuisartsId = 3 },
                new Patient { Id = 4, Voornaam = "Carla", Achternaam = "Dirksen", Geboortedatum = DateTime.Parse("2001-01-30"), Straatnaam = "Herstelweg", Huisnummer = "3", Telefoonnummer = "645678901", Email = "carla.d@example.com", HuisartsId = 4 },
                new Patient { Id = 5, Voornaam = "Dirk", Achternaam = "Meijer", Geboortedatum = DateTime.Parse("1965-09-10"), Straatnaam = "Revalidatieplein", Huisnummer = "7", Telefoonnummer = "656789012", Email = "dirk.m@example.com", HuisartsId = 3 },
                new Patient { Id = 6, Voornaam = "Eva", Achternaam = "Peters", Geboortedatum = DateTime.Parse("1999-04-05"), Straatnaam = "Fysioweg", Huisnummer = "22", Telefoonnummer = "667890123", Email = "eva.p@example.com", HuisartsId = 4 },
                new Patient { Id = 7, Voornaam = "Frank", Achternaam = "de Jong", Geboortedatum = DateTime.Parse("1970-02-18"), Straatnaam = "Gipsstraat", Huisnummer = "1", Telefoonnummer = "678901234", Email = "frank.dj@example.com", HuisartsId = 3 },
                new Patient { Id = 8, Voornaam = "Greetje", Achternaam = "Willems", Geboortedatum = DateTime.Parse("1985-12-03"), Straatnaam = "Rolstoelpad", Huisnummer = "8", Telefoonnummer = "689012345", Email = "greetje.w@example.com", HuisartsId = 4 },
                new Patient { Id = 9, Voornaam = "Hans", Achternaam = "Kramer", Geboortedatum = DateTime.Parse("1993-06-25"), Straatnaam = "Sportlaan", Huisnummer = "15", Telefoonnummer = "690123456", Email = "hans.k@example.com", HuisartsId = 3 },
                new Patient { Id = 10, Voornaam = "Irene", Achternaam = "Van Dam", Geboortedatum = DateTime.Parse("1972-10-14"), Straatnaam = "Zorgcentrum", Huisnummer = "4", Telefoonnummer = "601234567", Email = "irene.vd@example.com", HuisartsId = 4 },
                new Patient { Id = 11, Voornaam = "Jeroen", Achternaam = "Faber", Geboortedatum = DateTime.Parse("1980-08-08"), Straatnaam = "Gezondheidsstraat", Huisnummer = "2", Telefoonnummer = "611223344", Email = "jeroen.f@example.com", HuisartsId = 3 },
                new Patient { Id = 12, Voornaam = "Kim", Achternaam = "Schouten", Geboortedatum = DateTime.Parse("1997-01-20"), Straatnaam = "Pijnbestrijding", Huisnummer = "1", Telefoonnummer = "622334455", Email = "kim.s@example.com", HuisartsId = 4 },
                new Patient { Id = 13, Voornaam = "Lars", Achternaam = "Veenstra", Geboortedatum = DateTime.Parse("1960-05-12"), Straatnaam = "Welzijnshof", Huisnummer = "9", Telefoonnummer = "633445566", Email = "lars.v@example.com", HuisartsId = 3 },
                new Patient { Id = 14, Voornaam = "Mieke", Achternaam = "Bos", Geboortedatum = DateTime.Parse("1990-03-03"), Straatnaam = "Bewegingstraat", Huisnummer = "6", Telefoonnummer = "644556677", Email = "mieke.b@example.com", HuisartsId = 4 },
                new Patient { Id = 15, Voornaam = "Niels", Achternaam = "Dekker", Geboortedatum = DateTime.Parse("1978-07-28"), Straatnaam = "Vitaliteitslaan", Huisnummer = "11", Telefoonnummer = "655667788", Email = "niels.d@example.com", HuisartsId = 3 },
                new Patient { Id = 16, Voornaam = "Olga", Achternaam = "Postma", Geboortedatum = DateTime.Parse("1983-09-09"), Straatnaam = "Herstelstraat", Huisnummer = "14", Telefoonnummer = "666778899", Email = "olga.p@example.com", HuisartsId = 4 },
                new Patient { Id = 17, Voornaam = "Paul", Achternaam = "Kuipers", Geboortedatum = DateTime.Parse("1992-11-11"), Straatnaam = "Zorgpad", Huisnummer = "1", Telefoonnummer = "677889900", Email = "paul.k@example.com", HuisartsId = 3 },
                new Patient { Id = 18, Voornaam = "Quinn", Achternaam = "Vries", Geboortedatum = DateTime.Parse("1975-04-04"), Straatnaam = "Medicinelaan", Huisnummer = "3", Telefoonnummer = "688990011", Email = "quinn.v@example.com", HuisartsId = 4 },
                new Patient { Id = 19, Voornaam = "Ruben", Achternaam = "Smeets", Geboortedatum = DateTime.Parse("1987-02-02"), Straatnaam = "Oefenplein", Huisnummer = "5", Telefoonnummer = "699001122", Email = "ruben.s@example.com", HuisartsId = 3 },
                new Patient { Id = 20, Voornaam = "Sarah", Achternaam = "Koks", Geboortedatum = DateTime.Parse("1994-08-18"), Straatnaam = "Genezingsweg", Huisnummer = "10", Telefoonnummer = "600112233", Email = "sarah.k@example.com", HuisartsId = 4 },
                new Patient { Id = 21, Voornaam = "Tim", Achternaam = "Visser", Geboortedatum = DateTime.Parse("1968-10-25"), Straatnaam = "Sportlaan", Huisnummer = "2", Telefoonnummer = "610203040", Email = "tim.v@example.com", HuisartsId = 3 },
                new Patient { Id = 22, Voornaam = "Ursula", Achternaam = "Bouwman", Geboortedatum = DateTime.Parse("1981-06-07"), Straatnaam = "Kwaliteitsstraat", Huisnummer = "7", Telefoonnummer = "620304050", Email = "ursula.b@example.com", HuisartsId = 4 },
                new Patient { Id = 23, Voornaam = "Vincent", Achternaam = "Meijerink", Geboortedatum = DateTime.Parse("1996-03-29"), Straatnaam = "Zorglaan", Huisnummer = "9", Telefoonnummer = "630405060", Email = "vincent.m@example.com", HuisartsId = 3 },
                new Patient { Id = 24, Voornaam = "Wendy", Achternaam = "Van der Velde", Geboortedatum = DateTime.Parse("1979-01-13"), Straatnaam = "Therapieplein", Huisnummer = "12", Telefoonnummer = "640506070", Email = "wendy.vdv@example.com", HuisartsId = 4 },
                new Patient { Id = 25, Voornaam = "Xavier", Achternaam = "De Ruiter", Geboortedatum = DateTime.Parse("1986-05-05"), Straatnaam = "Revalidatiepad", Huisnummer = "15", Telefoonnummer = "650607080", Email = "xavier.dr@example.com", HuisartsId = 3 }
            );

            modelBuilder.Entity<Logboek>().HasData(
                new Logboek { Id = 1, PatientId = 1, DatumTijdActiviteit = DateTime.Parse("2025-01-10 12:00"), Beschrijving = "Rustdag na operatie." },
                new Logboek { Id = 2, PatientId = 1, DatumTijdActiviteit = DateTime.Parse("2025-01-11 15:00"), Beschrijving = "Korte wandeling met krukken rondom huis." },
                new Logboek { Id = 3, PatientId = 1, DatumTijdActiviteit = DateTime.Parse("2025-01-12 10:00"), Beschrijving = "Huishoudelijke taken (afwas)." },
                new Logboek { Id = 4, PatientId = 1, DatumTijdActiviteit = DateTime.Parse("2025-01-12 16:00"), Beschrijving = "Boodschappen gedaan met hulp." },
                new Logboek { Id = 5, PatientId = 1, DatumTijdActiviteit = DateTime.Parse("2025-01-13 11:00"), Beschrijving = "30 minuten fysiotherapie oefeningen gedaan." },
                new Logboek { Id = 6, PatientId = 1, DatumTijdActiviteit = DateTime.Parse("2025-01-13 17:00"), Beschrijving = "Korte fietstocht op hometrainer (15 min)." },
                new Logboek { Id = 7, PatientId = 1, DatumTijdActiviteit = DateTime.Parse("2025-01-14 09:00"), Beschrijving = "Oefeningen volgens schema uitgevoerd." },
                new Logboek { Id = 8, PatientId = 1, DatumTijdActiviteit = DateTime.Parse("2025-01-14 14:00"), Beschrijving = "Koken en voorbereiden maaltijden." },
                new Logboek { Id = 9, PatientId = 1, DatumTijdActiviteit = DateTime.Parse("2025-01-15 10:00"), Beschrijving = "Oefeningen gedaan" },
                new Logboek { Id = 10, PatientId = 1, DatumTijdActiviteit = DateTime.Parse("2025-01-15 16:00"), Beschrijving = "Wandeling buiten" },
                new Logboek { Id = 11, PatientId = 2, DatumTijdActiviteit = DateTime.Parse("2025-02-01 14:00"), Beschrijving = "Eerste lichte oefeningen voor rug." },
                new Logboek { Id = 12, PatientId = 2, DatumTijdActiviteit = DateTime.Parse("2025-02-02 09:00"), Beschrijving = "Half uur gefietst op hometrainer." },
                new Logboek { Id = 13, PatientId = 2, DatumTijdActiviteit = DateTime.Parse("2025-02-02 18:00"), Beschrijving = "Korte wandeling buiten" },
                new Logboek { Id = 14, PatientId = 2, DatumTijdActiviteit = DateTime.Parse("2025-02-03 10:00"), Beschrijving = "Rug oefeningen gedaan" },
                new Logboek { Id = 15, PatientId = 2, DatumTijdActiviteit = DateTime.Parse("2025-02-03 15:00"), Beschrijving = "Wandeling met hond." },
                new Logboek { Id = 16, PatientId = 3, DatumTijdActiviteit = DateTime.Parse("2025-01-16 09:00"), Beschrijving = "Oefeningen voor schouder mobiliteit." },
                new Logboek { Id = 17, PatientId = 3, DatumTijdActiviteit = DateTime.Parse("2025-01-17 11:00"), Beschrijving = "Lichte huishoudelijke taken." },
                new Logboek { Id = 18, PatientId = 4, DatumTijdActiviteit = DateTime.Parse("2025-02-06 10:00"), Beschrijving = "Enkel oefeningen volgens schema." },
                new Logboek { Id = 19, PatientId = 4, DatumTijdActiviteit = DateTime.Parse("2025-02-07 14:00"), Beschrijving = "Korte wandeling binnenshuis." },
                new Logboek { Id = 20, PatientId = 5, DatumTijdActiviteit = DateTime.Parse("2025-03-03 10:00"), Beschrijving = "Nekoefeningen met elastiek." },
                new Logboek { Id = 21, PatientId = 5, DatumTijdActiviteit = DateTime.Parse("2025-03-04 15:00"), Beschrijving = "Rustig gewandeld." }
            );

            modelBuilder.Entity<Afspraak>().HasData(
                new Afspraak { Id = 1, PatientId = 1, RevalidatieartsId = 5, DatumTijdAfspraak = DateTime.Parse("2025-01-10 14:00"), DuurMinuten = 60, TypeAfspraak = "intake", Status = "afgerond" },
                new Afspraak { Id = 2, PatientId = 1, RevalidatieartsId = 5, DatumTijdAfspraak = DateTime.Parse("2025-01-31 10:00"), DuurMinuten = 45, TypeAfspraak = "controle", Status = "afgerond" },
                new Afspraak { Id = 3, PatientId = 1, RevalidatieartsId = 5, DatumTijdAfspraak = DateTime.Parse("2025-02-21 11:00"), DuurMinuten = 45, TypeAfspraak = "controle", Status = "afgerond" },
                new Afspraak { Id = 4, PatientId = 1, RevalidatieartsId = 5, DatumTijdAfspraak = DateTime.Parse("2025-03-14 13:00"), DuurMinuten = 30, TypeAfspraak = "controle", Status = "afgerond" },
                new Afspraak { Id = 5, PatientId = 1, RevalidatieartsId = 5, DatumTijdAfspraak = DateTime.Parse("2025-04-04 09:00"), DuurMinuten = 30, TypeAfspraak = "controle", Status = "gepland" },
                new Afspraak { Id = 6, PatientId = 1, RevalidatieartsId = 5, DatumTijdAfspraak = DateTime.Parse("2025-04-25 10:00"), DuurMinuten = 30, TypeAfspraak = "controle", Status = "gepland" },
                new Afspraak { Id = 7, PatientId = 2, RevalidatieartsId = 6, DatumTijdAfspraak = DateTime.Parse("2025-02-01 09:00"), DuurMinuten = 60, TypeAfspraak = "intake", Status = "afgerond" },
                new Afspraak { Id = 8, PatientId = 2, RevalidatieartsId = 6, DatumTijdAfspraak = DateTime.Parse("2025-02-22 14:00"), DuurMinuten = 45, TypeAfspraak = "controle", Status = "afgerond" },
                new Afspraak { Id = 9, PatientId = 2, RevalidatieartsId = 6, DatumTijdAfspraak = DateTime.Parse("2025-03-15 10:00"), DuurMinuten = 45, TypeAfspraak = "controle", Status = "gepland" },
                new Afspraak { Id = 10, PatientId = 2, RevalidatieartsId = 6, DatumTijdAfspraak = DateTime.Parse("2025-04-05 11:00"), DuurMinuten = 30, TypeAfspraak = "controle", Status = "gepland" },
                new Afspraak { Id = 11, PatientId = 3, RevalidatieartsId = 5, DatumTijdAfspraak = DateTime.Parse("2025-01-15 16:00"), DuurMinuten = 60, TypeAfspraak = "intake", Status = "afgerond" },
                new Afspraak { Id = 12, PatientId = 3, RevalidatieartsId = 5, DatumTijdAfspraak = DateTime.Parse("2025-02-05 15:00"), DuurMinuten = 45, TypeAfspraak = "controle", Status = "afgerond" },
                new Afspraak { Id = 13, PatientId = 3, RevalidatieartsId = 5, DatumTijdAfspraak = DateTime.Parse("2025-02-26 14:00"), DuurMinuten = 30, TypeAfspraak = "controle", Status = "gepland" },
                new Afspraak { Id = 14, PatientId = 4, RevalidatieartsId = 7, DatumTijdAfspraak = DateTime.Parse("2025-02-05 10:00"), DuurMinuten = 60, TypeAfspraak = "intake", Status = "afgerond" },
                new Afspraak { Id = 15, PatientId = 4, RevalidatieartsId = 7, DatumTijdAfspraak = DateTime.Parse("2025-02-26 11:00"), DuurMinuten = 45, TypeAfspraak = "controle", Status = "afgerond" },
                new Afspraak { Id = 16, PatientId = 4, RevalidatieartsId = 7, DatumTijdAfspraak = DateTime.Parse("2025-03-19 13:00"), DuurMinuten = 30, TypeAfspraak = "controle", Status = "gepland" },
                new Afspraak { Id = 17, PatientId = 5, RevalidatieartsId = 6, DatumTijdAfspraak = DateTime.Parse("2025-03-01 09:00"), DuurMinuten = 60, TypeAfspraak = "intake", Status = "afgerond" },
                new Afspraak { Id = 18, PatientId = 5, RevalidatieartsId = 6, DatumTijdAfspraak = DateTime.Parse("2025-03-22 10:00"), DuurMinuten = 45, TypeAfspraak = "controle", Status = "gepland" },
                new Afspraak { Id = 19, PatientId = 6, RevalidatieartsId = 5, DatumTijdAfspraak = DateTime.Parse("2025-02-10 14:00"), DuurMinuten = 60, TypeAfspraak = "intake", Status = "afgerond" },
                new Afspraak { Id = 20, PatientId = 6, RevalidatieartsId = 5, DatumTijdAfspraak = DateTime.Parse("2025-03-03 15:00"), DuurMinuten = 45, TypeAfspraak = "controle", Status = "gepland" }
            );

            modelBuilder.Entity<Oefening>().HasData(
                new Oefening { Id = 1, Naam = "Kniebuiging", Beschrijving = "Langzaam knieën buigen tot 90 graden rug recht houden.", InstructieVideoUrl = "https://www.youtube.com/watch?v=00g8DINXtIY" },
                new Oefening { Id = 2, Naam = "Hamstring Stretch", Beschrijving = "Zittend één been gestrekt", InstructieVideoUrl = "https://www.youtube.com/shorts/ywgDXzYD6v8" },
                new Oefening { Id = 3, Naam = "Calf Raises", Beschrijving = "Op de tenen staan en langzaam zakken.", InstructieVideoUrl = "https://www.youtube.com/watch?v=eMTy3qylqnE" },
                new Oefening { Id = 4, Naam = "Core Stability Plank", Beschrijving = "Plankpositie rug recht", InstructieVideoUrl = "https://www.youtube.com/watch?v=pvIjsG5Svck" },
                new Oefening { Id = 5, Naam = "Schouder Rotaties", Beschrijving = "Cirkelbewegingen met de armen.", InstructieVideoUrl = "https://www.youtube.com/watch?v=oLwTC-lAJws&list=PLurruTwy8-10c7tAfYnJRkFVy_U1O3IZ1" }
            );

            modelBuilder.Entity<Oefenplan>().HasData(
                new Oefenplan { Id = 1, PatientId = 1, OefeningId = 1, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-01-12"), EindDatum = DateTime.Parse("2025-03-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Focus op stabiliteit" },
                new Oefenplan { Id = 2, PatientId = 1, OefeningId = 2, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-01-12"), EindDatum = DateTime.Parse("2025-03-01"), Herhalingen = 15, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Hamstring flexibiliteit" },
                new Oefenplan { Id = 3, PatientId = 1, OefeningId = 3, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-01-12"), EindDatum = DateTime.Parse("2025-03-01"), Herhalingen = 12, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Kuitspierversterking" },
                new Oefenplan { Id = 4, PatientId = 1, OefeningId = 4, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-01-20"), EindDatum = DateTime.Parse("2025-04-01"), Herhalingen = 1, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Houd 30 sec vast" },
                new Oefenplan { Id = 5, PatientId = 1, OefeningId = 1, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-03-02"), EindDatum = DateTime.Parse("2025-05-01"), Herhalingen = 12, Sets = 4, FrequentiePerDag = 2, Opmerkingen = "Verhoging intensiteit" },
                new Oefenplan { Id = 6, PatientId = 1, OefeningId = 2, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-03-02"), EindDatum = DateTime.Parse("2025-05-01"), Herhalingen = 20, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Verdieping stretch" },
                new Oefenplan { Id = 7, PatientId = 1, OefeningId = 5, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-03-10"), EindDatum = DateTime.Parse("2025-05-15"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Lichte schouderoefeningen voor algehele conditie" },
                new Oefenplan { Id = 8, PatientId = 1, OefeningId = 1, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-05-02"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 15, Sets = 4, FrequentiePerDag = 2, Opmerkingen = "Laatste fase knieherstel" },
                new Oefenplan { Id = 9, PatientId = 1, OefeningId = 4, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-04-05"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 1, Sets = 4, FrequentiePerDag = 1, Opmerkingen = "Core versterking gevorderd" },
                new Oefenplan { Id = 10, PatientId = 1, OefeningId = 3, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-04-10"), EindDatum = DateTime.Parse("2025-06-15"), Herhalingen = 15, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Meer explosieve kuitbewegingen" },

                new Oefenplan { Id = 11, PatientId = 2, OefeningId = 1, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-02-03"), EindDatum = DateTime.Parse("2025-04-01"), Herhalingen = 8, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Voorzichtige beweging" },
                new Oefenplan { Id = 12, PatientId = 2, OefeningId = 4, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-02-03"), EindDatum = DateTime.Parse("2025-04-01"), Herhalingen = 1, Sets = 2, FrequentiePerDag = 1, Opmerkingen = "Houd 20 sec vast" },
                new Oefenplan { Id = 13, PatientId = 2, OefeningId = 2, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-03-01"), EindDatum = DateTime.Parse("2025-05-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Verbeteren flexibiliteit onderrug" },
                new Oefenplan { Id = 14, PatientId = 2, OefeningId = 1, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-04-02"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Verhoging belasting" },
                new Oefenplan { Id = 15, PatientId = 2, OefeningId = 3, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-04-15"), EindDatum = DateTime.Parse("2025-06-30"), Herhalingen = 10, Sets = 2, FrequentiePerDag = 1, Opmerkingen = "Enkelstabiliteit" },

                new Oefenplan { Id = 16, PatientId = 3, OefeningId = 5, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-01-17"), EindDatum = DateTime.Parse("2025-03-01"), Herhalingen = 15, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Focus op rotatie" },
                new Oefenplan { Id = 17, PatientId = 3, OefeningId = 1, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-02-01"), EindDatum = DateTime.Parse("2025-04-01"), Herhalingen = 10, Sets = 2, FrequentiePerDag = 1, Opmerkingen = "Algemene conditie" },
                new Oefenplan { Id = 18, PatientId = 3, OefeningId = 2, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-03-05"), EindDatum = DateTime.Parse("2025-05-01"), Herhalingen = 12, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Schouderflexibiliteit" },
                new Oefenplan { Id = 19, PatientId = 3, OefeningId = 4, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-04-01"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 1, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Core stabiliteit" },

                new Oefenplan { Id = 20, PatientId = 4, OefeningId = 3, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-02-07"), EindDatum = DateTime.Parse("2025-04-01"), Herhalingen = 12, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Enkelversterking" },
                new Oefenplan { Id = 21, PatientId = 4, OefeningId = 1, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-02-07"), EindDatum = DateTime.Parse("2025-04-01"), Herhalingen = 8, Sets = 2, FrequentiePerDag = 1, Opmerkingen = "Lichte knieoefeningen ter ondersteuning" },
                new Oefenplan { Id = 22, PatientId = 4, OefeningId = 2, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-04-02"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Hamstring stretch voor balans" },
                new Oefenplan { Id = 23, PatientId = 4, OefeningId = 5, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-04-10"), EindDatum = DateTime.Parse("2025-06-30"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Schouder mobiliteit" },

                new Oefenplan { Id = 24, PatientId = 5, OefeningId = 1, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-03-03"), EindDatum = DateTime.Parse("2025-05-01"), Herhalingen = 8, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Nek- en rugoefeningen" },
                new Oefenplan { Id = 25, PatientId = 5, OefeningId = 4, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-03-03"), EindDatum = DateTime.Parse("2025-05-01"), Herhalingen = 1, Sets = 2, FrequentiePerDag = 1, Opmerkingen = "Nekstabiliteit" },
                new Oefenplan { Id = 26, PatientId = 5, OefeningId = 2, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-04-05"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Arm- en schouderstretch" },

                new Oefenplan { Id = 27, PatientId = 6, OefeningId = 1, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-02-12"), EindDatum = DateTime.Parse("2025-04-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Rugversterking" },
                new Oefenplan { Id = 28, PatientId = 6, OefeningId = 2, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-02-12"), EindDatum = DateTime.Parse("2025-04-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Onderrug stretch" },
                new Oefenplan { Id = 29, PatientId = 6, OefeningId = 4, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-03-01"), EindDatum = DateTime.Parse("2025-05-01"), Herhalingen = 1, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Core voor rug" },

                new Oefenplan { Id = 30, PatientId = 7, OefeningId = 1, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-03-07"), EindDatum = DateTime.Parse("2025-05-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Kniebuiging lichte belasting" },
                new Oefenplan { Id = 31, PatientId = 7, OefeningId = 2, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-03-07"), EindDatum = DateTime.Parse("2025-05-01"), Herhalingen = 15, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Hamstring flexibiliteit knie" },
                new Oefenplan { Id = 32, PatientId = 7, OefeningId = 3, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-03-20"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 12, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Kuitspieren en balans" },

                new Oefenplan { Id = 33, PatientId = 8, OefeningId = 4, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-04-03"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 1, Sets = 2, FrequentiePerDag = 1, Opmerkingen = "Algemene core stability" },
                new Oefenplan { Id = 34, PatientId = 8, OefeningId = 5, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-04-03"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Lichte mobiliteitsoefeningen" },

                new Oefenplan { Id = 35, PatientId = 9, OefeningId = 1, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-03-12"), EindDatum = DateTime.Parse("2025-05-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Algemene krachtopbouw" },
                new Oefenplan { Id = 36, PatientId = 9, OefeningId = 3, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-03-12"), EindDatum = DateTime.Parse("2025-05-01"), Herhalingen = 12, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Kuit- en enkelstabiliteit" },

                new Oefenplan { Id = 37, PatientId = 10, OefeningId = 1, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-04-07"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 8, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Lichte kniebuigingen" },
                new Oefenplan { Id = 38, PatientId = 10, OefeningId = 2, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-04-07"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Stretch voor mobiliteit" },

                new Oefenplan { Id = 39, PatientId = 11, OefeningId = 4, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-04-10"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 1, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Core stability oefeningen" },
                new Oefenplan { Id = 40, PatientId = 11, OefeningId = 5, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-04-10"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 10, Sets = 2, FrequentiePerDag = 1, Opmerkingen = "Schouder en nek mobiliteit" },

                new Oefenplan { Id = 41, PatientId = 12, OefeningId = 1, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-04-15"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Lichte belasting oefeningen" },
                new Oefenplan { Id = 42, PatientId = 12, OefeningId = 2, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-04-15"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 15, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Flexibiliteit" },

                new Oefenplan { Id = 43, PatientId = 13, OefeningId = 1, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-04-20"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 8, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Knieversterking" },
                new Oefenplan { Id = 44, PatientId = 13, OefeningId = 3, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-04-20"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 12, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Kuitspier activatie" },

                new Oefenplan { Id = 45, PatientId = 14, OefeningId = 2, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-04-25"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Algemene stretch" },
                new Oefenplan { Id = 46, PatientId = 14, OefeningId = 4, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-04-25"), EindDatum = DateTime.Parse("2025-06-01"), Herhalingen = 1, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Core stability algemeen" },

                new Oefenplan { Id = 47, PatientId = 15, OefeningId = 1, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-05-01"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Start herstel" },
                new Oefenplan { Id = 48, PatientId = 15, OefeningId = 2, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-05-01"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 15, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Flexibiliteit" },

                new Oefenplan { Id = 49, PatientId = 16, OefeningId = 3, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-05-05"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 12, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Kuit en enkel" },
                new Oefenplan { Id = 50, PatientId = 16, OefeningId = 4, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-05-05"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 1, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Core training" },

                new Oefenplan { Id = 51, PatientId = 17, OefeningId = 1, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-05-10"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Lichte kracht" },
                new Oefenplan { Id = 52, PatientId = 17, OefeningId = 5, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-05-10"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Schouder mobiliteit" },

                new Oefenplan { Id = 53, PatientId = 18, OefeningId = 2, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-05-15"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 15, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Algemene stretch" },
                new Oefenplan { Id = 54, PatientId = 18, OefeningId = 3, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-05-15"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 12, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Kuit en balans" },

                new Oefenplan { Id = 55, PatientId = 19, OefeningId = 4, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-05-20"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 1, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Core stability" },
                new Oefenplan { Id = 56, PatientId = 19, OefeningId = 1, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-05-20"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Lichte knie" },

                new Oefenplan { Id = 57, PatientId = 20, OefeningId = 5, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-05-25"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Schouder en nek" },
                new Oefenplan { Id = 58, PatientId = 20, OefeningId = 2, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-05-25"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 15, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Hamstring" },

                new Oefenplan { Id = 59, PatientId = 21, OefeningId = 3, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-05-28"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 12, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Kuitversterking" },
                new Oefenplan { Id = 60, PatientId = 21, OefeningId = 1, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-05-28"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Kniebuigingen" },

                new Oefenplan { Id = 61, PatientId = 22, OefeningId = 4, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-05-30"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 1, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Core oefeningen" },
                new Oefenplan { Id = 62, PatientId = 22, OefeningId = 2, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-05-30"), EindDatum = DateTime.Parse("2025-07-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Flexibiliteit" },

                new Oefenplan { Id = 63, PatientId = 23, OefeningId = 5, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-06-01"), EindDatum = DateTime.Parse("2025-08-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Schouder" },
                new Oefenplan { Id = 64, PatientId = 23, OefeningId = 1, RevalidatieartsId = 6, StartDatum = DateTime.Parse("2025-06-01"), EindDatum = DateTime.Parse("2025-08-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Knie" },

                new Oefenplan { Id = 65, PatientId = 24, OefeningId = 2, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-06-05"), EindDatum = DateTime.Parse("2025-08-01"), Herhalingen = 15, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Hamstring" },
                new Oefenplan { Id = 66, PatientId = 24, OefeningId = 3, RevalidatieartsId = 5, StartDatum = DateTime.Parse("2025-06-05"), EindDatum = DateTime.Parse("2025-08-01"), Herhalingen = 12, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Kuit" },

                new Oefenplan { Id = 67, PatientId = 25, OefeningId = 4, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-06-10"), EindDatum = DateTime.Parse("2025-08-01"), Herhalingen = 1, Sets = 3, FrequentiePerDag = 1, Opmerkingen = "Core" },
                new Oefenplan { Id = 68, PatientId = 25, OefeningId = 5, RevalidatieartsId = 7, StartDatum = DateTime.Parse("2025-06-10"), EindDatum = DateTime.Parse("2025-08-01"), Herhalingen = 10, Sets = 3, FrequentiePerDag = 2, Opmerkingen = "Schouder" }
            );

            modelBuilder.Entity<UitgevoerdeOefening>().HasData(
                new UitgevoerdeOefening { Id = 1, PatientOefenplanId = 1, DatumTijdAfgevinkt = DateTime.Parse("2025-01-13 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 2, PatientOefenplanId = 1, DatumTijdAfgevinkt = DateTime.Parse("2025-01-13 18:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 3, PatientOefenplanId = 1, DatumTijdAfgevinkt = DateTime.Parse("2025-01-14 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 4, PatientOefenplanId = 1, DatumTijdAfgevinkt = DateTime.Parse("2025-01-14 18:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 5, PatientOefenplanId = 1, DatumTijdAfgevinkt = DateTime.Parse("2025-01-15 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 6, PatientOefenplanId = 1, DatumTijdAfgevinkt = DateTime.Parse("2025-01-15 18:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 7, PatientOefenplanId = 1, DatumTijdAfgevinkt = DateTime.Parse("2025-01-16 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 8, PatientOefenplanId = 1, DatumTijdAfgevinkt = DateTime.Parse("2025-01-16 18:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 9, PatientOefenplanId = 1, DatumTijdAfgevinkt = DateTime.Parse("2025-01-17 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 10, PatientOefenplanId = 1, DatumTijdAfgevinkt = DateTime.Parse("2025-01-17 18:00"), IsAfgevinkt = true },

                new UitgevoerdeOefening { Id = 11, PatientOefenplanId = 2, DatumTijdAfgevinkt = DateTime.Parse("2025-01-13 09:15"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 12, PatientOefenplanId = 2, DatumTijdAfgevinkt = DateTime.Parse("2025-01-13 18:15"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 13, PatientOefenplanId = 2, DatumTijdAfgevinkt = DateTime.Parse("2025-01-14 09:15"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 14, PatientOefenplanId = 2, DatumTijdAfgevinkt = DateTime.Parse("2025-01-14 18:15"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 15, PatientOefenplanId = 2, DatumTijdAfgevinkt = DateTime.Parse("2025-01-15 09:15"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 16, PatientOefenplanId = 2, DatumTijdAfgevinkt = DateTime.Parse("2025-01-13 09:30"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 17, PatientOefenplanId = 3, DatumTijdAfgevinkt = DateTime.Parse("2025-01-13 18:30"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 18, PatientOefenplanId = 3, DatumTijdAfgevinkt = DateTime.Parse("2025-01-14 09:30"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 19, PatientOefenplanId = 4, DatumTijdAfgevinkt = DateTime.Parse("2025-01-21 10:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 20, PatientOefenplanId = 4, DatumTijdAfgevinkt = DateTime.Parse("2025-01-22 10:00"), IsAfgevinkt = true },

                new UitgevoerdeOefening { Id = 21, PatientOefenplanId = 4, DatumTijdAfgevinkt = DateTime.Parse("2025-01-23 10:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 22, PatientOefenplanId = 5, DatumTijdAfgevinkt = DateTime.Parse("2025-03-03 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 23, PatientOefenplanId = 5, DatumTijdAfgevinkt = DateTime.Parse("2025-03-03 18:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 24, PatientOefenplanId = 6, DatumTijdAfgevinkt = DateTime.Parse("2025-03-03 09:15"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 25, PatientOefenplanId = 7, DatumTijdAfgevinkt = DateTime.Parse("2025-03-11 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 26, PatientOefenplanId = 8, DatumTijdAfgevinkt = DateTime.Parse("2025-05-03 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 27, PatientOefenplanId = 9, DatumTijdAfgevinkt = DateTime.Parse("2025-04-06 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 28, PatientOefenplanId = 10, DatumTijdAfgevinkt = DateTime.Parse("2025-04-11 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 29, PatientOefenplanId = 11, DatumTijdAfgevinkt = DateTime.Parse("2025-02-04 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 30, PatientOefenplanId = 12, DatumTijdAfgevinkt = DateTime.Parse("2025-02-04 10:00"), IsAfgevinkt = true },

                new UitgevoerdeOefening { Id = 31, PatientOefenplanId = 13, DatumTijdAfgevinkt = DateTime.Parse("2025-03-02 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 32, PatientOefenplanId = 14, DatumTijdAfgevinkt = DateTime.Parse("2025-04-03 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 33, PatientOefenplanId = 15, DatumTijdAfgevinkt = DateTime.Parse("2025-04-16 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 34, PatientOefenplanId = 16, DatumTijdAfgevinkt = DateTime.Parse("2025-01-18 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 35, PatientOefenplanId = 17, DatumTijdAfgevinkt = DateTime.Parse("2025-02-02 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 36, PatientOefenplanId = 18, DatumTijdAfgevinkt = DateTime.Parse("2025-03-06 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 37, PatientOefenplanId = 19, DatumTijdAfgevinkt = DateTime.Parse("2025-04-02 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 38, PatientOefenplanId = 20, DatumTijdAfgevinkt = DateTime.Parse("2025-02-08 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 39, PatientOefenplanId = 21, DatumTijdAfgevinkt = DateTime.Parse("2025-02-08 10:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 40, PatientOefenplanId = 22, DatumTijdAfgevinkt = DateTime.Parse("2025-04-03 09:00"), IsAfgevinkt = true },

                new UitgevoerdeOefening { Id = 41, PatientOefenplanId = 23, DatumTijdAfgevinkt = DateTime.Parse("2025-04-11 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 42, PatientOefenplanId = 24, DatumTijdAfgevinkt = DateTime.Parse("2025-03-04 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 43, PatientOefenplanId = 25, DatumTijdAfgevinkt = DateTime.Parse("2025-03-04 10:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 44, PatientOefenplanId = 26, DatumTijdAfgevinkt = DateTime.Parse("2025-04-06 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 45, PatientOefenplanId = 27, DatumTijdAfgevinkt = DateTime.Parse("2025-02-13 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 46, PatientOefenplanId = 28, DatumTijdAfgevinkt = DateTime.Parse("2025-02-13 10:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 47, PatientOefenplanId = 29, DatumTijdAfgevinkt = DateTime.Parse("2025-03-02 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 48, PatientOefenplanId = 30, DatumTijdAfgevinkt = DateTime.Parse("2025-03-08 09:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 49, PatientOefenplanId = 31, DatumTijdAfgevinkt = DateTime.Parse("2025-03-08 10:00"), IsAfgevinkt = true },
                new UitgevoerdeOefening { Id = 50, PatientOefenplanId = 32, DatumTijdAfgevinkt = DateTime.Parse("2025-03-21 09:00"), IsAfgevinkt = true }
            );

            modelBuilder.Entity<PijnIndicatie>().HasData(
                new PijnIndicatie { Id = 1, PatientId = 1, DatumTijdRegistratie = DateTime.Parse("2025-01-10 10:00"), PijnScore = 7, Toelichting = "Na de operatie" },
                new PijnIndicatie { Id = 2, PatientId = 1, DatumTijdRegistratie = DateTime.Parse("2025-01-11 09:00"), PijnScore = 6, Toelichting = "Iets minder pijn" },
                new PijnIndicatie { Id = 3, PatientId = 1, DatumTijdRegistratie = DateTime.Parse("2025-01-12 09:30"), PijnScore = 5, Toelichting = "Pijn bij bewegen" },
                new PijnIndicatie { Id = 4, PatientId = 1, DatumTijdRegistratie = DateTime.Parse("2025-01-13 09:00"), PijnScore = 5, Toelichting = "Tijdens oefeningen wat meer pijn" },
                new PijnIndicatie { Id = 5, PatientId = 1, DatumTijdRegistratie = DateTime.Parse("2025-01-14 09:00"), PijnScore = 4, Toelichting = "Begin van de dag goed" },
                new PijnIndicatie { Id = 6, PatientId = 1, DatumTijdRegistratie = DateTime.Parse("2025-01-15 09:00"), PijnScore = 4, Toelichting = "Pijn nam licht toe na intensievere oefening" },
                new PijnIndicatie { Id = 7, PatientId = 1, DatumTijdRegistratie = DateTime.Parse("2025-01-16 09:00"), PijnScore = 3, Toelichting = "Gaat steeds beter" },
                new PijnIndicatie { Id = 8, PatientId = 1, DatumTijdRegistratie = DateTime.Parse("2025-01-17 09:00"), PijnScore = 3, Toelichting = "Alleen nog lichte pijn bij langdurig staan" },
                new PijnIndicatie { Id = 9, PatientId = 1, DatumTijdRegistratie = DateTime.Parse("2025-01-18 09:00"), PijnScore = 2, Toelichting = "Bijna pijnvrij in rust" },
                new PijnIndicatie { Id = 10, PatientId = 1, DatumTijdRegistratie = DateTime.Parse("2025-01-19 09:00"), PijnScore = 2, Toelichting = "Soms een steek bij onverwachte beweging" },

                new PijnIndicatie { Id = 11, PatientId = 2, DatumTijdRegistratie = DateTime.Parse("2025-02-01 11:00"), PijnScore = 8, Toelichting = "Erge uitstralende pijn in been" },
                new PijnIndicatie { Id = 12, PatientId = 2, DatumTijdRegistratie = DateTime.Parse("2025-02-02 10:00"), PijnScore = 7, Toelichting = "Medicatie helpt iets" },
                new PijnIndicatie { Id = 13, PatientId = 2, DatumTijdRegistratie = DateTime.Parse("2025-02-03 09:00"), PijnScore = 6, Toelichting = "Na fysiotherapie beter" },
                new PijnIndicatie { Id = 14, PatientId = 2, DatumTijdRegistratie = DateTime.Parse("2025-02-04 09:00"), PijnScore = 6, Toelichting = "Pijn blijft hoog na lange zit" },
                new PijnIndicatie { Id = 15, PatientId = 2, DatumTijdRegistratie = DateTime.Parse("2025-02-05 09:00"), PijnScore = 5, Toelichting = "Iets beter na meer rust" },

                new PijnIndicatie { Id = 16, PatientId = 3, DatumTijdRegistratie = DateTime.Parse("2025-01-15 14:00"), PijnScore = 6, Toelichting = "Pijn bij bovenhandse bewegingen" },
                new PijnIndicatie { Id = 17, PatientId = 3, DatumTijdRegistratie = DateTime.Parse("2025-01-16 10:00"), PijnScore = 5, Toelichting = "Bijna geen pijn" },

                new PijnIndicatie { Id = 18, PatientId = 4, DatumTijdRegistratie = DateTime.Parse("2025-02-05 13:00"), PijnScore = 7, Toelichting = "Flink gezwollen enkel" },
                new PijnIndicatie { Id = 19, PatientId = 4, DatumTijdRegistratie = DateTime.Parse("2025-02-06 09:00"), PijnScore = 6, Toelichting = "Minder zwelling" },

                new PijnIndicatie { Id = 20, PatientId = 5, DatumTijdRegistratie = DateTime.Parse("2025-03-01 16:00"), PijnScore = 9, Toelichting = "Ondraaglijke nekpijn en tintelingen" },
                new PijnIndicatie { Id = 21, PatientId = 5, DatumTijdRegistratie = DateTime.Parse("2025-03-02 09:00"), PijnScore = 9, Toelichting = "Medicatie helpt" }
            );

            modelBuilder.Entity<Vergoedingsregel>().HasData(
                new Vergoedingsregel { Id = 1, Naam = "Huisarts consult", MaxVergoedBedrag = 3500, MaxVergoedPercentage = 100, Voorwaarden = "Geen eigen risico", GeldigVanaf = DateTime.Parse("2025-01-01"), Actief = true },
                new Vergoedingsregel { Id = 2, Naam = "Fysiotherapie sessie", MaxVergoedBedrag = 4500, MaxVergoedPercentage = 100, Voorwaarden = "Max 9 sessies per jaar;daarna aanvullend", GeldigVanaf = DateTime.Parse("2025-01-01"), Actief = true },
                new Vergoedingsregel { Id = 3, Naam = "Revalidatiearts intake", MaxVergoedBedrag = 7000, MaxVergoedPercentage = 100, Voorwaarden = "Eenmalig per traject", GeldigVanaf = DateTime.Parse("2025-01-01"), Actief = true },
                new Vergoedingsregel { Id = 4, Naam = "Krukken", MaxVergoedBedrag = 5000, MaxVergoedPercentage = 100, Voorwaarden = "Medische noodzaak", GeldigVanaf = DateTime.Parse("2025-01-01"), Actief = true },
                new Vergoedingsregel { Id = 5, Naam = "Rugbrace", MaxVergoedBedrag = 12000, MaxVergoedPercentage = 80, Voorwaarden = "Met voorschrift specialist", GeldigVanaf = DateTime.Parse("2025-01-01"), Actief = true },
                new Vergoedingsregel { Id = 6, Naam = "MedicatieA-Antibiotica", MaxVergoedBedrag = 100, MaxVergoedPercentage = 100, Voorwaarden = "Basisverzekering", GeldigVanaf = DateTime.Parse("2025-01-01"), Actief = true },
                new Vergoedingsregel { Id = 7, Naam = "Revalidatiearts controle consult", MaxVergoedBedrag = 6000, MaxVergoedPercentage = 100, Voorwaarden = "Conform behandelplan", GeldigVanaf = DateTime.Parse("2025-01-01"), Actief = true },
                new Vergoedingsregel { Id = 8, Naam = "Enkelbrace", MaxVergoedBedrag = 7500, MaxVergoedPercentage = 90, Voorwaarden = "Met voorschrift", GeldigVanaf = DateTime.Parse("2025-01-01"), Actief = true },
                new Vergoedingsregel { Id = 9, Naam = "Nekbrace", MaxVergoedBedrag = 9000, MaxVergoedPercentage = 85, Voorwaarden = "Met voorschrift specialist", GeldigVanaf = DateTime.Parse("2025-01-01"), Actief = true }
            );

            // Zet alle relaties in de hele DbContext op Restrict
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }


        }
    }
}
