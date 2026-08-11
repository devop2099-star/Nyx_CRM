# Índices (Indexes)

Para garantizar el rendimiento, especialmente en el módulo de Búsqueda y en los Dashboards, se han definido los siguientes índices estratégicos:

## Índices B-Tree
- **`idx_leads_email`**: Sobre `crm.leads(email)` para búsquedas rápidas al registrar nuevos leads y evitar duplicados.
- **`idx_sales_orders_advisor_status`**: Índice compuesto sobre `crm.sales_orders(advisor_id, status)` para optimizar la carga del dashboard del asesor.

## Índices Parciales
- **`idx_active_users`**: Sobre `auth.users(username)` WHERE `is_active = true`. Optimiza el proceso de login.
