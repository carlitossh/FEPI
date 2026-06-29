using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fepi.Api.Migrations
{
    /// <inheritdoc />
    public partial class BackendV2Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntidadId",
                table: "ArchivosEvidencia");

            migrationBuilder.DropColumn(
                name: "EntidadRelacionada",
                table: "ArchivosEvidencia");

            migrationBuilder.AlterColumn<int>(
                name: "EstadoAnterior",
                table: "EstimacionHistoriales",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Accion",
                table: "EstimacionHistoriales",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Aplicado",
                table: "ConveniosModificatorios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAplicacion",
                table: "ConveniosModificatorios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "ConveniosModificatorios",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EntidadId",
                table: "ConvenioCambios",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoCambio",
                table: "ConvenioCambios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "SupervisorExternoNombre",
                table: "Contratos",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SuperintendenteNombre",
                table: "Contratos",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResidenteNombre",
                table: "Contratos",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinancialUsuarioId",
                table: "Contratos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "IvaPorcentaje",
                table: "Contratos",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ResidenteUsuarioId",
                table: "Contratos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SuperintendenteUsuarioId",
                table: "Contratos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorExternoUsuarioId",
                table: "Contratos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConvenioHistoriales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConvenioModificatorioId = table.Column<int>(type: "integer", nullable: false),
                    Accion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EstadoNuevo = table.Column<int>(type: "integer", nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvenioHistoriales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConvenioHistoriales_ConveniosModificatorios_ConvenioModific~",
                        column: x => x.ConvenioModificatorioId,
                        principalTable: "ConveniosModificatorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConvenioHistoriales_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_FinancialUsuarioId",
                table: "Contratos",
                column: "FinancialUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_ResidenteUsuarioId",
                table: "Contratos",
                column: "ResidenteUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_SuperintendenteUsuarioId",
                table: "Contratos",
                column: "SuperintendenteUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_SupervisorExternoUsuarioId",
                table: "Contratos",
                column: "SupervisorExternoUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioHistoriales_ConvenioModificatorioId",
                table: "ConvenioHistoriales",
                column: "ConvenioModificatorioId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioHistoriales_UsuarioId",
                table: "ConvenioHistoriales",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Usuarios_FinancialUsuarioId",
                table: "Contratos",
                column: "FinancialUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Usuarios_ResidenteUsuarioId",
                table: "Contratos",
                column: "ResidenteUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Usuarios_SuperintendenteUsuarioId",
                table: "Contratos",
                column: "SuperintendenteUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Usuarios_SupervisorExternoUsuarioId",
                table: "Contratos",
                column: "SupervisorExternoUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Usuarios_FinancialUsuarioId",
                table: "Contratos");

            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Usuarios_ResidenteUsuarioId",
                table: "Contratos");

            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Usuarios_SuperintendenteUsuarioId",
                table: "Contratos");

            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Usuarios_SupervisorExternoUsuarioId",
                table: "Contratos");

            migrationBuilder.DropTable(
                name: "ConvenioHistoriales");

            migrationBuilder.DropIndex(
                name: "IX_Contratos_FinancialUsuarioId",
                table: "Contratos");

            migrationBuilder.DropIndex(
                name: "IX_Contratos_ResidenteUsuarioId",
                table: "Contratos");

            migrationBuilder.DropIndex(
                name: "IX_Contratos_SuperintendenteUsuarioId",
                table: "Contratos");

            migrationBuilder.DropIndex(
                name: "IX_Contratos_SupervisorExternoUsuarioId",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "Accion",
                table: "EstimacionHistoriales");

            migrationBuilder.DropColumn(
                name: "Aplicado",
                table: "ConveniosModificatorios");

            migrationBuilder.DropColumn(
                name: "FechaAplicacion",
                table: "ConveniosModificatorios");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "ConveniosModificatorios");

            migrationBuilder.DropColumn(
                name: "EntidadId",
                table: "ConvenioCambios");

            migrationBuilder.DropColumn(
                name: "TipoCambio",
                table: "ConvenioCambios");

            migrationBuilder.DropColumn(
                name: "FinancialUsuarioId",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "IvaPorcentaje",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ResidenteUsuarioId",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "SuperintendenteUsuarioId",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "SupervisorExternoUsuarioId",
                table: "Contratos");

            migrationBuilder.AlterColumn<int>(
                name: "EstadoAnterior",
                table: "EstimacionHistoriales",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SupervisorExternoNombre",
                table: "Contratos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SuperintendenteNombre",
                table: "Contratos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResidenteNombre",
                table: "Contratos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EntidadId",
                table: "ArchivosEvidencia",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EntidadRelacionada",
                table: "ArchivosEvidencia",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
