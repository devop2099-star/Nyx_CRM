# Arquitectura Hexagonal en CRM

## Principio General
El sistema `CRM.ApiHub` está diseñado bajo el patrón de **Arquitectura Hexagonal** (Ports and Adapters) para mantener una separación estricta entre las reglas de negocio (Domain) y los detalles técnicos (Infrastructure / Presentation).

## Capas del Sistema

### 1. Domain (Núcleo)
Contiene las reglas de negocio puras.
- **Entities**: Objetos de negocio con identidad (ej. `Lead`, `SalesOrder`).
- **Value Objects**: Conceptos sin identidad propia (ej. `Money`, `EmailAddress`).
- **Domain Events**: Eventos disparados cuando ocurren cambios importantes.
- **Repository Interfaces**: Puertos de salida que definen *qué* datos se necesitan persistir, pero no *cómo*.

### 2. Application (Casos de Uso)
Orquesta el flujo de negocio sin conocer detalles técnicos.
- **Commands / Queries**: Patrón CQRS para separar lecturas de escrituras.
- **Handlers**: Ejecutan la lógica del caso de uso.
- **DTOs**: Modelos de datos para entrada/salida.

### 3. Infrastructure (Adaptadores de Salida)
Implementa los detalles técnicos.
- **Repositories**: Implementación concreta usando Entity Framework Core y PostgreSQL FDW.
- **External Services**: Integración con GLPI, SMTP.
- **Identity**: Gestión de tokens JWT y roles.

### 4. Presentation (Adaptadores de Entrada)
El punto de entrada al sistema.
- **API Controllers (CRM.ApiHub)**: Reciben peticiones HTTP y despachan Commands/Queries.
- **Blazor Frontend (CRM.WebFrontend)**: Consume la API y maneja la interacción con el usuario.

## Regla de Dependencia
El código siempre debe depender hacia adentro (hacia el Dominio). El Dominio **no debe** tener ninguna referencia a las capas exteriores.
