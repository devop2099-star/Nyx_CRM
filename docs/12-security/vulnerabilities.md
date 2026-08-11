# Matriz de Vulnerabilidades & Mitigaciones OWASP Top 10

Este documento especifica cómo el CRM mitiga las principales amenazas y vectores de ataque identificados por el estándar **OWASP Top 10 (2021/2025)**.

---

## 🛡️ Matriz de Mitigaciones Técnicas

| Vector de Ataque OWASP | Nivel de Riesgo | Mecanismo de Mitigación Implementado en CRM.ApiHub |
|---|---|---|
| **A01: Broken Access Control** | **Crítico** | Control de acceso basado en roles (**RBAC**) verificado a nivel de endpoint mediante atributos `[Authorize(Roles = "Supervisor,Admin")]` y validación de propiedad en handlers de MediatR. |
| **A02: Cryptographic Failures** | **Alto** | Todas las contraseñas se almacenan mediante **BCrypt con Work Factor 12**. Tráfico TLS 1.3 forzado mediante NGINX. Tokens JWT firmados con algoritmo HMAC-SHA256 y secretos de 512 bits. |
| **A03: Injection (SQL / NoSQL)** | **Crítico** | Consultas parametrizadas al 100% mediante **Entity Framework Core 8**. En consultas crudas o FDW se utiliza `NpgsqlParameter` para evitar concatenación de cadenas. |
| **A04: Insecure Design** | **Medio** | Máquina de estados estricta en el dominio. Una orden de venta no puede avanzar a estado `Activated` sin pasar por la aprobación previa de `ApprovedAudit`. |
| **A05: Security Misconfiguration** | **Alto** | Headers de seguridad HTTP configurados en NGINX (`X-Content-Type-Options: nosniff`, `X-Frame-Options: DENY`, `Content-Security-Policy`). Supresión de cabeceras de versión (`Server`, `X-Powered-By`). |
| **A06: Vulnerable & Outdated Components** | **Medio** | Análisis continuo de dependencias con `dotnet list package --vulnerable`. Uso de imágenes Docker oficiales de Alpine y Debian bookworm actualizadas semanalmente. |
| **A07: Identification & Auth Failures** | **Alto** | Access Tokens de corta duración (15 minutos) con **Refresh Tokens rotativos** y almacenamiento de tokens revocados en lista negra de Redis 7 con expiración automática. |
| **A08: Software & Data Integrity Failures** | **Alto** | Verificación criptográfica inmutable mediante **SHA256** para todos los contratos y cédulas subidos por los asesores antes de su persistencia en disco. |
| **A09: Security Logging & Monitoring Failures** | **Medio** | Logs estructurados con **Serilog** registrando cada intento fallido de autenticación (`401 Unauthorized`), cambios de rol y revocaciones de órdenes con IP y User-Agent. |
| **A10: Server-Side Request Forgery (SSRF)** | **Medio** | Validación de listas blancas de URLs y hostnames permitidos para la integración con GLPI y Asterisk VoIP, impidiendo llamadas a direcciones IP privadas no autorizadas. |

---

## 🛑 Política contra Fuerza Bruta & Rate Limiting
- **Endpoint de Login (`/api/auth/login`):** Máximo 5 intentos fallidos cada 60 segundos por dirección IP.
- **Bloqueo Temporal:** Tras 5 intentos fallidos consecutivos, la cuenta de usuario se bloquea temporalmente por 15 minutos en Redis.
