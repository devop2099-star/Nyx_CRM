# Tablas Principales

## Esquema `crm`

### `crm.leads`
Almacena los prospectos o clientes potenciales.
- **Clave primaria**: `id` (UUID)
- **Campos clave**: `first_name`, `last_name`, `email`, `phone`, `status`.

### `crm.sales_orders`
Órdenes de venta generadas por los asesores.
- **Clave primaria**: `id` (UUID)
- **Campos clave**: `lead_id` (FK), `advisor_id` (FK), `amount`, `status`, `created_at`.

## Esquema `auth`

### `auth.users`
Usuarios del sistema.
- **Clave primaria**: `id` (UUID)
- **Campos clave**: `username`, `password_hash`, `role_id`, `is_active`.
