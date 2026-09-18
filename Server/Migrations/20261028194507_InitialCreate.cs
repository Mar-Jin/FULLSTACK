using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RevaliNow.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Oefeningen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Beschrijving = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstructieVideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oefeningen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vergoedingsregels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaxVergoedBedrag = table.Column<int>(type: "int", nullable: false),
                    MaxVergoedPercentage = table.Column<int>(type: "int", nullable: false),
                    Voorwaarden = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeldigVanaf = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Actief = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vergoedingsregels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zorgverzekeraars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zorgverzekeraars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gebruikers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gebruikersnaam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WachtwoordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Voornaam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Achternaam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganisatieId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gebruikers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gebruikers_Zorgverzekeraars_OrganisatieId",
                        column: x => x.OrganisatieId,
                        principalTable: "Zorgverzekeraars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Patienten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Voornaam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Achternaam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Geboortedatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Straatnaam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Huisnummer = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Telefoonnummer = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HuisartsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patienten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patienten_Gebruikers_HuisartsId",
                        column: x => x.HuisartsId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Afspraken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    RevalidatieartsId = table.Column<int>(type: "int", nullable: false),
                    DatumTijdAfspraak = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuurMinuten = table.Column<int>(type: "int", nullable: false),
                    TypeAfspraak = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Afspraken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Afspraken_Gebruikers_RevalidatieartsId",
                        column: x => x.RevalidatieartsId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Afspraken_Patienten_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patienten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Logboek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DatumTijdActiviteit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Beschrijving = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logboek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logboek_Patienten_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patienten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Oefenplannen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    OefeningId = table.Column<int>(type: "int", nullable: false),
                    RevalidatieartsId = table.Column<int>(type: "int", nullable: false),
                    StartDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EindDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Herhalingen = table.Column<int>(type: "int", nullable: false),
                    Sets = table.Column<int>(type: "int", nullable: false),
                    FrequentiePerDag = table.Column<int>(type: "int", nullable: false),
                    Opmerkingen = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oefenplannen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Oefenplannen_Gebruikers_RevalidatieartsId",
                        column: x => x.RevalidatieartsId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Oefenplannen_Oefeningen_OefeningId",
                        column: x => x.OefeningId,
                        principalTable: "Oefeningen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Oefenplannen_Patienten_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patienten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PijnIndicaties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DatumTijdRegistratie = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PijnScore = table.Column<int>(type: "int", nullable: false),
                    Toelichting = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PijnIndicaties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PijnIndicaties_Patienten_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patienten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UitgevoerdeOefeningen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientOefenplanId = table.Column<int>(type: "int", nullable: false),
                    DatumTijdAfgevinkt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAfgevinkt = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UitgevoerdeOefeningen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UitgevoerdeOefeningen_Oefenplannen_PatientOefenplanId",
                        column: x => x.PatientOefenplanId,
                        principalTable: "Oefenplannen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Gebruikers",
                columns: new[] { "Id", "Achternaam", "Gebruikersnaam", "OrganisatieId", "Rol", "Voornaam", "WachtwoordHash" },
                values: new object[,]
                {
                    { 3, "Bakker", "ha_bakker", null, "Huisarts", "Lisa", "AQAAAAIAAYagAAAAEF39z5hGdv0L4PLYjHFU9rjcIJejcuqCzW3jhAIMaS5Fl7heXGkdSK76evpFMp+V6w==" },
                    { 4, "Janssen", "ha_janssen", null, "Huisarts", "Thomas", "AQAAAAIAAYagAAAAEF39z5hGdv0L4PLYjHFU9rjcIJejcuqCzW3jhAIMaS5Fl7heXGkdSK76evpFMp+V6w==" },
                    { 5, "Smit", "ra_smit", null, "Revalidatiearts", "Emma", "AQAAAAIAAYagAAAAEF39z5hGdv0L4PLYjHFU9rjcIJejcuqCzW3jhAIMaS5Fl7heXGkdSK76evpFMp+V6w==" },
                    { 6, "Groen", "ra_groen", null, "Revalidatiearts", "David", "AQAAAAIAAYagAAAAEF39z5hGdv0L4PLYjHFU9rjcIJejcuqCzW3jhAIMaS5Fl7heXGkdSK76evpFMp+V6w==" },
                    { 7, "Visser", "ra_visser", null, "Revalidatiearts", "Linda", "AQAAAAIAAYagAAAAEF39z5hGdv0L4PLYjHFU9rjcIJejcuqCzW3jhAIMaS5Fl7heXGkdSK76evpFMp+V6w==" }
                });

            migrationBuilder.InsertData(
                table: "Oefeningen",
                columns: new[] { "Id", "Beschrijving", "InstructieVideoUrl", "Naam" },
                values: new object[,]
                {
                    { 1, "Langzaam knieën buigen tot 90 graden rug recht houden.", "https://www.youtube.com/watch?v=00g8DINXtIY", "Kniebuiging" },
                    { 2, "Zittend één been gestrekt", "https://www.youtube.com/shorts/ywgDXzYD6v8", "Hamstring Stretch" },
                    { 3, "Op de tenen staan en langzaam zakken.", "https://www.youtube.com/watch?v=eMTy3qylqnE", "Calf Raises" },
                    { 4, "Plankpositie rug recht", "https://www.youtube.com/watch?v=pvIjsG5Svck", "Core Stability Plank" },
                    { 5, "Cirkelbewegingen met de armen.", "https://www.youtube.com/watch?v=oLwTC-lAJws&list=PLurruTwy8-10c7tAfYnJRkFVy_U1O3IZ1", "Schouder Rotaties" }
                });

            migrationBuilder.InsertData(
                table: "Vergoedingsregels",
                columns: new[] { "Id", "Actief", "GeldigVanaf", "MaxVergoedBedrag", "MaxVergoedPercentage", "Naam", "Voorwaarden" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3500, 100, "Huisarts consult", "Geen eigen risico" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4500, 100, "Fysiotherapie sessie", "Max 9 sessies per jaar;daarna aanvullend" },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7000, 100, "Revalidatiearts intake", "Eenmalig per traject" },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000, 100, "Krukken", "Medische noodzaak" },
                    { 5, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 12000, 80, "Rugbrace", "Met voorschrift specialist" },
                    { 6, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 100, 100, "MedicatieA-Antibiotica", "Basisverzekering" },
                    { 7, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6000, 100, "Revalidatiearts controle consult", "Conform behandelplan" },
                    { 8, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7500, 90, "Enkelbrace", "Met voorschrift" },
                    { 9, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 9000, 85, "Nekbrace", "Met voorschrift specialist" }
                });

            migrationBuilder.InsertData(
                table: "Zorgverzekeraars",
                columns: new[] { "Id", "Naam" },
                values: new object[,]
                {
                    { 1, "ZorgDirect" },
                    { 2, "GezondVooruit" }
                });

            migrationBuilder.InsertData(
                table: "Gebruikers",
                columns: new[] { "Id", "Achternaam", "Gebruikersnaam", "OrganisatieId", "Rol", "Voornaam", "WachtwoordHash" },
                values: new object[,]
                {
                    { 1, "Jansen", "zvm_jansen", 1, "Zorgverzekeraar", "Sophie", "AQAAAAIAAYagAAAAEF39z5hGdv0L4PLYjHFU9rjcIJejcuqCzW3jhAIMaS5Fl7heXGkdSK76evpFMp+V6w==" },
                    { 2, "de Vries", "zvm_devries", 2, "Zorgverzekeraar", "Mark", "AQAAAAIAAYagAAAAEF39z5hGdv0L4PLYjHFU9rjcIJejcuqCzW3jhAIMaS5Fl7heXGkdSK76evpFMp+V6w==" }
                });

            migrationBuilder.InsertData(
                table: "Patienten",
                columns: new[] { "Id", "Achternaam", "Email", "Geboortedatum", "HuisartsId", "Huisnummer", "Straatnaam", "Telefoonnummer", "Voornaam" },
                values: new object[,]
                {
                    { 1, "Voetballer", "freddy.v@example.com", new DateTime(1995, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "10", "Kruisbandstraat", "612345678", "Freddy" },
                    { 2, "Kamer", "anna.k@example.com", new DateTime(1988, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "5", "Therapiestraat", "623456789", "Anna" },
                    { 3, "Verkade", "bas.v@example.com", new DateTime(1976, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "12", "Oefenlaan", "634567890", "Bas" },
                    { 4, "Dirksen", "carla.d@example.com", new DateTime(2001, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "3", "Herstelweg", "645678901", "Carla" },
                    { 5, "Meijer", "dirk.m@example.com", new DateTime(1965, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "7", "Revalidatieplein", "656789012", "Dirk" },
                    { 6, "Peters", "eva.p@example.com", new DateTime(1999, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "22", "Fysioweg", "667890123", "Eva" },
                    { 7, "de Jong", "frank.dj@example.com", new DateTime(1970, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "1", "Gipsstraat", "678901234", "Frank" },
                    { 8, "Willems", "greetje.w@example.com", new DateTime(1985, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "8", "Rolstoelpad", "689012345", "Greetje" },
                    { 9, "Kramer", "hans.k@example.com", new DateTime(1993, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "15", "Sportlaan", "690123456", "Hans" },
                    { 10, "Van Dam", "irene.vd@example.com", new DateTime(1972, 10, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "4", "Zorgcentrum", "601234567", "Irene" },
                    { 11, "Faber", "jeroen.f@example.com", new DateTime(1980, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "2", "Gezondheidsstraat", "611223344", "Jeroen" },
                    { 12, "Schouten", "kim.s@example.com", new DateTime(1997, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "1", "Pijnbestrijding", "622334455", "Kim" },
                    { 13, "Veenstra", "lars.v@example.com", new DateTime(1960, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "9", "Welzijnshof", "633445566", "Lars" },
                    { 14, "Bos", "mieke.b@example.com", new DateTime(1990, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "6", "Bewegingstraat", "644556677", "Mieke" },
                    { 15, "Dekker", "niels.d@example.com", new DateTime(1978, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "11", "Vitaliteitslaan", "655667788", "Niels" },
                    { 16, "Postma", "olga.p@example.com", new DateTime(1983, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "14", "Herstelstraat", "666778899", "Olga" },
                    { 17, "Kuipers", "paul.k@example.com", new DateTime(1992, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "1", "Zorgpad", "677889900", "Paul" },
                    { 18, "Vries", "quinn.v@example.com", new DateTime(1975, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "3", "Medicinelaan", "688990011", "Quinn" },
                    { 19, "Smeets", "ruben.s@example.com", new DateTime(1987, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "5", "Oefenplein", "699001122", "Ruben" },
                    { 20, "Koks", "sarah.k@example.com", new DateTime(1994, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "10", "Genezingsweg", "600112233", "Sarah" },
                    { 21, "Visser", "tim.v@example.com", new DateTime(1968, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "2", "Sportlaan", "610203040", "Tim" },
                    { 22, "Bouwman", "ursula.b@example.com", new DateTime(1981, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "7", "Kwaliteitsstraat", "620304050", "Ursula" },
                    { 23, "Meijerink", "vincent.m@example.com", new DateTime(1996, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "9", "Zorglaan", "630405060", "Vincent" },
                    { 24, "Van der Velde", "wendy.vdv@example.com", new DateTime(1979, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "12", "Therapieplein", "640506070", "Wendy" },
                    { 25, "De Ruiter", "xavier.dr@example.com", new DateTime(1986, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "15", "Revalidatiepad", "650607080", "Xavier" }
                });

            migrationBuilder.InsertData(
                table: "Afspraken",
                columns: new[] { "Id", "DatumTijdAfspraak", "DuurMinuten", "PatientId", "RevalidatieartsId", "Status", "TypeAfspraak" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 10, 14, 0, 0, 0, DateTimeKind.Unspecified), 60, 1, 5, "afgerond", "intake" },
                    { 2, new DateTime(2025, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), 45, 1, 5, "afgerond", "controle" },
                    { 3, new DateTime(2025, 2, 21, 11, 0, 0, 0, DateTimeKind.Unspecified), 45, 1, 5, "afgerond", "controle" },
                    { 4, new DateTime(2025, 3, 14, 13, 0, 0, 0, DateTimeKind.Unspecified), 30, 1, 5, "afgerond", "controle" },
                    { 5, new DateTime(2025, 4, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), 30, 1, 5, "gepland", "controle" },
                    { 6, new DateTime(2025, 4, 25, 10, 0, 0, 0, DateTimeKind.Unspecified), 30, 1, 5, "gepland", "controle" },
                    { 7, new DateTime(2025, 2, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), 60, 2, 6, "afgerond", "intake" },
                    { 8, new DateTime(2025, 2, 22, 14, 0, 0, 0, DateTimeKind.Unspecified), 45, 2, 6, "afgerond", "controle" },
                    { 9, new DateTime(2025, 3, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), 45, 2, 6, "gepland", "controle" },
                    { 10, new DateTime(2025, 4, 5, 11, 0, 0, 0, DateTimeKind.Unspecified), 30, 2, 6, "gepland", "controle" },
                    { 11, new DateTime(2025, 1, 15, 16, 0, 0, 0, DateTimeKind.Unspecified), 60, 3, 5, "afgerond", "intake" },
                    { 12, new DateTime(2025, 2, 5, 15, 0, 0, 0, DateTimeKind.Unspecified), 45, 3, 5, "afgerond", "controle" },
                    { 13, new DateTime(2025, 2, 26, 14, 0, 0, 0, DateTimeKind.Unspecified), 30, 3, 5, "gepland", "controle" },
                    { 14, new DateTime(2025, 2, 5, 10, 0, 0, 0, DateTimeKind.Unspecified), 60, 4, 7, "afgerond", "intake" },
                    { 15, new DateTime(2025, 2, 26, 11, 0, 0, 0, DateTimeKind.Unspecified), 45, 4, 7, "afgerond", "controle" },
                    { 16, new DateTime(2025, 3, 19, 13, 0, 0, 0, DateTimeKind.Unspecified), 30, 4, 7, "gepland", "controle" },
                    { 17, new DateTime(2025, 3, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), 60, 5, 6, "afgerond", "intake" },
                    { 18, new DateTime(2025, 3, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), 45, 5, 6, "gepland", "controle" },
                    { 19, new DateTime(2025, 2, 10, 14, 0, 0, 0, DateTimeKind.Unspecified), 60, 6, 5, "afgerond", "intake" },
                    { 20, new DateTime(2025, 3, 3, 15, 0, 0, 0, DateTimeKind.Unspecified), 45, 6, 5, "gepland", "controle" }
                });

            migrationBuilder.InsertData(
                table: "Logboek",
                columns: new[] { "Id", "Beschrijving", "DatumTijdActiviteit", "PatientId" },
                values: new object[,]
                {
                    { 1, "Rustdag na operatie.", new DateTime(2025, 1, 10, 12, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, "Korte wandeling met krukken rondom huis.", new DateTime(2025, 1, 11, 15, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, "Huishoudelijke taken (afwas).", new DateTime(2025, 1, 12, 10, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 4, "Boodschappen gedaan met hulp.", new DateTime(2025, 1, 12, 16, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 5, "30 minuten fysiotherapie oefeningen gedaan.", new DateTime(2025, 1, 13, 11, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 6, "Korte fietstocht op hometrainer (15 min).", new DateTime(2025, 1, 13, 17, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 7, "Oefeningen volgens schema uitgevoerd.", new DateTime(2025, 1, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 8, "Koken en voorbereiden maaltijden.", new DateTime(2025, 1, 14, 14, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 9, "Oefeningen gedaan", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 10, "Wandeling buiten", new DateTime(2025, 1, 15, 16, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 11, "Eerste lichte oefeningen voor rug.", new DateTime(2025, 2, 1, 14, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 12, "Half uur gefietst op hometrainer.", new DateTime(2025, 2, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 13, "Korte wandeling buiten", new DateTime(2025, 2, 2, 18, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 14, "Rug oefeningen gedaan", new DateTime(2025, 2, 3, 10, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 15, "Wandeling met hond.", new DateTime(2025, 2, 3, 15, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 16, "Oefeningen voor schouder mobiliteit.", new DateTime(2025, 1, 16, 9, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 17, "Lichte huishoudelijke taken.", new DateTime(2025, 1, 17, 11, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 18, "Enkel oefeningen volgens schema.", new DateTime(2025, 2, 6, 10, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 19, "Korte wandeling binnenshuis.", new DateTime(2025, 2, 7, 14, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 20, "Nekoefeningen met elastiek.", new DateTime(2025, 3, 3, 10, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 21, "Rustig gewandeld.", new DateTime(2025, 3, 4, 15, 0, 0, 0, DateTimeKind.Unspecified), 5 }
                });

            migrationBuilder.InsertData(
                table: "Oefenplannen",
                columns: new[] { "Id", "EindDatum", "FrequentiePerDag", "Herhalingen", "OefeningId", "Opmerkingen", "PatientId", "RevalidatieartsId", "Sets", "StartDatum" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 1, "Focus op stabiliteit", 1, 5, 3, new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 15, 2, "Hamstring flexibiliteit", 1, 5, 3, new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 12, 3, "Kuitspierversterking", 1, 5, 3, new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 4, "Houd 30 sec vast", 1, 5, 3, new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 12, 1, "Verhoging intensiteit", 1, 5, 4, new DateTime(2025, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 20, 2, "Verdieping stretch", 1, 5, 3, new DateTime(2025, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 5, "Lichte schouderoefeningen voor algehele conditie", 1, 5, 3, new DateTime(2025, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 15, 1, "Laatste fase knieherstel", 1, 5, 4, new DateTime(2025, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 4, "Core versterking gevorderd", 1, 5, 4, new DateTime(2025, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 15, 3, "Meer explosieve kuitbewegingen", 1, 5, 3, new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 8, 1, "Voorzichtige beweging", 2, 6, 3, new DateTime(2025, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 4, "Houd 20 sec vast", 2, 6, 2, new DateTime(2025, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 2, "Verbeteren flexibiliteit onderrug", 2, 6, 3, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 1, "Verhoging belasting", 2, 6, 3, new DateTime(2025, 4, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 15, new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 3, "Enkelstabiliteit", 2, 6, 2, new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 16, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 15, 5, "Focus op rotatie", 3, 5, 3, new DateTime(2025, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 17, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 1, "Algemene conditie", 3, 5, 2, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 18, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 12, 2, "Schouderflexibiliteit", 3, 5, 3, new DateTime(2025, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 19, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 4, "Core stabiliteit", 3, 5, 3, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 20, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 12, 3, "Enkelversterking", 4, 7, 3, new DateTime(2025, 2, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 21, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 8, 1, "Lichte knieoefeningen ter ondersteuning", 4, 7, 2, new DateTime(2025, 2, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 22, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 2, "Hamstring stretch voor balans", 4, 7, 3, new DateTime(2025, 4, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 23, new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 5, "Schouder mobiliteit", 4, 7, 3, new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 24, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 8, 1, "Nek- en rugoefeningen", 5, 6, 3, new DateTime(2025, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 25, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 4, "Nekstabiliteit", 5, 6, 2, new DateTime(2025, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 26, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 2, "Arm- en schouderstretch", 5, 6, 3, new DateTime(2025, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 27, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 1, "Rugversterking", 6, 5, 3, new DateTime(2025, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 28, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 2, "Onderrug stretch", 6, 5, 3, new DateTime(2025, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 29, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 4, "Core voor rug", 6, 5, 3, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 30, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 1, "Kniebuiging lichte belasting", 7, 7, 3, new DateTime(2025, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 31, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 15, 2, "Hamstring flexibiliteit knie", 7, 7, 3, new DateTime(2025, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 32, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 12, 3, "Kuitspieren en balans", 7, 7, 3, new DateTime(2025, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 33, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 4, "Algemene core stability", 8, 6, 2, new DateTime(2025, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 34, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 5, "Lichte mobiliteitsoefeningen", 8, 6, 3, new DateTime(2025, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 35, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 1, "Algemene krachtopbouw", 9, 5, 3, new DateTime(2025, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 36, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 12, 3, "Kuit- en enkelstabiliteit", 9, 5, 3, new DateTime(2025, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 37, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 8, 1, "Lichte kniebuigingen", 10, 7, 3, new DateTime(2025, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 38, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 2, "Stretch voor mobiliteit", 10, 7, 3, new DateTime(2025, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 39, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 4, "Core stability oefeningen", 11, 6, 3, new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 40, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 5, "Schouder en nek mobiliteit", 11, 6, 2, new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 41, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 1, "Lichte belasting oefeningen", 12, 5, 3, new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 42, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 15, 2, "Flexibiliteit", 12, 5, 3, new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 43, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 8, 1, "Knieversterking", 13, 7, 3, new DateTime(2025, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 44, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 12, 3, "Kuitspier activatie", 13, 7, 3, new DateTime(2025, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 45, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 2, "Algemene stretch", 14, 6, 3, new DateTime(2025, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 46, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 4, "Core stability algemeen", 14, 6, 3, new DateTime(2025, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 47, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 1, "Start herstel", 15, 5, 3, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 48, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 15, 2, "Flexibiliteit", 15, 5, 3, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 49, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 12, 3, "Kuit en enkel", 16, 7, 3, new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 50, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 4, "Core training", 16, 7, 3, new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 51, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 1, "Lichte kracht", 17, 6, 3, new DateTime(2025, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 52, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 5, "Schouder mobiliteit", 17, 6, 3, new DateTime(2025, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 53, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 15, 2, "Algemene stretch", 18, 5, 3, new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 54, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 12, 3, "Kuit en balans", 18, 5, 3, new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 55, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 4, "Core stability", 19, 7, 3, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 56, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 1, "Lichte knie", 19, 7, 3, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 57, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 5, "Schouder en nek", 20, 6, 3, new DateTime(2025, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 58, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 15, 2, "Hamstring", 20, 6, 3, new DateTime(2025, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 59, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 12, 3, "Kuitversterking", 21, 5, 3, new DateTime(2025, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 60, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 1, "Kniebuigingen", 21, 5, 3, new DateTime(2025, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 61, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 4, "Core oefeningen", 22, 7, 3, new DateTime(2025, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 62, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 2, "Flexibiliteit", 22, 7, 3, new DateTime(2025, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 63, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, 5, "Schouder", 23, 6, 3, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 64, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 1, "Knie", 23, 6, 3, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 65, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 15, 2, "Hamstring", 24, 5, 3, new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 66, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 12, 3, "Kuit", 24, 5, 3, new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 67, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 4, "Core", 25, 7, 3, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 68, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10, 5, "Schouder", 25, 7, 3, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "PijnIndicaties",
                columns: new[] { "Id", "DatumTijdRegistratie", "PatientId", "PijnScore", "Toelichting" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 10, 10, 0, 0, 0, DateTimeKind.Unspecified), 1, 7, "Na de operatie" },
                    { 2, new DateTime(2025, 1, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, 6, "Iets minder pijn" },
                    { 3, new DateTime(2025, 1, 12, 9, 30, 0, 0, DateTimeKind.Unspecified), 1, 5, "Pijn bij bewegen" },
                    { 4, new DateTime(2025, 1, 13, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, "Tijdens oefeningen wat meer pijn" },
                    { 5, new DateTime(2025, 1, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, 4, "Begin van de dag goed" },
                    { 6, new DateTime(2025, 1, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, 4, "Pijn nam licht toe na intensievere oefening" },
                    { 7, new DateTime(2025, 1, 16, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, "Gaat steeds beter" },
                    { 8, new DateTime(2025, 1, 17, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, "Alleen nog lichte pijn bij langdurig staan" },
                    { 9, new DateTime(2025, 1, 18, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Bijna pijnvrij in rust" },
                    { 10, new DateTime(2025, 1, 19, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Soms een steek bij onverwachte beweging" },
                    { 11, new DateTime(2025, 2, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), 2, 8, "Erge uitstralende pijn in been" },
                    { 12, new DateTime(2025, 2, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), 2, 7, "Medicatie helpt iets" },
                    { 13, new DateTime(2025, 2, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), 2, 6, "Na fysiotherapie beter" },
                    { 14, new DateTime(2025, 2, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), 2, 6, "Pijn blijft hoog na lange zit" },
                    { 15, new DateTime(2025, 2, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), 2, 5, "Iets beter na meer rust" },
                    { 16, new DateTime(2025, 1, 15, 14, 0, 0, 0, DateTimeKind.Unspecified), 3, 6, "Pijn bij bovenhandse bewegingen" },
                    { 17, new DateTime(2025, 1, 16, 10, 0, 0, 0, DateTimeKind.Unspecified), 3, 5, "Bijna geen pijn" },
                    { 18, new DateTime(2025, 2, 5, 13, 0, 0, 0, DateTimeKind.Unspecified), 4, 7, "Flink gezwollen enkel" },
                    { 19, new DateTime(2025, 2, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), 4, 6, "Minder zwelling" },
                    { 20, new DateTime(2025, 3, 1, 16, 0, 0, 0, DateTimeKind.Unspecified), 5, 9, "Ondraaglijke nekpijn en tintelingen" },
                    { 21, new DateTime(2025, 3, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), 5, 9, "Medicatie helpt" }
                });

            migrationBuilder.InsertData(
                table: "UitgevoerdeOefeningen",
                columns: new[] { "Id", "DatumTijdAfgevinkt", "IsAfgevinkt", "PatientOefenplanId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 13, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 1 },
                    { 2, new DateTime(2025, 1, 13, 18, 0, 0, 0, DateTimeKind.Unspecified), true, 1 },
                    { 3, new DateTime(2025, 1, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 1 },
                    { 4, new DateTime(2025, 1, 14, 18, 0, 0, 0, DateTimeKind.Unspecified), true, 1 },
                    { 5, new DateTime(2025, 1, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 1 },
                    { 6, new DateTime(2025, 1, 15, 18, 0, 0, 0, DateTimeKind.Unspecified), true, 1 },
                    { 7, new DateTime(2025, 1, 16, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 1 },
                    { 8, new DateTime(2025, 1, 16, 18, 0, 0, 0, DateTimeKind.Unspecified), true, 1 },
                    { 9, new DateTime(2025, 1, 17, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 1 },
                    { 10, new DateTime(2025, 1, 17, 18, 0, 0, 0, DateTimeKind.Unspecified), true, 1 },
                    { 11, new DateTime(2025, 1, 13, 9, 15, 0, 0, DateTimeKind.Unspecified), true, 2 },
                    { 12, new DateTime(2025, 1, 13, 18, 15, 0, 0, DateTimeKind.Unspecified), true, 2 },
                    { 13, new DateTime(2025, 1, 14, 9, 15, 0, 0, DateTimeKind.Unspecified), true, 2 },
                    { 14, new DateTime(2025, 1, 14, 18, 15, 0, 0, DateTimeKind.Unspecified), true, 2 },
                    { 15, new DateTime(2025, 1, 15, 9, 15, 0, 0, DateTimeKind.Unspecified), true, 2 },
                    { 16, new DateTime(2025, 1, 13, 9, 30, 0, 0, DateTimeKind.Unspecified), true, 2 },
                    { 17, new DateTime(2025, 1, 13, 18, 30, 0, 0, DateTimeKind.Unspecified), true, 3 },
                    { 18, new DateTime(2025, 1, 14, 9, 30, 0, 0, DateTimeKind.Unspecified), true, 3 },
                    { 19, new DateTime(2025, 1, 21, 10, 0, 0, 0, DateTimeKind.Unspecified), true, 4 },
                    { 20, new DateTime(2025, 1, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), true, 4 },
                    { 21, new DateTime(2025, 1, 23, 10, 0, 0, 0, DateTimeKind.Unspecified), true, 4 },
                    { 22, new DateTime(2025, 3, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 5 },
                    { 23, new DateTime(2025, 3, 3, 18, 0, 0, 0, DateTimeKind.Unspecified), true, 5 },
                    { 24, new DateTime(2025, 3, 3, 9, 15, 0, 0, DateTimeKind.Unspecified), true, 6 },
                    { 25, new DateTime(2025, 3, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 7 },
                    { 26, new DateTime(2025, 5, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 8 },
                    { 27, new DateTime(2025, 4, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 9 },
                    { 28, new DateTime(2025, 4, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 10 },
                    { 29, new DateTime(2025, 2, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 11 },
                    { 30, new DateTime(2025, 2, 4, 10, 0, 0, 0, DateTimeKind.Unspecified), true, 12 },
                    { 31, new DateTime(2025, 3, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 13 },
                    { 32, new DateTime(2025, 4, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 14 },
                    { 33, new DateTime(2025, 4, 16, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 15 },
                    { 34, new DateTime(2025, 1, 18, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 16 },
                    { 35, new DateTime(2025, 2, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 17 },
                    { 36, new DateTime(2025, 3, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 18 },
                    { 37, new DateTime(2025, 4, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 19 },
                    { 38, new DateTime(2025, 2, 8, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 20 },
                    { 39, new DateTime(2025, 2, 8, 10, 0, 0, 0, DateTimeKind.Unspecified), true, 21 },
                    { 40, new DateTime(2025, 4, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 22 },
                    { 41, new DateTime(2025, 4, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 23 },
                    { 42, new DateTime(2025, 3, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 24 },
                    { 43, new DateTime(2025, 3, 4, 10, 0, 0, 0, DateTimeKind.Unspecified), true, 25 },
                    { 44, new DateTime(2025, 4, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 26 },
                    { 45, new DateTime(2025, 2, 13, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 27 },
                    { 46, new DateTime(2025, 2, 13, 10, 0, 0, 0, DateTimeKind.Unspecified), true, 28 },
                    { 47, new DateTime(2025, 3, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 29 },
                    { 48, new DateTime(2025, 3, 8, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 30 },
                    { 49, new DateTime(2025, 3, 8, 10, 0, 0, 0, DateTimeKind.Unspecified), true, 31 },
                    { 50, new DateTime(2025, 3, 21, 9, 0, 0, 0, DateTimeKind.Unspecified), true, 32 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Afspraken_PatientId",
                table: "Afspraken",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Afspraken_RevalidatieartsId",
                table: "Afspraken",
                column: "RevalidatieartsId");

            migrationBuilder.CreateIndex(
                name: "IX_Gebruikers_OrganisatieId",
                table: "Gebruikers",
                column: "OrganisatieId");

            migrationBuilder.CreateIndex(
                name: "IX_Logboek_PatientId",
                table: "Logboek",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Oefenplannen_OefeningId",
                table: "Oefenplannen",
                column: "OefeningId");

            migrationBuilder.CreateIndex(
                name: "IX_Oefenplannen_PatientId",
                table: "Oefenplannen",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Oefenplannen_RevalidatieartsId",
                table: "Oefenplannen",
                column: "RevalidatieartsId");

            migrationBuilder.CreateIndex(
                name: "IX_Patienten_HuisartsId",
                table: "Patienten",
                column: "HuisartsId");

            migrationBuilder.CreateIndex(
                name: "IX_PijnIndicaties_PatientId",
                table: "PijnIndicaties",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_UitgevoerdeOefeningen_PatientOefenplanId",
                table: "UitgevoerdeOefeningen",
                column: "PatientOefenplanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Afspraken");

            migrationBuilder.DropTable(
                name: "Logboek");

            migrationBuilder.DropTable(
                name: "PijnIndicaties");

            migrationBuilder.DropTable(
                name: "UitgevoerdeOefeningen");

            migrationBuilder.DropTable(
                name: "Vergoedingsregels");

            migrationBuilder.DropTable(
                name: "Oefenplannen");

            migrationBuilder.DropTable(
                name: "Oefeningen");

            migrationBuilder.DropTable(
                name: "Patienten");

            migrationBuilder.DropTable(
                name: "Gebruikers");

            migrationBuilder.DropTable(
                name: "Zorgverzekeraars");
        }
    }
}
