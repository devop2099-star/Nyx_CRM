# Guía KB: Recuperación de Conexión FDW PostgreSQL

## Cuándo Utilizar esta Guía
Utilizar si en los logs del backend aparece el siguiente error:
`Npgsql.PostgresException (0x80004005): 08006: could not connect to server "fdw_legacy_server"`

---

## Diagnóstico

### Paso 1: Comprobar Conectividad de Red
Ejecutar desde la máquina donde corre `CRM.ApiHub`:
```bash
ping -c 3 192.168.1.50 # IP del servidor PostgreSQL legacy
nc -zv 192.168.1.50 5432
```

### Paso 2: Verificar la User Mapping de FDW en PostgreSQL
Conectarse a la BD primaria del CRM con `psql`:
```sql
SELECT * FROM pg_foreign_server;
SELECT * FROM pg_user_mappings;
```

---

## Solución

### Paso 1: Reiniciar Conexiones FDW
Ejecutar la consulta de reconexión:
```sql
ALTER SERVER fdw_legacy_server OPTIONS (SET host '192.168.1.50');
```

### Paso 2: Reiniciar el Servicio Backend API
```bash
sudo systemctl restart crm-apihub.service
```

---

## Validación
- [ ] El endpoint `GET /api/v1/health` retorna status `Healthy`.
- [ ] La consulta a la tabla foránea retorna resultados sin timeout.
