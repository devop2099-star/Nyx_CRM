# Arquitectura de Base de Datos — PostgreSQL & FDW

## 1. Motor de Base de Datos
- **Motor Principal**: PostgreSQL 16
- **Mapeador ORM**: Entity Framework Core 8 (`Npgsql.EntityFrameworkCore.PostgreSQL`)

---

## 2. Esquemas de Base de Datos

| Esquema | Propósito |
|---|---|
| `public` | Tablas locales del CRM (Usuarios, PreVentas, Órdenes, Comisiones, Auditorías, Incidentes). |
| `fdw_legacy` | Tablas foráneas vinculadas mediante Foreign Data Wrapper hacia sistemas externos transaccionales. |
| `audit` | Histórico y logs de cambios de estado auditados por disparadores y repositorios. |

---

## 3. Tablas Principales (`public`)
- `Users`: Usuarios, roles, passwords encriptadas (BCrypt/Argon2) y estado.
- `PreSales`: Registro preliminar de prospectos.
- `SalesOrders`: Órdenes comerciales formalizadas.
- `AuditAudios`: Auditoría de grabaciones de llamadas.
- `Commissions`: Comisiones liquidadas por venta efectiva.
- `KbArticles`: Artículos de la base de conocimiento interna.
- `Incidents`: Reportes de errores e incidentes técnicos/operativos.

---

## 4. Resiliencia FDW
Las tablas de `fdw_legacy` cuentan con un interceptor de EF Core y políticas de reintento (`ResiliencePipeline`) para reestablecer automáticamente la conexión en caso de micro-cortes de red con el servidor remoto.
