# Referencia: Variables de Entorno y Configuración

## 1. Variables de Entorno de `CRM.ApiHub`

| Variable | Descripción | Valor Ejemplo (Dev) | Requerido |
|---|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | Entorno de ejecución | `Development` / `Production` | Sí |
| `ConnectionStrings__DefaultConnection` | Cadena de conexión a PostgreSQL CRM | `Host=localhost;Database=crm_db;Username=postgres;Password=xxx` | Sí |
| `Jwt__Secret` | Clave secreta para firmar tokens JWT | `SuperSecretKeyForSigningJwtTokensMinimum32Chars!` | Sí |
| `Jwt__Issuer` | Emisor del token | `CRM.ApiHub` | Sí |
| `Jwt__Audience` | Audiencia del token | `CRM.WebFrontend` | Sí |
| `Smtp__Host` | Servidor SMTP para envío de correos | `smtp.office365.com` | No |

---

## 2. Variables de Entorno de `CRM.WebFrontend`

| Variable | Descripción | Valor Ejemplo | Requerido |
|---|---|---|---|
| `ApiBaseUrl` | URL base de la API REST backend | `http://localhost:5000` | Sí |
