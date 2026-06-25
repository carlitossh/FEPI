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
                Codigo = "AVISO"
            },
            new BitacoraTipoNota
            {
                Nombre = "Solicitud de modificación al proyecto ejecutivo",
                Codigo = "SOL_MOD_PROY_EJEC"
            },
            new BitacoraTipoNota
            {
                Nombre = "Solicitud de cambio de procedimiento constructivo",
                Codigo = "SOL_CAM_PROC_CONS"
            },
            new BitacoraTipoNota
            {
                Nombre = "Solicitud de convenio modificatorio",
                Codigo = "SOL_CON_MOD"
            },
            new BitacoraTipoNota
            {
                Nombre = "Aviso de terminación de trabajos",
                Codigo = "AVISO_TERM_TRAB"
            }
        };

        context.BitacoraTiposNota.AddRange(tipos);
        await context.SaveChangesAsync(ct);
    }
}