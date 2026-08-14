# Referencia de Servicios Activos

Lista de servicios en ejecución en el ecosistema de producción.

| Servicio | Nombre en Docker | Puerto Interno | Puerto Expuesto | Descripción |
|---|---|---|---|---|
| **API Backend** | `crm_api` | 5000 | - | .NET Kestrel |
| **Frontend Web** | `crm_web` | 8080 | - | Nginx (Archivos estáticos WASM) |
| **Database** | `crm_db` | 5432 | - | PostgreSQL 16 |
| **Cache** | `crm_redis` | 6379 | - | Redis 7 |
| **Reverse Proxy** | `crm_nginx` | 80 | 80/443 | Nginx Proxy |
| **Certbot** | `crm_certbot` | - | - | Tarea Cron efímera |
