using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

/// <summary>
/// SV-06 — El catálogo describe únicamente un "panel persistente de alertas visibles según rol".
/// No se implementa envío de correo: no está en el catálogo.
/// </summary>
public class AlertaService : IAlertaService
{
    private readonly FepiDbContext _context;

    public AlertaService(FepiDbContext context) => _context = context;

    public async Task EmitirAsync(int contratoId, TipoAlerta tipo, int entidadId, string entidadTipo,
        RolSistema rolDestino, string mensaje, CancellationToken ct = default)
    {
        _context.Alertas.Add(new Alerta
        {
            ContratoId = contratoId, Tipo = tipo, EntidadReferenciaId = entidadId,
            EntidadReferenciaTipo = entidadTipo, RolDestino = rolDestino, Mensaje = mensaje
        });

        await _context.SaveChangesAsync(ct);
    }

    public async Task ResolverAsync(TipoAlerta tipo, int entidadId, CancellationToken ct = default)
    {
        var alertas = await _context.Alertas.AsNoTracking()
            .Where(a => a.Tipo == tipo && a.EntidadReferenciaId == entidadId && a.Estado == EstadoAlerta.Activa)
            .ToListAsync(ct);

        foreach (var alerta in alertas)
            alerta.Estado = EstadoAlerta.Resuelta;

        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<AlertaDto>> ObtenerActivasPorRolAsync(RolSistema rol, int? contratoId = null, CancellationToken ct = default)
    {
        var query = _context.Alertas.AsNoTracking().Where(a => a.RolDestino == rol && a.Estado == EstadoAlerta.Activa);

        if (contratoId.HasValue)
            query = query.Where(a => a.ContratoId == contratoId.Value);

        var alertas = await query.OrderBy(a => a.FechaGeneracion).ToListAsync(ct);
        return alertas.Select(a => new AlertaDto(a.Id, a.Tipo, a.Mensaje, a.FechaGeneracion)).ToList();
    }
}
