# FEPI Backend API v1 — Guía para Frontend

Documento para implementar el frontend consumiendo la API actual del backend FEPI.

## Formato general

La API usa principalmente JSON.

### Headers recomendados

```http
Content-Type: application/json
Accept: application/json
```

Para archivos:

```http
Content-Type: multipart/form-data
```

### Formato de respuesta estándar

Algunos endpoints ya regresan `ApiResponse<T>`:

```json
{
  "success": true,
  "message": "Operación realizada correctamente",
  "data": {}
}
```

Algunos endpoints todavía regresan DTO directo o `200 OK` sin cuerpo. En esos casos, el frontend debe validar por status code.

### IDs

El backend actual usa IDs tipo `int`.

Ejemplo:

```json
{
  "contratoId": 1,
  "usuarioId": 2,
  "empresaId": 3
}
```

### Enums

En Swagger los enums aparecen como números. El frontend debe mandar números, no texto.

Ejemplo:

```json
{
  "rol": 1,
  "tipoPeriodo": 1,
  "modalidadPago": 1
}
```

> Nota: confirmar en el backend el significado exacto de cada número de enum.

---

# Flujo general recomendado

```text
1. Login
2. Crear usuarios
3. Crear empresa
4. Crear contrato
5. Crear secciones del contrato
6. Crear conceptos dentro de secciones
7. Registrar programa de obra por sección
8. Abrir bitácora
9. Crear estimaciones
10. Revisar/aprobar/pagar estimaciones
11. Crear y resolver convenios
12. Consultar dashboard, alertas y avance
13. Iniciar cierre y registrar finiquito
```

---

# 1. Auth

## Login

```http
POST /api/auth/login
```

### Body

```json
{
  "username": "admin",
  "password": "123456"
}
```

### Devuelve

```json
{
  "success": true,
  "message": "Login correcto",
  "data": {
    "id": 1,
    "nombre": "Administrador",
    "username": "admin",
    "rol": 1
  }
}
```

### Uso en frontend

Guardar en localStorage o estado global:

```json
{
  "usuarioId": 1,
  "nombre": "Administrador",
  "username": "admin",
  "rol": 1
}
```

---

# 2. Usuarios

## Listar usuarios

```http
GET /api/usuarios
```

### Devuelve

```json
{
  "success": true,
  "message": "Usuarios obtenidos correctamente",
  "data": [
    {
      "id": 1,
      "nombre": "Juan Pérez",
      "correo": "juan@test.com",
      "username": "juan",
      "rol": 2,
      "activo": true
    }
  ]
}
```

---

## Obtener usuario por ID

```http
GET /api/usuarios/{usuarioId}
```

### Devuelve

```json
{
  "success": true,
  "data": {
    "id": 1,
    "nombre": "Juan Pérez",
    "correo": "juan@test.com",
    "username": "juan",
    "rol": 2,
    "activo": true
  }
}
```

---

## Crear usuario

```http
POST /api/usuarios
```

### Body

```json
{
  "nombre": "Juan Pérez",
  "correo": "juan@test.com",
  "username": "juan",
  "password": "123456",
  "rol": 2
}
```

### Devuelve

```json
{
  "success": true,
  "message": "Usuario creado correctamente",
  "data": {
    "id": 1,
    "nombre": "Juan Pérez",
    "correo": "juan@test.com",
    "username": "juan",
    "rol": 2,
    "activo": true
  }
}
```

---

## Actualizar usuario

```http
PUT /api/usuarios/{usuarioId}
```

### Body

```json
{
  "nombre": "Juan Pérez",
  "correo": "juan@test.com",
  "username": "juan",
  "password": "123456",
  "rol": 2
}
```

---

## Cambiar rol

```http
PATCH /api/usuarios/{usuarioId}/rol
```

### Body

```json
{
  "rol": 3
}
```

---

## Suspender usuario

```http
POST /api/usuarios/{usuarioId}/suspender
```

---

## Activar usuario

```http
POST /api/usuarios/{usuarioId}/activar
```

---

# 3. Empresas

## Listar empresas

```http
GET /api/empresas
```

### Devuelve

```json
[
  {
    "id": 1,
    "nombre": "Constructora ABC",
    "representanteUsuarioId": 4,
    "representanteNombre": "Carlos López"
  }
]
```

---

## Obtener empresa

```http
GET /api/empresas/{empresaId}
```

---

## Crear empresa

```http
POST /api/empresas
```

### Body

```json
{
  "nombre": "Constructora ABC",
  "representanteUsuarioId": 4
}
```

### Regla para frontend

El representante debe ser un usuario con rol de superintendente.

---

# 4. Contratos

## Listar contratos

```http
GET /api/contratos
```

### Query params opcionales

```http
GET /api/contratos?dependencia=Secretaría&estado=1
```

### Devuelve

```json
[
  {
    "id": 1,
    "numeroContrato": "OP-2024-001",
    "nombreObra": "Pavimentación de avenida principal",
    "dependenciaContratante": "Secretaría de Obras",
    "empresaNombre": "Constructora ABC",
    "montoContratado": 2500000,
    "fechaInicio": "2024-07-01",
    "fechaTermino": "2025-07-01",
    "estado": 1
  }
]
```

---

## Crear contrato

```http
POST /api/contratos
```

### Body

```json
{
  "numeroContrato": "OP-2024-001",
  "tipo": 1,
  "montoContratado": 2500000,
  "fechaInicio": "2024-07-01",
  "fechaTermino": "2025-07-01",
  "dependenciaContratante": "Secretaría de Obras",
  "empresaId": 1,
  "numeroLicitacion": "LIC-2024-09",
  "nombreObra": "Pavimentación de avenida principal",
  "ubicacionExacta": "Av. Principal, CDMX",
  "tipoPeriodo": 1,
  "modalidadPago": 1,
  "porcentajeAnticipo": 30,
  "residenteNombre": "Juan Residente",
  "supervisorExternoNombre": "Ana Supervisora",
  "superintendenteNombre": "Carlos Superintendente",
  "garantias": [
    {
      "tipo": 1,
      "monto": 250000,
      "porcentaje": 10,
      "vigencia": "2025-07-01"
    }
  ]
}
```

### Devuelve

```json
{
  "success": true,
  "message": "Contrato creado correctamente",
  "data": 1
}
```

### Nota importante

Este endpoint solo crea el contrato. Los conceptos y el programa de obra se registran después en endpoints separados.

---

## Obtener contrato

```http
GET /api/contratos/{contratoId}
```

### Devuelve

```json
{
  "id": 1,
  "numeroContrato": "OP-2024-001",
  "nombreObra": "Pavimentación de avenida principal",
  "montoContratado": 2500000,
  "fechaInicio": "2024-07-01",
  "fechaTermino": "2025-07-01",
  "dependenciaContratante": "Secretaría de Obras",
  "empresaId": 1,
  "estado": 1
}
```

---

## Actualizar contrato

```http
PUT /api/contratos/{contratoId}
```

### Body

Similar al body de crear contrato, con los campos editables.

---

## Actualizar monto

```http
PATCH /api/contratos/{contratoId}/monto
```

### Body

```json
{
  "nuevoMonto": 2800000,
  "motivo": "Ajuste por convenio modificatorio"
}
```

---

## Obtener detalle completo del contrato

```http
GET /api/contratos/{contratoId}/detalle
```

### Uso

Este es el endpoint principal para la pantalla de detalle del contrato.

### Devuelve

```json
{
  "success": true,
  "message": "Detalle del contrato obtenido correctamente",
  "data": {
    "contratoId": 1,
    "numeroContrato": "OP-2024-001",
    "nombreObra": "Pavimentación de avenida principal",
    "dependenciaContratante": "Secretaría de Obras",
    "ubicacionExacta": "Av. Principal, CDMX",
    "fechaInicio": "2024-07-01",
    "fechaTermino": "2025-07-01",
    "montoContratado": 2500000,
    "estado": 1,
    "empresa": {
      "empresaId": 1,
      "nombre": "Constructora ABC",
      "representanteUsuarioId": 4,
      "representanteNombre": "Carlos López"
    },
    "responsables": {
      "residenteNombre": "Juan Residente",
      "supervisorNombre": "Ana Supervisora",
      "superintendenteNombre": "Carlos Superintendente"
    },
    "conceptosResumen": {
      "totalSecciones": 3,
      "totalConceptos": 20,
      "importeTotalConceptosActivos": 2500000,
      "contratoCuadraConConceptos": true
    },
    "programaResumen": {
      "tieneProgramaObra": true,
      "avanceProgramadoAcumulado": 0.45
    },
    "estimacionesResumen": {
      "totalEstimaciones": 5,
      "importeEstimadoAcumulado": 1200000,
      "importePagadoAcumulado": 1000000,
      "saldoPorEstimar": 1300000
    },
    "conveniosResumen": {
      "totalConvenios": 2,
      "conveniosPendientes": 1,
      "conveniosAplicados": 1
    },
    "bitacoraResumen": {
      "bitacoraAbierta": true,
      "totalNotas": 12,
      "notasPendientesFirma": 3
    },
    "finiquitoResumen": {
      "tieneFiniquito": false,
      "puedeIniciarCierre": true,
      "puedeRegistrarFiniquito": false,
      "motivoBloqueoFiniquito": "Todavía no se ha iniciado el cierre del contrato."
    }
  }
}
```

---

## Usuarios del contrato

```http
GET /api/contratos/{contratoId}/usuarios
```

---

## Invitar usuario al contrato

```http
POST /api/contratos/{contratoId}/usuarios
```

### Body

```json
{
  "usuarioId": 2,
  "rol": 3
}
```

---

# 5. Secciones y conceptos

## Obtener secciones del contrato

```http
GET /api/contratos/{contratoId}/conceptos/secciones
```

### Devuelve

```json
[
  {
    "id": 1,
    "nombre": "Pavimentado",
    "descripcion": "Trabajos de pavimentación",
    "importeTotal": 60000,
    "peso": 0.024,
    "conceptos": [
      {
        "id": 1,
        "clave": "PAV-001",
        "descripcion": "Colocación de carpeta asfáltica",
        "unidadMedida": "m2",
        "cantidadContratada": 500,
        "precioUnitario": 120,
        "precioTotal": 60000,
        "activo": true
      }
    ]
  }
]
```

---

## Crear sección

```http
POST /api/contratos/{contratoId}/conceptos/secciones
```

### Body

```json
{
  "nombre": "Pavimentado",
  "descripcion": "Trabajos de pavimentación"
}
```

---

## Crear concepto dentro de una sección

```http
POST /api/contratos/{contratoId}/conceptos/secciones/{seccionId}/conceptos
```

### Body

```json
{
  "seccionConceptoId": 1,
  "clave": "PAV-001",
  "descripcion": "Colocación de carpeta asfáltica",
  "unidadMedida": "m2",
  "cantidadContratada": 500,
  "precioUnitario": 120,
  "esExtraordinario": false,
  "convenioModificatorioId": null
}
```

### Nota

Aunque `seccionConceptoId` va en la URL como `seccionId`, el DTO también lo pide. Mandar el mismo ID en ambos lugares para evitar errores.

---

## Editar concepto

```http
PUT /api/contratos/{contratoId}/conceptos/{conceptoId}
```

### Body

```json
{
  "seccionConceptoId": 1,
  "clave": "PAV-001",
  "descripcion": "Colocación de carpeta asfáltica modificada",
  "unidadMedida": "m2",
  "cantidadContratada": 600,
  "precioUnitario": 120,
  "esExtraordinario": false,
  "convenioModificatorioId": null
}
```

---

## Desactivar concepto

```http
DELETE /api/contratos/{contratoId}/conceptos/{conceptoId}
```

### Nota

No debe borrar físicamente. Debe desactivar el concepto.

---

# 6. Programa de obra

## Obtener programa de obra

```http
GET /api/contratos/{contratoId}/programa-obra
```

### Devuelve

```json
{
  "contratoId": 1,
  "secciones": [
    {
      "seccionConceptoId": 1,
      "nombreSeccion": "Pavimentado",
      "periodoInicioId": 1,
      "periodoFinId": 3,
      "periodos": [
        {
          "periodoContratoId": 1,
          "numeroPeriodo": 1,
          "importePlanificado": 20000,
          "avanceParcialPlanificado": 0.008
        }
      ]
    }
  ]
}
```

---

## Registrar sección en programa de obra

```http
POST /api/contratos/{contratoId}/programa-obra/secciones
```

### Body

```json
{
  "seccionConceptoId": 1,
  "periodoInicioId": 1,
  "periodoFinId": 3
}
```

### Regla

El backend reparte automáticamente el monto/avance de la sección entre los periodos seleccionados.

---

## Obtener avance planificado

```http
GET /api/contratos/{contratoId}/programa-obra/avance-planificado
```

### Devuelve

```json
[
  {
    "periodoContratoId": 1,
    "numeroPeriodo": 1,
    "avancePlanificadoPeriodo": 0.08,
    "avancePlanificadoAcumulado": 0.08
  }
]
```

---

# 7. Bitácora

## Abrir bitácora

```http
POST /api/bitacora/abrir
```

### Body

```json
{
  "contratoId": 1,
  "nombreContrato": "Pavimentación de avenida principal",
  "numeroContrato": "OP-2024-001",
  "tipoContrato": "Obra pública",
  "dependenciaContratante": "Secretaría de Obras",
  "contratistaEmpresa": "Constructora ABC",
  "residenteNombre": "Juan Residente",
  "supervisorDesignadoNombre": "Ana Supervisora",
  "superintendenteNombre": "Carlos Superintendente",
  "fechaAperturaFormal": "2024-07-01"
}
```

### Devuelve

```json
{
  "id": 1,
  "contratoId": 1,
  "folioUnicoBitacora": "BIT-001",
  "numeroContrato": "OP-2024-001",
  "nombreContrato": "Pavimentación de avenida principal",
  "abierta": true
}
```

---

## Listar notas de bitácora

```http
GET /api/bitacora/{bitacoraId}/notas
```

### Filtros opcionales

```http
GET /api/bitacora/{bitacoraId}/notas?asunto=inicio&fechaInicio=2024-07-01&fechaFin=2024-07-31&actorId=1
```

### Devuelve

```json
[
  {
    "id": 1,
    "folio": "BIT-001",
    "asunto": "Inicio de trabajos",
    "contenido": "Se da inicio formal a los trabajos.",
    "fecha": "2024-07-01T10:00:00",
    "estadoFirma": 1
  }
]
```

---

## Crear nota

```http
POST /api/bitacora/notas
```

### Body

```json
{
  "bitacoraId": 1,
  "tipoNotaCatalogoId": 1,
  "asunto": "Inicio de trabajos",
  "contenido": "Se da inicio formal a los trabajos del contrato.",
  "folioVinculadoId": null,
  "usuarioEmisorId": 1,
  "rolEmisor": 1
}
```

### Devuelve

```json
{
  "id": 1,
  "folio": "BIT-001",
  "asunto": "Inicio de trabajos",
  "contenido": "Se da inicio formal a los trabajos del contrato.",
  "estadoFirma": 1
}
```

---

## Firmar nota

```http
POST /api/bitacora/notas/{notaId}/firmar
```

### Body

```json
{
  "usuarioId": 1,
  "rol": 1
}
```

---

## Eventos de bitácora

```http
GET /api/bitacora/{bitacoraId}/eventos
```

---

## Crear minuta

```http
POST /api/bitacora/minutas
```

---

## Crear incidencia

```http
POST /api/bitacora/incidencias
```

---

## Generar nota desde incidencia

```http
POST /api/bitacora/incidencias/generar-nota
```

---

# 8. Estimaciones

## Crear estimación

```http
POST /api/estimaciones
```

### Body

```json
{
  "contratoId": 1,
  "periodoContratoId": 1
}
```

### Devuelve

```json
{
  "id": 1,
  "contratoId": 1,
  "numeroEstimacion": 1,
  "periodoContratoId": 1,
  "estado": 1,
  "importeTotal": 0
}
```

---

## Listar estimaciones de contrato

```http
GET /api/estimaciones/contrato/{contratoId}
```

### Devuelve

```json
[
  {
    "id": 1,
    "numeroEstimacion": 1,
    "periodoContratoId": 1,
    "estado": 1,
    "importeTotal": 12000
  }
]
```

---

## Obtener detalle de estimación

```http
GET /api/estimaciones/{estimacionId}
```

### Devuelve

```json
{
  "id": 1,
  "contratoId": 1,
  "numeroEstimacion": 1,
  "estado": 1,
  "conceptos": [
    {
      "conceptoContratoId": 1,
      "clave": "PAV-001",
      "cantidadEjecutada": 100,
      "precioUnitario": 120,
      "importe": 12000
    }
  ],
  "observaciones": [],
  "pagos": []
}
```

---

## Actualizar conceptos de estimación

```http
PUT /api/estimaciones/{estimacionId}/conceptos
```

### Body

```json
{
  "conceptos": [
    {
      "conceptoContratoId": 1,
      "cantidadEjecutada": 100
    }
  ]
}
```

---

## Vincular notas de bitácora

```http
POST /api/estimaciones/{estimacionId}/notas-bitacora
```

### Body

```json
{
  "notasBitacoraIds": [1, 2, 3]
}
```

---

## Enviar estimación

```http
POST /api/estimaciones/{estimacionId}/enviar?usuarioId=1
```

---

## Crear observación

```http
POST /api/estimaciones/{estimacionId}/observaciones?usuarioId=1
```

### Body

```json
{
  "contenido": "Falta justificar cantidades ejecutadas."
}
```

---

## Cambiar estado

```http
PATCH /api/estimaciones/{estimacionId}/estado
```

### Body

```json
{
  "nuevoEstado": 2,
  "comentario": "Se aprueba por supervisión",
  "usuarioId": 1
}
```

---

## Registrar pago

```http
POST /api/estimaciones/{estimacionId}/pago
```

### Body

```json
{
  "fechaPago": "2024-08-15",
  "referenciaBancaria": "REF-12345",
  "montoPagado": 12000,
  "usuarioRegistroId": 1
}
```

---

## Historial de estimación

```http
GET /api/estimaciones/{estimacionId}/historial
```

---

## Pagos de estimación

```http
GET /api/estimaciones/{estimacionId}/pagos
```

---

# 9. Convenios

## Listar convenios de contrato

```http
GET /api/contratos/{contratoId}/convenios
```

### Devuelve

```json
[
  {
    "id": 1,
    "contratoId": 1,
    "numeroConvenio": "CM-001",
    "tipo": 1,
    "estado": 1,
    "montoSolicitado": 300000,
    "plazoDiasSolicitado": 60
  }
]
```

---

## Crear convenio

```http
POST /api/contratos/{contratoId}/convenios
```

### Body

```json
{
  "contratoId": 1,
  "tipo": 1,
  "justificacion": "Ampliación por condiciones climáticas",
  "montoSolicitado": 300000,
  "plazoDiasSolicitado": 60,
  "solicitanteId": 1,
  "urlsDocumentos": [
    "documentos/convenio-001.pdf"
  ],
  "empresaId": 1,
  "numeroConvenio": "CM-001"
}
```

---

## Obtener convenio

```http
GET /api/convenios/{convenioId}
```

---

## Revisar convenio

```http
POST /api/convenios/{convenioId}/revisar
```

### Body

```json
{
  "decision": 1,
  "justificacion": "Procede técnicamente",
  "supervisorId": 2
}
```

---

## Promover convenio

```http
POST /api/convenios/{convenioId}/promover
```

### Body

```json
{
  "residenteId": 3
}
```

---

## Resolver convenio

```http
POST /api/convenios/{convenioId}/resolver
```

### Body

```json
{
  "aprobado": true,
  "motivoRechazo": null,
  "usuarioDependenciaId": 1
}
```

---

# 10. Avance

## Obtener curva S / avance de contrato

```http
GET /api/contratos/{contratoId}/avance
```

### Devuelve

```json
{
  "contratoId": 1,
  "puntos": [
    {
      "periodo": "Periodo 1",
      "programado": 0.08,
      "real": 0.05
    }
  ]
}
```

---

## Registrar avance

```http
POST /api/contratos/{contratoId}/avance
```

### Body

```json
{
  "contratoId": 1,
  "periodoContratoId": 1,
  "avanceReal": 0.05,
  "usuarioId": 1
}
```

---

## Resumen general de avance

```http
GET /api/contratos/avance/resumen
```

### Query opcional

```http
GET /api/contratos/avance/resumen?dependencia=Secretaría
```

---

# 11. Dashboard

## Dashboard de contrato

```http
GET /api/contratos/{contratoId}/dashboard
```

### Devuelve

```json
{
  "contratoId": 1,
  "avanceFisico": 0.45,
  "avanceFinanciero": 0.40,
  "estimacionesPendientes": 2,
  "conveniosPendientes": 1,
  "notasPendientesFirma": 3
}
```

---

# 12. Alertas

## Obtener alertas del usuario

```http
GET /api/alertas?usuarioId=1
```

### Devuelve

```json
[
  {
    "id": 1,
    "usuarioId": 1,
    "contratoId": 1,
    "titulo": "Convenio pendiente",
    "mensaje": "El convenio CM-001 está pendiente de revisión.",
    "tipo": 1,
    "prioridad": 2,
    "fechaCreacion": "2024-08-01T10:00:00",
    "leida": false,
    "entidadRelacionada": "Convenio",
    "entidadId": 1
  }
]
```

---

## Obtener alertas no leídas

```http
GET /api/alertas/no-leidas?usuarioId=1
```

---

## Obtener alertas por rol

```http
GET /api/alertas/rol?rol=2&contratoId=1
```

---

## Marcar alerta como leída

```http
POST /api/alertas/{alertaId}/leer
```

---

# 13. Archivos

## Subir archivo

```http
POST /api/archivos
```

### Formato

`multipart/form-data`

### Campos

```text
Archivo: file
Entidad: number
EntidadId: number
UsuarioId: number
```

### Ejemplo conceptual

```text
Archivo = evidencia.pdf
Entidad = 1
EntidadId = 10
UsuarioId = 1
```

### Devuelve

```json
{
  "id": 1,
  "nombreOriginal": "evidencia.pdf",
  "rutaLocal": "uploads/evidencia.pdf",
  "tipoContenido": "application/pdf",
  "tamanoBytes": 250000,
  "fechaSubida": "2024-08-01T10:00:00"
}
```

---

## Obtener archivos de una entidad

```http
GET /api/archivos/{entidad}/{entidadId}
```

---

## Obtener metadata

```http
GET /api/archivos/{archivoId}/metadata
```

---

## Descargar archivo

```http
GET /api/archivos/{archivoId}/descargar
```

### Devuelve

Archivo físico.

---

# 14. Registros diarios

## Listar registros diarios

```http
GET /api/contratos/{contratoId}/registros-diarios
```

---

## Crear registro diario

```http
POST /api/contratos/{contratoId}/registros-diarios
```

### Body

```json
{
  "fecha": "2024-07-15",
  "responsableUsuarioId": 1,
  "descripcion": "Se realizó avance en pavimentación del tramo 1."
}
```

---

# 15. Finiquito

## Iniciar cierre

```http
POST /api/contratos/{contratoId}/finiquito/iniciar-cierre
```

### Body

```json
{
  "usuarioId": 1
}
```

### Devuelve

```json
{
  "finiquitoId": 1,
  "contratoId": 1,
  "estado": 1,
  "bitacoraNotaCierreId": 15,
  "mensaje": "Cierre iniciado. Se creó la nota obligatoria de bitácora."
}
```

---

## Obtener finiquito de contrato

```http
GET /api/contratos/{contratoId}/finiquito
```

---

## Obtener resumen de finiquito

```http
GET /api/contratos/{contratoId}/finiquito/resumen
```

### Devuelve

```json
{
  "contratoId": 1,
  "importeContratoOriginal": 2500000,
  "importeConvenios": 300000,
  "importeContratoFinal": 2800000,
  "importeEstimadoTotal": 2700000,
  "importePagadoTotal": 2600000,
  "saldoPendiente": 200000,
  "puedeRegistrarFiniquito": true,
  "motivoBloqueo": null
}
```

---

## Registrar finiquito

```http
POST /api/finiquitos/{finiquitoId}/registrar
```

### Body

```json
{
  "fechaFiniquito": "2025-09-10T10:00:00",
  "deductivas": 0,
  "retenciones": 0,
  "penasConvencionales": 0,
  "observaciones": "Se registra finiquito del contrato sin adeudos adicionales."
}
```

---

## Aprobar finiquito

```http
POST /api/finiquitos/{finiquitoId}/aprobar
```

---

## Cerrar contrato

```http
POST /api/finiquitos/{finiquitoId}/cerrar
```

---

# Pantallas sugeridas para frontend

```text
Login
Dashboard
Contratos
Detalle de contrato
  - Resumen
  - Conceptos
  - Programa de obra
  - Bitácora
  - Estimaciones
  - Convenios
  - Avance
  - Archivos
  - Alertas
  - Finiquito
Empresas
Usuarios
```

---

# Recomendación importante

Para la pantalla principal del contrato, usar primero:

```http
GET /api/contratos/{contratoId}/detalle
```

Ese endpoint debe alimentar las cards principales, resumen económico, estado del contrato, resumen de estimaciones, convenios, bitácora y finiquito.

Después, cuando el usuario entre a una pestaña específica, consumir los endpoints particulares de ese módulo.
