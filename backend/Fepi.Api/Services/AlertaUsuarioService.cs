using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class AlertaUsuarioService : IAlertaUsuarioService
{
    private readonly FepiDbContext _context;

    public AlertaUsuarioService(FepiDbContext context) => _context = context;

    public async Task<int> CrearAlertaAsync(
        int usuarioId, int? contratoId, string entidadRelacionada, int? entidadId,
        string titulo, string mensaje, TipoAlerta tipo, PrioridadAlerta prioridad,
        CancellationToken ct = default)
    {
        var alerta = new AlertaUsuario
        {
            UsuarioId = usuarioId,
            ContratoId = contratoId,
            EntidadRelacionada = entidadRelacionada,
            EntidadId = entidadId,
            Titulo = titulo,
            Mensaje = mensaje,
            Tipo = tipo,
            Prioridad = prioridad,
            FechaCreacion = DateTime.UtcNow,
            Leida = false
        };

        _context.AlertasUsuario.Add(alerta);
        await _context.SaveChangesAsync(ct);
        return alerta.Id;
    }

    public async Task<List<AlertaUsuarioResponse>> ObtenerAlertasUsuarioAsync(int usuarioId, CancellationToken ct = default)
    {
        var alertas = await _context.AlertasUsuario
            .Where(a => a.UsuarioId == usuarioId)
            .OrderByDescending(a => a.FechaCreacion)
            .ToListAsync(ct);

        return alertas.Select(MapResponse).ToList();
    }

    public async Task<List<AlertaUsuarioResponse>> ObtenerAlertasNoLeidasUsuarioAsync(int usuarioId, CancellationToken ct = default)
    {
        var alertas = await _context.AlertasUsuario
            .Where(a => a.UsuarioId == usuarioId && !a.Leida)
            .OrderByDescending(a => a.FechaCreacion)
            .ToListAsync(ct);

        return alertas.Select(MapResponse).ToList();
    }

    public async Task MarcarComoLeidaAsync(int alertaId, CancellationToken ct = default)
    {
        var alerta = await _context.AlertasUsuario.FirstOrDefaultAsync(a => a.Id == alertaId, ct)
            ?? throw new InvalidOperationException("Alerta no encontrada.");

        if (alerta.Leida) return;

        alerta.Leida = true;
        alerta.FechaLectura = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
    }

    public async Task CrearAlertasSegunEventoAsync(TipoAlerta tipo, int contratoId, int entidadId, CancellationToken ct = default)
    {
        var rolesDestino = tipo switch
        {
            TipoAlerta.EstimacionPendiente => new[] { RolSistema.SupervisorExterno },
            TipoAlerta.ConvenioPendiente => new[] { RolSistema.Superintendente },
            TipoAlerta.BitacoraFirmaPendiente => new[] { RolSistema.Residencia, RolSistema.SupervisorExterno, RolSistema.Superintendente },
            TipoAlerta.FiniquitoPendiente => new[] { RolSistema.Residencia },
            TipoAlerta.ContratoPorCerrar => new[] { RolSistema.Residencia, RolSistema.Administrador },
            _ => Array.Empty<RolSistema>()
        };

        if (rolesDestino.Length == 0) return;

        var usuarioIds = await _context.UsuarioContratos
            .Where(uc => uc.ContratoId == contratoId && rolesDestino.Contains(uc.Rol))
            .Select(uc => uc.UsuarioId)
            .Distinct()
            .ToListAsync(ct);

        var (titulo, mensaje) = tipo switch
        {
            TipoAlerta.EstimacionPendiente => ("Estimación pendiente de revisión", "Una estimación fue enviada y requiere revisión de Supervisión."),
            TipoAlerta.ConvenioPendiente => ("Convenio pendiente de aprobación", "Un convenio modificatorio requiere aprobación."),
            TipoAlerta.BitacoraFirmaPendiente => ("Nota de bitácora pendiente de firma", "Una nota de bitácora requiere su firma."),
            TipoAlerta.FiniquitoPendiente => ("Nota de cierre firmada — registrar finiquito", "La nota de cierre está firmada por todos. Puede registrar el finiquito."),
            TipoAlerta.ContratoPorCerrar => ("Contrato en proceso de cierre", "Se ha iniciado el proceso de cierre del contrato."),
            _ => ("Notificación", "Tiene una nueva notificación en el sistema.")
        };

        var prioridad = tipo switch
        {
            TipoAlerta.FiniquitoPendiente or TipoAlerta.ContratoPorCerrar => PrioridadAlerta.Alta,
            TipoAlerta.BitacoraFirmaPendiente or TipoAlerta.ConvenioPendiente => PrioridadAlerta.Media,
            _ => PrioridadAlerta.Baja
        };

        foreach (var usuarioId in usuarioIds)
        {
            _context.AlertasUsuario.Add(new AlertaUsuario
            {
                UsuarioId = usuarioId,
                ContratoId = contratoId,
                EntidadRelacionada = tipo.ToString(),
                EntidadId = entidadId,
                Titulo = titulo,
                Mensaje = mensaje,
                Tipo = tipo,
                Prioridad = prioridad,
                FechaCreacion = DateTime.UtcNow,
                Leida = false
            });
        }

        if (usuarioIds.Count > 0)
            await _context.SaveChangesAsync(ct);
    }

    private static AlertaUsuarioResponse MapResponse(AlertaUsuario a) => new(
        a.Id,
        a.UsuarioId,
        a.ContratoId,
        a.EntidadRelacionada,
        a.EntidadId,
        a.Titulo,
        a.Mensaje,
        a.Tipo,
        a.Prioridad,
        a.FechaCreacion,
        a.Leida,
        a.FechaLectura
    );
}
