using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fepi.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BitacoraTiposNota",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Codigo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitacoraTiposNota", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contratos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumeroContrato = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    MontoContratado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaTermino = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodoEstimacion = table.Column<int>(type: "integer", nullable: false),
                    DependenciaContratante = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContratistaEmpresa = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContratistaRepresentante = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Correo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    EntidadReferenciaId = table.Column<int>(type: "integer", nullable: false),
                    EntidadReferenciaTipo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RolDestino = table.Column<int>(type: "integer", nullable: false),
                    Mensaje = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alertas_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bitacoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bitacoras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bitacoras_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConceptosContrato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    Clave = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UnidadMedida = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CantidadContratada = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Importe = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptosContrato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConceptosContrato_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosContrato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TipoDocumento = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    UrlArchivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosContrato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentosContrato_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntregasRecepcion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    FechaEntrega = table.Column<DateOnly>(type: "date", nullable: false),
                    EstadoObraDescripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    EstadoGarantiasDescripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntregasRecepcion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntregasRecepcion_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Finiquitos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    TotalPagado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPendiente = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDeductivas = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalRetenciones = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    UrlReporteFiniquito = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaEmision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Finiquitos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Finiquitos_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Garantias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Porcentaje = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    Vigencia = table.Column<DateOnly>(type: "date", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Garantias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Garantias_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramaObraItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    Periodo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    PorcentajeProgramado = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    MontoProgramado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramaObraItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramaObraItems_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConveniosModificatorios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Justificacion = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    MontoSolicitado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    PlazoDiasSolicitado = table.Column<int>(type: "integer", nullable: true),
                    VariacionAcumuladaPorcentaje = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SolicitanteId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConveniosModificatorios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConveniosModificatorios_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConveniosModificatorios_Usuarios_SolicitanteId",
                        column: x => x.SolicitanteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Estimaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    NumeroCorrelativo = table.Column<int>(type: "integer", nullable: false),
                    Periodo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioEnvioId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estimaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estimaciones_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Estimaciones_Usuarios_UsuarioEnvioId",
                        column: x => x.UsuarioEnvioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioContratos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    Rol = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioContratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioContratos_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioContratos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BitacoraMinutas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BitacoraId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Lugar = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContenidoAcuerdos = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitacoraMinutas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BitacoraMinutas_Bitacoras_BitacoraId",
                        column: x => x.BitacoraId,
                        principalTable: "Bitacoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BitacoraNotas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BitacoraId = table.Column<int>(type: "integer", nullable: false),
                    Folio = table.Column<int>(type: "integer", nullable: false),
                    TipoRegistro = table.Column<int>(type: "integer", nullable: false),
                    TipoNotaCatalogoId = table.Column<int>(type: "integer", nullable: true),
                    Asunto = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Contenido = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    FolioVinculadoId = table.Column<int>(type: "integer", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitacoraNotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BitacoraNotas_BitacoraNotas_FolioVinculadoId",
                        column: x => x.FolioVinculadoId,
                        principalTable: "BitacoraNotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BitacoraNotas_BitacoraTiposNota_TipoNotaCatalogoId",
                        column: x => x.TipoNotaCatalogoId,
                        principalTable: "BitacoraTiposNota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BitacoraNotas_Bitacoras_BitacoraId",
                        column: x => x.BitacoraId,
                        principalTable: "Bitacoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaratulasBitacora",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BitacoraId = table.Column<int>(type: "integer", nullable: false),
                    FolioBitacora = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    NombreContrato = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    NumeroContrato = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    TipoContrato = table.Column<int>(type: "integer", nullable: false),
                    Dependencia = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Contratista = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Residente = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Supervisor = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Superintendente = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    FechaApertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaratulasBitacora", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaratulasBitacora_Bitacoras_BitacoraId",
                        column: x => x.BitacoraId,
                        principalTable: "Bitacoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AvancesDiarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    ConceptoContratoId = table.Column<int>(type: "integer", nullable: false),
                    CantidadEjecutada = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    DescripcionActividad = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ActorId = table.Column<int>(type: "integer", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvancesDiarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvancesDiarios_ConceptosContrato_ConceptoContratoId",
                        column: x => x.ConceptoContratoId,
                        principalTable: "ConceptosContrato",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AvancesDiarios_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AvancesDiarios_Usuarios_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntregaRecepcionEvidencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntregaRecepcionId = table.Column<int>(type: "integer", nullable: false),
                    UrlArchivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntregaRecepcionEvidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntregaRecepcionEvidencias_EntregasRecepcion_EntregaRecepci~",
                        column: x => x.EntregaRecepcionId,
                        principalTable: "EntregasRecepcion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConvenioDocumentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConvenioModificatorioId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UrlArchivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvenioDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConvenioDocumentos_ConveniosModificatorios_ConvenioModifica~",
                        column: x => x.ConvenioModificatorioId,
                        principalTable: "ConveniosModificatorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConvenioPromocionesResidencia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConvenioModificatorioId = table.Column<int>(type: "integer", nullable: false),
                    ResidenteId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvenioPromocionesResidencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConvenioPromocionesResidencia_ConveniosModificatorios_Conve~",
                        column: x => x.ConvenioModificatorioId,
                        principalTable: "ConveniosModificatorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConvenioPromocionesResidencia_Usuarios_ResidenteId",
                        column: x => x.ResidenteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConvenioResolucionesDependencia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConvenioModificatorioId = table.Column<int>(type: "integer", nullable: false),
                    Aprobado = table.Column<bool>(type: "boolean", nullable: false),
                    MotivoRechazo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    UsuarioDependenciaId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvenioResolucionesDependencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConvenioResolucionesDependencia_ConveniosModificatorios_Con~",
                        column: x => x.ConvenioModificatorioId,
                        principalTable: "ConveniosModificatorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConvenioResolucionesDependencia_Usuarios_UsuarioDependencia~",
                        column: x => x.UsuarioDependenciaId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConvenioRevisionesSupervision",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConvenioModificatorioId = table.Column<int>(type: "integer", nullable: false),
                    Decision = table.Column<int>(type: "integer", nullable: false),
                    Justificacion = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    SupervisorId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvenioRevisionesSupervision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConvenioRevisionesSupervision_ConveniosModificatorios_Conve~",
                        column: x => x.ConvenioModificatorioId,
                        principalTable: "ConveniosModificatorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConvenioRevisionesSupervision_Usuarios_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstimacionConceptos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EstimacionId = table.Column<int>(type: "integer", nullable: false),
                    ConceptoContratoId = table.Column<int>(type: "integer", nullable: false),
                    CantidadEjecutada = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Importe = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimacionConceptos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimacionConceptos_ConceptosContrato_ConceptoContratoId",
                        column: x => x.ConceptoContratoId,
                        principalTable: "ConceptosContrato",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstimacionConceptos_Estimaciones_EstimacionId",
                        column: x => x.EstimacionId,
                        principalTable: "Estimaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstimacionDocumentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EstimacionId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TipoDocumento = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    UrlArchivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimacionDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimacionDocumentos_Estimaciones_EstimacionId",
                        column: x => x.EstimacionId,
                        principalTable: "Estimaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstimacionHistoriales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EstimacionId = table.Column<int>(type: "integer", nullable: false),
                    EstadoAnterior = table.Column<int>(type: "integer", nullable: false),
                    EstadoNuevo = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimacionHistoriales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimacionHistoriales_Estimaciones_EstimacionId",
                        column: x => x.EstimacionId,
                        principalTable: "Estimaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstimacionHistoriales_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstimacionObservaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EstimacionId = table.Column<int>(type: "integer", nullable: false),
                    Texto = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimacionObservaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimacionObservaciones_Estimaciones_EstimacionId",
                        column: x => x.EstimacionId,
                        principalTable: "Estimaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstimacionObservaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstimacionPagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EstimacionId = table.Column<int>(type: "integer", nullable: false),
                    FechaPago = table.Column<DateOnly>(type: "date", nullable: false),
                    ReferenciaBancaria = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    MontoPagado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimacionPagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimacionPagos_Estimaciones_EstimacionId",
                        column: x => x.EstimacionId,
                        principalTable: "Estimaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BitacoraMinutaParticipantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BitacoraMinutaId = table.Column<int>(type: "integer", nullable: false),
                    NombreParticipante = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitacoraMinutaParticipantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BitacoraMinutaParticipantes_BitacoraMinutas_BitacoraMinutaId",
                        column: x => x.BitacoraMinutaId,
                        principalTable: "BitacoraMinutas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BitacoraFirmas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BitacoraNotaId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    RolFirmante = table.Column<int>(type: "integer", nullable: false),
                    EsEmisor = table.Column<bool>(type: "boolean", nullable: false),
                    Firmado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaFirma = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitacoraFirmas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BitacoraFirmas_BitacoraNotas_BitacoraNotaId",
                        column: x => x.BitacoraNotaId,
                        principalTable: "BitacoraNotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BitacoraFirmas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BitacoraIncidencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BitacoraId = table.Column<int>(type: "integer", nullable: false),
                    FechaEvento = table.Column<DateOnly>(type: "date", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    UrlFotografia = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ActorRegistroId = table.Column<int>(type: "integer", nullable: false),
                    NotaGeneradaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitacoraIncidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BitacoraIncidencias_BitacoraNotas_NotaGeneradaId",
                        column: x => x.NotaGeneradaId,
                        principalTable: "BitacoraNotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BitacoraIncidencias_Bitacoras_BitacoraId",
                        column: x => x.BitacoraId,
                        principalTable: "Bitacoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BitacoraIncidencias_Usuarios_ActorRegistroId",
                        column: x => x.ActorRegistroId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstimacionNotasBitacora",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EstimacionId = table.Column<int>(type: "integer", nullable: false),
                    NotaBitacoraId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimacionNotasBitacora", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimacionNotasBitacora_BitacoraNotas_NotaBitacoraId",
                        column: x => x.NotaBitacoraId,
                        principalTable: "BitacoraNotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstimacionNotasBitacora_Estimaciones_EstimacionId",
                        column: x => x.EstimacionId,
                        principalTable: "Estimaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AvanceEvidencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AvanceDiarioId = table.Column<int>(type: "integer", nullable: false),
                    UrlFoto = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvanceEvidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvanceEvidencias_AvancesDiarios_AvanceDiarioId",
                        column: x => x.AvanceDiarioId,
                        principalTable: "AvancesDiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_ContratoId",
                table: "Alertas",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_AvanceEvidencias_AvanceDiarioId",
                table: "AvanceEvidencias",
                column: "AvanceDiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_AvancesDiarios_ActorId",
                table: "AvancesDiarios",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_AvancesDiarios_ConceptoContratoId",
                table: "AvancesDiarios",
                column: "ConceptoContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_AvancesDiarios_ContratoId",
                table: "AvancesDiarios",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraFirmas_BitacoraNotaId",
                table: "BitacoraFirmas",
                column: "BitacoraNotaId");

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraFirmas_UsuarioId",
                table: "BitacoraFirmas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraIncidencias_ActorRegistroId",
                table: "BitacoraIncidencias",
                column: "ActorRegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraIncidencias_BitacoraId",
                table: "BitacoraIncidencias",
                column: "BitacoraId");

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraIncidencias_NotaGeneradaId",
                table: "BitacoraIncidencias",
                column: "NotaGeneradaId");

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraMinutaParticipantes_BitacoraMinutaId",
                table: "BitacoraMinutaParticipantes",
                column: "BitacoraMinutaId");

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraMinutas_BitacoraId",
                table: "BitacoraMinutas",
                column: "BitacoraId");

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraNotas_BitacoraId_Folio",
                table: "BitacoraNotas",
                columns: new[] { "BitacoraId", "Folio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraNotas_FolioVinculadoId",
                table: "BitacoraNotas",
                column: "FolioVinculadoId");

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraNotas_TipoNotaCatalogoId",
                table: "BitacoraNotas",
                column: "TipoNotaCatalogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Bitacoras_ContratoId",
                table: "Bitacoras",
                column: "ContratoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraTiposNota_Codigo",
                table: "BitacoraTiposNota",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaratulasBitacora_BitacoraId",
                table: "CaratulasBitacora",
                column: "BitacoraId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaratulasBitacora_FolioBitacora",
                table: "CaratulasBitacora",
                column: "FolioBitacora",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConceptosContrato_ContratoId",
                table: "ConceptosContrato",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_NumeroContrato",
                table: "Contratos",
                column: "NumeroContrato",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioDocumentos_ConvenioModificatorioId",
                table: "ConvenioDocumentos",
                column: "ConvenioModificatorioId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioPromocionesResidencia_ConvenioModificatorioId",
                table: "ConvenioPromocionesResidencia",
                column: "ConvenioModificatorioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioPromocionesResidencia_ResidenteId",
                table: "ConvenioPromocionesResidencia",
                column: "ResidenteId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioResolucionesDependencia_ConvenioModificatorioId",
                table: "ConvenioResolucionesDependencia",
                column: "ConvenioModificatorioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioResolucionesDependencia_UsuarioDependenciaId",
                table: "ConvenioResolucionesDependencia",
                column: "UsuarioDependenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioRevisionesSupervision_ConvenioModificatorioId",
                table: "ConvenioRevisionesSupervision",
                column: "ConvenioModificatorioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioRevisionesSupervision_SupervisorId",
                table: "ConvenioRevisionesSupervision",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_ConveniosModificatorios_ContratoId",
                table: "ConveniosModificatorios",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_ConveniosModificatorios_SolicitanteId",
                table: "ConveniosModificatorios",
                column: "SolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosContrato_ContratoId",
                table: "DocumentosContrato",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_EntregaRecepcionEvidencias_EntregaRecepcionId",
                table: "EntregaRecepcionEvidencias",
                column: "EntregaRecepcionId");

            migrationBuilder.CreateIndex(
                name: "IX_EntregasRecepcion_ContratoId",
                table: "EntregasRecepcion",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimacionConceptos_ConceptoContratoId",
                table: "EstimacionConceptos",
                column: "ConceptoContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimacionConceptos_EstimacionId",
                table: "EstimacionConceptos",
                column: "EstimacionId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimacionDocumentos_EstimacionId",
                table: "EstimacionDocumentos",
                column: "EstimacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Estimaciones_ContratoId_NumeroCorrelativo",
                table: "Estimaciones",
                columns: new[] { "ContratoId", "NumeroCorrelativo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estimaciones_UsuarioEnvioId",
                table: "Estimaciones",
                column: "UsuarioEnvioId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimacionHistoriales_EstimacionId",
                table: "EstimacionHistoriales",
                column: "EstimacionId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimacionHistoriales_UsuarioId",
                table: "EstimacionHistoriales",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimacionNotasBitacora_EstimacionId",
                table: "EstimacionNotasBitacora",
                column: "EstimacionId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimacionNotasBitacora_NotaBitacoraId",
                table: "EstimacionNotasBitacora",
                column: "NotaBitacoraId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimacionObservaciones_EstimacionId",
                table: "EstimacionObservaciones",
                column: "EstimacionId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimacionObservaciones_UsuarioId",
                table: "EstimacionObservaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimacionPagos_EstimacionId",
                table: "EstimacionPagos",
                column: "EstimacionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Finiquitos_ContratoId",
                table: "Finiquitos",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Garantias_ContratoId",
                table: "Garantias",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramaObraItems_ContratoId",
                table: "ProgramaObraItems",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioContratos_ContratoId",
                table: "UsuarioContratos",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioContratos_UsuarioId_ContratoId_Rol",
                table: "UsuarioContratos",
                columns: new[] { "UsuarioId", "ContratoId", "Rol" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alertas");

            migrationBuilder.DropTable(
                name: "AvanceEvidencias");

            migrationBuilder.DropTable(
                name: "BitacoraFirmas");

            migrationBuilder.DropTable(
                name: "BitacoraIncidencias");

            migrationBuilder.DropTable(
                name: "BitacoraMinutaParticipantes");

            migrationBuilder.DropTable(
                name: "CaratulasBitacora");

            migrationBuilder.DropTable(
                name: "ConvenioDocumentos");

            migrationBuilder.DropTable(
                name: "ConvenioPromocionesResidencia");

            migrationBuilder.DropTable(
                name: "ConvenioResolucionesDependencia");

            migrationBuilder.DropTable(
                name: "ConvenioRevisionesSupervision");

            migrationBuilder.DropTable(
                name: "DocumentosContrato");

            migrationBuilder.DropTable(
                name: "EntregaRecepcionEvidencias");

            migrationBuilder.DropTable(
                name: "EstimacionConceptos");

            migrationBuilder.DropTable(
                name: "EstimacionDocumentos");

            migrationBuilder.DropTable(
                name: "EstimacionHistoriales");

            migrationBuilder.DropTable(
                name: "EstimacionNotasBitacora");

            migrationBuilder.DropTable(
                name: "EstimacionObservaciones");

            migrationBuilder.DropTable(
                name: "EstimacionPagos");

            migrationBuilder.DropTable(
                name: "Finiquitos");

            migrationBuilder.DropTable(
                name: "Garantias");

            migrationBuilder.DropTable(
                name: "ProgramaObraItems");

            migrationBuilder.DropTable(
                name: "UsuarioContratos");

            migrationBuilder.DropTable(
                name: "AvancesDiarios");

            migrationBuilder.DropTable(
                name: "BitacoraMinutas");

            migrationBuilder.DropTable(
                name: "ConveniosModificatorios");

            migrationBuilder.DropTable(
                name: "EntregasRecepcion");

            migrationBuilder.DropTable(
                name: "BitacoraNotas");

            migrationBuilder.DropTable(
                name: "Estimaciones");

            migrationBuilder.DropTable(
                name: "ConceptosContrato");

            migrationBuilder.DropTable(
                name: "BitacoraTiposNota");

            migrationBuilder.DropTable(
                name: "Bitacoras");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Contratos");
        }
    }
}
