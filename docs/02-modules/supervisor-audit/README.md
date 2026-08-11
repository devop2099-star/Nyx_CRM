# Módulo de Auditoría y Supervisión

## Propósito
Permite a los Supervisores de Operaciones revisar las grabaciones de audio de llamadas, auditar formularios de pre-venta y evaluar el cumplimiento normativo antes de autorizar la orden hacia Backoffice.

## Componentes Involucrados
- **Backend (`CRM.ApiHub`)**: `SupervisorController.cs`, `AuditController.cs`.
- **Frontend (`CRM.WebFrontend`)**: `AudioAudit.razor`, `Supervisor.razor`, `SupervisorReports.razor`.

## Endpoints Principales

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/v1/supervisor/audits/pending` | Lista de solicitudes pendientes de auditoría | Supervisor, Admin |
| POST | `/api/v1/supervisor/audits/{id}/approve` | Aprobar auditoría de llamada y datos | Supervisor |
| POST | `/api/v1/supervisor/audits/{id}/reject` | Rechazar solicitud indicando faltante | Supervisor |

## Reglas de Negocio
1. La auditoría exige la reproducción total del audio y la confirmación de la lectura de términos legales.
2. Todo rechazo debe incluir una categoría obligatoria (ej: `Audio inaudible`, `Falta confirmación de identidad`).
