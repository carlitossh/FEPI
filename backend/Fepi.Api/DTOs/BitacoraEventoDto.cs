namespace Fepi.Api.DTOs;

public record BitacoraEventoDto(
    int Id,
    string Folio,
    string Tipo,
    DateTime Fecha,
    string Asunto,
    string Contenido,
    string Firmas,
    string? FolioRef
);