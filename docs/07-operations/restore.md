# Restauración de Backups (Restore)

## Procedimiento de Emergencia

### 1. Detener la aplicación
Para evitar inconsistencias durante la restauración:
`docker compose stop api`

### 2. Eliminar BD actual y recrear
> [!CAUTION]
> Esto eliminará todos los datos actuales de forma irrecuperable.
```bash
dropdb -U postgres crm_db
createdb -U postgres crm_db
```

### 3. Restaurar desde pg_dump
```bash
pg_restore -U postgres -d crm_db -1 /ruta/al/backup.backup
```

### 4. Reiniciar servicios
`docker compose start api`
