# Entornos e Infraestructura

## 1. Entornos del Sistema

| Entorno | Servidor / Host | Backend URL | Frontend URL | Base de Datos |
|---|---|---|---|---|
| **Development** | `localhost` | `http://localhost:5000` | `http://localhost:5200` | PostgreSQL local (`crm_dev`) |
| **Staging / QA** | `qa.crm.internal` | `https://api-qa.crm.internal` | `https://qa.crm.internal` | PostgreSQL QA + FDW Staging |
| **Production** | `crm.empresa.com` | `https://api.crm.empresa.com` | `https://crm.empresa.com` | Cluster PostgreSQL Prod HA |

---

## 2. Docker & Contenedores
El proyecto dispone de un `docker-compose.yml` base para instanciar el entorno de desarrollo y pruebas:

```yaml
version: '3.8'
services:
  crm.apihub:
    build:
      context: .
      dockerfile: CRM.ApiHub/Dockerfile
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=crm_db;Username=crm_user;Password=secret

  crm.webfrontend:
    build:
      context: .
      dockerfile: CRM.WebFrontend/Dockerfile
    ports:
      - "5200:80"
```
