# Relaciones y Foreign Keys

## Relaciones Principales

- **`crm.sales_orders.lead_id` → `crm.leads.id`**
  - Una orden de venta pertenece obligatoriamente a un lead.
  - ON DELETE RESTRICT: No se puede borrar un lead si tiene órdenes asociadas.

- **`crm.sales_orders.advisor_id` → `auth.users.id`**
  - Una orden está asignada a un asesor.

- **`legacy_fdw.customers`**
  - Las consultas a clientes históricos se hacen de forma cruzada, usando el DNI/Documento como llave de búsqueda lógica, no como FK física.
