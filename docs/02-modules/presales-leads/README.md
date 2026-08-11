# Módulo de Pre-Ventas y Leads

## Propósito
Permite a los Asesores capturar solicitudes iniciales de clientes, registrar datos demográficos, validar cobertura y calificar prospectos.

## Componentes Involucrados
- **Backend (`CRM.ApiHub`)**: `PreSaleController.cs`, `LeadController.cs`, `FormController.cs`.
- **Frontend (`CRM.WebFrontend`)**: `AsesorDashboard.razor`, `AsesorOrderDetail.razor`, `AdminUserFields.razor`.

## Endpoints Principales

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/v1/presales` | Listar pre-ventas asignadas al asesor | Asesor, Supervisor |
| POST | `/api/v1/presales` | Registrar una nueva pre-venta | Asesor |
| GET | `/api/v1/forms/{id}` | Obtener esquema dinámico de formulario | Asesor, Supervisor |

## Reglas de Negocio
1. Toda pre-venta requiere validación de cédula y número de contacto.
2. Si el cliente registra duplicidad de solicitud en los últimos 30 días, la pre-venta se marca como `Duplicada`.

## Documentación Visual
- 🌐 [Abrir Guía Visual del Módulo](index.html)

