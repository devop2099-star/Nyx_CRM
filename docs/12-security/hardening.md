# Hardening de Seguridad de la API (`CRM.ApiHub`)

## 1. Políticas de Seguridad Implementadas

### A. Encabezados de Seguridad HTTP (Security Headers)
El middleware global agrega automáticamente en cada respuesta HTTP:
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `X-XSS-Protection: 1; mode=block`
- `Content-Security-Policy`: Restricción estricta de fuentes de script e imágenes.
- `Strict-Transport-Security` (HSTS): Exigencia de HTTPS en producción.

### B. Protección contra Fuerza Bruta y Rate Limiting
- Configuración de `AspNetCoreRateLimiting` limitando las peticiones a `/api/v1/auth/login` a **5 intentos por minuto por dirección IP**.

### C. Manejo Seguro de Secretos
- Las claves de firma JWT (`JwtSecret`) y cadenas de conexión a PostgreSQL se leen exclusivamente desde variables de entorno de producción o Azure Key Vault.
- **Regla Estricta**: Queda prohibido hardcodear credenciales en `appsettings.json` o código C#.
