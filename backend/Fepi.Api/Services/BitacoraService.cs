using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class BitacoraService : IBitacoraService
{
    private readonly IBitacoraNotaRepository _notaRepo;
    private readonly FepiDbContext _context;
    private readonly IAlertaService _alertaService;

    private const int UsuarioSistemaId = 1;

    public BitacoraService(
        IBitacoraNotaRepository notaRepo,
        FepiDbContext context,
        IAlertaService alertaService)
    {
        _notaRepo = notaRepo;
        _context = context;
        _alertaService = alertaService;
    }

    public async Task<CaratulaBitacoraDto> AbrirBitacoraAsync(
        AbrirBitacoraDto dto,
        CancellationToken ct = default)
    {
        var contratoExiste = await _context.Contratos
            .AnyAsync(c => c.Id == dto.ContratoId, ct);

        if (!contratoExiste)
            throw new InvalidOperationException("Contrato no encontrado.");

        var bitacora = await _context.Bitacoras
            .Include(b => b.Caratula)
            .FirstOrDefaultAsync(b => b.ContratoId == dto.ContratoId, ct);

        if (bitacora is null)
        {
            bitacora = new Bitacora
            {
                ContratoId = dto.ContratoId
            };

            _context.Bitacoras.Add(bitacora);
            await _context.SaveChangesAsync(ct);
        }

        if (bitacora.Caratula is not null)
        {
            return new CaratulaBitacoraDto(
                bitacora.Caratula.Id,
                bitacora.Caratula.FolioBitacora,
                true
            );
        }

        var folio = $"BEOP-{dto.NumeroContrato}-{DateTime.UtcNow:yyyy}";

        var caratula = new CaratulaBitacora
        {
            BitacoraId = bitacora.Id,
            FolioBitacora = folio,
            NombreContrato = dto.NombreContrato,
            NumeroContrato = dto.NumeroContrato,
            TipoContrato = Enum.Parse<TipoContrato>(dto.TipoContrato),
            Dependencia = dto.DependenciaContratante,
            Contratista = dto.ContratistaEmpresa,
            Residente = dto.ResidenteNombre,
            Supervisor = dto.SupervisorDesignadoNombre,
            Superintendente = dto.SuperintendenteNombre,
            FechaApertura = dto.FechaAperturaFormal.ToDateTime(TimeOnly.MinValue)
        };

        _context.CaratulasBitacora.Add(caratula);
        await _context.SaveChangesAsync(ct);

        var notaApertura = new BitacoraNota
        {
            BitacoraId = bitacora.Id,
            Folio = 1,
            TipoRegistro = TipoNotaBitacora.Apertura,
            TipoNotaCatalogoId = null,
            Asunto = "Apertura formal de bitácora",
            Contenido = $"Apertura de bitácora del contrato {dto.NumeroContrato} — {dto.NombreContrato}.",
            FechaRegistro = DateTime.UtcNow
        };

        notaApertura.Firmas.Add(new BitacoraFirma
        {
            UsuarioId = UsuarioSistemaId,
            RolFirmante = RolSistema.Residencia,
            EsEmisor = true,
            Firmado = true,
            FechaFirma = DateTime.UtcNow
        });

        notaApertura.Firmas.Add(new BitacoraFirma
        {
            UsuarioId = UsuarioSistemaId,
            RolFirmante = RolSistema.SupervisorExterno,
            EsEmisor = false,
            Firmado = false
        });

        notaApertura.Firmas.Add(new BitacoraFirma
        {
            UsuarioId = UsuarioSistemaId,
            RolFirmante = RolSistema.Superintendente,
            EsEmisor = false,
            Firmado = false
        });

        _context.BitacoraNotas.Add(notaApertura);
        await _context.SaveChangesAsync(ct);

        return new CaratulaBitacoraDto(
            caratula.Id,
            caratula.FolioBitacora,
            true
        );
    }

    public async Task<BitacoraNotaDto> CrearNotaAsync(
        CrearNotaBitacoraDto dto,
        CancellationToken ct = default)
    {
        var bitacora = await _context.Bitacoras
            .FirstOrDefaultAsync(b => b.Id == dto.BitacoraId, ct)
            ?? throw new InvalidOperationException("Bitácora no encontrada.");

        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u => u.Id == dto.UsuarioEmisorId, ct);

        if (!usuarioExiste)
            throw new InvalidOperationException("Usuario emisor no encontrado.");

        var tipoExiste = await _context.BitacoraTiposNota
            .AnyAsync(t => t.Id == dto.TipoNotaCatalogoId, ct);

        if (!tipoExiste)
            throw new InvalidOperationException("Tipo de nota no encontrado.");

        var ultimoFolio = await _notaRepo.GetUltimoFolioAsync(dto.BitacoraId, ct);
        var fechaRegistro = DateTime.UtcNow;

        var nota = new BitacoraNota
        {
            BitacoraId = dto.BitacoraId,
            Folio = ultimoFolio + 1,
            TipoRegistro = TipoNotaBitacora.Nota,
            TipoNotaCatalogoId = dto.TipoNotaCatalogoId,
            Asunto = dto.Asunto,
            Contenido = dto.Contenido,
            FolioVinculadoId = dto.FolioVinculadoId,
            FechaRegistro = fechaRegistro
        };

        var rolesEnterados = Enum.GetValues<RolSistema>()
            .Where(r =>
                r is RolSistema.Residencia
                    or RolSistema.SupervisorExterno
                    or RolSistema.Superintendente)
            .Where(r => r != dto.RolEmisor)
            .ToList();

        nota.Firmas.Add(new BitacoraFirma
        {
            UsuarioId = dto.UsuarioEmisorId,
            RolFirmante = dto.RolEmisor,
            EsEmisor = true,
            Firmado = true,
            FechaFirma = fechaRegistro
        });

        foreach (var rol in rolesEnterados)
        {
            nota.Firmas.Add(new BitacoraFirma
            {
                UsuarioId = dto.UsuarioEmisorId,
                RolFirmante = rol,
                EsEmisor = false,
                Firmado = false
            });
        }

        await _notaRepo.AddAsync(nota, ct);
        await _notaRepo.SaveChangesAsync(ct);

        foreach (var rol in rolesEnterados)
        {
            await _alertaService.EmitirAsync(
                bitacora.ContratoId,
                TipoAlerta.BitacoraFirmaPendiente,
                nota.Id,
                nameof(BitacoraNota),
                rol,
                $"Nota de bitácora folio {nota.Folio} pendiente de firma.",
                ct
            );
        }

        var notaCreada = await _context.BitacoraNotas
    .Include(n => n.Firmas)
    .FirstAsync(n => n.Id == nota.Id, ct);

return MapNotaDto(notaCreada);
    }

    public async Task FirmarNotaAsync(
    int notaId,
    FirmarNotaDto dto,
    CancellationToken ct = default)
{
    var nota = await _context.BitacoraNotas
        .Include(n => n.Firmas)
        .FirstOrDefaultAsync(n => n.Id == notaId, ct)
        ?? throw new InvalidOperationException("Nota no encontrada.");

    var firma = nota.Firmas
        .Where(f => f.RolFirmante == dto.Rol)
        .OrderBy(f => f.Firmado)
        .FirstOrDefault();

    if (firma is null)
        throw new InvalidOperationException($"La nota no tiene firma registrada para el rol {dto.Rol}.");

    if (firma.Firmado)
        return;

    firma.UsuarioId = dto.UsuarioId;
    firma.Firmado = true;
    firma.FechaFirma = DateTime.UtcNow;

    await _context.SaveChangesAsync(ct);

    if (nota.Firmas.All(f => f.Firmado))
    {
        await _alertaService.ResolverAsync(
            TipoAlerta.BitacoraFirmaPendiente,
            nota.Id,
            ct
        );
    }
}
    public async Task<List<BitacoraNotaDto>> BuscarNotasAsync(
        int BitacoraId,
        string? asunto,
        DateOnly? fechaInicio,
        DateOnly? fechaFin,
        int? actorId,
        CancellationToken ct = default)
    {
        var notas = await _notaRepo.BuscarAsync(
            BitacoraId,
            asunto,
            fechaInicio,
            fechaFin,
            actorId,
            ct
        );

        return notas.Select(MapNotaDto).ToList();
    }

    public async Task CrearMinutaAsync(
        CrearMinutaDto dto,
        CancellationToken ct = default)
    {
        var bitacoraExiste = await _context.Bitacoras
            .AnyAsync(b => b.Id == dto.BitacoraId, ct);

        if (!bitacoraExiste)
            throw new InvalidOperationException("Bitácora no encontrada.");

        var minuta = new BitacoraMinuta
        {
            BitacoraId = dto.BitacoraId,
            Fecha = dto.Fecha,
            Lugar = dto.Lugar,
            ContenidoAcuerdos = dto.ContenidoAcuerdos
        };

        foreach (var p in dto.Participantes)
        {
            minuta.Participantes.Add(new BitacoraMinutaParticipante
            {
                NombreParticipante = p
            });
        }

        _context.BitacoraMinutas.Add(minuta);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<int> CrearIncidenciaAsync(
        CrearIncidenciaDto dto,
        CancellationToken ct = default)
    {
        var bitacoraExiste = await _context.Bitacoras
            .AnyAsync(b => b.Id == dto.BitacoraId, ct);

        if (!bitacoraExiste)
            throw new InvalidOperationException("Bitácora no encontrada.");

        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u => u.Id == dto.ActorRegistroId, ct);

        if (!usuarioExiste)
            throw new InvalidOperationException("Usuario actor no encontrado.");

        var incidencia = new BitacoraIncidencia
        {
            BitacoraId = dto.BitacoraId,
            FechaEvento = dto.FechaEvento,
            Descripcion = dto.Descripcion,
            UrlFotografia = dto.UrlFotografia,
            ActorRegistroId = dto.ActorRegistroId
        };

        _context.BitacoraIncidencias.Add(incidencia);
        await _context.SaveChangesAsync(ct);

        return incidencia.Id;
    }

    public async Task<BitacoraNotaDto> GenerarNotaDesdeIncidenciaAsync(
        GenerarNotaDesdeIncidenciaDto dto,
        CancellationToken ct = default)
    {
        var incidencia = await _context.BitacoraIncidencias
            .FirstOrDefaultAsync(i => i.Id == dto.IncidenciaId, ct)
            ?? throw new InvalidOperationException("Incidencia no encontrada.");

        var asunto = incidencia.Descripcion.Length > 80
            ? incidencia.Descripcion[..80]
            : incidencia.Descripcion;

        var notaDto = await CrearNotaAsync(
            new CrearNotaBitacoraDto(
                incidencia.BitacoraId,
                dto.TipoNotaCatalogoId,
                $"Derivado de incidencia: {asunto}",
                incidencia.Descripcion,
                null,
                dto.UsuarioEmisorId,
                dto.RolEmisor
            ),
            ct
        );

        incidencia.NotaGeneradaId = notaDto.Id;
        await _context.SaveChangesAsync(ct);

        return notaDto;
    }

    private static BitacoraNotaDto MapNotaDto(BitacoraNota n) => new(
        n.Id,
        n.Folio,
        n.TipoRegistro,
        n.Asunto,
        n.Contenido,
        n.FechaRegistro,
        n.Cerrada,
        n.Firmas.Select(f => new FirmaDto(
            f.UsuarioId,
            f.RolFirmante,
            f.EsEmisor,
            f.Firmado,
            f.FechaFirma
        )).ToList()
    );
}