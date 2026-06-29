using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FepiDbContext>();

        await context.Database.MigrateAsync();

        if (await context.Contratos.AnyAsync())
            return;

        await SeedUsuariosAsync(context);
        await SeedEmpresasAsync(context);
        await SeedContratosAsync(context);
        await SeedCatalogoProgramaGarantiasAsync(context);
        await SeedEstimacionesAsync(context);
        await SeedBitacoraYConveniosAsync(context);
        await SeedCierreAsync(context);
        await SeedAlertasUsuarioAsync(context);
    }

    // ── Usuarios ────────────────────────────────────────────────────────────

    private static async Task SeedUsuariosAsync(FepiDbContext context)
    {
        var usuarios = new List<Usuario>
        {
            new() { Nombre = "Administrador FEPI",            Correo = "admin@fepi.test",           Username = "admin",       Password = "admin123",      Rol = RolUsuario.Admin,           Activo = true },
            new() { Nombre = "Ing. Ana Herrera",              Correo = "residente@fepi.test",       Username = "aherrera",    Password = "residente123",  Rol = RolUsuario.Residente,       Activo = true },
            new() { Nombre = "Ing. Luis Martínez",            Correo = "supervisor@fepi.test",      Username = "lmartinez",   Password = "supervisor123", Rol = RolUsuario.Supervision,     Activo = true },
            new() { Nombre = "Ing. Raúl Domínguez",           Correo = "superintendente@fepi.test", Username = "rdominguez",  Password = "super123",      Rol = RolUsuario.Superintendente, Activo = true },
            new() { Nombre = "María González",                Correo = "financiera@fepi.test",      Username = "mgonzalez",   Password = "finanzas123",   Rol = RolUsuario.Finanzas,        Activo = true },
            new() { Nombre = "Dirección de Obras Públicas",   Correo = "dependencia@fepi.test",     Username = "dependencia", Password = "dep123",        Rol = RolUsuario.Admin,           Activo = true }
        };
        context.Usuarios.AddRange(usuarios);
        await context.SaveChangesAsync();
    }

    // ── Empresas ────────────────────────────────────────────────────────────

    private static async Task SeedEmpresasAsync(FepiDbContext context)
    {
        var superintendente = await context.Usuarios.FirstAsync(u => u.Correo == "superintendente@fepi.test");
        context.Empresas.AddRange(
            new Empresa { Nombre = "Constructora Domínguez S.A. de C.V.",    RepresentanteUsuarioId = superintendente.Id },
            new Empresa { Nombre = "Infraestructura del Centro S.A. de C.V.", RepresentanteUsuarioId = superintendente.Id },
            new Empresa { Nombre = "Supervisión Técnica Integral S.C.",        RepresentanteUsuarioId = superintendente.Id }
        );
        await context.SaveChangesAsync();
    }

    // ── Contratos ────────────────────────────────────────────────────────────

    private static async Task SeedContratosAsync(FepiDbContext context)
    {
        var empresas = await context.Empresas.OrderBy(e => e.Id).ToListAsync();
        var (emp1, emp2, emp3) = (empresas[0], empresas[1], empresas[2]);

        var usuarios = await context.Usuarios.ToListAsync();
        var admin        = usuarios.First(x => x.Correo == "admin@fepi.test");
        var residente    = usuarios.First(x => x.Correo == "residente@fepi.test");
        var supervisor   = usuarios.First(x => x.Correo == "supervisor@fepi.test");
        var super        = usuarios.First(x => x.Correo == "superintendente@fepi.test");
        var financiera   = usuarios.First(x => x.Correo == "financiera@fepi.test");
        var dependencia  = usuarios.First(x => x.Correo == "dependencia@fepi.test");

        decimal m1 = 11_000_000m, m2 = 8_500_000m, m3 = 3_200_000m;

        var contratos = new List<Contrato>
        {
            MakeContrato("CT-2026-014", TipoContrato.ObraPublica,           emp1.Id, m1, new DateOnly(2026,3,1),  new DateOnly(2026,9,30),  TipoPeriodoEstimacion.Mensual,    7,  "SOPOT Edo. de México",                   residente.Id, supervisor.Id, super.Id, financiera.Id),
            MakeContrato("CT-2026-021", TipoContrato.ObraPublica,           emp2.Id, m2, new DateOnly(2026,2,15), new DateOnly(2026,8,15),  TipoPeriodoEstimacion.Mensual,    6,  "SOPOT Edo. de México",                   residente.Id, supervisor.Id, super.Id, financiera.Id),
            MakeContrato("CT-2026-032", TipoContrato.ServiciosRelacionados, emp3.Id, m3, new DateOnly(2026,1,10), new DateOnly(2026,7,10),  TipoPeriodoEstimacion.Quincenal, 12,  "Secretaría de Infraestructura Municipal", residente.Id, supervisor.Id, super.Id, financiera.Id),
        };
        context.Contratos.AddRange(contratos);
        await context.SaveChangesAsync();

        var relaciones = new List<UsuarioContrato>();
        foreach (var c in contratos)
        {
            relaciones.AddRange(new[]
            {
                new UsuarioContrato { UsuarioId = admin.Id,       ContratoId = c.Id, Rol = RolSistema.Administrador    },
                new UsuarioContrato { UsuarioId = residente.Id,   ContratoId = c.Id, Rol = RolSistema.Residencia        },
                new UsuarioContrato { UsuarioId = supervisor.Id,  ContratoId = c.Id, Rol = RolSistema.SupervisorExterno },
                new UsuarioContrato { UsuarioId = super.Id,       ContratoId = c.Id, Rol = RolSistema.Superintendente   },
                new UsuarioContrato { UsuarioId = financiera.Id,  ContratoId = c.Id, Rol = RolSistema.AreaFinanciera    },
                new UsuarioContrato { UsuarioId = dependencia.Id, ContratoId = c.Id, Rol = RolSistema.Dependencia       },
            });
        }
        context.UsuarioContratos.AddRange(relaciones);
        await context.SaveChangesAsync();
    }

    private static Contrato MakeContrato(
        string num, TipoContrato tipo, int empId, decimal monto,
        DateOnly inicio, DateOnly termino, TipoPeriodoEstimacion periodo, int nPeriodos, string dep,
        int residenteId, int? supervisorId = null, int? superintendenteId = null, int? financieroId = null)
    {
        const decimal iva = 16m;
        var sinIva = Math.Round(monto / (1 + iva / 100m), 2);
        return new()
        {
            NumeroContrato              = num,
            Tipo                        = tipo,
            EmpresaId                   = empId,
            IvaPorcentaje               = iva,
            ImporteSinIVA               = sinIva,
            IVA                         = Math.Round(sinIva * iva / 100m, 2),
            ImporteTotal                = monto,
            ModalidadPago               = ModalidadPago.PrecioUnitario,
            PorcentajeAnticipo          = 10m,
            MontoAnticipo               = Math.Round(monto * 0.10m, 2),
            FechaInicio                 = inicio,
            FechaTermino                = termino,
            TipoPeriodo                 = periodo,
            NumeroPeriodos              = nPeriodos,
            DependenciaContratante      = dep,
            ResidenteUsuarioId          = residenteId,
            SupervisorExternoUsuarioId  = supervisorId,
            SuperintendenteUsuarioId    = superintendenteId,
            FinancialUsuarioId          = financieroId,
            Estado                      = EstadoContrato.Activo,
        };
    }

    // ── Catálogo: conceptos, programa de obra, garantías ────────────────────

    private static async Task SeedCatalogoProgramaGarantiasAsync(FepiDbContext context)
    {
        var contratos = await context.Contratos.ToListAsync();

        foreach (var contrato in contratos)
        {
            context.ConceptosContrato.AddRange(
                new ConceptoContrato { ContratoId = contrato.Id, Clave = "01.001", Descripcion = "Limpieza y trazo del terreno",             UnidadMedida = "m²",  CantidadContratada = 1200m, PrecioUnitario = 45m,   Activo = true },
                new ConceptoContrato { ContratoId = contrato.Id, Clave = "02.001", Descripcion = "Excavación manual en material tipo II",     UnidadMedida = "m³",  CantidadContratada = 450m,  PrecioUnitario = 320m,  Activo = true },
                new ConceptoContrato { ContratoId = contrato.Id, Clave = "03.012", Descripcion = "Losa de cimentación f'c=250 kg/cm²",        UnidadMedida = "m³",  CantidadContratada = 300m,  PrecioUnitario = 4050m, Activo = true },
                new ConceptoContrato { ContratoId = contrato.Id, Clave = "05.004", Descripcion = "Trabe de liga eje B-C",                     UnidadMedida = "ml",  CantidadContratada = 180m,  PrecioUnitario = 6185m, Activo = true },
                new ConceptoContrato { ContratoId = contrato.Id, Clave = "06.001", Descripcion = "Castillo K-1 armado y colado",              UnidadMedida = "pza", CantidadContratada = 90m,   PrecioUnitario = 5300m, Activo = true },
                new ConceptoContrato { ContratoId = contrato.Id, Clave = "07.002", Descripcion = "Muro de block hueco 15 cm",                 UnidadMedida = "m²",  CantidadContratada = 850m,  PrecioUnitario = 1903m, Activo = true }
            );

            context.ProgramaObraItems.AddRange(
                new ProgramaObraItem { ContratoId = contrato.Id, Periodo = "2026-03", PorcentajeProgramado = 8m,  MontoProgramado = contrato.ImporteTotal * 0.08m },
                new ProgramaObraItem { ContratoId = contrato.Id, Periodo = "2026-04", PorcentajeProgramado = 16m, MontoProgramado = contrato.ImporteTotal * 0.16m },
                new ProgramaObraItem { ContratoId = contrato.Id, Periodo = "2026-05", PorcentajeProgramado = 21m, MontoProgramado = contrato.ImporteTotal * 0.21m },
                new ProgramaObraItem { ContratoId = contrato.Id, Periodo = "2026-06", PorcentajeProgramado = 23m, MontoProgramado = contrato.ImporteTotal * 0.23m },
                new ProgramaObraItem { ContratoId = contrato.Id, Periodo = "2026-07", PorcentajeProgramado = 16m, MontoProgramado = contrato.ImporteTotal * 0.16m },
                new ProgramaObraItem { ContratoId = contrato.Id, Periodo = "2026-08", PorcentajeProgramado = 10m, MontoProgramado = contrato.ImporteTotal * 0.10m },
                new ProgramaObraItem { ContratoId = contrato.Id, Periodo = "2026-09", PorcentajeProgramado = 6m,  MontoProgramado = contrato.ImporteTotal * 0.06m }
            );

            context.Garantias.AddRange(
                new Garantia { ContratoId = contrato.Id, Tipo = TipoGarantia.Cumplimiento,   Monto = contrato.ImporteTotal * 0.10m, Porcentaje = 10m, Vigencia = contrato.FechaTermino,             Estado = EstadoGarantia.Vigente },
                new Garantia { ContratoId = contrato.Id, Tipo = TipoGarantia.Anticipo,        Monto = contrato.ImporteTotal * 0.10m, Porcentaje = 10m, Vigencia = contrato.FechaTermino.AddMonths(-2), Estado = EstadoGarantia.Vigente },
                new Garantia { ContratoId = contrato.Id, Tipo = TipoGarantia.ViciosOcultos,   Monto = contrato.ImporteTotal * 0.05m, Porcentaje = 5m,  Vigencia = contrato.FechaTermino.AddMonths(6),  Estado = EstadoGarantia.Vigente }
            );
        }

        await context.SaveChangesAsync();
    }

    // ── Estimaciones ─────────────────────────────────────────────────────────
    //
    // Contrato 1 (idx 0): EST-3=Enviada, EST-4=Borrador   → para probar flujo de aprobación
    // Contrato 2 (idx 1): todas AprobadaResidencia         → listo para iniciar-cierre
    // Contrato 3 (idx 2): EST-3=Enviada, EST-4=Borrador   → para probar flujo de aprobación

    private static async Task SeedEstimacionesAsync(FepiDbContext context)
    {
        var contratos = await context.Contratos
            .Include(c => c.ConceptoContratos)
            .OrderBy(c => c.Id)
            .ToListAsync();

        var usuarios     = await context.Usuarios.ToListAsync();
        var super        = usuarios.First(x => x.Correo == "superintendente@fepi.test");
        var supervisor   = usuarios.First(x => x.Correo == "supervisor@fepi.test");
        var financiera   = usuarios.First(x => x.Correo == "financiera@fepi.test");
        var residente    = usuarios.First(x => x.Correo == "residente@fepi.test");

        for (int idx = 0; idx < contratos.Count; idx++)
        {
            var contrato        = contratos[idx];
            bool finiquitoReady = idx == 1; // CT-2026-021: todas las estimaciones cerradas

            var conceptos    = contrato.ConceptoContratos.ToList();
            if (conceptos.Count == 0) continue;

            var cLosa  = conceptos.First(c => c.Clave == "03.012");
            var cTrabe = conceptos.First(c => c.Clave == "05.004");
            var cMuro  = conceptos.First(c => c.Clave == "07.002");

            // ── Avances diarios (todos los contratos iguales) ──────────────
            var avances = new List<AvanceDiario>
            {
                new() { ContratoId = contrato.Id, ConceptoContratoId = cLosa.Id,  ActorId = supervisor.Id, Fecha = new DateOnly(2026,3,12), CantidadEjecutada = 18m, DescripcionActividad = "Inicio de trabajos de cimentación y limpieza de área." },
                new() { ContratoId = contrato.Id, ConceptoContratoId = cLosa.Id,  ActorId = supervisor.Id, Fecha = new DateOnly(2026,4,10), CantidadEjecutada = 36m, DescripcionActividad = "Colado parcial de losa de cimentación." },
                new() { ContratoId = contrato.Id, ConceptoContratoId = cTrabe.Id, ActorId = supervisor.Id, Fecha = new DateOnly(2026,5,15), CantidadEjecutada = 22m, DescripcionActividad = "Armado y colado de trabe de liga." },
                new() { ContratoId = contrato.Id, ConceptoContratoId = cMuro.Id,  ActorId = supervisor.Id, Fecha = new DateOnly(2026,6,18), CantidadEjecutada = 95m, DescripcionActividad = "Avance en muro de block hueco de 15 cm." },
            };
            avances[0].Evidencias.Add(new AvanceEvidencia { UrlFoto = "/evidencias/avance-cimentacion-01.jpg" });
            avances[1].Evidencias.Add(new AvanceEvidencia { UrlFoto = "/evidencias/avance-cimentacion-02.jpg" });
            avances[2].Evidencias.Add(new AvanceEvidencia { UrlFoto = "/evidencias/avance-trabe-01.jpg" });
            avances[3].Evidencias.Add(new AvanceEvidencia { UrlFoto = "/evidencias/avance-muro-01.jpg" });
            context.AvancesDiarios.AddRange(avances);

            // ── EST-1: Marzo — AprobadaResidencia, Pagada (todos) ──────────
            decimal cantMarzo = 18m, importeMarzo = cantMarzo * cLosa.PrecioUnitario;
            var est1 = new Estimacion
            {
                ContratoId = contrato.Id, NumeroEstimacion = 1, Periodo = "2026-03",
                Estado = EstadoEstimacion.AprobadaResidencia, EstadoPago = EstadoPagoEstimacion.Pagada,
                MontoPagadoAcumulado = importeMarzo,
                FechaEnvio = new DateTime(2026,3,25,10,0,0,DateTimeKind.Utc), UsuarioEnvioId = super.Id,
                FechaAprobacionSupervision = new DateTime(2026,3,27,9,0,0,DateTimeKind.Utc), UsuarioAprobacionSupervisionId = supervisor.Id,
                FechaAprobacionResidencia  = new DateTime(2026,3,28,11,0,0,DateTimeKind.Utc), UsuarioAprobacionResidenciaId  = residente.Id,
            };
            est1.Conceptos.Add(new EstimacionConcepto { ConceptoContratoId = cLosa.Id, CantidadEjecutadaPeriodo = cantMarzo, PrecioUnitarioActual = cLosa.PrecioUnitario, ImporteTotal = importeMarzo, CantidadAcumuladaAnterior = 0m, CantidadAcumuladaActual = cantMarzo, CantidadPorEjecutar = cLosa.CantidadContratada - cantMarzo });
            est1.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.Borrador,           EstadoNuevo = EstadoEstimacion.Enviada,            Fecha = new DateTime(2026,3,25,10,0,0,DateTimeKind.Utc), UsuarioId = super.Id,     Comentario = "Estimación enviada para revisión de supervisión." });
            est1.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.Enviada,            EstadoNuevo = EstadoEstimacion.AprobadaSupervision, Fecha = new DateTime(2026,3,27,9,0,0, DateTimeKind.Utc), UsuarioId = supervisor.Id, Comentario = "Aprobada técnicamente por supervisión." });
            est1.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.AprobadaSupervision, EstadoNuevo = EstadoEstimacion.AprobadaResidencia,  Fecha = new DateTime(2026,3,28,11,0,0,DateTimeKind.Utc), UsuarioId = residente.Id,  Comentario = "Aprobada por residencia para pago." });
            est1.Pagos.Add(new EstimacionPago { FechaPago = new DateOnly(2026,3,30), ReferenciaBancaria = $"CLC-{contrato.Id}-001", MontoPagado = importeMarzo, UsuarioRegistroId = financiera.Id, FechaRegistro = new DateTime(2026,3,30,14,0,0,DateTimeKind.Utc) });

            // ── EST-2: Abril — AprobadaResidencia, PagoParcial (todos) ─────
            decimal cantAbril = 36m, importeAbril = cantAbril * cLosa.PrecioUnitario, pagoAbril = Math.Round(importeAbril * 0.6m, 2);
            var est2 = new Estimacion
            {
                ContratoId = contrato.Id, NumeroEstimacion = 2, Periodo = "2026-04",
                Estado = EstadoEstimacion.AprobadaResidencia, EstadoPago = EstadoPagoEstimacion.PagoParcial,
                MontoPagadoAcumulado = pagoAbril,
                FechaEnvio = new DateTime(2026,4,25,10,0,0,DateTimeKind.Utc), UsuarioEnvioId = super.Id,
                FechaAprobacionSupervision = new DateTime(2026,4,27,10,0,0,DateTimeKind.Utc), UsuarioAprobacionSupervisionId = supervisor.Id,
                FechaAprobacionResidencia  = new DateTime(2026,4,28,12,0,0,DateTimeKind.Utc), UsuarioAprobacionResidenciaId  = residente.Id,
            };
            est2.Conceptos.Add(new EstimacionConcepto { ConceptoContratoId = cLosa.Id, CantidadEjecutadaPeriodo = cantAbril, PrecioUnitarioActual = cLosa.PrecioUnitario, ImporteTotal = importeAbril, CantidadAcumuladaAnterior = cantMarzo, CantidadAcumuladaActual = cantMarzo + cantAbril, CantidadPorEjecutar = cLosa.CantidadContratada - (cantMarzo + cantAbril) });
            est2.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.Borrador,           EstadoNuevo = EstadoEstimacion.Enviada,            Fecha = new DateTime(2026,4,25,10,0,0,DateTimeKind.Utc), UsuarioId = super.Id,     Comentario = "Estimación enviada para revisión." });
            est2.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.Enviada,            EstadoNuevo = EstadoEstimacion.Observada,          Fecha = new DateTime(2026,4,26,9,0,0, DateTimeKind.Utc), UsuarioId = supervisor.Id, Comentario = "Se observa: falta de anexo de generadores firmados." });
            est2.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.Observada,          EstadoNuevo = EstadoEstimacion.Enviada,            Fecha = new DateTime(2026,4,26,16,0,0,DateTimeKind.Utc), UsuarioId = super.Id,     Comentario = "Corregida y re-enviada con generadores adjuntos." });
            est2.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.Enviada,            EstadoNuevo = EstadoEstimacion.AprobadaSupervision, Fecha = new DateTime(2026,4,27,10,0,0,DateTimeKind.Utc), UsuarioId = supervisor.Id, Comentario = "Aprobada por supervisión." });
            est2.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.AprobadaSupervision, EstadoNuevo = EstadoEstimacion.AprobadaResidencia,  Fecha = new DateTime(2026,4,28,12,0,0,DateTimeKind.Utc), UsuarioId = residente.Id,  Comentario = "Aprobada por residencia." });
            est2.Observaciones.Add(new EstimacionObservacion { Texto = "Falta de anexo de generadores firmados para el concepto de losa.", UsuarioId = supervisor.Id });
            est2.Pagos.Add(new EstimacionPago { FechaPago = new DateOnly(2026,5,5), ReferenciaBancaria = $"CLC-{contrato.Id}-002A", MontoPagado = pagoAbril, UsuarioRegistroId = financiera.Id, FechaRegistro = new DateTime(2026,5,5,10,0,0,DateTimeKind.Utc) });

            // ── EST-3: Mayo ─────────────────────────────────────────────────
            decimal cantMayo = 22m, importeMayo = cantMayo * cTrabe.PrecioUnitario;
            Estimacion est3;

            if (finiquitoReady)
            {
                // Contrato 2: AprobadaResidencia, Pagada — sin bloqueos para finiquito
                est3 = new Estimacion
                {
                    ContratoId = contrato.Id, NumeroEstimacion = 3, Periodo = "2026-05",
                    Estado = EstadoEstimacion.AprobadaResidencia, EstadoPago = EstadoPagoEstimacion.Pagada,
                    MontoPagadoAcumulado = importeMayo,
                    FechaEnvio = new DateTime(2026,5,25,10,0,0,DateTimeKind.Utc), UsuarioEnvioId = super.Id,
                    FechaAprobacionSupervision = new DateTime(2026,5,27,9,0,0,DateTimeKind.Utc), UsuarioAprobacionSupervisionId = supervisor.Id,
                    FechaAprobacionResidencia  = new DateTime(2026,5,28,11,0,0,DateTimeKind.Utc), UsuarioAprobacionResidenciaId  = residente.Id,
                };
                est3.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.Borrador,            EstadoNuevo = EstadoEstimacion.Enviada,            Fecha = new DateTime(2026,5,25,10,0,0,DateTimeKind.Utc), UsuarioId = super.Id,     Comentario = "Estimación enviada con documentación completa." });
                est3.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.Enviada,             EstadoNuevo = EstadoEstimacion.AprobadaSupervision, Fecha = new DateTime(2026,5,27,9,0,0, DateTimeKind.Utc), UsuarioId = supervisor.Id, Comentario = "Aprobada técnicamente." });
                est3.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.AprobadaSupervision, EstadoNuevo = EstadoEstimacion.AprobadaResidencia,  Fecha = new DateTime(2026,5,28,11,0,0,DateTimeKind.Utc), UsuarioId = residente.Id,  Comentario = "Aprobada por residencia para pago." });
                est3.Pagos.Add(new EstimacionPago { FechaPago = new DateOnly(2026,6,5), ReferenciaBancaria = $"CLC-{contrato.Id}-003", MontoPagado = importeMayo, UsuarioRegistroId = financiera.Id, FechaRegistro = new DateTime(2026,6,5,10,0,0,DateTimeKind.Utc) });
            }
            else
            {
                // Contratos 1 y 3: Enviada — pendiente de revisión (para probar flujo de aprobación)
                est3 = new Estimacion
                {
                    ContratoId = contrato.Id, NumeroEstimacion = 3, Periodo = "2026-05",
                    Estado = EstadoEstimacion.Enviada,
                    FechaEnvio = new DateTime(2026,5,25,10,0,0,DateTimeKind.Utc), UsuarioEnvioId = super.Id,
                };
                est3.Observaciones.Add(new EstimacionObservacion { Texto = "Falta anexar generador firmado para el concepto de trabe.", UsuarioId = supervisor.Id });
                est3.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.Borrador, EstadoNuevo = EstadoEstimacion.Enviada, Fecha = new DateTime(2026,5,25,10,0,0,DateTimeKind.Utc), UsuarioId = super.Id, Comentario = "Estimación enviada con documentación parcial." });
            }
            est3.Conceptos.Add(new EstimacionConcepto { ConceptoContratoId = cTrabe.Id, CantidadEjecutadaPeriodo = cantMayo, PrecioUnitarioActual = cTrabe.PrecioUnitario, ImporteTotal = importeMayo, CantidadAcumuladaAnterior = 0m, CantidadAcumuladaActual = cantMayo, CantidadPorEjecutar = cTrabe.CantidadContratada - cantMayo });

            // ── EST-4: Junio ─────────────────────────────────────────────────
            decimal cantJunio = 95m, importeJunio = cantJunio * cMuro.PrecioUnitario;
            Estimacion est4;

            if (finiquitoReady)
            {
                // Contrato 2: AprobadaResidencia, SinPago — saldo pendiente va al finiquito
                est4 = new Estimacion
                {
                    ContratoId = contrato.Id, NumeroEstimacion = 4, Periodo = "2026-06",
                    Estado = EstadoEstimacion.AprobadaResidencia, EstadoPago = EstadoPagoEstimacion.SinPago,
                    MontoPagadoAcumulado = 0m,
                    FechaEnvio = new DateTime(2026,6,25,10,0,0,DateTimeKind.Utc), UsuarioEnvioId = super.Id,
                    FechaAprobacionSupervision = new DateTime(2026,6,27,9,0,0,DateTimeKind.Utc), UsuarioAprobacionSupervisionId = supervisor.Id,
                    FechaAprobacionResidencia  = new DateTime(2026,6,28,11,0,0,DateTimeKind.Utc), UsuarioAprobacionResidenciaId  = residente.Id,
                };
                est4.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.Borrador,            EstadoNuevo = EstadoEstimacion.Enviada,            Fecha = new DateTime(2026,6,25,10,0,0,DateTimeKind.Utc), UsuarioId = super.Id,     Comentario = "Estimación enviada." });
                est4.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.Enviada,             EstadoNuevo = EstadoEstimacion.AprobadaSupervision, Fecha = new DateTime(2026,6,27,9,0,0, DateTimeKind.Utc), UsuarioId = supervisor.Id, Comentario = "Aprobada técnicamente." });
                est4.Historial.Add(new EstimacionHistorial { EstadoAnterior = EstadoEstimacion.AprobadaSupervision, EstadoNuevo = EstadoEstimacion.AprobadaResidencia,  Fecha = new DateTime(2026,6,28,11,0,0,DateTimeKind.Utc), UsuarioId = residente.Id,  Comentario = "Aprobada. Saldo se incluirá en finiquito." });
            }
            else
            {
                // Contratos 1 y 3: Borrador — para probar flujo de envío y aprobación
                est4 = new Estimacion
                {
                    ContratoId = contrato.Id, NumeroEstimacion = 4, Periodo = "2026-06",
                    Estado = EstadoEstimacion.Borrador,
                };
            }
            est4.Conceptos.Add(new EstimacionConcepto { ConceptoContratoId = cMuro.Id, CantidadEjecutadaPeriodo = cantJunio, PrecioUnitarioActual = cMuro.PrecioUnitario, ImporteTotal = importeJunio, CantidadAcumuladaAnterior = 0m, CantidadAcumuladaActual = cantJunio, CantidadPorEjecutar = cMuro.CantidadContratada - cantJunio });

            context.Estimaciones.AddRange(est1, est2, est3, est4);
        }

        await context.SaveChangesAsync();
    }

    // ── Bitácora, convenios y alertas del sistema ────────────────────────────
    //
    // Contrato 2 (idx 1): convenioPlazo → Aplicado (sin bloqueos para finiquito)
    // Contratos 1 y 3:    convenioPlazo → AprobadoResidencia (pendiente resolución)
    //
    // Bug fix: las notas se guardan antes de crear las alertas,
    //          así EntidadReferenciaId usa el ID real de la nota.

    private static async Task SeedBitacoraYConveniosAsync(FepiDbContext context)
    {
        var contratos = await context.Contratos.Include(c => c.Empresa).OrderBy(c => c.Id).ToListAsync();
        var usuarios  = await context.Usuarios.ToListAsync();

        var residente   = usuarios.First(x => x.Correo == "residente@fepi.test");
        var supervisor  = usuarios.First(x => x.Correo == "supervisor@fepi.test");
        var super       = usuarios.First(x => x.Correo == "superintendente@fepi.test");
        var dependencia = usuarios.First(x => x.Correo == "dependencia@fepi.test");

        if (!await context.BitacoraTiposNota.AnyAsync())
        {
            context.BitacoraTiposNota.AddRange(
                new BitacoraTipoNota { Codigo = "APERTURA",    Nombre = "Apertura de bitácora" },
                new BitacoraTipoNota { Codigo = "AVANCE",      Nombre = "Avance de obra" },
                new BitacoraTipoNota { Codigo = "OBSERVACION", Nombre = "Observación de supervisión" },
                new BitacoraTipoNota { Codigo = "INCIDENCIA",  Nombre = "Incidencia" }
            );
            await context.SaveChangesAsync();
        }

        var tipoApertura    = await context.BitacoraTiposNota.FirstAsync(x => x.Codigo == "APERTURA");
        var tipoAvance      = await context.BitacoraTiposNota.FirstAsync(x => x.Codigo == "AVANCE");
        var tipoObservacion = await context.BitacoraTiposNota.FirstAsync(x => x.Codigo == "OBSERVACION");
        var tipoIncidencia  = await context.BitacoraTiposNota.FirstAsync(x => x.Codigo == "INCIDENCIA");

        for (int idx = 0; idx < contratos.Count; idx++)
        {
            var contrato        = contratos[idx];
            bool finiquitoReady = idx == 1; // CT-2026-021: convenio plazo resuelto

            // ── Bitácora ────────────────────────────────────────────────────
            var bitacora = new Bitacora { ContratoId = contrato.Id };
            context.Bitacoras.Add(bitacora);
            await context.SaveChangesAsync();

            var caratula = new CaratulaBitacora
            {
                BitacoraId              = bitacora.Id,
                FolioBitacora           = $"BEOP-{contrato.NumeroContrato}-2026",
                NombreContrato          = $"Bitácora electrónica del contrato {contrato.NumeroContrato}",
                NumeroContrato          = contrato.NumeroContrato,
                TipoContrato            = contrato.Tipo,
                Dependencia             = contrato.DependenciaContratante,
                Contratista             = contrato.Empresa?.Nombre ?? "",
                Residente               = residente.Nombre,
                Supervisor              = supervisor.Nombre,
                Superintendente         = super.Nombre,
                MontoContratadoConIVA   = contrato.ImporteTotal,
                MontoContratadoSinIVA   = contrato.ImporteSinIVA,
                FechaInicio             = contrato.FechaInicio,
                FechaTerminoProgramada  = contrato.FechaTermino,
                ResidenteObraUsuarioId  = residente.Id,
                SupervisorObraUsuarioId = supervisor.Id,
                SuperintendenteUsuarioId = super.Id,
                Abierta                 = true,
                FechaApertura           = contrato.FechaInicio.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            };
            context.CaratulasBitacora.Add(caratula);

            // ── Notas ───────────────────────────────────────────────────────
            // Se guardan antes de crear alertas para obtener IDs reales.

            var notaApertura = new BitacoraNota
            {
                BitacoraId = bitacora.Id, Folio = 1, TipoRegistro = TipoNotaBitacora.Apertura,
                TipoNotaCatalogoId = tipoApertura.Id,
                Asunto = "Apertura formal de bitácora",
                Contenido = $"Se realiza la apertura formal de la bitácora electrónica correspondiente al contrato {contrato.NumeroContrato}.",
                FechaRegistro = contrato.FechaInicio.ToDateTime(new TimeOnly(9,0), DateTimeKind.Utc),
                AutorUsuarioId = residente.Id, CantidadFirmasRequeridas = 3,
                EstadoFirma = EstadoFirmaNota.Completa,
            };
            notaApertura.Firmas.Add(new BitacoraFirma { UsuarioId = residente.Id,  RolFirmante = RolSistema.Residencia,       EsEmisor = true,  Firmado = true, FechaFirma = contrato.FechaInicio.ToDateTime(new TimeOnly(9,10), DateTimeKind.Utc), OrdenFirma = 1 });
            notaApertura.Firmas.Add(new BitacoraFirma { UsuarioId = supervisor.Id, RolFirmante = RolSistema.SupervisorExterno, EsEmisor = false, Firmado = true, FechaFirma = contrato.FechaInicio.ToDateTime(new TimeOnly(9,15), DateTimeKind.Utc), OrdenFirma = 2 });
            notaApertura.Firmas.Add(new BitacoraFirma { UsuarioId = super.Id,      RolFirmante = RolSistema.Superintendente,   EsEmisor = false, Firmado = true, FechaFirma = contrato.FechaInicio.ToDateTime(new TimeOnly(9,20), DateTimeKind.Utc), OrdenFirma = 3 });

            var notaAvance = new BitacoraNota
            {
                BitacoraId = bitacora.Id, Folio = 2, TipoRegistro = TipoNotaBitacora.Nota,
                TipoNotaCatalogoId = tipoAvance.Id,
                Asunto = "Registro de avance de cimentación",
                Contenido = "Se reporta avance en trabajos de cimentación y preparación de elementos estructurales.",
                FechaRegistro = contrato.FechaInicio.AddMonths(1).ToDateTime(new TimeOnly(11,0), DateTimeKind.Utc),
                AutorUsuarioId = super.Id, CantidadFirmasRequeridas = 2,
                EstadoFirma = EstadoFirmaNota.Pendiente,
            };
            notaAvance.Firmas.Add(new BitacoraFirma { UsuarioId = super.Id,      RolFirmante = RolSistema.Superintendente,   EsEmisor = true,  Firmado = true,  FechaFirma = contrato.FechaInicio.AddMonths(1).ToDateTime(new TimeOnly(11,5), DateTimeKind.Utc), OrdenFirma = 1 });
            notaAvance.Firmas.Add(new BitacoraFirma { UsuarioId = supervisor.Id, RolFirmante = RolSistema.SupervisorExterno, EsEmisor = false, Firmado = false,                                                                                                    OrdenFirma = 2 });

            var notaObservacion = new BitacoraNota
            {
                BitacoraId = bitacora.Id, Folio = 3, TipoRegistro = TipoNotaBitacora.Nota,
                TipoNotaCatalogoId = tipoObservacion.Id,
                Asunto = "Observación sobre generadores",
                Contenido = "Se solicita revisar la consistencia entre los volúmenes ejecutados y los generadores presentados.",
                FechaRegistro = contrato.FechaInicio.AddMonths(2).ToDateTime(new TimeOnly(12,30), DateTimeKind.Utc),
                AutorUsuarioId = supervisor.Id, CantidadFirmasRequeridas = 2,
                EstadoFirma = EstadoFirmaNota.Pendiente,
            };
            notaObservacion.Firmas.Add(new BitacoraFirma { UsuarioId = supervisor.Id, RolFirmante = RolSistema.SupervisorExterno, EsEmisor = true,  Firmado = true,  FechaFirma = contrato.FechaInicio.AddMonths(2).ToDateTime(new TimeOnly(12,35), DateTimeKind.Utc), OrdenFirma = 1 });
            notaObservacion.Firmas.Add(new BitacoraFirma { UsuarioId = super.Id,      RolFirmante = RolSistema.Superintendente,   EsEmisor = false, Firmado = false,                                                                                                       OrdenFirma = 2 });

            context.BitacoraNotas.AddRange(notaApertura, notaAvance, notaObservacion);
            await context.SaveChangesAsync(); // IDs reales asignados aquí

            // ── Minuta e incidencia ─────────────────────────────────────────
            var minuta = new BitacoraMinuta
            {
                BitacoraId = bitacora.Id, Fecha = contrato.FechaInicio.AddMonths(2),
                Lugar = "Sala de juntas de obra",
                ContenidoAcuerdos = "Se acuerda reforzar la entrega de evidencia fotográfica y actualizar el programa de obra.",
            };
            minuta.Participantes.Add(new BitacoraMinutaParticipante { NombreParticipante = residente.Nombre });
            minuta.Participantes.Add(new BitacoraMinutaParticipante { NombreParticipante = supervisor.Nombre });
            minuta.Participantes.Add(new BitacoraMinutaParticipante { NombreParticipante = super.Nombre });
            context.BitacoraMinutas.Add(minuta);

            context.BitacoraIncidencias.Add(new BitacoraIncidencia
            {
                BitacoraId = bitacora.Id, FechaEvento = contrato.FechaInicio.AddMonths(3),
                Descripcion = "Se registró una interrupción temporal por falla eléctrica en el área de trabajo.",
                UrlFotografia = "/evidencias/incidencia-electrica.jpg", ActorRegistroId = supervisor.Id,
            });

            // ── Convenios ───────────────────────────────────────────────────
            var convenioMonto = new ConvenioModificatorio
            {
                ContratoId = contrato.Id, Tipo = TipoConvenio.ModificacionMonto,
                Justificacion = "Incremento de volúmenes por condiciones físicas no previstas en excavación.",
                Estado = EstadoConvenio.Aplicado, MontoSolicitado = contrato.ImporteTotal * 0.05m,
                VariacionAcumuladaPorcentaje = 5m, SolicitanteId = super.Id,
            };
            convenioMonto.Documentos.Add(new ConvenioDocumento { Nombre = "justificacion-tecnica.pdf", UrlArchivo = "/documentos/convenio-monto.pdf" });
            convenioMonto.RevisionSupervision = new ConvenioRevisionSupervision { Decision = DictamenTecnico.Procedente, Justificacion = "La documentación técnica justifica los volúmenes adicionales.", SupervisorId = supervisor.Id, Fecha = DateTime.UtcNow.AddDays(-20) };
            convenioMonto.PromocionResidencia = new ConvenioPromocionResidencia { ResidenteId = residente.Id, Fecha = DateTime.UtcNow.AddDays(-15) };
            convenioMonto.ResolucionDependencia = new ConvenioResolucionDependencia { Aprobado = true, UsuarioDependenciaId = dependencia.Id, Fecha = DateTime.UtcNow.AddDays(-10) };

            var convenioPlazo = new ConvenioModificatorio
            {
                ContratoId = contrato.Id, Tipo = TipoConvenio.AmpliacionPlazo,
                Justificacion = "Condiciones climatológicas atípicas retrasaron actividades críticas.",
                Estado = finiquitoReady ? EstadoConvenio.Aplicado : EstadoConvenio.AprobadoResidencia,
                PlazoDiasSolicitado = 20, VariacionAcumuladaPorcentaje = 5m, SolicitanteId = super.Id,
            };
            convenioPlazo.Documentos.Add(new ConvenioDocumento { Nombre = "evidencia-climatologica.pdf", UrlArchivo = "/documentos/convenio-plazo.pdf" });
            convenioPlazo.RevisionSupervision = new ConvenioRevisionSupervision { Decision = DictamenTecnico.Procedente, Justificacion = "Se acredita la afectación de actividades críticas.", SupervisorId = supervisor.Id, Fecha = DateTime.UtcNow.AddDays(-8) };
            convenioPlazo.PromocionResidencia = new ConvenioPromocionResidencia { ResidenteId = residente.Id, Fecha = DateTime.UtcNow.AddDays(-5) };
            if (finiquitoReady)
                convenioPlazo.ResolucionDependencia = new ConvenioResolucionDependencia { Aprobado = true, UsuarioDependenciaId = dependencia.Id, Fecha = DateTime.UtcNow.AddDays(-3) };

            context.ConveniosModificatorios.AddRange(convenioMonto, convenioPlazo);

            // ── Alertas del sistema (legado) — usando IDs reales de notas ──
            context.Alertas.Add(new Alerta
            {
                ContratoId = contrato.Id, Tipo = TipoAlerta.BitacoraFirmaPendiente,
                EntidadReferenciaId = notaAvance.Id, EntidadReferenciaTipo = nameof(BitacoraNota),
                RolDestino = RolSistema.SupervisorExterno,
                Mensaje = $"Nota de bitácora folio {notaAvance.Folio} pendiente de firma en contrato {contrato.NumeroContrato}.",
                Estado = EstadoAlerta.Activa,
            });

            if (!finiquitoReady)
            {
                context.Alertas.Add(new Alerta
                {
                    ContratoId = contrato.Id, Tipo = TipoAlerta.ConvenioPendienteResolucion,
                    EntidadReferenciaId = 0, EntidadReferenciaTipo = nameof(ConvenioModificatorio),
                    RolDestino = RolSistema.Dependencia,
                    Mensaje = $"Convenio de plazo pendiente de resolución en contrato {contrato.NumeroContrato}.",
                    Estado = EstadoAlerta.Activa,
                });
            }

            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
            var garantias = await context.Garantias
                .Where(g => g.ContratoId == contrato.Id && g.Estado == EstadoGarantia.Vigente
                         && g.Vigencia.DayNumber >= hoy.DayNumber
                         && g.Vigencia.DayNumber - hoy.DayNumber <= 30)
                .ToListAsync();
            foreach (var g in garantias)
            {
                context.Alertas.Add(new Alerta
                {
                    ContratoId = contrato.Id, Tipo = TipoAlerta.GarantiaPorVencer,
                    EntidadReferenciaId = g.Id, EntidadReferenciaTipo = nameof(Garantia),
                    RolDestino = RolSistema.Administrador,
                    Mensaje = $"La garantía {g.Tipo} del contrato {contrato.NumeroContrato} vence el {g.Vigencia:dd/MM/yyyy}.",
                    Estado = EstadoAlerta.Activa,
                });
            }

            await context.SaveChangesAsync();
        }
    }

    // ── Cierre y finiquito legacy (Contrato 1) ────────────────────────────────

    private static async Task SeedCierreAsync(FepiDbContext context)
    {
        var contratos = await context.Contratos.OrderBy(c => c.Id).ToListAsync();
        var contratoCerrado = contratos.FirstOrDefault();
        if (contratoCerrado is null) return;

        if (!await context.EntregasRecepcion.AnyAsync(e => e.ContratoId == contratoCerrado.Id))
        {
            var entrega = new EntregaRecepcion
            {
                ContratoId = contratoCerrado.Id,
                FechaEntrega = new DateOnly(2026,9,25),
                EstadoObraDescripcion = "La obra se recibe con trabajos principales concluidos y observaciones menores documentadas.",
                EstadoGarantiasDescripcion = "Las garantías de cumplimiento y vicios ocultos se encuentran vigentes al momento de la entrega.",
            };
            entrega.Evidencias.Add(new EntregaRecepcionEvidencia { UrlArchivo = "/evidencias/entrega-recepcion-acta.pdf" });
            entrega.Evidencias.Add(new EntregaRecepcionEvidencia { UrlArchivo = "/evidencias/entrega-recepcion-fotos.zip" });
            context.EntregasRecepcion.Add(entrega);
        }

        if (!await context.Finiquitos.AnyAsync(f => f.ContratoId == contratoCerrado.Id))
        {
            var totalPagado = await context.EstimacionPagos
                .Where(p => p.Estimacion != null && p.Estimacion.ContratoId == contratoCerrado.Id && p.Estimacion.Estado == EstadoEstimacion.AprobadaResidencia)
                .SumAsync(p => p.MontoPagado);

            var totalPendiente = await context.EstimacionConceptos
                .Where(c => c.Estimacion != null && c.Estimacion.ContratoId == contratoCerrado.Id && c.Estimacion.Estado == EstadoEstimacion.AprobadaResidencia)
                .SumAsync(c => c.ImporteTotal);

            context.Finiquitos.Add(new Finiquito
            {
                ContratoId = contratoCerrado.Id,
                TotalPagado = totalPagado, TotalPendiente = totalPendiente,
                TotalDeductivas = 15_000m, TotalRetenciones = 20_000m,
                UrlReporteFiniquito = $"/reportes/finiquito-{contratoCerrado.Id}.pdf",
                FechaEmision = DateTime.UtcNow,
            });
        }

        // Alertas de estimaciones enviadas pendientes de revisión
        var estimacionesEnviadas = await context.Estimaciones
            .Where(e => e.ContratoId == contratoCerrado.Id && e.Estado == EstadoEstimacion.Enviada)
            .ToListAsync();

        foreach (var est in estimacionesEnviadas)
        {
            var existe = await context.Alertas.AnyAsync(a =>
                a.ContratoId == contratoCerrado.Id &&
                a.Tipo == TipoAlerta.EstimacionPlazoRevision &&
                a.EntidadReferenciaId == est.Id &&
                a.Estado == EstadoAlerta.Activa);

            if (!existe)
            {
                context.Alertas.Add(new Alerta
                {
                    ContratoId = contratoCerrado.Id, Tipo = TipoAlerta.EstimacionPlazoRevision,
                    EntidadReferenciaId = est.Id, EntidadReferenciaTipo = nameof(Estimacion),
                    RolDestino = RolSistema.SupervisorExterno,
                    Mensaje = $"La estimación {est.NumeroEstimacion} del contrato {contratoCerrado.NumeroContrato} está pendiente de revisión.",
                    Estado = EstadoAlerta.Activa,
                });
            }
        }

        if (contratoCerrado.Estado != EstadoContrato.Terminado)
            contratoCerrado.Estado = EstadoContrato.Terminado;

        await context.SaveChangesAsync();
    }

    // ── AlertasUsuario ────────────────────────────────────────────────────────
    //
    // Demonstra el nuevo modelo AlertaUsuario (por usuario, no por rol).
    // Se usa contrato 3 para alertas activas de flujos pendientes,
    // y contrato 2 para notificar que está listo para iniciar cierre.

    private static async Task SeedAlertasUsuarioAsync(FepiDbContext context)
    {
        var usuarios     = await context.Usuarios.ToListAsync();
        var admin        = usuarios.First(x => x.Correo == "admin@fepi.test");
        var residente    = usuarios.First(x => x.Correo == "residente@fepi.test");
        var supervisor   = usuarios.First(x => x.Correo == "supervisor@fepi.test");
        var super        = usuarios.First(x => x.Correo == "superintendente@fepi.test");

        var contratos  = await context.Contratos.OrderBy(c => c.Id).ToListAsync();
        var contrato2  = contratos[1]; // CT-2026-021 — listo para finiquito
        var contrato3  = contratos[2]; // CT-2026-032 — flujos activos

        var bitacora3 = await context.Bitacoras.FirstAsync(b => b.ContratoId == contrato3.Id);
        var notasC3   = await context.BitacoraNotas
            .Where(n => n.BitacoraId == bitacora3.Id)
            .OrderBy(n => n.Folio)
            .ToListAsync();
        var notaAvanceC3      = notasC3.FirstOrDefault(n => n.Folio == 2);
        var notaObservacionC3 = notasC3.FirstOrDefault(n => n.Folio == 3);

        var estimEnviadaC3 = await context.Estimaciones
            .FirstOrDefaultAsync(e => e.ContratoId == contrato3.Id && e.Estado == EstadoEstimacion.Enviada);

        var convenioPlazoC3 = await context.ConveniosModificatorios
            .FirstOrDefaultAsync(c => c.ContratoId == contrato3.Id && c.Tipo == TipoConvenio.AmpliacionPlazo);

        var alertas = new List<AlertaUsuario>
        {
            // Admin: contrato 2 listo para cierre
            new()
            {
                UsuarioId = admin.Id, ContratoId = contrato2.Id,
                EntidadRelacionada = nameof(Contrato), EntidadId = contrato2.Id,
                Titulo = "Contrato listo para finiquito",
                Mensaje = $"El contrato {contrato2.NumeroContrato} tiene todas sus estimaciones aprobadas y convenios resueltos. Puede iniciar el proceso de cierre.",
                Tipo = TipoAlerta.ContratoPorCerrar, Prioridad = PrioridadAlerta.Alta,
                FechaCreacion = DateTime.UtcNow.AddHours(-1), Leida = false,
            },

            // Residente: convenio pendiente de dependencia en contrato 3
            new()
            {
                UsuarioId = residente.Id, ContratoId = contrato3.Id,
                EntidadRelacionada = nameof(ConvenioModificatorio), EntidadId = convenioPlazoC3?.Id,
                Titulo = "Convenio pendiente de resolución",
                Mensaje = $"El convenio de ampliación de plazo del contrato {contrato3.NumeroContrato} está pendiente de resolución por la dependencia contratante.",
                Tipo = TipoAlerta.ConvenioPendiente, Prioridad = PrioridadAlerta.Media,
                FechaCreacion = DateTime.UtcNow.AddDays(-1), Leida = false,
            },
        };

        // Supervisor: estimación pendiente de revisión en contrato 3
        if (estimEnviadaC3 is not null)
        {
            alertas.Add(new AlertaUsuario
            {
                UsuarioId = supervisor.Id, ContratoId = contrato3.Id,
                EntidadRelacionada = nameof(Estimacion), EntidadId = estimEnviadaC3.Id,
                Titulo = "Estimación pendiente de revisión",
                Mensaje = $"La estimación del periodo {estimEnviadaC3.Periodo} del contrato {contrato3.NumeroContrato} requiere tu revisión técnica.",
                Tipo = TipoAlerta.EstimacionPendiente, Prioridad = PrioridadAlerta.Alta,
                FechaCreacion = DateTime.UtcNow.AddHours(-6), Leida = false,
            });
        }

        // Supervisor: firma pendiente en nota de avance contrato 3
        if (notaAvanceC3 is not null)
        {
            alertas.Add(new AlertaUsuario
            {
                UsuarioId = supervisor.Id, ContratoId = contrato3.Id,
                EntidadRelacionada = nameof(BitacoraNota), EntidadId = notaAvanceC3.Id,
                Titulo = "Nota de bitácora pendiente de firma",
                Mensaje = $"La nota de avance de obra (folio {notaAvanceC3.Folio}) del contrato {contrato3.NumeroContrato} requiere tu firma.",
                Tipo = TipoAlerta.BitacoraFirmaPendiente, Prioridad = PrioridadAlerta.Media,
                FechaCreacion = DateTime.UtcNow.AddDays(-2), Leida = true, FechaLectura = DateTime.UtcNow.AddDays(-1),
            });
        }

        // Superintendente: firma pendiente en nota de observación contrato 3
        if (notaObservacionC3 is not null)
        {
            alertas.Add(new AlertaUsuario
            {
                UsuarioId = super.Id, ContratoId = contrato3.Id,
                EntidadRelacionada = nameof(BitacoraNota), EntidadId = notaObservacionC3.Id,
                Titulo = "Observación pendiente de firma",
                Mensaje = $"La observación de supervisión (folio {notaObservacionC3.Folio}) del contrato {contrato3.NumeroContrato} requiere tu firma.",
                Tipo = TipoAlerta.BitacoraFirmaPendiente, Prioridad = PrioridadAlerta.Media,
                FechaCreacion = DateTime.UtcNow.AddDays(-3), Leida = false,
            });
        }

        context.AlertasUsuario.AddRange(alertas);
        await context.SaveChangesAsync();
    }
}
