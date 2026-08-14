# Módulo de Órdenes de Venta

## Propósito
Gestiona el ciclo de vida completo de las órdenes de venta formalizadas, incluyendo anexos, contratos digitales y asignación a supervisión.

## Componentes Involucrados
- **Backend (`CRM.ApiHub`)**: `SalesOrderController.cs`, `DocumentController.cs`.
- **Frontend (`CRM.WebFrontend`)**: `AsesorOrderDetail.razor`, `Dashboard.razor`.

## Endpoints Principales

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/v1/orders` | Obtener catálogo de órdenes con filtros de fecha y estado | Todos |
| GET | `/api/v1/orders/{id}` | Obtener detalle completo de orden | Todos |
| POST | `/api/v1/orders/{id}/documents` | Cargar documento adjunto (cédula, contrato) | Asesor, Backoffice |

## Reglas de Negocio
1. Una orden no puede pasar a estado `Por Auditar` sin tener adjunto el documento de identidad del cliente.
2. Cada cambio de estado de la orden genera una entrada auditada en la tabla `AuditLogs`.
