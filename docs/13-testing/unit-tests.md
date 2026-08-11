# Pruebas Unitarias (Unit Tests)

## Estrategia
Las pruebas unitarias cubren exclusivamente la capa de `Domain` y los Handlers de `Application`. No tocan la base de datos ni servicios externos.

## Librerías
- **xUnit**: Framework principal.
- **Moq**: Para crear mocks de repositorios y servicios de infraestructura.
- **FluentAssertions**: Para assertions más legibles.

## Ejecución
```bash
dotnet test src/CRM.Tests/UnitTests/
```

## Cobertura Mínima
Se requiere un 80% de code coverage en la capa de Domain para que el PR sea aprobado en CI.
