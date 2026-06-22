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

    public BitacoraService(IBitacoraNotaRepository notaRepo, FepiDbContext context, IAlertaService alertaService)
    {
        _notaRepo = notaRepo;
        _context = context;
        _alertaService = alertaService;
    }

    public async Task<CaratulaBitacoraDto> AbrirBitacoraAsync(AbrirBitacoraDto dto, CancellationToken ct = default)
    {
        var folio = $"BEOP-{dto.NumeroContrato}-{DateTime.UtcNow:yyyy}";

        var caratula = new CaratulaBitacora
        {
            Id = int.Newint(), ContratoId = dto.ContratoId, FolioBitacora = folio,
            NombreContrato = dto.NombreContrato, NumeroContrato = dto.NumeroContrato, TipoContrato = dto.TipoContrato,
            DependenciaContratante = dto.DependenciaContratante, ContratistaEmpresa = dto.ContratistaEmpresa,
            ResidenteNombre = dto.ResidenteNombre, SupervisorDesignadoNombre = dto.SupervisorDesignadoNombre,
            SuperintendenteNombre = dto.SuperintendenteNombre, FechaAperturaFormal = dto.FechaAperturaFormal, Abierta = true
        };

        _context.CaratulaBitacoras.Add(caratula);
        await _context.SaveChangesAsync(ct);

        var notaApertura = new BitacoraNota
        {
            Id = int.Newint(), BitacoraId = caratula.Id, Folio = 1, TipoRegistro = TipoNotaBitacora.Apertura,
            Asunto = "Apertura formal de bitácora",
            Contenido = $"Apertura de bitácora del contrato {dto.NumeroContrato} — {dto.NombreContrato}."
        };

        notaApertura.Firmas.Add(new BitacoraFirma { Id = int.Newint(), RolFirmante = RolSistema.Residencia, EsEmisor = true, Firmado = true, FechaFirma = DateTime.UtcNow, UsuarioId = int.Empty });
        notaApertura.Firmas.Add(new BitacoraFirma { Id = int.Newint(), RolFirmante = RolSistema.SupervisorExterno, EsEmisor = false, Firmado = false, UsuarioId = int.Empty });
        notaApertura.Firmas.Add(new BitacoraFirma { Id = int.Newint(), RolFirmante = RolSistema.Superintendente, EsEmisor = false, Firmado = false, UsuarioId = int.Empty });

        _context.BitacoraNotas.Add(notaApertura);
        await _context.SaveChangesAsync(ct);

        return new CaratulaBitacoraDto(caratula.Id, caratula.FolioBitacora, caratula.Abierta);
    }

    public async Task<BitacoraNotaDto> CrearNotaAsync(CrearNotaBitacoraDto dto, CancellationToken ct = default)
    {
        var ultimoFolio = await _notaRepo.GetUltimoFolioAsync(dto.BitacoraId, ct);
        var fechaRegistro = DateTime.UtcNow;

        var nota = new BitacoraNota
        {
            Id = int.Newint(), BitacoraId = dto.BitacoraId, Folio = ultimoFolio + 1,
            TipoRegistro = TipoNotaBitacora.Nota, TipoNotaCatalogoId = dto.TipoNotaCatalogoId,
            Asunto = dto.Asunto, Contenido = dto.Contenido, FolioVinculadoId = dto.FolioVinculadoId, FechaRegistro = fechaRegistro
        };

        var rolesEnterados = Enum.GetValues<RolSistema>()
            .Where(r => r is RolSistema.Residencia or RolSistema.SupervisorExterno or RolSistema.Superintendente)
            .Where(r => r != dto.RolEmisor)
            .ToList();

        nota.Firmas.Add(new BitacoraFirma { Id = int.Newint(), UsuarioId = dto.UsuarioEmisorId, RolFirmante = dto.RolEmisor, EsEmisor = true, Firmado = true, FechaFirma = fechaRegistro });

        foreach (var rol in rolesEnterados)
            nota.Firmas.Add(new BitacoraFirma { Id = int.Newint(), UsuarioId = int.Empty, RolFirmante = rol, EsEmisor = false, Firmado = false });

        await _notaRepo.AddAsync(nota, ct);
        await _notaRepo.SaveChangesAsync(ct);

        var caratula = await _context.CaratulaBitacoras.FindAsync(new object[] { dto.BitacoraId }, ct);

        foreach (var rol in rolesEnterados)
            await _alertaService.EmitirAsync(caratula!.ContratoId, TipoAlerta.BitacoraFirmaPendiente, nota.Id,
                nameof(BitacoraNota), rol, $"Nota de bitácora folio {nota.Folio} pendiente de firma.", ct);

        return MapNotaDto(nota);
    }

    public async Task FirmarNotaAsync(int notaId, FirmarNotaDto dto, CancellationToken ct = default)
    {
        var nota = await _notaRepo.GetConFirmasAsync(notaId, ct)
            ?? throw new InvalidOperationException("Nota no encontrada.");

        var firma = nota.Firmas.FirstOrDefault(f => f.RolFirmante == dto.Rol && !f.EsEmisor)
            ?? throw new InvalidOperationException("No existe una firma pendiente para este rol en esta nota.");

        firma.UsuarioId = dto.UsuarioId;
        firma.Firmado = true;
        firma.FechaFirma = DateTime.UtcNow;

        await _notaRepo.SaveChangesAsync(ct);

        if (nota.Firmas.All(f => f.Firmado))
            await _alertaService.ResolverAsync(TipoAlerta.BitacoraFirmaPendiente, nota.Id, ct);
    }

    public async Task<List<BitacoraNotaDto>> BuscarNotasAsync(int BitacoraId, string? asunto, DateOnly? fechaInicio, DateOnly? fechaFin, int? actorId, CancellationToken ct = default)
    {
        var notas = await _notaRepo.BuscarAsync(BitacoraId, asunto, fechaInicio, fechaFin, actorId, ct);
        return notas.Select(MapNotaDto).ToList();
    }

    public async Task CrearMinutaAsync(CrearMinutaDto dto, CancellationToken ct = default)
    {
        var minuta = new BitacoraMinuta
        {
            Id = int.Newint(), BitacoraId = dto.BitacoraId, Fecha = dto.Fecha,
            Lugar = dto.Lugar, ContenidoAcuerdos = dto.ContenidoAcuerdos
        };

        foreach (var p in dto.Participantes)
            minuta.Participantes.Add(new BitacoraMinutaParticipante { Id = int.Newint(), NombreParticipante = p });

        _context.BitacoraMinutas.Add(minuta);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<int> CrearIncidenciaAsync(CrearIncidenciaDto dto, CancellationToken ct = default)
    {
        var incidencia = new BitacoraIncidencia
        {
            Id = int.Newint(), BitacoraId = dto.BitacoraId, FechaEvento = dto.FechaEvento,
            Descripcion = dto.Descripcion, UrlFotografia = dto.UrlFotografia, ActorRegistroId = dto.ActorRegistroId
        };

        _context.BitacoraIncidencias.Add(incidencia);
        await _context.SaveChangesAsync(ct);
        return incidencia.Id;
    }

    public async Task<BitacoraNotaDto> GenerarNotaDesdeIncidenciaAsync(GenerarNotaDesdeIncidenciaDto dto, CancellationToken ct = default)
    {
        var incidencia = await _context.BitacoraIncidencias.FindAsync(new object[] { dto.IncidenciaId }, ct)
            ?? throw new InvalidOperationException("Incidencia no encontrada.");

        var notaDto = await CrearNotaAsync(new CrearNotaBitacoraDto(
            incidencia.BitacoraId, dto.TipoNotaCatalogoId,
            $"Derivado de incidencia: {incidencia.Descripcion[..Math.Min(80, incidencia.Descripcion.Length)]}",
            incidencia.Descripcion, null, dto.UsuarioEmisorId, dto.RolEmisor), ct);

        incidencia.NotaGeneradaId = notaDto.Id;
        await _context.SaveChangesAsync(ct);

        return notaDto;
    }

    private static BitacoraNotaDto MapNotaDto(BitacoraNota n) => new(
        n.Id, n.Folio, n.TipoRegistro, n.Asunto, n.Contenido, n.FechaRegistro, n.Cerrada,
        n.Firmas.Select(f => new FirmaDto(f.UsuarioId, f.RolFirmante, f.EsEmisor, f.Firmado, f.FechaFirma)).ToList());
}
