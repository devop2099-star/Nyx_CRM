# Docker y Contenedores

El proyecto está dockerizado para facilitar despliegues reproducibles.

## Imágenes
- **API (CRM.ApiHub)**: Se construye usando un multi-stage build de .NET 8 (SDK para compilar, ASP.NET runtime para correr).
- **Frontend (CRM.WebFrontend)**: Multi-stage con Nginx como servidor web estático (si es WASM) o contenedor ASP.NET Core (si es Blazor Server).

## Docker Compose
Existe un archivo `docker-compose.yml` en la raíz del proyecto para entornos locales:
- `api`: Puerto 5000.
- `db`: PostgreSQL 16 local con configuración predefinida de usuario/password.
- `redis`: Redis 7 para cacheo distribuido.

### Comandos
- Levantar entorno local: `docker compose up -d`
- Ver logs: `docker compose logs -f api`
- Bajar entorno: `docker compose down`
