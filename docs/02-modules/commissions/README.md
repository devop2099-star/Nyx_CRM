# Módulo de Comisiones

## Propósito
Calcula, liquida y audita las comisiones devengadas por los Asesores de Venta según el estado final de las órdenes comerciales activadas.

## Componentes Involucrados
- **Backend (`CRM.ApiHub`)**: `CommissionController.cs`, `CommissionCalculatorService.cs`.
- **Frontend (`CRM.WebFrontend`)**: `AsesorDashboard.razor`, `SupervisorReports.razor`.

## Endpoints Principales

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/v1/commissions/summary` | Resumen de comisiones por asesor y periodo | Asesor, Supervisor |
| POST | `/api/v1/commissions/calculate` | Ejecutar cierre y cálculo de comisiones | Admin |

## Reglas de Negocio
1. Una comisión solo se liquida cuando la orden pasa a estado `Activado` en el módulo de Backoffice.
2. Si una venta es anulada por el cliente dentro de los 30 días posteriores, la comisión entra en estado `Reversada`.
