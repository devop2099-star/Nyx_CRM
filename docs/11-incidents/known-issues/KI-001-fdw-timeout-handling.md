# KI-001 - Timeouts esporádicos en conexión FDW

## Estado
🔴 Activo

## Descripción
En horas pico (10:00 AM - 12:00 PM), las consultas que hacen JOIN contra la tabla `legacy_fdw.customers` a veces fallan con un timeout de 30 segundos.

## Impacto
Los asesores no pueden cargar el historial completo del cliente durante la llamada.

## Workaround (Mitigación)
Los asesores deben recargar la página (F5) para reintentar la petición. A nivel de infraestructura, se ha implementado un reintento automático (Retry Policy con Polly) en el backend, pero si la BD externa está saturada, seguirá fallando.

## Resolución a Largo Plazo
Migrar los datos maestros de clientes desde el ERP legacy hacia la base de datos propia del CRM mediante un proceso ETL nocturno, eliminando la dependencia en tiempo real del FDW para consultas de historial.
