using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fepi.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFullSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContratistaEmpresa",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ContratistaRepresentante",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "Importe",
                table: "ConceptosContrato");

            migrationBuilder.RenameColumn(
                name: "NumeroCorrelativo",
                table: "Estimaciones",
                newName: "NumeroEstimacion");

            migrationBuilder.RenameIndex(
                name: "IX_Estimaciones_ContratoId_NumeroCorrelativo",
                table: "Estimaciones",
                newName: "IX_Estimaciones_ContratoId_NumeroEstimacion");

            migrationBuilder.RenameColumn(
                name: "Importe",
                table: "EstimacionConceptos",
                newName: "PrecioUnitarioActual");

            migrationBuilder.RenameColumn(
                name: "CantidadEjecutada",
                table: "EstimacionConceptos",
                newName: "CantidadPorEjecutar");

            migrationBuilder.RenameColumn(
                name: "FechaSolicitud",
                table: "ConveniosModificatorios",
                newName: "FechaEmision");

            migrationBuilder.RenameColumn(
                name: "PeriodoEstimacion",
                table: "Contratos",
                newName: "TipoPeriodo");

            migrationBuilder.RenameColumn(
                name: "MontoContratado",
                table: "Contratos",
                newName: "MontoAnticipo");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Usuarios",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Rol",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Usuarios",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ArchivoComprobantePagoId",
                table: "Estimaciones",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaPago",
                table: "Estimaciones",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PeriodoContratoId",
                table: "Estimaciones",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CantidadAcumuladaActual",
                table: "EstimacionConceptos",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CantidadAcumuladaAnterior",
                table: "EstimacionConceptos",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CantidadEjecutadaPeriodo",
                table: "EstimacionConceptos",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ImporteTotal",
                table: "EstimacionConceptos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "ConveniosModificatorios",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAutorizacion",
                table: "ConveniosModificatorios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroConvenio",
                table: "ConveniosModificatorios",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "NombreObra",
                table: "Contratos",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Contratos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "IVA",
                table: "Contratos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ImporteSinIVA",
                table: "Contratos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ImporteTotal",
                table: "Contratos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ModalidadPago",
                table: "Contratos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NumeroLicitacion",
                table: "Contratos",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumeroPeriodos",
                table: "Contratos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PorcentajeAnticipo",
                table: "Contratos",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UbicacionExacta",
                table: "Contratos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "ConceptosContrato",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ConvenioModificatorioId",
                table: "ConceptosContrato",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsExtraordinario",
                table: "ConceptosContrato",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SeccionConceptoId",
                table: "ConceptosContrato",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Abierta",
                table: "CaratulasBitacora",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "FechaInicio",
                table: "CaratulasBitacora",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "FechaTerminoProgramada",
                table: "CaratulasBitacora",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<decimal>(
                name: "MontoContratadoConIVA",
                table: "CaratulasBitacora",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoContratadoSinIVA",
                table: "CaratulasBitacora",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ResidenteObraUsuarioId",
                table: "CaratulasBitacora",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuperintendenteUsuarioId",
                table: "CaratulasBitacora",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorObraUsuarioId",
                table: "CaratulasBitacora",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AutorUsuarioId",
                table: "BitacoraNotas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CantidadFirmasRequeridas",
                table: "BitacoraNotas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoFirma",
                table: "BitacoraNotas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrdenFirma",
                table: "BitacoraFirmas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ArchivosEvidencia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreOriginal = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    NombreGuardado = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    RutaLocal = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TipoContenido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TamanoBytes = table.Column<long>(type: "bigint", nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioSubioId = table.Column<int>(type: "integer", nullable: false),
                    EntidadRelacionada = table.Column<int>(type: "integer", nullable: false),
                    EntidadId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivosEvidencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivosEvidencia_Usuarios_UsuarioSubioId",
                        column: x => x.UsuarioSubioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConvenioCambios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConvenioModificatorioId = table.Column<int>(type: "integer", nullable: false),
                    EntidadAfectada = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CampoAfectado = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ValorAnterior = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ValorNuevo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DescripcionCambio = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvenioCambios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConvenioCambios_ConveniosModificatorios_ConvenioModificator~",
                        column: x => x.ConvenioModificatorioId,
                        principalTable: "ConveniosModificatorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RepresentanteUsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Empresas_Usuarios_RepresentanteUsuarioId",
                        column: x => x.RepresentanteUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PeriodosContrato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodosContrato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeriodosContrato_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosDiarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    ResponsableUsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosDiarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosDiarios_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrosDiarios_Usuarios_ResponsableUsuarioId",
                        column: x => x.ResponsableUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SeccionesConcepto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeccionesConcepto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeccionesConcepto_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramaObraSecciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    SeccionConceptoId = table.Column<int>(type: "integer", nullable: false),
                    PeriodoInicioId = table.Column<int>(type: "integer", nullable: false),
                    PeriodoFinId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramaObraSecciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramaObraSecciones_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramaObraSecciones_PeriodosContrato_PeriodoFinId",
                        column: x => x.PeriodoFinId,
                        principalTable: "PeriodosContrato",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramaObraSecciones_PeriodosContrato_PeriodoInicioId",
                        column: x => x.PeriodoInicioId,
                        principalTable: "PeriodosContrato",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramaObraSecciones_SeccionesConcepto_SeccionConceptoId",
                        column: x => x.SeccionConceptoId,
                        principalTable: "SeccionesConcepto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProgramaObraPeriodos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProgramaObraSeccionId = table.Column<int>(type: "integer", nullable: false),
                    PeriodoContratoId = table.Column<int>(type: "integer", nullable: false),
                    PorcentajePlanificadoPeriodo = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: false),
                    ImportePlanificadoPeriodo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    AvanceParcialPlanificado = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramaObraPeriodos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramaObraPeriodos_PeriodosContrato_PeriodoContratoId",
                        column: x => x.PeriodoContratoId,
                        principalTable: "PeriodosContrato",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramaObraPeriodos_ProgramaObraSecciones_ProgramaObraSecc~",
                        column: x => x.ProgramaObraSeccionId,
                        principalTable: "ProgramaObraSecciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Username",
                table: "Usuarios",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estimaciones_ArchivoComprobantePagoId",
                table: "Estimaciones",
                column: "ArchivoComprobantePagoId");

            migrationBuilder.CreateIndex(
                name: "IX_Estimaciones_PeriodoContratoId",
                table: "Estimaciones",
                column: "PeriodoContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_EmpresaId",
                table: "Contratos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConceptosContrato_ConvenioModificatorioId",
                table: "ConceptosContrato",
                column: "ConvenioModificatorioId");

            migrationBuilder.CreateIndex(
                name: "IX_ConceptosContrato_SeccionConceptoId",
                table: "ConceptosContrato",
                column: "SeccionConceptoId");

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraNotas_AutorUsuarioId",
                table: "BitacoraNotas",
                column: "AutorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivosEvidencia_UsuarioSubioId",
                table: "ArchivosEvidencia",
                column: "UsuarioSubioId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioCambios_ConvenioModificatorioId",
                table: "ConvenioCambios",
                column: "ConvenioModificatorioId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_RepresentanteUsuarioId",
                table: "Empresas",
                column: "RepresentanteUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosContrato_ContratoId_Numero",
                table: "PeriodosContrato",
                columns: new[] { "ContratoId", "Numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramaObraPeriodos_PeriodoContratoId",
                table: "ProgramaObraPeriodos",
                column: "PeriodoContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramaObraPeriodos_ProgramaObraSeccionId",
                table: "ProgramaObraPeriodos",
                column: "ProgramaObraSeccionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramaObraSecciones_ContratoId",
                table: "ProgramaObraSecciones",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramaObraSecciones_PeriodoFinId",
                table: "ProgramaObraSecciones",
                column: "PeriodoFinId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramaObraSecciones_PeriodoInicioId",
                table: "ProgramaObraSecciones",
                column: "PeriodoInicioId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramaObraSecciones_SeccionConceptoId",
                table: "ProgramaObraSecciones",
                column: "SeccionConceptoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosDiarios_ContratoId",
                table: "RegistrosDiarios",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosDiarios_ResponsableUsuarioId",
                table: "RegistrosDiarios",
                column: "ResponsableUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SeccionesConcepto_ContratoId",
                table: "SeccionesConcepto",
                column: "ContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_BitacoraNotas_Usuarios_AutorUsuarioId",
                table: "BitacoraNotas",
                column: "AutorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConceptosContrato_ConveniosModificatorios_ConvenioModificat~",
                table: "ConceptosContrato",
                column: "ConvenioModificatorioId",
                principalTable: "ConveniosModificatorios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ConceptosContrato_SeccionesConcepto_SeccionConceptoId",
                table: "ConceptosContrato",
                column: "SeccionConceptoId",
                principalTable: "SeccionesConcepto",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Empresas_EmpresaId",
                table: "Contratos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Estimaciones_ArchivosEvidencia_ArchivoComprobantePagoId",
                table: "Estimaciones",
                column: "ArchivoComprobantePagoId",
                principalTable: "ArchivosEvidencia",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Estimaciones_PeriodosContrato_PeriodoContratoId",
                table: "Estimaciones",
                column: "PeriodoContratoId",
                principalTable: "PeriodosContrato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BitacoraNotas_Usuarios_AutorUsuarioId",
                table: "BitacoraNotas");

            migrationBuilder.DropForeignKey(
                name: "FK_ConceptosContrato_ConveniosModificatorios_ConvenioModificat~",
                table: "ConceptosContrato");

            migrationBuilder.DropForeignKey(
                name: "FK_ConceptosContrato_SeccionesConcepto_SeccionConceptoId",
                table: "ConceptosContrato");

            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Empresas_EmpresaId",
                table: "Contratos");

            migrationBuilder.DropForeignKey(
                name: "FK_Estimaciones_ArchivosEvidencia_ArchivoComprobantePagoId",
                table: "Estimaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Estimaciones_PeriodosContrato_PeriodoContratoId",
                table: "Estimaciones");

            migrationBuilder.DropTable(
                name: "ArchivosEvidencia");

            migrationBuilder.DropTable(
                name: "ConvenioCambios");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "ProgramaObraPeriodos");

            migrationBuilder.DropTable(
                name: "RegistrosDiarios");

            migrationBuilder.DropTable(
                name: "ProgramaObraSecciones");

            migrationBuilder.DropTable(
                name: "PeriodosContrato");

            migrationBuilder.DropTable(
                name: "SeccionesConcepto");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Username",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Estimaciones_ArchivoComprobantePagoId",
                table: "Estimaciones");

            migrationBuilder.DropIndex(
                name: "IX_Estimaciones_PeriodoContratoId",
                table: "Estimaciones");

            migrationBuilder.DropIndex(
                name: "IX_Contratos_EmpresaId",
                table: "Contratos");

            migrationBuilder.DropIndex(
                name: "IX_ConceptosContrato_ConvenioModificatorioId",
                table: "ConceptosContrato");

            migrationBuilder.DropIndex(
                name: "IX_ConceptosContrato_SeccionConceptoId",
                table: "ConceptosContrato");

            migrationBuilder.DropIndex(
                name: "IX_BitacoraNotas_AutorUsuarioId",
                table: "BitacoraNotas");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ArchivoComprobantePagoId",
                table: "Estimaciones");

            migrationBuilder.DropColumn(
                name: "FechaPago",
                table: "Estimaciones");

            migrationBuilder.DropColumn(
                name: "PeriodoContratoId",
                table: "Estimaciones");

            migrationBuilder.DropColumn(
                name: "CantidadAcumuladaActual",
                table: "EstimacionConceptos");

            migrationBuilder.DropColumn(
                name: "CantidadAcumuladaAnterior",
                table: "EstimacionConceptos");

            migrationBuilder.DropColumn(
                name: "CantidadEjecutadaPeriodo",
                table: "EstimacionConceptos");

            migrationBuilder.DropColumn(
                name: "ImporteTotal",
                table: "EstimacionConceptos");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "ConveniosModificatorios");

            migrationBuilder.DropColumn(
                name: "FechaAutorizacion",
                table: "ConveniosModificatorios");

            migrationBuilder.DropColumn(
                name: "NumeroConvenio",
                table: "ConveniosModificatorios");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "IVA",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ImporteSinIVA",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ImporteTotal",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "ModalidadPago",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "NumeroLicitacion",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "NumeroPeriodos",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "PorcentajeAnticipo",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "UbicacionExacta",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "ConceptosContrato");

            migrationBuilder.DropColumn(
                name: "ConvenioModificatorioId",
                table: "ConceptosContrato");

            migrationBuilder.DropColumn(
                name: "EsExtraordinario",
                table: "ConceptosContrato");

            migrationBuilder.DropColumn(
                name: "SeccionConceptoId",
                table: "ConceptosContrato");

            migrationBuilder.DropColumn(
                name: "Abierta",
                table: "CaratulasBitacora");

            migrationBuilder.DropColumn(
                name: "FechaInicio",
                table: "CaratulasBitacora");

            migrationBuilder.DropColumn(
                name: "FechaTerminoProgramada",
                table: "CaratulasBitacora");

            migrationBuilder.DropColumn(
                name: "MontoContratadoConIVA",
                table: "CaratulasBitacora");

            migrationBuilder.DropColumn(
                name: "MontoContratadoSinIVA",
                table: "CaratulasBitacora");

            migrationBuilder.DropColumn(
                name: "ResidenteObraUsuarioId",
                table: "CaratulasBitacora");

            migrationBuilder.DropColumn(
                name: "SuperintendenteUsuarioId",
                table: "CaratulasBitacora");

            migrationBuilder.DropColumn(
                name: "SupervisorObraUsuarioId",
                table: "CaratulasBitacora");

            migrationBuilder.DropColumn(
                name: "AutorUsuarioId",
                table: "BitacoraNotas");

            migrationBuilder.DropColumn(
                name: "CantidadFirmasRequeridas",
                table: "BitacoraNotas");

            migrationBuilder.DropColumn(
                name: "EstadoFirma",
                table: "BitacoraNotas");

            migrationBuilder.DropColumn(
                name: "OrdenFirma",
                table: "BitacoraFirmas");

            migrationBuilder.RenameColumn(
                name: "NumeroEstimacion",
                table: "Estimaciones",
                newName: "NumeroCorrelativo");

            migrationBuilder.RenameIndex(
                name: "IX_Estimaciones_ContratoId_NumeroEstimacion",
                table: "Estimaciones",
                newName: "IX_Estimaciones_ContratoId_NumeroCorrelativo");

            migrationBuilder.RenameColumn(
                name: "PrecioUnitarioActual",
                table: "EstimacionConceptos",
                newName: "Importe");

            migrationBuilder.RenameColumn(
                name: "CantidadPorEjecutar",
                table: "EstimacionConceptos",
                newName: "CantidadEjecutada");

            migrationBuilder.RenameColumn(
                name: "FechaEmision",
                table: "ConveniosModificatorios",
                newName: "FechaSolicitud");

            migrationBuilder.RenameColumn(
                name: "TipoPeriodo",
                table: "Contratos",
                newName: "PeriodoEstimacion");

            migrationBuilder.RenameColumn(
                name: "MontoAnticipo",
                table: "Contratos",
                newName: "MontoContratado");

            migrationBuilder.AlterColumn<string>(
                name: "NombreObra",
                table: "Contratos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContratistaEmpresa",
                table: "Contratos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContratistaRepresentante",
                table: "Contratos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Importe",
                table: "ConceptosContrato",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
