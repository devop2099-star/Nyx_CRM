# Enlaces & Accesos Directos del Sistema — CRM Call Center

Directorio de herramientas, consolas de administración, documentación interactiva y tableros de observabilidad.

---

## 🌐 Entornos del Sistema

| Servicio / Herramienta | Entorno de Staging (Pruebas) | Entorno de Producción |
|---|---|---|
| **Portal Web (Blazor Frontend)** | `https://staging-crm.callcenter.local` | `https://crm.callcenter.local` |
| **API Swagger UI (OpenAPI 3.0)** | `https://staging-crm.callcenter.local/swagger` | Deshabilitado en Producción por seguridad |
| **Endpoint de Salud (Healthz)** | `https://staging-crm.callcenter.local/healthz` | `https://crm.callcenter.local/healthz` |
| **Mesa de Ayuda GLPI** | `https://glpi-test.callcenter.local` | `https://glpi.callcenter.local` |
| **Panel de Métricas (Grafana)** | `https://grafana.callcenter.local:3000` | `https://grafana.callcenter.local:3000` |
| **Servidor de Correo (MailHog Test)**| `http://localhost:8025` (Local) | Servidor SMTP Corporativo |

---

## 📚 Documentación Técnica Local
- **[Portal Maestro de Documentación](../index.html)**: `http://localhost:8080/index.html`
- **[Diagrama de Arquitectura C4](../09-visual-docs/diagrams/system-architecture-diagram.html)**
- **[Diagrama ERD de Base de Datos](../09-visual-docs/diagrams/database-er-diagram.html)**
- **[Walkthrough Interactivo del Ciclo de Venta](../09-visual-docs/walkthroughs/asesor-sales-walkthrough.html)**
