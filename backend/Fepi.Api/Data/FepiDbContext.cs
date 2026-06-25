using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Data;

public class FepiDbContext : DbContext
{
    public FepiDbContext(DbContextOptions<FepiDbContext> options)
        : base(options)
    {
    }

    // =====================
    // CONTRATOS
    // =====================
    public DbSet<Contrato> Contratos => Set<Contrato>();
    public DbSet<ConceptoContrato> ConceptosContrato => Set<ConceptoContrato>();
    public DbSet<ProgramaObraItem> ProgramaObraItems => Set<ProgramaObraItem>();
    public DbSet<Garantia> Garantias => Set<Garantia>();
    public DbSet<DocumentoContrato> DocumentosContrato => Set<DocumentoContrato>();

    // =====================
    // USUARIOS
    // =====================
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<UsuarioContrato> UsuarioContratos => Set<UsuarioContrato>();

    // =====================
    // ESTIMACIONES
    // =====================
    public DbSet<Estimacion> Estimaciones => Set<Estimacion>();
    public DbSet<EstimacionConcepto> EstimacionConceptos => Set<EstimacionConcepto>();
    public DbSet<EstimacionDocumento> EstimacionDocumentos => Set<EstimacionDocumento>();
    public DbSet<EstimacionNotaBitacora> EstimacionNotasBitacora => Set<EstimacionNotaBitacora>();
    public DbSet<EstimacionObservacion> EstimacionObservaciones => Set<EstimacionObservacion>();
    public DbSet<EstimacionHistorial> EstimacionHistoriales => Set<EstimacionHistorial>();
    public DbSet<EstimacionPago> EstimacionPagos => Set<EstimacionPago>();

    // =====================
    // AVANCE
    // =====================
    public DbSet<AvanceDiario> AvancesDiarios => Set<AvanceDiario>();
    public DbSet<AvanceEvidencia> AvanceEvidencias => Set<AvanceEvidencia>();

    // =====================
    // BITÁCORA
    // =====================
    public DbSet<Bitacora> Bitacoras => Set<Bitacora>();
    public DbSet<CaratulaBitacora> CaratulasBitacora => Set<CaratulaBitacora>();
    public DbSet<BitacoraNota> BitacoraNotas => Set<BitacoraNota>();
    public DbSet<BitacoraFirma> BitacoraFirmas => Set<BitacoraFirma>();
    public DbSet<BitacoraMinuta> BitacoraMinutas => Set<BitacoraMinuta>();
    public DbSet<BitacoraMinutaParticipante> BitacoraMinutaParticipantes => Set<BitacoraMinutaParticipante>();
    public DbSet<BitacoraIncidencia> BitacoraIncidencias => Set<BitacoraIncidencia>();
    public DbSet<BitacoraTipoNota> BitacoraTiposNota { get; set; }

    // =====================
    // CONVENIOS
    // =====================
    public DbSet<ConvenioModificatorio> ConveniosModificatorios => Set<ConvenioModificatorio>();
    public DbSet<ConvenioDocumento> ConvenioDocumentos => Set<ConvenioDocumento>();
    public DbSet<ConvenioRevisionSupervision> ConvenioRevisionesSupervision => Set<ConvenioRevisionSupervision>();
    public DbSet<ConvenioPromocionResidencia> ConvenioPromocionesResidencia => Set<ConvenioPromocionResidencia>();
    public DbSet<ConvenioResolucionDependencia> ConvenioResolucionesDependencia => Set<ConvenioResolucionDependencia>();

    // =====================
    // ENTREGA / FINIQUITO
    // =====================
    public DbSet<EntregaRecepcion> EntregasRecepcion => Set<EntregaRecepcion>();
    public DbSet<EntregaRecepcionEvidencia> EntregaRecepcionEvidencias => Set<EntregaRecepcionEvidencia>();
    public DbSet<Finiquito> Finiquitos => Set<Finiquito>();

    // =====================
    // ALERTAS
    // =====================
    public DbSet<Alerta> Alertas => Set<Alerta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================
        // CONTRATO
        // =====================
        modelBuilder.Entity<Contrato>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.NumeroContrato)
                .IsUnique();

            entity.Property(x => x.NumeroContrato)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(x => x.MontoContratado)
                .HasPrecision(18, 2);

            entity.Property(x => x.DependenciaContratante)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.ContratistaEmpresa)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.ContratistaRepresentante)
                .HasMaxLength(200)
                .IsRequired();
        });

        modelBuilder.Entity<ConceptoContrato>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Clave)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Descripcion)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(x => x.UnidadMedida)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.CantidadContratada)
                .HasPrecision(18, 4);

            entity.Property(x => x.PrecioUnitario)
                .HasPrecision(18, 2);

            entity.Property(x => x.Importe)
                .HasPrecision(18, 2);

            entity.HasOne(x => x.Contrato)
                .WithMany(x => x.ConceptoContratos)
                .HasForeignKey(x => x.ContratoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProgramaObraItem>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Periodo)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(x => x.PorcentajeProgramado)
                .HasPrecision(5, 2);

            entity.Property(x => x.MontoProgramado)
                .HasPrecision(18, 2);

            entity.HasOne(x => x.Contrato)
                .WithMany(x => x.ProgramaObra)
                .HasForeignKey(x => x.ContratoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Garantia>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Monto)
                .HasPrecision(18, 2);

            entity.Property(x => x.Porcentaje)
                .HasPrecision(5, 2);

            entity.HasOne(x => x.Contrato)
                .WithMany(x => x.Garantias)
                .HasForeignKey(x => x.ContratoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DocumentoContrato>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Nombre)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.TipoDocumento)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(x => x.UrlArchivo)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasOne(x => x.Contrato)
                .WithMany(x => x.Documentos)
                .HasForeignKey(x => x.ContratoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================
        // USUARIOS
        // =====================
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Correo)
                .IsUnique();

            entity.Property(x => x.Nombre)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Correo)
                .HasMaxLength(150)
                .IsRequired();
        });

        modelBuilder.Entity<UsuarioContrato>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.UsuarioId, x.ContratoId, x.Rol })
                .IsUnique();

            entity.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Contrato)
                .WithMany(x => x.Usuarios)
                .HasForeignKey(x => x.ContratoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================
        // ESTIMACIONES
        // =====================
        modelBuilder.Entity<Estimacion>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.ContratoId, x.NumeroCorrelativo })
                .IsUnique();

            entity.Property(x => x.Periodo)
                .HasMaxLength(80)
                .IsRequired();

            entity.HasOne(x => x.Contrato)
                .WithMany()
                .HasForeignKey(x => x.ContratoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.UsuarioEnvio)
                .WithMany()
                .HasForeignKey(x => x.UsuarioEnvioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EstimacionConcepto>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.CantidadEjecutada)
                .HasPrecision(18, 4);

            entity.Property(x => x.Importe)
                .HasPrecision(18, 2);

            entity.HasOne(x => x.Estimacion)
                .WithMany(x => x.Conceptos)
                .HasForeignKey(x => x.EstimacionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ConceptoContrato)
                .WithMany()
                .HasForeignKey(x => x.ConceptoContratoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EstimacionDocumento>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Nombre)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.TipoDocumento)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(x => x.UrlArchivo)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasOne(x => x.Estimacion)
                .WithMany(x => x.Documentos)
                .HasForeignKey(x => x.EstimacionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EstimacionNotaBitacora>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Estimacion)
                .WithMany(x => x.NotasVinculadas)
                .HasForeignKey(x => x.EstimacionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.NotaBitacora)
                .WithMany()
                .HasForeignKey(x => x.NotaBitacoraId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EstimacionObservacion>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Texto)
                .HasMaxLength(1000)
                .IsRequired();

            entity.HasOne(x => x.Estimacion)
                .WithMany(x => x.Observaciones)
                .HasForeignKey(x => x.EstimacionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EstimacionHistorial>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Comentario)
                .HasMaxLength(1000);

            entity.HasOne(x => x.Estimacion)
                .WithMany(x => x.Historial)
                .HasForeignKey(x => x.EstimacionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EstimacionPago>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.EstimacionId)
                .IsUnique();

            entity.Property(x => x.ReferenciaBancaria)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.MontoPagado)
                .HasPrecision(18, 2);

            entity.HasOne(x => x.Estimacion)
                .WithOne(x => x.Pago)
                .HasForeignKey<EstimacionPago>(x => x.EstimacionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================
        // AVANCE
        // =====================
        modelBuilder.Entity<AvanceDiario>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.CantidadEjecutada)
                .HasPrecision(18, 4);

            entity.Property(x => x.DescripcionActividad)
                .HasMaxLength(1000)
                .IsRequired();

            entity.HasOne(x => x.Contrato)
                .WithMany()
                .HasForeignKey(x => x.ContratoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ConceptoContrato)
                .WithMany()
                .HasForeignKey(x => x.ConceptoContratoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Actor)
                .WithMany()
                .HasForeignKey(x => x.ActorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AvanceEvidencia>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.UrlFoto)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasOne(x => x.AvanceDiario)
                .WithMany(x => x.Evidencias)
                .HasForeignKey(x => x.AvanceDiarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================
        // BITÁCORA
        // =====================
        modelBuilder.Entity<Bitacora>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.ContratoId)
                .IsUnique();

            entity.HasOne(x => x.Contrato)
                .WithMany()
                .HasForeignKey(x => x.ContratoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CaratulaBitacora>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.BitacoraId)
                .IsUnique();

            entity.HasIndex(x => x.FolioBitacora)
                .IsUnique();

            entity.Property(x => x.FolioBitacora)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(x => x.NombreContrato)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(x => x.NumeroContrato)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(x => x.Dependencia)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Contratista)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Residente)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Supervisor)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Superintendente)
                .HasMaxLength(150)
                .IsRequired();

            entity.HasOne(x => x.Bitacora)
                .WithOne(x => x.Caratula)
                .HasForeignKey<CaratulaBitacora>(x => x.BitacoraId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BitacoraNota>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.BitacoraId, x.Folio })
                .IsUnique();

            entity.Property(x => x.Asunto)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(x => x.Contenido)
                .HasMaxLength(4000)
                .IsRequired();

            entity.HasOne(x => x.Bitacora)
                .WithMany(x => x.Notas)
                .HasForeignKey(x => x.BitacoraId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.TipoNotaCatalogo)
                .WithMany()
                .HasForeignKey(x => x.TipoNotaCatalogoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.FolioVinculado)
                .WithMany()
                .HasForeignKey(x => x.FolioVinculadoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BitacoraFirma>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.BitacoraNota)
                .WithMany(x => x.Firmas)
                .HasForeignKey(x => x.BitacoraNotaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BitacoraMinuta>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Lugar)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.ContenidoAcuerdos)
                .HasMaxLength(4000)
                .IsRequired();

            entity.HasOne(x => x.Bitacora)
                .WithMany(x => x.Minutas)
                .HasForeignKey(x => x.BitacoraId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BitacoraMinutaParticipante>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.NombreParticipante)
                .HasMaxLength(150)
                .IsRequired();

            entity.HasOne(x => x.BitacoraMinuta)
                .WithMany(x => x.Participantes)
                .HasForeignKey(x => x.BitacoraMinutaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BitacoraIncidencia>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Descripcion)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(x => x.UrlFotografia)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasOne(x => x.Bitacora)
                .WithMany(x => x.Incidencias)
                .HasForeignKey(x => x.BitacoraId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ActorRegistro)
                .WithMany()
                .HasForeignKey(x => x.ActorRegistroId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.NotaGenerada)
                .WithMany()
                .HasForeignKey(x => x.NotaGeneradaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<BitacoraTipoNota>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Codigo)
                .IsUnique();

            entity.Property(x => x.Nombre)
                .HasMaxLength(150)
                .IsRequired();
        });

        // =====================
        // CONVENIOS
        // =====================
        modelBuilder.Entity<ConvenioModificatorio>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Justificacion)
                .HasMaxLength(2000)
                .IsRequired();

            entity.Property(x => x.MontoSolicitado)
                .HasPrecision(18, 2);

            entity.Property(x => x.VariacionAcumuladaPorcentaje)
                .HasPrecision(5, 2);

            entity.HasOne(x => x.Contrato)
                .WithMany()
                .HasForeignKey(x => x.ContratoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Solicitante)
                .WithMany()
                .HasForeignKey(x => x.SolicitanteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ConvenioDocumento>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Nombre)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.UrlArchivo)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasOne(x => x.ConvenioModificatorio)
                .WithMany(x => x.Documentos)
                .HasForeignKey(x => x.ConvenioModificatorioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ConvenioRevisionSupervision>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.ConvenioModificatorioId)
                .IsUnique();

            entity.Property(x => x.Justificacion)
                .HasMaxLength(2000)
                .IsRequired();

            entity.HasOne(x => x.ConvenioModificatorio)
                .WithOne(x => x.RevisionSupervision)
                .HasForeignKey<ConvenioRevisionSupervision>(x => x.ConvenioModificatorioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Supervisor)
                .WithMany()
                .HasForeignKey(x => x.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ConvenioPromocionResidencia>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.ConvenioModificatorioId)
                .IsUnique();

            entity.HasOne(x => x.ConvenioModificatorio)
                .WithOne(x => x.PromocionResidencia)
                .HasForeignKey<ConvenioPromocionResidencia>(x => x.ConvenioModificatorioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Residente)
                .WithMany()
                .HasForeignKey(x => x.ResidenteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ConvenioResolucionDependencia>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.ConvenioModificatorioId)
                .IsUnique();

            entity.Property(x => x.MotivoRechazo)
                .HasMaxLength(1000);

            entity.HasOne(x => x.ConvenioModificatorio)
                .WithOne(x => x.ResolucionDependencia)
                .HasForeignKey<ConvenioResolucionDependencia>(x => x.ConvenioModificatorioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.UsuarioDependencia)
                .WithMany()
                .HasForeignKey(x => x.UsuarioDependenciaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================
        // ENTREGA / FINIQUITO
        // =====================
        modelBuilder.Entity<EntregaRecepcion>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.EstadoObraDescripcion)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(x => x.EstadoGarantiasDescripcion)
                .HasMaxLength(1000)
                .IsRequired();

            entity.HasOne(x => x.Contrato)
                .WithMany()
                .HasForeignKey(x => x.ContratoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EntregaRecepcionEvidencia>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.UrlArchivo)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasOne(x => x.EntregaRecepcion)
                .WithMany(x => x.Evidencias)
                .HasForeignKey(x => x.EntregaRecepcionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Finiquito>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.TotalPagado)
                .HasPrecision(18, 2);

            entity.Property(x => x.TotalPendiente)
                .HasPrecision(18, 2);

            entity.Property(x => x.TotalDeductivas)
                .HasPrecision(18, 2);

            entity.Property(x => x.TotalRetenciones)
                .HasPrecision(18, 2);

            entity.Ignore(x => x.MontoFinal);

            entity.Property(x => x.UrlReporteFiniquito)
                .HasMaxLength(500);

            entity.HasOne(x => x.Contrato)
                .WithMany()
                .HasForeignKey(x => x.ContratoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================
        // ALERTAS
        // =====================
        modelBuilder.Entity<Alerta>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.EntidadReferenciaTipo)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Mensaje)
                .HasMaxLength(1000)
                .IsRequired();

            entity.HasOne(x => x.Contrato)
                .WithMany()
                .HasForeignKey(x => x.ContratoId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}