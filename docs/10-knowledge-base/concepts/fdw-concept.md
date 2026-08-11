# Comprendiendo el flujo de Foreign Data Wrapper (FDW)

## ¿Qué es FDW?
PostgreSQL Foreign Data Wrapper permite conectar la base de datos del CRM (`crm_db`) directamente con la base de datos del ERP antiguo (`legacy_db`).

## ¿Cómo funciona en nuestro sistema?
1. En `crm_db` existe un esquema virtual llamado `legacy_fdw`.
2. Las tablas en `legacy_fdw` (como `legacy_fdw.customers`) no almacenan datos, apuntan a las tablas reales de `legacy_db`.
3. Cuando la aplicación de .NET hace un `SELECT * FROM legacy_fdw.customers WHERE id = 1`, PostgreSQL traduce esa consulta, viaja por la red a `legacy_db`, y devuelve el resultado de forma transparente.

## Limitaciones
- **Escrituras**: Están restringidas intencionalmente a solo lectura.
- **Rendimiento**: Un `JOIN` entre una tabla local y una externa puede ser lento si no se filtran primero los datos en la tabla externa.
