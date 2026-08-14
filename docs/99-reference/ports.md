# Referencia:Puertos del Sistema

| Servicio / Proceso | Puerto | Protocolo | Uso |
|---|---|---|---|
| `CRM.ApiHub` (HTTP) | `5000` | HTTP | Backend REST API (Dev) |
| `CRM.ApiHub` (HTTPS) | `5001` | HTTPS | Backend REST API SSL (Dev) |
| `CRM.WebFrontend` (HTTP) | `5200` | HTTP | Frontend Blazor Web App (Dev) |
| `CRM.WebFrontend` (HTTPS) | `5201` | HTTPS | Frontend Blazor Web App SSL (Dev) |
| `PostgreSQL` | `5432` | TCP | Base de Datos Principal CRM |
| `PostgreSQL Legacy` | `5433` | TCP | Base de Datos Transaccional FDW |
| `Nginx` (Reverse Proxy) | `80` / `443` | HTTP/HTTPS | Entrada única en Producción |
