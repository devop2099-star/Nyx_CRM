# Consultas de Mantenimiento (Maintenance Queries)

## 1. Limpieza de tokens JWT expirados (si se guardan en BD)
```sql
DELETE FROM auth.refresh_tokens
WHERE expires_at < CURRENT_TIMESTAMP;
```

## 2. Re-generar índices (VACUUM / REINDEX)
Se recomienda correr en ventanas de mantenimiento si hay degradación de rendimiento.
```sql
VACUUM ANALYZE crm.sales_orders;
REINDEX TABLE crm.leads;
```
