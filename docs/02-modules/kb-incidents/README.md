# Módulo de Base de Conocimiento e Incidentes

## Propósito
Permite la creación, administración y consulta de artículos de la Base de Conocimiento (KB), así como el reporte y seguimiento de bugs e incidentes técnicos/operativos.

## Componentes Involucrados
- **Backend (`CRM.ApiHub`)**: `KBController.cs`, `IncidentController.cs`.
- **Frontend (`CRM.WebFrontend`)**: `KbAdmin.razor`, `Incidents.razor`.

## Endpoints Principales

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/v1/kb/articles` | Consultar artículos de la Base de Conocimiento | Todos |
| POST | `/api/v1/incidents` | Reportar un nuevo bug o incidente | Todos |

## Reglas de Negocio
1. Todo incidente resuelto que pueda repetirse debe generar un artículo de troubleshooting en la KB.
