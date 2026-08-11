# Pruebas de Integración

## Alcance
Valida que la aplicación (`CRM.ApiHub`) se comunique correctamente con la base de datos (PostgreSQL) y servicios externos simulados.

## Entorno de Prueba
Se utiliza `Testcontainers` para levantar instancias efímeras de PostgreSQL en Docker durante la ejecución de las pruebas.

## Ejecución
```bash
dotnet test src/CRM.Tests/IntegrationTests/
```
> [!NOTE]
> Requiere Docker corriendo en la máquina local o el runner de CI.
