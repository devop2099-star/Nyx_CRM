# Runbook: Conmutación por Error de Base de Datos de Emergencia (PostgreSQL Failover)

## Nivel de Criticidad: P1 (Emergencia Operativa)
Procedimiento de ejecución manual para promover el nodo secundario `srv-crm-db02` a nodo primario de lectura y escritura en caso de falla de hardware irrecuperable en el nodo primario `srv-crm-db01`.

---

## Cuándo Ejecutar este Runbook
- El nodo primario `10.10.20.21` no responde a ping, SSH ni conexiones TCP 5432 por más de 3 minutos consecutivos.
- Daño en el arreglo RAID o corte de energía irrecuperable en el centro de datos principal.

---

## Procedimiento Paso a Paso

### Paso 1: Promover la Réplica Secundaria a Primaria
Conéctate vía SSH al servidor secundario `srv-crm-db02` (`10.10.20.22`):

```bash
# Ejecutar comando de promoción nativo de PostgreSQL
sudo -u postgres pg_ctl promote -D /var/lib/postgresql/16/main
```

Verifica en los logs que la base de datos haya salido del modo de recuperación (`Standby`):
```bash
sudo tail -n 20 /var/log/postgresql/postgresql-16-main.log
# Salida esperada: "database system is ready to accept read-write connections"
```

### Paso 2: Actualizar la Cadena de Conexión en CRM.ApiHub
Edita el archivo de variables de entorno `.env` en los servidores de aplicación `srv-crm-app01` y `srv-crm-app02`:

```bash
# Cambiar el host de la base de datos hacia el nuevo primario (10.10.20.22)
ConnectionStrings__DefaultConnection="Host=10.10.20.22;Port=5432;Database=crm_production;Username=crm_app;Password=SuperSecretPassword;SSL Mode=Require;"
```

### Paso 3: Reiniciar los Contenedores de la API
```bash
cd /opt/crm-deployment
docker compose restart api
```

### Paso 4: Validar Endpoints de Salud
```bash
curl -i http://localhost:5000/healthz
# Salida esperada: HTTP/1.1 200 OK  {"status": "Healthy", "database": "Healthy"}
```

---

## Plan de Rollback / Normalización
Una vez reparado el servidor original `srv-crm-db01`, se debe reconfigurar como réplica del nuevo primario (`10.10.20.22`) utilizando `pg_basebackup` antes de cualquier reintegración.
