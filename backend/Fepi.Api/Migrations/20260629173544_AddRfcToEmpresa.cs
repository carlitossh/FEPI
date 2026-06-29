using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fepi.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRfcToEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rfc",
                table: "Empresas",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_Rfc",
                table: "Empresas",
                column: "Rfc",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Empresas_Rfc",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "Rfc",
                table: "Empresas");
        }
    }
}
