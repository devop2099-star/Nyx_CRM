# Consultas de Troubleshooting

## 1. Buscar conexiones activas (Bloqueos)
```sql
SELECT pid, usename, state, query, wait_event_type
FROM pg_stat_activity
WHERE state != 'idle' AND usename = 'crm_user';
```

## 2. Verificar estado del FDW (Foreign Data Wrapper)
```sql
SELECT * FROM pg_foreign_server;
SELECT * FROM pg_user_mappings;
```
