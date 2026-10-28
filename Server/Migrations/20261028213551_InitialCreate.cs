using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

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
                    Beschrijving = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    InstructieVideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
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
                    Voorwaarden = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GeldigVanaf = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeldigTot = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Actief = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vergoedingsregels", x => x.Id);
                    table.CheckConstraint("CK_Vergoedingsregels_MaxVergoedBedrag", "[MaxVergoedBedrag] >= 0");
                    table.CheckConstraint("CK_Vergoedingsregels_MaxVergoedPercentage", "[MaxVergoedPercentage] BETWEEN 0 AND 100");
                    table.CheckConstraint("CK_Vergoedingsregels_Periode", "[GeldigTot] IS NULL OR [GeldigTot] >= [GeldigVanaf]");
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
                name: "Accessoires",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    HuisartsId = table.Column<int>(type: "int", nullable: false),
                    Naam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdviesDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerwachteGebruiksperiode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accessoires", x => x.Id);
                    table.CheckConstraint("CK_Accessoires_Status", "[Status] IN ('Actief', 'Afgerond')");
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
                    TypeAfspraak = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Afspraken", x => x.Id);
                    table.CheckConstraint("CK_Afspraken_DuurMinuten", "[DuurMinuten] BETWEEN 1 AND 480");
                    table.CheckConstraint("CK_Afspraken_Status", "[Status] IN ('Gepland', 'Afgerond')");
                    table.CheckConstraint("CK_Afspraken_TypeAfspraak", "[TypeAfspraak] IN ('Intake', 'Controle')");
                });

            migrationBuilder.CreateTable(
                name: "Declaraties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    TypeDeclaratie = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GedeclareerdDoorGebruikerId = table.Column<int>(type: "int", nullable: false),
                    DatumHandeling = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VergoedingsregelId = table.Column<int>(type: "int", nullable: false),
                    Omschrijving = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GedeclareerdBedrag = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Declaraties", x => x.Id);
                    table.CheckConstraint("CK_Declaraties_GedeclareerdBedrag", "[GedeclareerdBedrag] >= 0");
                    table.CheckConstraint("CK_Declaraties_Status", "[Status] IN ('Nieuw', 'Verwerkt')");
                    table.CheckConstraint("CK_Declaraties_TypeDeclaratie", "[TypeDeclaratie] IN ('Huisarts', 'Revalidatiearts')");
                    table.ForeignKey(
                        name: "FK_Declaraties_Vergoedingsregels_VergoedingsregelId",
                        column: x => x.VergoedingsregelId,
                        principalTable: "Vergoedingsregels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Declaratieverwerkingen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeclaratieId = table.Column<int>(type: "int", nullable: false),
                    ZorgverzekeraarMedewerkerId = table.Column<int>(type: "int", nullable: false),
                    DatumVerwerking = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusVerwerking = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    VergoedBedrag = table.Column<int>(type: "int", nullable: false),
                    AfgekeurdBedrag = table.Column<int>(type: "int", nullable: false),
                    Toelichting = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Declaratieverwerkingen", x => x.Id);
                    table.CheckConstraint("CK_Declaratieverwerkingen_Bedragen", "[VergoedBedrag] >= 0 AND [AfgekeurdBedrag] >= 0");
                    table.CheckConstraint("CK_Declaratieverwerkingen_StatusVerwerking", "[StatusVerwerking] IN ('Goedgekeurd', 'Gedeeltelijk goedgekeurd', 'Afgekeurd')");
                    table.ForeignKey(
                        name: "FK_Declaratieverwerkingen_Declaraties_DeclaratieId",
                        column: x => x.DeclaratieId,
                        principalTable: "Declaraties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Gebruikers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gebruikersnaam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WachtwoordHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Voornaam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Achternaam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrganisatieId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gebruikers", x => x.Id);
                    table.CheckConstraint("CK_Gebruikers_OrganisatieId", "([Rol] = 'Zorgverzekeraar' AND [OrganisatieId] IS NOT NULL) OR ([Rol] <> 'Zorgverzekeraar' AND [OrganisatieId] IS NULL)");
                    table.CheckConstraint("CK_Gebruikers_PatientId", "([Rol] = 'Patient' AND [PatientId] IS NOT NULL) OR ([Rol] <> 'Patient' AND [PatientId] IS NULL)");
                    table.CheckConstraint("CK_Gebruikers_Rol", "[Rol] IN ('Patient', 'Huisarts', 'Revalidatiearts', 'Zorgverzekeraar')");
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
                    Geboortedatum = table.Column<DateOnly>(type: "date", nullable: false),
                    Adres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefoonnummer = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HuisartsId = table.Column<int>(type: "int", nullable: false),
                    ZorgverzekeraarId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patienten", x => x.Id);
                    table.CheckConstraint("CK_Patienten_Email", "[Email] LIKE '%_@_%._%'");
                    table.ForeignKey(
                        name: "FK_Patienten_Gebruikers_HuisartsId",
                        column: x => x.HuisartsId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Patienten_Zorgverzekeraars_ZorgverzekeraarId",
                        column: x => x.ZorgverzekeraarId,
                        principalTable: "Zorgverzekeraars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Intakegesprekken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    RevalidatieartsId = table.Column<int>(type: "int", nullable: false),
                    Diagnose = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ErnstBlessure = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Behandeldoelen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DatumIntake = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intakegesprekken", x => x.Id);
                    table.CheckConstraint("CK_Intakegesprekken_ErnstBlessure", "[ErnstBlessure] IN ('Licht', 'Licht tot matig', 'Matig', 'Ernstig', 'Chronisch')");
                    table.ForeignKey(
                        name: "FK_Intakegesprekken_Gebruikers_RevalidatieartsId",
                        column: x => x.RevalidatieartsId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intakegesprekken_Patienten_PatientId",
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
                    Beschrijving = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
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
                name: "Medicaties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    HuisartsId = table.Column<int>(type: "int", nullable: false),
                    Naam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Dosering = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Frequentie = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EindDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicaties", x => x.Id);
                    table.CheckConstraint("CK_Medicaties_Periode", "[EindDatum] >= [StartDatum]");
                    table.CheckConstraint("CK_Medicaties_Status", "[Status] IN ('Actief', 'Afgerond')");
                    table.ForeignKey(
                        name: "FK_Medicaties_Gebruikers_HuisartsId",
                        column: x => x.HuisartsId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Medicaties_Patienten_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patienten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    AuteurGebruikerId = table.Column<int>(type: "int", nullable: false),
                    DatumTijdNotitie = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Inhoud = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notities_Gebruikers_AuteurGebruikerId",
                        column: x => x.AuteurGebruikerId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notities_Patienten_PatientId",
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
                    Opmerkingen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oefenplannen", x => x.Id);
                    table.CheckConstraint("CK_Oefenplannen_FrequentiePerDag", "[FrequentiePerDag] BETWEEN 1 AND 10");
                    table.CheckConstraint("CK_Oefenplannen_Herhalingen", "[Herhalingen] BETWEEN 1 AND 100");
                    table.CheckConstraint("CK_Oefenplannen_Periode", "[EindDatum] >= [StartDatum]");
                    table.CheckConstraint("CK_Oefenplannen_Sets", "[Sets] BETWEEN 1 AND 20");
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
                name: "PatientRevalidatieartsen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    RevalidatieartsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientRevalidatieartsen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientRevalidatieartsen_Gebruikers_RevalidatieartsId",
                        column: x => x.RevalidatieartsId,
                        principalTable: "Gebruikers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientRevalidatieartsen_Patienten_PatientId",
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
                    Toelichting = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PijnIndicaties", x => x.Id);
                    table.CheckConstraint("CK_PijnIndicaties_PijnScore", "[PijnScore] BETWEEN 0 AND 10");
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
                    OefenplanId = table.Column<int>(type: "int", nullable: false),
                    DatumTijdAfgevinkt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAfgevinkt = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UitgevoerdeOefeningen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UitgevoerdeOefeningen_Oefenplannen_OefenplanId",
                        column: x => x.OefenplanId,
                        principalTable: "Oefenplannen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accessoires_HuisartsId",
                table: "Accessoires",
                column: "HuisartsId");

            migrationBuilder.CreateIndex(
                name: "IX_Accessoires_PatientId",
                table: "Accessoires",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Afspraken_PatientId",
                table: "Afspraken",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Afspraken_RevalidatieartsId",
                table: "Afspraken",
                column: "RevalidatieartsId");

            migrationBuilder.CreateIndex(
                name: "IX_Declaraties_GedeclareerdDoorGebruikerId",
                table: "Declaraties",
                column: "GedeclareerdDoorGebruikerId");

            migrationBuilder.CreateIndex(
                name: "IX_Declaraties_PatientId",
                table: "Declaraties",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Declaraties_VergoedingsregelId",
                table: "Declaraties",
                column: "VergoedingsregelId");

            migrationBuilder.CreateIndex(
                name: "IX_Declaratieverwerkingen_DeclaratieId",
                table: "Declaratieverwerkingen",
                column: "DeclaratieId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Declaratieverwerkingen_ZorgverzekeraarMedewerkerId",
                table: "Declaratieverwerkingen",
                column: "ZorgverzekeraarMedewerkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Gebruikers_Gebruikersnaam",
                table: "Gebruikers",
                column: "Gebruikersnaam",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gebruikers_OrganisatieId",
                table: "Gebruikers",
                column: "OrganisatieId");

            migrationBuilder.CreateIndex(
                name: "IX_Gebruikers_PatientId",
                table: "Gebruikers",
                column: "PatientId",
                unique: true,
                filter: "[PatientId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Intakegesprekken_PatientId",
                table: "Intakegesprekken",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Intakegesprekken_RevalidatieartsId",
                table: "Intakegesprekken",
                column: "RevalidatieartsId");

            migrationBuilder.CreateIndex(
                name: "IX_Logboek_PatientId",
                table: "Logboek",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicaties_HuisartsId",
                table: "Medicaties",
                column: "HuisartsId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicaties_PatientId",
                table: "Medicaties",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Notities_AuteurGebruikerId",
                table: "Notities",
                column: "AuteurGebruikerId");

            migrationBuilder.CreateIndex(
                name: "IX_Notities_PatientId",
                table: "Notities",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Oefeningen_Naam",
                table: "Oefeningen",
                column: "Naam",
                unique: true);

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
                name: "IX_Patienten_Email",
                table: "Patienten",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patienten_HuisartsId",
                table: "Patienten",
                column: "HuisartsId");

            migrationBuilder.CreateIndex(
                name: "IX_Patienten_ZorgverzekeraarId",
                table: "Patienten",
                column: "ZorgverzekeraarId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRevalidatieartsen_PatientId_RevalidatieartsId",
                table: "PatientRevalidatieartsen",
                columns: new[] { "PatientId", "RevalidatieartsId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientRevalidatieartsen_RevalidatieartsId",
                table: "PatientRevalidatieartsen",
                column: "RevalidatieartsId");

            migrationBuilder.CreateIndex(
                name: "IX_PijnIndicaties_PatientId",
                table: "PijnIndicaties",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_UitgevoerdeOefeningen_OefenplanId",
                table: "UitgevoerdeOefeningen",
                column: "OefenplanId");

            migrationBuilder.CreateIndex(
                name: "IX_Zorgverzekeraars_Naam",
                table: "Zorgverzekeraars",
                column: "Naam",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accessoires_Gebruikers_HuisartsId",
                table: "Accessoires",
                column: "HuisartsId",
                principalTable: "Gebruikers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accessoires_Patienten_PatientId",
                table: "Accessoires",
                column: "PatientId",
                principalTable: "Patienten",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Afspraken_Gebruikers_RevalidatieartsId",
                table: "Afspraken",
                column: "RevalidatieartsId",
                principalTable: "Gebruikers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Afspraken_Patienten_PatientId",
                table: "Afspraken",
                column: "PatientId",
                principalTable: "Patienten",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Declaraties_Gebruikers_GedeclareerdDoorGebruikerId",
                table: "Declaraties",
                column: "GedeclareerdDoorGebruikerId",
                principalTable: "Gebruikers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Declaraties_Patienten_PatientId",
                table: "Declaraties",
                column: "PatientId",
                principalTable: "Patienten",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Declaratieverwerkingen_Gebruikers_ZorgverzekeraarMedewerkerId",
                table: "Declaratieverwerkingen",
                column: "ZorgverzekeraarMedewerkerId",
                principalTable: "Gebruikers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gebruikers_Patienten_PatientId",
                table: "Gebruikers",
                column: "PatientId",
                principalTable: "Patienten",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patienten_Gebruikers_HuisartsId",
                table: "Patienten");

            migrationBuilder.DropTable(
                name: "Accessoires");

            migrationBuilder.DropTable(
                name: "Afspraken");

            migrationBuilder.DropTable(
                name: "Declaratieverwerkingen");

            migrationBuilder.DropTable(
                name: "Intakegesprekken");

            migrationBuilder.DropTable(
                name: "Logboek");

            migrationBuilder.DropTable(
                name: "Medicaties");

            migrationBuilder.DropTable(
                name: "Notities");

            migrationBuilder.DropTable(
                name: "PatientRevalidatieartsen");

            migrationBuilder.DropTable(
                name: "PijnIndicaties");

            migrationBuilder.DropTable(
                name: "UitgevoerdeOefeningen");

            migrationBuilder.DropTable(
                name: "Declaraties");

            migrationBuilder.DropTable(
                name: "Oefenplannen");

            migrationBuilder.DropTable(
                name: "Vergoedingsregels");

            migrationBuilder.DropTable(
                name: "Oefeningen");

            migrationBuilder.DropTable(
                name: "Gebruikers");

            migrationBuilder.DropTable(
                name: "Patienten");

            migrationBuilder.DropTable(
                name: "Zorgverzekeraars");
        }
    }
}
