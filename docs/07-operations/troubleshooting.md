# Troubleshooting Operativo

## Falla: La API no responde (502 Bad Gateway)
1. Verificar si Nginx está corriendo: `systemctl status nginx`
2. Verificar si el contenedor API está corriendo: `docker ps`
3. Revisar logs del contenedor: `docker compose logs --tail 50 api`

## Falla: Lentitud extrema en búsquedas
1. Verificar si el cuello de botella es la base de datos local o el FDW.
2. Revisar la vista `pg_stat_activity` (ver `04-database/queries/troubleshooting.md`).
3. Confirmar la conexión al ERP legacy.

## Falla: No llegan los correos
1. Revisar los logs del backend para excepciones SMTP.
2. Verificar en el dashboard del proveedor SMTP si los correos rebotan.
