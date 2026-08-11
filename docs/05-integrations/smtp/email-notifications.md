# Integración SMTP (Email Notifications)

## Propósito
El CRM envía correos electrónicos transaccionales a los asesores y clientes para confirmar operaciones (ej. restablecimiento de contraseña, confirmación de orden creada).

## Proveedor
- **Servicio**: Postmark / SendGrid / Servidor SMTP Interno.
- **Protocolo**: SMTP seguro (TLS).

## Configuración Requerida
Variables de entorno necesarias (ver `99-reference/environment-variables.md`):
- `SMTP_HOST`
- `SMTP_PORT`
- `SMTP_USER`
- `SMTP_PASSWORD`

## Componente Responsable
La interfaz `IEmailService` (ubicada en `CRM.Application/Interfaces`) es implementada en `CRM.Infrastructure/Services/SmtpEmailService`.

## Retries
Si el servidor SMTP no responde, se emplea **Polly** con una política de retry exponencial (3 reintentos).
