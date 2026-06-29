using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fepi.Api.Migrations
{
    /// <inheritdoc />
    public partial class EstimacionesFlujoPagos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EstimacionPagos_EstimacionId",
                table: "EstimacionPagos");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "EstimacionPagos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UsuarioRegistroId",
                table: "EstimacionPagos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstadoPago",
                table: "Estimaciones",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAprobacionResidencia",
                table: "Estimaciones",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAprobacionSupervision",
                table: "Estimaciones",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoPagadoAcumulado",
                table: "Estimaciones",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioAprobacionResidenciaId",
                table: "Estimaciones",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioAprobacionSupervisionId",
                table: "Estimaciones",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstimacionPagos_EstimacionId",
                table: "EstimacionPagos",
                column: "EstimacionId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimacionPagos_UsuarioRegistroId",
                table: "EstimacionPagos",
                column: "UsuarioRegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_Estimaciones_UsuarioAprobacionResidenciaId",
                table: "Estimaciones",
                column: "UsuarioAprobacionResidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Estimaciones_UsuarioAprobacionSupervisionId",
                table: "Estimaciones",
                column: "UsuarioAprobacionSupervisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Estimaciones_Usuarios_UsuarioAprobacionResidenciaId",
                table: "Estimaciones",
                column: "UsuarioAprobacionResidenciaId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Estimaciones_Usuarios_UsuarioAprobacionSupervisionId",
                table: "Estimaciones",
                column: "UsuarioAprobacionSupervisionId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EstimacionPagos_Usuarios_UsuarioRegistroId",
                table: "EstimacionPagos",
                column: "UsuarioRegistroId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estimaciones_Usuarios_UsuarioAprobacionResidenciaId",
                table: "Estimaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Estimaciones_Usuarios_UsuarioAprobacionSupervisionId",
                table: "Estimaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_EstimacionPagos_Usuarios_UsuarioRegistroId",
                table: "EstimacionPagos");

            migrationBuilder.DropIndex(
                name: "IX_EstimacionPagos_EstimacionId",
                table: "EstimacionPagos");

            migrationBuilder.DropIndex(
                name: "IX_EstimacionPagos_UsuarioRegistroId",
                table: "EstimacionPagos");

            migrationBuilder.DropIndex(
                name: "IX_Estimaciones_UsuarioAprobacionResidenciaId",
                table: "Estimaciones");

            migrationBuilder.DropIndex(
                name: "IX_Estimaciones_UsuarioAprobacionSupervisionId",
                table: "Estimaciones");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "EstimacionPagos");

            migrationBuilder.DropColumn(
                name: "UsuarioRegistroId",
                table: "EstimacionPagos");

            migrationBuilder.DropColumn(
                name: "EstadoPago",
                table: "Estimaciones");

            migrationBuilder.DropColumn(
                name: "FechaAprobacionResidencia",
                table: "Estimaciones");

            migrationBuilder.DropColumn(
                name: "FechaAprobacionSupervision",
                table: "Estimaciones");

            migrationBuilder.DropColumn(
                name: "MontoPagadoAcumulado",
                table: "Estimaciones");

            migrationBuilder.DropColumn(
                name: "UsuarioAprobacionResidenciaId",
                table: "Estimaciones");

            migrationBuilder.DropColumn(
                name: "UsuarioAprobacionSupervisionId",
                table: "Estimaciones");

            migrationBuilder.CreateIndex(
                name: "IX_EstimacionPagos_EstimacionId",
                table: "EstimacionPagos",
                column: "EstimacionId",
                unique: true);
        }
    }
}
