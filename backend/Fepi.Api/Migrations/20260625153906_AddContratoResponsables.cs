using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fepi.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddContratoResponsables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NombreObra",
                table: "Contratos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResidenteNombre",
                table: "Contratos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuperintendenteNombre",
                table: "Contratos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupervisorExternoNombre",
                table: "Contratos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreObra",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ResidenteNombre",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "SuperintendenteNombre",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "SupervisorExternoNombre",
                table: "Contratos");
        }
    }
}
