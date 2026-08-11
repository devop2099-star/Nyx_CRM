# Monitoreo (Monitoring)

## Herramientas
- **Prometheus**: Recolección de métricas de la API (.NET `prometheus-net`).
- **Grafana**: Dashboards operativos (AHT, Errores 500, Latencia, Estado de FDW).

## Métricas Clave (KPIs Técnicos)
- **FDW Latency**: Si las consultas a `legacy_fdw` superan los 500ms de forma constante.
- **HTTP 5xx Rate**: Alarma si > 1% de las peticiones fallan.
- **Active Connections**: Alertar al alcanzar el 80% del `max_connections` de PostgreSQL.
