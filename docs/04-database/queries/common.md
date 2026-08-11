# Consultas Comunes (Common Queries)

## 1. Obtener leads recientes sin asignar
```sql
SELECT id, first_name, last_name, email, created_at
FROM crm.leads
WHERE status = 'New' AND advisor_id IS NULL
ORDER BY created_at DESC
LIMIT 50;
```

## 2. Ventas del día por asesor
```sql
SELECT u.username, COUNT(o.id) as total_sales, SUM(o.amount) as total_amount
FROM crm.sales_orders o
JOIN auth.users u ON o.advisor_id = u.id
WHERE o.created_at >= CURRENT_DATE
GROUP BY u.username;
```
