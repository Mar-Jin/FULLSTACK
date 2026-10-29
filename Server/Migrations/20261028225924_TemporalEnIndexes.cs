using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RevaliNow.Migrations
{
    /// <inheritdoc />
    public partial class TemporalEnIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UitgevoerdeOefeningen_OefenplanId",
                table: "UitgevoerdeOefeningen");

            migrationBuilder.DropIndex(
                name: "IX_PijnIndicaties_PatientId",
                table: "PijnIndicaties");

            migrationBuilder.DropIndex(
                name: "IX_Logboek_PatientId",
                table: "Logboek");

            migrationBuilder.DropIndex(
                name: "IX_Afspraken_PatientId",
                table: "Afspraken");

            migrationBuilder.AlterTable(
                name: "PijnIndicaties")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "PijnIndicatiesHistorie")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "GeldigTot")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "GeldigVanaf");

            migrationBuilder.AlterTable(
                name: "Logboek")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LogboekHistorie")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "GeldigTot")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "GeldigVanaf");

            migrationBuilder.AddColumn<DateTime>(
                name: "GeldigTot",
                table: "PijnIndicaties",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GeldigVanaf",
                table: "PijnIndicaties",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GeldigTot",
                table: "Logboek",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GeldigVanaf",
                table: "Logboek",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.CreateIndex(
                name: "IX_UitgevoerdeOefeningen_OefenplanId_DatumTijdAfgevinkt",
                table: "UitgevoerdeOefeningen",
                columns: new[] { "OefenplanId", "DatumTijdAfgevinkt" })
                .Annotation("SqlServer:Include", new[] { "IsAfgevinkt" });

            migrationBuilder.CreateIndex(
                name: "IX_PijnIndicaties_PatientId_DatumTijdRegistratie",
                table: "PijnIndicaties",
                columns: new[] { "PatientId", "DatumTijdRegistratie" },
                descending: new[] { false, true })
                .Annotation("SqlServer:Include", new[] { "PijnScore", "Toelichting" });

            migrationBuilder.CreateIndex(
                name: "IX_Logboek_PatientId_DatumTijdActiviteit",
                table: "Logboek",
                columns: new[] { "PatientId", "DatumTijdActiviteit" },
                descending: new[] { false, true })
                .Annotation("SqlServer:Include", new[] { "Beschrijving" });

            migrationBuilder.CreateIndex(
                name: "IX_Afspraken_PatientId_DatumTijdAfspraak",
                table: "Afspraken",
                columns: new[] { "PatientId", "DatumTijdAfspraak" })
                .Annotation("SqlServer:Include", new[] { "RevalidatieartsId", "DuurMinuten", "TypeAfspraak", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UitgevoerdeOefeningen_OefenplanId_DatumTijdAfgevinkt",
                table: "UitgevoerdeOefeningen");

            migrationBuilder.DropIndex(
                name: "IX_PijnIndicaties_PatientId_DatumTijdRegistratie",
                table: "PijnIndicaties");

            migrationBuilder.DropIndex(
                name: "IX_Logboek_PatientId_DatumTijdActiviteit",
                table: "Logboek");

            migrationBuilder.DropIndex(
                name: "IX_Afspraken_PatientId_DatumTijdAfspraak",
                table: "Afspraken");

            migrationBuilder.DropColumn(
                name: "GeldigTot",
                table: "PijnIndicaties")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "GeldigVanaf",
                table: "PijnIndicaties")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.DropColumn(
                name: "GeldigTot",
                table: "Logboek")
                .Annotation("SqlServer:TemporalIsPeriodEndColumn", true);

            migrationBuilder.DropColumn(
                name: "GeldigVanaf",
                table: "Logboek")
                .Annotation("SqlServer:TemporalIsPeriodStartColumn", true);

            migrationBuilder.AlterTable(
                name: "PijnIndicaties")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "PijnIndicatiesHistorie")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "GeldigTot")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "GeldigVanaf");

            migrationBuilder.AlterTable(
                name: "Logboek")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LogboekHistorie")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "GeldigTot")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "GeldigVanaf");

            migrationBuilder.CreateIndex(
                name: "IX_UitgevoerdeOefeningen_OefenplanId",
                table: "UitgevoerdeOefeningen",
                column: "OefenplanId");

            migrationBuilder.CreateIndex(
                name: "IX_PijnIndicaties_PatientId",
                table: "PijnIndicaties",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Logboek_PatientId",
                table: "Logboek",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Afspraken_PatientId",
                table: "Afspraken",
                column: "PatientId");
        }
    }
}
