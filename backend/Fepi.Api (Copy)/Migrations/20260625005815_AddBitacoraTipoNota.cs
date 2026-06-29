using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fepi.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBitacoraTipoNota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "BitacoraTiposNota",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "BitacoraTiposNota",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BitacoraTipoNotaId",
                table: "BitacoraNotas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraNotas_BitacoraTipoNotaId",
                table: "BitacoraNotas",
                column: "BitacoraTipoNotaId");

            migrationBuilder.AddForeignKey(
                name: "FK_BitacoraNotas_BitacoraTiposNota_BitacoraTipoNotaId",
                table: "BitacoraNotas",
                column: "BitacoraTipoNotaId",
                principalTable: "BitacoraTiposNota",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BitacoraNotas_BitacoraTiposNota_BitacoraTipoNotaId",
                table: "BitacoraNotas");

            migrationBuilder.DropIndex(
                name: "IX_BitacoraNotas_BitacoraTipoNotaId",
                table: "BitacoraNotas");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "BitacoraTiposNota");

            migrationBuilder.DropColumn(
                name: "BitacoraTipoNotaId",
                table: "BitacoraNotas");

            migrationBuilder.AlterColumn<int>(
                name: "Codigo",
                table: "BitacoraTiposNota",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
