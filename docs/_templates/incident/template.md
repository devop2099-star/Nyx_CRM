# INC-[XXX] — [Título del Incidente de Servicio]

## Estado
Abierto / Investigando / Mitigado / Resuelto / Cerrado

## Severidad & Nivel de Impacto
- **Severidad:** P1 (Crítica) / P2 (Alta) / P3 (Media) / P4 (Baja)
- **Servicios Afectados:** API Hub / Base de Datos / Redis / Blazor Frontend / Integración Externa
- **Usuarios / Roles Afectados:** Asesores / Supervisores / Backoffice / Clientes Finales
- **Fecha & Hora de Inicio (UTC-5):** YYYY-MM-DD HH:MM
- **Fecha & Hora de Mitigación (UTC-5):** YYYY-MM-DD HH:MM
- **Tiempo Total de Interrupción (Downtime):** XX minutos

---

# 1. Resumen Ejecutivo
Descripción breve y concisa de lo sucedido, el impacto operacional y la resolución aplicada para la gerencia técnica.

---

# 2. Cronología de Eventos (Timeline)
- **HH:MM** — Se detecta anomalía en métricas / alerta de monitoreo Prometheus / reporte de usuarios.
- **HH:MM** — Se inicia la investigación técnica por el equipo de guardia.
- **HH:MM** — Se identifica el origen del fallo (ej. saturación de conexiones en PostgreSQL).
- **HH:MM** — Se aplica acción de mitigación inmediata (ej. reinicio de contenedor / failover).
- **HH:MM** — Se confirma la recuperación del servicio y validación de endpoints de salud `/healthz`.

---

# 3. Causa Raíz (Root Cause Analysis - RCA)
Explicación técnica detallada del motivo exacto por el cual ocurrió el incidente (con nivel de certeza: Confirmada / Probable / No Identificada).

---

# 4. Acciones de Mitigación y Resolución
1. **Mitigación Temporal:** Acciones tomadas para restablecer el servicio inmediatamente.
2. **Solución Definitiva:** Correcciones de código, configuración o infraestructura para prevenir su recurrencia.

---

# 5. Medidas Preventivas & Tareas de Seguimiento
- [ ] Implementar alerta de monitoreo adicional en Prometheus/Grafana.
- [ ] Actualizar política de reintentos en Polly / timeout de conexión.
- [ ] Documentar guía de procedimiento en la Knowledge Base.

---

# 6. Referencias & Enlaces
- **Guía de Knowledge Base:** [Runbook / Troubleshooting](../../10-knowledge-base/troubleshooting/...)
- **Pull Request / Commit de Corrección:** #...
- **Dashboard de Métricas:** Grafana Dashboard
