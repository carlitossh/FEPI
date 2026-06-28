using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fepi.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixDbContextRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BitacoraNotas_BitacoraTiposNota_BitacoraTipoNotaId",
                table: "BitacoraNotas");

            migrationBuilder.DropIndex(
                name: "IX_BitacoraNotas_BitacoraTipoNotaId",
                table: "BitacoraNotas");

            migrationBuilder.DropColumn(
                name: "BitacoraTipoNotaId",
                table: "BitacoraNotas");

            migrationBuilder.CreateTable(
                name: "ContratoRepresentantesHistorial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    RepresentanteUsuarioId = table.Column<int>(type: "integer", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratoRepresentantesHistorial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContratoRepresentantesHistorial_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContratoRepresentantesHistorial_Usuarios_RepresentanteUsuar~",
                        column: x => x.RepresentanteUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaratulasBitacora_ResidenteObraUsuarioId",
                table: "CaratulasBitacora",
                column: "ResidenteObraUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CaratulasBitacora_SuperintendenteUsuarioId",
                table: "CaratulasBitacora",
                column: "SuperintendenteUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CaratulasBitacora_SupervisorObraUsuarioId",
                table: "CaratulasBitacora",
                column: "SupervisorObraUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratoRepresentantesHistorial_ContratoId",
                table: "ContratoRepresentantesHistorial",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratoRepresentantesHistorial_RepresentanteUsuarioId",
                table: "ContratoRepresentantesHistorial",
                column: "RepresentanteUsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaratulasBitacora_Usuarios_ResidenteObraUsuarioId",
                table: "CaratulasBitacora",
                column: "ResidenteObraUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CaratulasBitacora_Usuarios_SuperintendenteUsuarioId",
                table: "CaratulasBitacora",
                column: "SuperintendenteUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CaratulasBitacora_Usuarios_SupervisorObraUsuarioId",
                table: "CaratulasBitacora",
                column: "SupervisorObraUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaratulasBitacora_Usuarios_ResidenteObraUsuarioId",
                table: "CaratulasBitacora");

            migrationBuilder.DropForeignKey(
                name: "FK_CaratulasBitacora_Usuarios_SuperintendenteUsuarioId",
                table: "CaratulasBitacora");

            migrationBuilder.DropForeignKey(
                name: "FK_CaratulasBitacora_Usuarios_SupervisorObraUsuarioId",
                table: "CaratulasBitacora");

            migrationBuilder.DropTable(
                name: "ContratoRepresentantesHistorial");

            migrationBuilder.DropIndex(
                name: "IX_CaratulasBitacora_ResidenteObraUsuarioId",
                table: "CaratulasBitacora");

            migrationBuilder.DropIndex(
                name: "IX_CaratulasBitacora_SuperintendenteUsuarioId",
                table: "CaratulasBitacora");

            migrationBuilder.DropIndex(
                name: "IX_CaratulasBitacora_SupervisorObraUsuarioId",
                table: "CaratulasBitacora");

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
    }
}
