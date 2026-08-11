# Chuleta de Comandos Técnicos Frecuentes — CRM.ApiHub

Referencia rápida de comandos de terminal para desarrollo, base de datos, pruebas y despliegue.

---

## 🛠️ Comandos de Desarrollo .NET 8 CLI

```bash
# Compilar toda la solución en modo Release
dotnet build -c Release

# Ejecutar la API en modo desarrollo con recarga en caliente (Hot Reload)
dotnet watch --project src/CRM.ApiHub

# Ejecutar todas las pruebas unitarias y de integración
dotnet test --logger "console;verbosity=detailed"

# Generar reporte de cobertura de código
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

---

## 🗄️ Migraciones de Base de Datos (EF Core Tools)

```bash
# Crear una nueva migración
dotnet ef migrations add AddCommissionTables --project src/CRM.Infrastructure --startup-project src/CRM.ApiHub

# Aplicar migraciones pendientes a la base de datos
dotnet ef database update --project src/CRM.Infrastructure --startup-project src/CRM.ApiHub

# Generar script SQL puro de migraciones sin aplicar directamente
dotnet ef migrations script -i -o migrations_script.sql --project src/CRM.Infrastructure --startup-project src/CRM.ApiHub
```

---

## 🐳 Contenedores Docker & Docker Compose

```bash
# Iniciar todos los servicios en segundo plano (API, Postgres, Redis, Nginx)
docker compose up -d

# Ver logs en tiempo real del contenedor de la API
docker compose logs -f api

# Reiniciar el servicio de base de datos
docker compose restart postgres

# Acceder al shell interactivo de PostgreSQL dentro del contenedor
docker compose exec postgres psql -U crm_app -d crm_database

# Detener todos los contenedores sin eliminar volúmenes
docker compose down
```

---

## ⚡ Comandos Útiles de Redis CLI

```bash
# Conectar a la consola de Redis
redis-cli -h 127.0.0.1 -p 6379 -a SuperSecretRedisPassword

# Comprobar estado de conectividad
PING
# Retorna: PONG

# Ver claves activas de la lista negra de JWT
KEYS "blacklist:jwt:*"

# Ver tiempo de vida restante de un token revocado (en segundos)
TTL "blacklist:jwt:a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"
```
