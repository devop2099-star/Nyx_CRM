# ADR-003 — Autenticación Stateless basada en JWT Bearer Tokens

## Estado
Aceptado

## Fecha
2026-08-03

## Contexto
Se requiere autenticar de forma segura las peticiones desde el frontend Blazor (`CRM.WebFrontend`) hacia la API backend (`CRM.ApiHub`), garantizando propagación de roles (Asesor, Supervisor, Backoffice, Admin).

## Decisión
Implementar tokens **JSON Web Tokens (JWT)** firmados con clave simétrica `HS256`, incluyendo claims de identificador de usuario, nombre, correo y roles. El frontend Blazor almacena y transmite el token mediante el encabezado `Authorization: Bearer <token>`.

## Consecuencias
- **Positivas**: Autenticación stateless escalar sin sobrecargar la sesión en el servidor API.
- **Negativas**: Necesidad de refresco de tokens (`RefreshToken`) para mantener la sesión activa sin requerir login frecuente.
