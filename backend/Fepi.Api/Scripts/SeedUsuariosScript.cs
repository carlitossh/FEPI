using Fepi.Api.Data;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Scripts;

public static class SeedUsuariosScript
{
    public static async Task<int> CrearUsuarioConRolAsync(
        FepiDbContext context,
        string nombre,
        string correo,
        int contratoId,
        RolSistema rol,
        CancellationToken ct = default)
    {
        var usuario = await context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == correo, ct);

        if (usuario == null)
        {
            usuario = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Activo = true
            };

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync(ct);
        }

        bool yaExisteRol = await context.UsuarioContratos.AnyAsync(x =>
            x.UsuarioId == usuario.Id &&
            x.ContratoId == contratoId &&
            x.Rol == rol, ct);

        if (!yaExisteRol)
        {
            context.UsuarioContratos.Add(new UsuarioContrato
            {
                UsuarioId = usuario.Id,
                ContratoId = contratoId,
                Rol = rol
            });

            await context.SaveChangesAsync(ct);
        }

        return usuario.Id;
    }

    public static async Task SeedCatalogoTiposNotaAsync(
        FepiDbContext context,
        CancellationToken ct = default)
    {
        if (await context.BitacoraTiposNota.AnyAsync(ct))
            return;

        var tipos = new[]
        {
            new BitacoraTipoNota
            {
                Nombre = "Aviso general",
                Codigo = CatalogoTipoNota.AvisoGeneral
            },
            new BitacoraTipoNota
            {
                Nombre = "Solicitud de modificación al proyecto ejecutivo",
                Codigo = CatalogoTipoNota.SolicitudModificacionProyectoEjecutivo
            },
            new BitacoraTipoNota
            {
                Nombre = "Solicitud de cambio de procedimiento constructivo",
                Codigo = CatalogoTipoNota.SolicitudCambioProcedimientoConstructivo
            },
            new BitacoraTipoNota
            {
                Nombre = "Solicitud de convenio modificatorio",
                Codigo = CatalogoTipoNota.SolicitudConvenioModificatorio
            },
            new BitacoraTipoNota
            {
                Nombre = "Aviso de terminación de trabajos",
                Codigo = CatalogoTipoNota.AvisoTerminacionTrabajos
            }
        };

        context.BitacoraTiposNota.AddRange(tipos);
        await context.SaveChangesAsync(ct);
    }
}