# Integración y Resiliencia FDW (Foreign Data Wrapper)

## Contexto Técnico
El sistema CRM consulta tablas remotas para la validación de clientes y contratos legacy mediante la extensión `postgres_fdw` de PostgreSQL.

---

## 🛠️ Estrategia de Tolerancia a Fallos
1. **Detección de Caída de Red FDW**:
   - En caso de falla de conexión con el servidor foráneo, PostgreSQL retorna errores del tipo `SQLSTATE 08006` o `connection_failure`.
2. **Re-Intento Automático en Repository**:
   - La capa `CRM.ApiHub.Infrastructure` captura las excepciones de Npgsql y ejecuta un reintento con backoff exponencial (1s, 3s, 5s).
3. **Mecanismo Fallback / Circuit Breaker**:
   - Si tras 3 intentos el servidor remoto no responde, la consulta responde con datos en caché temporal o activa el modo *Degraded Operation*, permitiendo registrar la pre-venta en cola local para su posterior sincronización.

---

## ⚡ Optimizaciones de Rendimiento
- **Pushdown Query Tuning**: Se configuran las opciones `use_remote_estimate 'true'` y `fetch_size 1000` en la definición de la Foreign Server para permitir que las cláusulas `WHERE` y `JOIN` se ejecuten en el servidor remoto antes de transmitir datos por la red.
