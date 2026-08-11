# Concepto: Autenticación Stateless con Tokens JWT & Rotación de Refresh Tokens

## Propósito Arquitectónico
Garantizar una arquitectura de alta escalabilidad horizontal donde múltiples instancias de `CRM.ApiHub` puedan validar peticiones HTTP de forma **stateless** (sin consultar la base de datos en cada request), manteniendo la capacidad de **revocación inmediata** mediante una lista negra distribuida en Redis 7.

---

## 🔄 Ciclo de Vida de los Tokens

```
[ Cliente Blazor ]                   [ CRM.ApiHub ]                   [ Redis 7 Cache ]
       |                                   |                                  |
       |-- 1. POST /api/auth/login ------->|                                  |
       |                                   |-- 2. Validar BCrypt Hash         |
       |                                   |-- 3. Emitir Access Token (15m)   |
       |                                   |-- 4. Emitir Refresh Token (7d) --|
       |<-- 5. Retornar Par de Tokens -----|                                  |
       |                                   |                                  |
       |-- 6. GET /api/orders (Bearer) --->|                                  |
       |                                   |-- 7. Validar Firma & Expiración  |
       |                                   |-- 8. Comprobar Lista Negra ----->| (Retorna: No revocado)
       |<-- 9. Retornar Datos (200 OK) ----|                                  |
```

---

## 🔑 Ventajas de este Modelo Híbrido
1. **Velocidad Extrema:** El 99.9% de las peticiones validan la firma criptográfica en memoria sin tocar PostgreSQL.
2. **Corta Exposición (15 minutos):** Si un Access Token es interceptado, su ventana de validez expira rápidamente.
3. **Revocación Instantánea en Logout:** Al cerrar sesión, el `jti` (JWT ID) se almacena en Redis con un TTL igual al tiempo de vida restante del token, impidiendo su reutilización en cualquier réplica.
