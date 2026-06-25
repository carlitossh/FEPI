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
        await SeedContratosAsync(context);
        await SeedCatalogoProgramaGarantiasAsync(context);
        await SeedAvancesEstimacionesAsync(context);
        await SeedBitacoraConveniosAsync(context);
        await SeedCierreAsync(context);
    }

    private static async Task SeedUsuariosAsync(FepiDbContext context)
    {
        var usuarios = new List<Usuario>
        {
            new()
            {
                Nombre = "Administrador FEPI",
                Correo = "admin@fepi.test",
                Activo = true
            },
            new()
            {
                Nombre = "Ing. Ana Herrera",
                Correo = "residente@fepi.test",
                Activo = true
            },
            new()
            {
                Nombre = "Ing. Luis Martínez",
                Correo = "supervisor@fepi.test",
                Activo = true
            },
            new()
            {
                Nombre = "Ing. Raúl Domínguez",
                Correo = "superintendente@fepi.test",
                Activo = true
            },
            new()
            {
                Nombre = "María González",
                Correo = "financiera@fepi.test",
                Activo = true
            },
            new()
            {
                Nombre = "Dirección de Obras Públicas",
                Correo = "dependencia@fepi.test",
                Activo = true
            }
        };

        context.Usuarios.AddRange(usuarios);
        await context.SaveChangesAsync();
    }

    private static async Task SeedContratosAsync(FepiDbContext context)
    {
        var contratos = new List<Contrato>
        {
            new()
            {
                NumeroContrato = "CT-2026-014",
                Tipo = TipoContrato.ObraPublica,
                MontoContratado = 11_000_000m,
                FechaInicio = new DateOnly(2026, 3, 1),
                FechaTermino = new DateOnly(2026, 9, 30),
                PeriodoEstimacion = PeriodoEstimacion.Mensual,
                DependenciaContratante = "SOPOT Edo. de México",
                ContratistaEmpresa = "Constructora Domínguez S.A. de C.V.",
                ContratistaRepresentante = "Ing. Raúl Domínguez",
                Estado = EstadoContrato.Activo
            },
            new()
            {
                NumeroContrato = "CT-2026-021",
                Tipo = TipoContrato.ObraPublica,
                MontoContratado = 8_500_000m,
                FechaInicio = new DateOnly(2026, 2, 15),
                FechaTermino = new DateOnly(2026, 8, 15),
                PeriodoEstimacion = PeriodoEstimacion.Mensual,
                DependenciaContratante = "SOPOT Edo. de México",
                ContratistaEmpresa = "Infraestructura del Centro S.A. de C.V.",
                ContratistaRepresentante = "Ing. Carlos Ríos",
                Estado = EstadoContrato.Activo
            },
            new()
            {
                NumeroContrato = "CT-2026-032",
                Tipo = TipoContrato.ServiciosRelacionados,
                MontoContratado = 3_200_000m,
                FechaInicio = new DateOnly(2026, 1, 10),
                FechaTermino = new DateOnly(2026, 7, 10),
                PeriodoEstimacion = PeriodoEstimacion.Quincenal,
                DependenciaContratante = "Secretaría de Infraestructura Municipal",
                ContratistaEmpresa = "Supervisión Técnica Integral S.C.",
                ContratistaRepresentante = "Arq. Pamela Torres",
                Estado = EstadoContrato.Activo
            }
        };

        context.Contratos.AddRange(contratos);
        await context.SaveChangesAsync();

        var usuarios = await context.Usuarios.ToListAsync();

        var admin = usuarios.First(x => x.Correo == "admin@fepi.test");
        var residente = usuarios.First(x => x.Correo == "residente@fepi.test");
        var supervisor = usuarios.First(x => x.Correo == "supervisor@fepi.test");
        var superintendente = usuarios.First(x => x.Correo == "superintendente@fepi.test");
        var financiera = usuarios.First(x => x.Correo == "financiera@fepi.test");
        var dependencia = usuarios.First(x => x.Correo == "dependencia@fepi.test");

        var relaciones = new List<UsuarioContrato>();

        foreach (var contrato in contratos)
        {
            relaciones.AddRange(new[]
            {
                new UsuarioContrato
                {
                    UsuarioId = admin.Id,
                    ContratoId = contrato.Id,
                    Rol = RolSistema.Administrador
                },
                new UsuarioContrato
                {
                    UsuarioId = residente.Id,
                    ContratoId = contrato.Id,
                    Rol = RolSistema.Residencia
                },
                new UsuarioContrato
                {
                    UsuarioId = supervisor.Id,
                    ContratoId = contrato.Id,
                    Rol = RolSistema.SupervisorExterno
                },
                new UsuarioContrato
                {
                    UsuarioId = superintendente.Id,
                    ContratoId = contrato.Id,
                    Rol = RolSistema.Superintendente
                },
                new UsuarioContrato
                {
                    UsuarioId = financiera.Id,
                    ContratoId = contrato.Id,
                    Rol = RolSistema.AreaFinanciera
                },
                new UsuarioContrato
                {
                    UsuarioId = dependencia.Id,
                    ContratoId = contrato.Id,
                    Rol = RolSistema.Dependencia
                }
            });
        }

        context.UsuarioContratos.AddRange(relaciones);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCatalogoProgramaGarantiasAsync(FepiDbContext context)
{
    var contratos = await context.Contratos.ToListAsync();

    foreach (var contrato in contratos)
    {
        var conceptos = new List<ConceptoContrato>
        {
            new()
            {
                ContratoId = contrato.Id,
                Clave = "01.001",
                Descripcion = "Limpieza y trazo del terreno",
                UnidadMedida = "m²",
                CantidadContratada = 1200m,
                PrecioUnitario = 45m,
                Importe = 1200m * 45m
            },
            new()
            {
                ContratoId = contrato.Id,
                Clave = "02.001",
                Descripcion = "Excavación manual en material tipo II",
                UnidadMedida = "m³",
                CantidadContratada = 450m,
                PrecioUnitario = 320m,
                Importe = 450m * 320m
            },
            new()
            {
                ContratoId = contrato.Id,
                Clave = "03.012",
                Descripcion = "Losa de cimentación f'c=250 kg/cm²",
                UnidadMedida = "m³",
                CantidadContratada = 300m,
                PrecioUnitario = 4050m,
                Importe = 300m * 4050m
            },
            new()
            {
                ContratoId = contrato.Id,
                Clave = "05.004",
                Descripcion = "Trabe de liga eje B-C",
                UnidadMedida = "ml",
                CantidadContratada = 180m,
                PrecioUnitario = 6185m,
                Importe = 180m * 6185m
            },
            new()
            {
                ContratoId = contrato.Id,
                Clave = "06.001",
                Descripcion = "Castillo K-1 armado y colado",
                UnidadMedida = "pza",
                CantidadContratada = 90m,
                PrecioUnitario = 5300m,
                Importe = 90m * 5300m
            },
            new()
            {
                ContratoId = contrato.Id,
                Clave = "07.002",
                Descripcion = "Muro de block hueco 15 cm",
                UnidadMedida = "m²",
                CantidadContratada = 850m,
                PrecioUnitario = 1903m,
                Importe = 850m * 1903m
            }
        };

        context.ConceptosContrato.AddRange(conceptos);

        var programa = new List<ProgramaObraItem>
        {
            new()
            {
                ContratoId = contrato.Id,
                Periodo = "2026-03",
                PorcentajeProgramado = 8m,
                MontoProgramado = contrato.MontoContratado * 0.08m
            },
            new()
            {
                ContratoId = contrato.Id,
                Periodo = "2026-04",
                PorcentajeProgramado = 16m,
                MontoProgramado = contrato.MontoContratado * 0.16m
            },
            new()
            {
                ContratoId = contrato.Id,
                Periodo = "2026-05",
                PorcentajeProgramado = 21m,
                MontoProgramado = contrato.MontoContratado * 0.21m
            },
            new()
            {
                ContratoId = contrato.Id,
                Periodo = "2026-06",
                PorcentajeProgramado = 23m,
                MontoProgramado = contrato.MontoContratado * 0.23m
            },
            new()
            {
                ContratoId = contrato.Id,
                Periodo = "2026-07",
                PorcentajeProgramado = 16m,
                MontoProgramado = contrato.MontoContratado * 0.16m
            },
            new()
            {
                ContratoId = contrato.Id,
                Periodo = "2026-08",
                PorcentajeProgramado = 10m,
                MontoProgramado = contrato.MontoContratado * 0.10m
            },
            new()
            {
                ContratoId = contrato.Id,
                Periodo = "2026-09",
                PorcentajeProgramado = 6m,
                MontoProgramado = contrato.MontoContratado * 0.06m
            }
        };

        context.ProgramaObraItems.AddRange(programa);

        var garantias = new List<Garantia>
        {
            new()
            {
                ContratoId = contrato.Id,
                Tipo = TipoGarantia.Cumplimiento,
                Monto = contrato.MontoContratado * 0.10m,
                Porcentaje = 10m,
                Vigencia = contrato.FechaTermino,
                Estado = EstadoGarantia.Vigente
            },
            new()
            {
                ContratoId = contrato.Id,
                Tipo = TipoGarantia.Anticipo,
                Monto = contrato.MontoContratado * 0.10m,
                Porcentaje = 10m,
                Vigencia = contrato.FechaTermino.AddMonths(-2),
                Estado = EstadoGarantia.Vigente
            },
            new()
            {
                ContratoId = contrato.Id,
                Tipo = TipoGarantia.ViciosOcultos,
                Monto = contrato.MontoContratado * 0.05m,
                Porcentaje = 5m,
                Vigencia = contrato.FechaTermino.AddMonths(6),
                Estado = EstadoGarantia.Vigente
            }
        };

        context.Garantias.AddRange(garantias);
    }

    await context.SaveChangesAsync();
}

private static async Task SeedAvancesEstimacionesAsync(FepiDbContext context)
{
    var contratos = await context.Contratos
        .Include(c => c.ConceptoContratos)
        .ToListAsync();

    var usuarios = await context.Usuarios.ToListAsync();

    var superintendente = usuarios.First(x => x.Correo == "superintendente@fepi.test");
    var supervisor = usuarios.First(x => x.Correo == "supervisor@fepi.test");
    var financiera = usuarios.First(x => x.Correo == "financiera@fepi.test");

    foreach (var contrato in contratos)
    {
        var conceptos = contrato.ConceptoContratos.ToList();

        if (conceptos.Count == 0)
            continue;

        var conceptoLosa = conceptos.FirstOrDefault(c => c.Clave == "03.012") ?? conceptos[0];
        var conceptoTrabe = conceptos.FirstOrDefault(c => c.Clave == "05.004") ?? conceptos[0];
        var conceptoMuro = conceptos.FirstOrDefault(c => c.Clave == "07.002") ?? conceptos[0];

        var avances = new List<AvanceDiario>
        {
            new()
            {
                ContratoId = contrato.Id,
                ConceptoContratoId = conceptoLosa.Id,
                ActorId = supervisor.Id,
                Fecha = new DateOnly(2026, 3, 12),
                CantidadEjecutada = 18m,
                DescripcionActividad = "Inicio de trabajos de cimentación y limpieza de área."
            },
            new()
            {
                ContratoId = contrato.Id,
                ConceptoContratoId = conceptoLosa.Id,
                ActorId = supervisor.Id,
                Fecha = new DateOnly(2026, 4, 10),
                CantidadEjecutada = 36m,
                DescripcionActividad = "Colado parcial de losa de cimentación."
            },
            new()
            {
                ContratoId = contrato.Id,
                ConceptoContratoId = conceptoTrabe.Id,
                ActorId = supervisor.Id,
                Fecha = new DateOnly(2026, 5, 15),
                CantidadEjecutada = 22m,
                DescripcionActividad = "Armado y colado de trabe de liga."
            },
            new()
            {
                ContratoId = contrato.Id,
                ConceptoContratoId = conceptoMuro.Id,
                ActorId = supervisor.Id,
                Fecha = new DateOnly(2026, 6, 18),
                CantidadEjecutada = 95m,
                DescripcionActividad = "Avance en muro de block hueco de 15 cm."
            }
        };

        avances[0].Evidencias.Add(new AvanceEvidencia
        {
            UrlFoto = "/evidencias/avance-cimentacion-01.jpg"
        });

        avances[1].Evidencias.Add(new AvanceEvidencia
        {
            UrlFoto = "/evidencias/avance-cimentacion-02.jpg"
        });

        avances[2].Evidencias.Add(new AvanceEvidencia
        {
            UrlFoto = "/evidencias/avance-trabe-01.jpg"
        });

        avances[3].Evidencias.Add(new AvanceEvidencia
        {
            UrlFoto = "/evidencias/avance-muro-01.jpg"
        });

        context.AvancesDiarios.AddRange(avances);

        var estimacionMarzo = new Estimacion
        {
            ContratoId = contrato.Id,
            NumeroCorrelativo = 1,
            Periodo = "2026-03",
            Estado = EstadoEstimacion.Pagada,
            FechaEnvio = new DateTime(2026, 3, 25, 10, 0, 0, DateTimeKind.Utc),
            UsuarioEnvioId = superintendente.Id
        };

        estimacionMarzo.Conceptos.Add(new EstimacionConcepto
        {
            ConceptoContratoId = conceptoLosa.Id,
            CantidadEjecutada = 18m,
            Importe = 18m * conceptoLosa.PrecioUnitario
        });

        estimacionMarzo.Historial.Add(new EstimacionHistorial
        {
            EstadoAnterior = EstadoEstimacion.Borrador,
            EstadoNuevo = EstadoEstimacion.Enviada,
            Fecha = new DateTime(2026, 3, 25, 10, 0, 0, DateTimeKind.Utc),
            UsuarioId = superintendente.Id,
            Comentario = "Estimación enviada para revisión."
        });

        estimacionMarzo.Historial.Add(new EstimacionHistorial
        {
            EstadoAnterior = EstadoEstimacion.Aprobada,
            EstadoNuevo = EstadoEstimacion.Pagada,
            Fecha = new DateTime(2026, 3, 30, 14, 0, 0, DateTimeKind.Utc),
            UsuarioId = financiera.Id,
            Comentario = "Pago registrado por área financiera."
        });

        estimacionMarzo.Pago = new EstimacionPago
        {
            FechaPago = new DateOnly(2026, 3, 30),
            ReferenciaBancaria = $"CLC-{contrato.Id}-001",
            MontoPagado = estimacionMarzo.Conceptos.Sum(c => c.Importe)
        };

        var estimacionAbril = new Estimacion
        {
            ContratoId = contrato.Id,
            NumeroCorrelativo = 2,
            Periodo = "2026-04",
            Estado = EstadoEstimacion.Aprobada,
            FechaEnvio = new DateTime(2026, 4, 25, 10, 0, 0, DateTimeKind.Utc),
            UsuarioEnvioId = superintendente.Id
        };

        estimacionAbril.Conceptos.Add(new EstimacionConcepto
        {
            ConceptoContratoId = conceptoLosa.Id,
            CantidadEjecutada = 36m,
            Importe = 36m * conceptoLosa.PrecioUnitario
        });

        estimacionAbril.Historial.Add(new EstimacionHistorial
        {
            EstadoAnterior = EstadoEstimacion.Borrador,
            EstadoNuevo = EstadoEstimacion.Enviada,
            Fecha = new DateTime(2026, 4, 25, 10, 0, 0, DateTimeKind.Utc),
            UsuarioId = superintendente.Id,
            Comentario = "Estimación enviada para revisión."
        });

        estimacionAbril.Historial.Add(new EstimacionHistorial
        {
            EstadoAnterior = EstadoEstimacion.Enviada,
            EstadoNuevo = EstadoEstimacion.Aprobada,
            Fecha = new DateTime(2026, 4, 28, 12, 0, 0, DateTimeKind.Utc),
            UsuarioId = supervisor.Id,
            Comentario = "Estimación aprobada por supervisión."
        });

        var estimacionMayo = new Estimacion
        {
            ContratoId = contrato.Id,
            NumeroCorrelativo = 3,
            Periodo = "2026-05",
            Estado = EstadoEstimacion.Enviada,
            FechaEnvio = new DateTime(2026, 5, 25, 10, 0, 0, DateTimeKind.Utc),
            UsuarioEnvioId = superintendente.Id
        };

        estimacionMayo.Conceptos.Add(new EstimacionConcepto
        {
            ConceptoContratoId = conceptoTrabe.Id,
            CantidadEjecutada = 22m,
            Importe = 22m * conceptoTrabe.PrecioUnitario
        });

        estimacionMayo.Observaciones.Add(new EstimacionObservacion
        {
            Texto = "Falta anexar generador firmado para el concepto de trabe.",
            UsuarioId = supervisor.Id
        });

        estimacionMayo.Historial.Add(new EstimacionHistorial
        {
            EstadoAnterior = EstadoEstimacion.Borrador,
            EstadoNuevo = EstadoEstimacion.Enviada,
            Fecha = new DateTime(2026, 5, 25, 10, 0, 0, DateTimeKind.Utc),
            UsuarioId = superintendente.Id,
            Comentario = "Estimación enviada con documentación parcial."
        });

        var estimacionJunio = new Estimacion
        {
            ContratoId = contrato.Id,
            NumeroCorrelativo = 4,
            Periodo = "2026-06",
            Estado = EstadoEstimacion.Borrador
        };

        estimacionJunio.Conceptos.Add(new EstimacionConcepto
        {
            ConceptoContratoId = conceptoMuro.Id,
            CantidadEjecutada = 95m,
            Importe = 95m * conceptoMuro.PrecioUnitario
        });

        context.Estimaciones.AddRange(
            estimacionMarzo,
            estimacionAbril,
            estimacionMayo,
            estimacionJunio
        );
    }

    await context.SaveChangesAsync();
}

private static async Task SeedBitacoraConveniosAsync(FepiDbContext context)
{
    var contratos = await context.Contratos.ToListAsync();

    var usuarios = await context.Usuarios.ToListAsync();

    var residente = usuarios.First(x => x.Correo == "residente@fepi.test");
    var supervisor = usuarios.First(x => x.Correo == "supervisor@fepi.test");
    var superintendente = usuarios.First(x => x.Correo == "superintendente@fepi.test");
    var dependencia = usuarios.First(x => x.Correo == "dependencia@fepi.test");

    if (!await context.BitacoraTiposNota.AnyAsync())
    {
        context.BitacoraTiposNota.AddRange(
            new BitacoraTipoNota
            {
                Codigo = "APERTURA",
                Nombre = "Apertura de bitácora"
            },
            new BitacoraTipoNota
            {
                Codigo = "AVANCE",
                Nombre = "Avance de obra"
            },
            new BitacoraTipoNota
            {
                Codigo = "OBSERVACION",
                Nombre = "Observación de supervisión"
            },
            new BitacoraTipoNota
            {
                Codigo = "INCIDENCIA",
                Nombre = "Incidencia"
            }
        );

        await context.SaveChangesAsync();
    }

    var tipoApertura = await context.BitacoraTiposNota
        .FirstAsync(x => x.Codigo == "APERTURA");

    var tipoAvance = await context.BitacoraTiposNota
        .FirstAsync(x => x.Codigo == "AVANCE");

    var tipoObservacion = await context.BitacoraTiposNota
        .FirstAsync(x => x.Codigo == "OBSERVACION");

    var tipoIncidencia = await context.BitacoraTiposNota
        .FirstAsync(x => x.Codigo == "INCIDENCIA");

    foreach (var contrato in contratos)
    {
        var bitacora = new Bitacora
        {
            ContratoId = contrato.Id
        };

        context.Bitacoras.Add(bitacora);
        await context.SaveChangesAsync();

        var caratula = new CaratulaBitacora
        {
            BitacoraId = bitacora.Id,
            FolioBitacora = $"BEOP-{contrato.NumeroContrato}-2026",
            NombreContrato = $"Bitácora electrónica del contrato {contrato.NumeroContrato}",
            NumeroContrato = contrato.NumeroContrato,
            TipoContrato = contrato.Tipo,
            Dependencia = contrato.DependenciaContratante,
            Contratista = contrato.ContratistaEmpresa,
            Residente = residente.Nombre,
            Supervisor = supervisor.Nombre,
            Superintendente = superintendente.Nombre,
            FechaApertura = contrato.FechaInicio.ToDateTime(TimeOnly.MinValue)
        };

        context.CaratulasBitacora.Add(caratula);

        var notaApertura = new BitacoraNota
        {
            BitacoraId = bitacora.Id,
            Folio = 1,
            TipoRegistro = TipoNotaBitacora.Apertura,
            TipoNotaCatalogoId = tipoApertura.Id,
            Asunto = "Apertura formal de bitácora",
            Contenido = $"Se realiza la apertura formal de la bitácora electrónica correspondiente al contrato {contrato.NumeroContrato}.",
            FechaRegistro = contrato.FechaInicio.ToDateTime(new TimeOnly(9, 0))
        };

        notaApertura.Firmas.Add(new BitacoraFirma
        {
            UsuarioId = residente.Id,
            RolFirmante = RolSistema.Residencia,
            EsEmisor = true,
            Firmado = true,
            FechaFirma = contrato.FechaInicio.ToDateTime(new TimeOnly(9, 10))
        });

        notaApertura.Firmas.Add(new BitacoraFirma
        {
            UsuarioId = supervisor.Id,
            RolFirmante = RolSistema.SupervisorExterno,
            EsEmisor = false,
            Firmado = true,
            FechaFirma = contrato.FechaInicio.ToDateTime(new TimeOnly(9, 15))
        });

        notaApertura.Firmas.Add(new BitacoraFirma
        {
            UsuarioId = superintendente.Id,
            RolFirmante = RolSistema.Superintendente,
            EsEmisor = false,
            Firmado = true,
            FechaFirma = contrato.FechaInicio.ToDateTime(new TimeOnly(9, 20))
        });

        var notaAvance = new BitacoraNota
        {
            BitacoraId = bitacora.Id,
            Folio = 2,
            TipoRegistro = TipoNotaBitacora.Nota,
            TipoNotaCatalogoId = tipoAvance.Id,
            Asunto = "Registro de avance de cimentación",
            Contenido = "Se reporta avance en trabajos de cimentación y preparación de elementos estructurales.",
            FechaRegistro = contrato.FechaInicio.AddMonths(1).ToDateTime(new TimeOnly(11, 0))
        };

        notaAvance.Firmas.Add(new BitacoraFirma
        {
            UsuarioId = superintendente.Id,
            RolFirmante = RolSistema.Superintendente,
            EsEmisor = true,
            Firmado = true,
            FechaFirma = contrato.FechaInicio.AddMonths(1).ToDateTime(new TimeOnly(11, 5))
        });

        notaAvance.Firmas.Add(new BitacoraFirma
        {
            UsuarioId = supervisor.Id,
            RolFirmante = RolSistema.SupervisorExterno,
            EsEmisor = false,
            Firmado = false
        });

        var notaObservacion = new BitacoraNota
        {
            BitacoraId = bitacora.Id,
            Folio = 3,
            TipoRegistro = TipoNotaBitacora.Nota,
            TipoNotaCatalogoId = tipoObservacion.Id,
            Asunto = "Observación sobre generadores",
            Contenido = "Se solicita revisar la consistencia entre los volúmenes ejecutados y los generadores presentados.",
            FechaRegistro = contrato.FechaInicio.AddMonths(2).ToDateTime(new TimeOnly(12, 30))
        };

        notaObservacion.Firmas.Add(new BitacoraFirma
        {
            UsuarioId = supervisor.Id,
            RolFirmante = RolSistema.SupervisorExterno,
            EsEmisor = true,
            Firmado = true,
            FechaFirma = contrato.FechaInicio.AddMonths(2).ToDateTime(new TimeOnly(12, 35))
        });

        notaObservacion.Firmas.Add(new BitacoraFirma
        {
            UsuarioId = superintendente.Id,
            RolFirmante = RolSistema.Superintendente,
            EsEmisor = false,
            Firmado = false
        });

        context.BitacoraNotas.AddRange(
            notaApertura,
            notaAvance,
            notaObservacion
        );

        var minuta = new BitacoraMinuta
        {
            BitacoraId = bitacora.Id,
            Fecha = contrato.FechaInicio.AddMonths(2),
            Lugar = "Sala de juntas de obra",
            ContenidoAcuerdos = "Se acuerda reforzar la entrega de evidencia fotográfica y actualizar el programa de obra."
        };

        minuta.Participantes.Add(new BitacoraMinutaParticipante
        {
            NombreParticipante = residente.Nombre
        });

        minuta.Participantes.Add(new BitacoraMinutaParticipante
        {
            NombreParticipante = supervisor.Nombre
        });

        minuta.Participantes.Add(new BitacoraMinutaParticipante
        {
            NombreParticipante = superintendente.Nombre
        });

        context.BitacoraMinutas.Add(minuta);

        var incidencia = new BitacoraIncidencia
        {
            BitacoraId = bitacora.Id,
            FechaEvento = contrato.FechaInicio.AddMonths(3),
            Descripcion = "Se registró una interrupción temporal por falla eléctrica en el área de trabajo.",
            UrlFotografia = "/evidencias/incidencia-electrica.jpg",
            ActorRegistroId = supervisor.Id
        };

        context.BitacoraIncidencias.Add(incidencia);

        var convenioMonto = new ConvenioModificatorio
        {
            ContratoId = contrato.Id,
            Tipo = TipoModificacionConvenio.Monto,
            Justificacion = "Incremento de volúmenes por condiciones físicas no previstas en excavación.",
            Estado = EstadoConvenio.Aprobada,
            MontoSolicitado = contrato.MontoContratado * 0.05m,
            PlazoDiasSolicitado = null,
            VariacionAcumuladaPorcentaje = 5m,
            SolicitanteId = superintendente.Id
        };

        convenioMonto.Documentos.Add(new ConvenioDocumento
        {
            Nombre = "justificacion-tecnica.pdf",
            UrlArchivo = "/documentos/convenio-monto.pdf"
        });

        convenioMonto.RevisionSupervision = new ConvenioRevisionSupervision
        {
            Decision = DictamenTecnico.Procedente,
            Justificacion = "La documentación técnica justifica los volúmenes adicionales.",
            SupervisorId = supervisor.Id,
            Fecha = DateTime.UtcNow.AddDays(-20)
        };

        convenioMonto.PromocionResidencia = new ConvenioPromocionResidencia
        {
            ResidenteId = residente.Id,
            Fecha = DateTime.UtcNow.AddDays(-15)
        };

        convenioMonto.ResolucionDependencia = new ConvenioResolucionDependencia
        {
            Aprobado = true,
            MotivoRechazo = null,
            UsuarioDependenciaId = dependencia.Id,
            Fecha = DateTime.UtcNow.AddDays(-10)
        };

        var convenioPlazo = new ConvenioModificatorio
        {
            ContratoId = contrato.Id,
            Tipo = TipoModificacionConvenio.Plazo,
            Justificacion = "Condiciones climatológicas atípicas retrasaron actividades críticas.",
            Estado = EstadoConvenio.PromovidaResidencia,
            MontoSolicitado = null,
            PlazoDiasSolicitado = 20,
            VariacionAcumuladaPorcentaje = 5m,
            SolicitanteId = superintendente.Id
        };

        convenioPlazo.Documentos.Add(new ConvenioDocumento
        {
            Nombre = "evidencia-climatologica.pdf",
            UrlArchivo = "/documentos/convenio-plazo.pdf"
        });

        convenioPlazo.RevisionSupervision = new ConvenioRevisionSupervision
        {
            Decision = DictamenTecnico.Procedente,
            Justificacion = "Se acredita la afectación de actividades críticas.",
            SupervisorId = supervisor.Id,
            Fecha = DateTime.UtcNow.AddDays(-8)
        };

        convenioPlazo.PromocionResidencia = new ConvenioPromocionResidencia
        {
            ResidenteId = residente.Id,
            Fecha = DateTime.UtcNow.AddDays(-5)
        };

        context.ConveniosModificatorios.AddRange(
            convenioMonto,
            convenioPlazo
        );

        context.Alertas.Add(new Alerta
        {
            ContratoId = contrato.Id,
            Tipo = TipoAlerta.BitacoraFirmaPendiente,
            EntidadReferenciaId = 2,
            EntidadReferenciaTipo = nameof(BitacoraNota),
            RolDestino = RolSistema.SupervisorExterno,
            Mensaje = $"Nota de bitácora folio 2 pendiente de firma en contrato {contrato.NumeroContrato}.",
            Estado = EstadoAlerta.Activa
        });

        context.Alertas.Add(new Alerta
        {
            ContratoId = contrato.Id,
            Tipo = TipoAlerta.ConvenioPendienteResolucion,
            EntidadReferenciaId = 0,
            EntidadReferenciaTipo = nameof(ConvenioModificatorio),
            RolDestino = RolSistema.Dependencia,
            Mensaje = $"Convenio de plazo pendiente de resolución en contrato {contrato.NumeroContrato}.",
            Estado = EstadoAlerta.Activa
        });

        await context.SaveChangesAsync();
    }
}

private static async Task SeedCierreAsync(FepiDbContext context)
{
    var contratos = await context.Contratos
        .OrderBy(c => c.Id)
        .ToListAsync();

    var contratoCerrado = contratos.FirstOrDefault();

    if (contratoCerrado is null)
        return;

    var existeEntrega = await context.EntregasRecepcion
        .AnyAsync(e => e.ContratoId == contratoCerrado.Id);

    if (!existeEntrega)
    {
        var entrega = new EntregaRecepcion
        {
            ContratoId = contratoCerrado.Id,
            FechaEntrega = new DateOnly(2026, 9, 25),
            EstadoObraDescripcion = "La obra se recibe con trabajos principales concluidos y observaciones menores documentadas.",
            EstadoGarantiasDescripcion = "Las garantías de cumplimiento y vicios ocultos se encuentran vigentes al momento de la entrega."
        };

        entrega.Evidencias.Add(new EntregaRecepcionEvidencia
        {
            UrlArchivo = "/evidencias/entrega-recepcion-acta.pdf"
        });

        entrega.Evidencias.Add(new EntregaRecepcionEvidencia
        {
            UrlArchivo = "/evidencias/entrega-recepcion-fotos.zip"
        });

        context.EntregasRecepcion.Add(entrega);
    }

    var totalPagado = await context.EstimacionPagos
        .Where(p =>
            p.Estimacion != null &&
            p.Estimacion.ContratoId == contratoCerrado.Id &&
            p.Estimacion.Estado == EstadoEstimacion.Pagada)
        .SumAsync(p => p.MontoPagado);

    var totalPendiente = await context.EstimacionConceptos
        .Where(c =>
            c.Estimacion != null &&
            c.Estimacion.ContratoId == contratoCerrado.Id &&
            c.Estimacion.Estado == EstadoEstimacion.Aprobada)
        .SumAsync(c => c.Importe);

    var existeFiniquito = await context.Finiquitos
        .AnyAsync(f => f.ContratoId == contratoCerrado.Id);

    if (!existeFiniquito)
    {
        context.Finiquitos.Add(new Finiquito
        {
            ContratoId = contratoCerrado.Id,
            TotalPagado = totalPagado,
            TotalPendiente = totalPendiente,
            TotalDeductivas = 15_000m,
            TotalRetenciones = 20_000m,
            UrlReporteFiniquito = $"/reportes/finiquito-{contratoCerrado.Id}.pdf",
            FechaEmision = DateTime.UtcNow
        });
    }

    var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

    foreach (var contrato in contratos)
    {
        var garantiasPorVencer = await context.Garantias
            .Where(g =>
                g.ContratoId == contrato.Id &&
                g.Estado == EstadoGarantia.Vigente &&
                g.Vigencia.DayNumber >= hoy.DayNumber &&
                g.Vigencia.DayNumber - hoy.DayNumber <= 30)
            .ToListAsync();

        foreach (var garantia in garantiasPorVencer)
        {
            var existeAlerta = await context.Alertas.AnyAsync(a =>
                a.ContratoId == contrato.Id &&
                a.Tipo == TipoAlerta.GarantiaPorVencer &&
                a.EntidadReferenciaId == garantia.Id &&
                a.Estado == EstadoAlerta.Activa);

            if (!existeAlerta)
            {
                context.Alertas.Add(new Alerta
                {
                    ContratoId = contrato.Id,
                    Tipo = TipoAlerta.GarantiaPorVencer,
                    EntidadReferenciaId = garantia.Id,
                    EntidadReferenciaTipo = nameof(Garantia),
                    RolDestino = RolSistema.Administrador,
                    Mensaje = $"La garantía {garantia.Tipo} del contrato {contrato.NumeroContrato} vence el {garantia.Vigencia:dd/MM/yyyy}.",
                    Estado = EstadoAlerta.Activa
                });
            }
        }

        var estimacionesEnviadas = await context.Estimaciones
            .Where(e =>
                e.ContratoId == contrato.Id &&
                e.Estado == EstadoEstimacion.Enviada)
            .ToListAsync();

        foreach (var estimacion in estimacionesEnviadas)
        {
            var existeAlerta = await context.Alertas.AnyAsync(a =>
                a.ContratoId == contrato.Id &&
                a.Tipo == TipoAlerta.EstimacionPlazoRevision &&
                a.EntidadReferenciaId == estimacion.Id &&
                a.Estado == EstadoAlerta.Activa);

            if (!existeAlerta)
            {
                context.Alertas.Add(new Alerta
                {
                    ContratoId = contrato.Id,
                    Tipo = TipoAlerta.EstimacionPlazoRevision,
                    EntidadReferenciaId = estimacion.Id,
                    EntidadReferenciaTipo = nameof(Estimacion),
                    RolDestino = RolSistema.SupervisorExterno,
                    Mensaje = $"La estimación {estimacion.NumeroCorrelativo} del contrato {contrato.NumeroContrato} está pendiente de revisión.",
                    Estado = EstadoAlerta.Activa
                });
            }
        }
    }

    if (contratoCerrado.Estado != EstadoContrato.Terminado)
    {
        contratoCerrado.Estado = EstadoContrato.Terminado;
    }

    await context.SaveChangesAsync();
}

}