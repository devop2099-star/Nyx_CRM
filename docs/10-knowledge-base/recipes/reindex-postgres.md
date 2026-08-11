# Recipe: Reindexación Concurrente de Tablas en PostgreSQL 16

## Propósito
Reconstruir los índices de las tablas con alta tasa de inserción y actualización (`crm.leads`, `crm.sales_orders`, `crm.audit_logs`) para eliminar la fragmentación y recuperar el rendimiento óptimo de las consultas sin bloquear las operaciones de lectura ni escritura.

---

## Comandos SQL (Ejecutar en `psql` o pgAdmin)

```sql
-- 1. Reindexar tabla de leads concurrentemente (Sin bloqueo)
REINDEX TABLE CONCURRENTLY crm.leads;

-- 2. Reindexar tabla de órdenes de venta
REINDEX TABLE CONCURRENTLY crm.sales_orders;

-- 3. Reindexar tabla de logs de auditoría
REINDEX TABLE CONCURRENTLY crm.audit_logs;

-- 4. Actualizar estadísticas del planificador de consultas
ANALYZE crm.leads;
ANALYZE crm.sales_orders;
```

---

## Comprobación del Tamaño de Índices

```sql
SELECT 
    schemaname,
    tablename,
    indexname,
    pg_size_pretty(pg_relation_size(indexrelid)) AS index_size
FROM pg_stat_user_indexes
WHERE schemaname = 'crm'
ORDER BY pg_relation_size(indexrelid) DESC;
```
