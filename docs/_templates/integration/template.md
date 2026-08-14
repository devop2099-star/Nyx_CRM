# [Nombre del Sistema Externo] — Ficha Técnica de Integración

## Estado de la Integración
Producción / Staging / En Desarrollo / Deprecado

## 1. Propósito & Responsabilidad
Descripción del rol funcional que cumple este sistema externo en la arquitectura del CRM (ej. Mesa de Ayuda, Pasarela de Pagos, Servidor de Correo, Base de Datos Legacy).

---

## 2. Esquema de Autenticación & Conectividad
- **Tipo de Autenticación:** Bearer Token / Basic Auth / API Key en Header / Conexión TLS
- **Ubicación de Credenciales:** Variables de entorno seguras (no hardcodeadas)
- **Base URL (Staging):** `https://staging-api.externo.com/api/v1`
- **Base URL (Producción):** `https://api.externo.com/api/v1`
- **Protocolo & Puerto:** HTTPS (TCP 443) / PostgreSQL (TCP 5432) / SMTP STARTTLS (TCP 587)

---

## 3. Endpoints Utilizados por CRM.ApiHub

| Método | Endpoint Relativo | Propósito en el CRM | Contrato C# / Handler |
|---|---|---|---|
| `POST` | `/tickets` | Creación de incidencias técnicas | `CreateGlpiTicketCommandHandler` |
| `GET` | `/customers/{dni}` | Consulta de clientes históricos | `GetLegacyCustomerQueryHandler` |

### Ejemplo de Payload de Solicitud (Request)
```json
{
  "campo_uno": "valor",
  "campo_dos": 123
}
```

### Ejemplo de Respuesta Exitosa (Response 200/201)
```json
{
  "id": 45892,
  "status": "created",
  "timestamp": "2026-08-10T12:00:00Z"
}
```

---

## 4. Manejo de Errores & Resiliencia
- **Política de Reintentos:** Polly Exponential Backoff con Jitter (3 reintentos: 2s, 4s, 8s).
- **Timeout Máximo:** 5000 ms por solicitud HTTP.
- **Circuit Breaker:** Apertura tras 5 fallos consecutivos durante 30 segundos.
- **Fallback:** Respuesta en caché o encolado local para reintento asíncrono.

---

## 5. Referencias & Documentación Oficial
- **Documentación del Proveedor:** `https://developer.proveedor.com/docs`
- **Contacto de Soporte Técnico:** `soporte-api@proveedor.com`
- **Módulo Interno en CRM:** [docs/05-integrations/](../index.html)
