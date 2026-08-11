# Esquemas de Base de Datos

La base de datos PostgreSQL está particionada lógicamente usando esquemas para separar dominios de negocio.

## Esquemas Principales
- **`public`**: Funciones utilitarias y extensiones.
- **`crm`**: Tablas core del sistema (Leads, Orders).
- **`auth`**: Tablas de gestión de usuarios y roles.
- **`legacy_fdw`**: Esquema virtual mapeado a la base de datos externa usando PostgreSQL FDW.

## Convenciones de Nombres
- Esquemas en minúscula y singular.
- Nombres descriptivos sin prefijos redundantes (ej. `crm`, no `tbl_crm`).
