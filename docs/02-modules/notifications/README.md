# Módulo de Notificaciones

## Propósito
Envía alertas en tiempo real en la interfaz Blazor y notificaciones por correo electrónico (SMTP) sobre cambios de estado en las órdenes comerciales.

## Componentes Involucrados
- **Backend (`CRM.ApiHub`)**: `NotificationController.cs`, `NotificationHub.cs` (SignalR), `SmtpEmailService.cs`.
- **Frontend (`CRM.WebFrontend`)**: `Alertas.razor`.

## Endpoints Principales

| Método | Endpoint | Descripción | Roles |
|---|---|---|---|
| GET | `/api/v1/notifications/unread` | Obtener notificaciones no leídas del usuario | Autenticado |
| POST | `/api/v1/notifications/{id}/read` | Marcar notificación como leída | Autenticado |
