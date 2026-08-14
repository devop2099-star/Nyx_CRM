# Módulo de Backoffice y Activaciones

## Propósito
Permite al equipo de Backoffice procesar las órdenes aprobadas por supervisión, realizar la activación de los servicios con los proveedores y gestionar fallas de aprovisionamiento.

## Componentes Involucrados
- **Backend (`CRM.ApiHub`)**: `BackofficeController.cs`, `ActivationController.cs`, `ProviderController.cs`.
- **Frontend (`CRM.WebFrontend`)**: `BackofficeActivations.razor`.

## Endpoints Principales

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/v1/backoffice/activations/pending` | Bandeja de órdenes pendientes de activación | Backoffice, Admin |
| POST | `/api/v1/backoffice/activations/{id}/approve` | Confirmar activación de servicio | Backoffice |
| POST | `/api/v1/backoffice/activations/{id}/reject` | Rechazar activación por inconsistencia | Backoffice |

## Reglas de Negocio
1. Al aprobar una activación, se actualiza el estado del cliente a `Activo` y se calcula la comisión del Asesor.
2. Si el proveedor rechaza la activación, la orden ingresa a estado `Rechazada en Backoffice` con causa justificada.
