# Rollback (Plan de Reversión)

## Objetivo
Regresar el sistema a su estado estable anterior en caso de falla crítica tras un despliegue.

## Procedimiento (Backend)
1. Bajar el contenedor actual: `docker compose stop api`
2. Modificar el `docker-compose.yml` apuntando a la etiqueta anterior (ej. `v1.0.1` en vez de `v1.0.2`).
3. Levantar el servicio: `docker compose up -d api`

## Procedimiento (Base de Datos)
> [!WARNING]
> Hacer rollback de base de datos suele implicar pérdida de datos. Solo hacerlo si el sistema es inoperable.
1. Utilizar el script `down.sql` correspondiente a la migración que falló.
2. `dotnet ef database update <PreviousMigrationName>`

## Procedimiento (Frontend)
El frontend web suele actualizarse redesplegando la imagen anterior de la misma manera que el backend.
