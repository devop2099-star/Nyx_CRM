# ADR-002 — Integración de PostgreSQL FDW (Foreign Data Wrapper) para Persistencia Unificada

## Estado
Aceptado

## Fecha
2026-08-02

## Contexto
El sistema CRM requiere interactuar con datos legacy y bases de datos transaccionales externas de telecomunicaciones sin duplicar físicamente la información ni depender de procesos ETL síncronos complejos.

## Decisión
Utilizar la extensión **Foreign Data Wrapper (FDW)** de PostgreSQL (`postgres_fdw`) para consultar y sincronizar tablas foráneas directamente en la capa de base de datos primaria del CRM.

## Consecuencias
- **Positivas**: Acceso transparente a datos externos desde EF Core como si fueran tablas locales.
- **Negativas**: Sensibilidad a la latencia de red y desconexiones temporales entre la BD primaria del CRM y el servidor foráneo (resuelto con mecanismo de resiliencia y reconexión en la capa de repositorios).
