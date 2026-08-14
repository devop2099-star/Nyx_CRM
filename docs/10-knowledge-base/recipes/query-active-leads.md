# Recipe: Consultas SQL Frecuentes para Supervisión de Leads

## Propósito
Snippets de consultas SQL optimizadas para obtener métricas operativas del embudo de ventas directamente en la base de datos de PostgreSQL 16.

---

## 1. Conteo de Leads por Estado en el Mes en Curso

```sql
SELECT 
    status,
    COUNT(*) AS total_leads,
    ROUND(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER (), 2) AS porcentaje
FROM crm.leads
WHERE created_at >= DATE_TRUNC('month', CURRENT_DATE)
GROUP BY status
ORDER BY total_leads DESC;
```

---

## 2. Detección de Leads Estancados (> 48 horas sin gestión)

```sql
SELECT 
    l.id AS lead_id,
    l.dni,
    l.first_name || ' ' || l.last_name AS cliente,
    u.full_name AS asesor_asignado,
    l.created_at,
    NOW() - l.updated_at AS tiempo_sin_gestion
FROM crm.leads l
JOIN auth.users u ON l.assigned_to = u.id
WHERE l.status IN ('New', 'InContact')
  AND l.updated_at < NOW() - INTERVAL '48 hours'
ORDER BY l.updated_at ASC;
```

---

## 3. Top 5 Asesores con Mayor Tasa de Conversión (Leads a Órdenes)

```sql
SELECT 
    u.full_name AS asesor,
    COUNT(l.id) AS total_leads_gestionados,
    COUNT(so.id) AS ordenes_creadas,
    ROUND((COUNT(so.id)::DECIMAL / NULLIF(COUNT(l.id), 0)) * 100, 2) AS tasa_conversion_pct
FROM auth.users u
JOIN crm.leads l ON u.id = l.assigned_to
LEFT JOIN crm.sales_orders so ON l.id = so.lead_id
WHERE l.created_at >= DATE_TRUNC('month', CURRENT_DATE)
GROUP BY u.id, u.full_name
HAVING COUNT(l.id) >= 10
ORDER BY tasa_conversion_pct DESC
LIMIT 5;
```
