# Migraciones (Migrations)

El sistema utiliza **Entity Framework Core Migrations** para gestionar los cambios en el esquema de la base de datos.

## Estrategia
- Las migraciones se generan automáticamente a partir del modelo de dominio.
- No se ejecutan automáticamente al iniciar la aplicación en Producción. Se ejecutan como parte del pipeline de despliegue continuo (CI/CD) utilizando comandos idempotentes.

## Comandos Útiles
- **Generar migración**: `dotnet ef migrations add <Name> --project src/CRM.Infrastructure`
- **Generar script SQL**: `dotnet ef migrations script --project src/CRM.Infrastructure`
- **Aplicar migración local**: `dotnet ef database update --project src/CRM.Infrastructure`
