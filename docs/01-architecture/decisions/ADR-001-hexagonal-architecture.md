# ADR-001 — Adopción de Arquitectura Hexagonal en CRM.ApiHub

## Estado
Aceptado

## Fecha
2026-08-01

## Contexto
El backend `CRM.ApiHub` debe soportar lógica de negocio compleja para la gestión de ventas, auditoría de llamadas y reglas de comisiones. Se requiere un diseño acoplable a múltiples fuentes de datos (PostgreSQL, tablas foráneas FDW, servicios de ticketing externo GLPI) sin que la base de datos o frameworks afecten la mantenibilidad.

## Decisión
Implementar el patrón de **Arquitectura Hexagonal (Ports and Adapters)** organizando el proyecto en:
- `Domain`: Entidades puras e interfaces de puertos.
- `Application`: Casos de uso.
- `Infrastructure`: Adaptadores de acceso a datos PostgreSQL, FDW y servicios externos.
- `Api`: Adaptadores HTTP Controllers.

## Consecuencias
- **Positivas**: Facilidad para realizar pruebas unitarias del núcleo sin base de datos real; desacoplamiento de ORM y controladores.
- **Negativas**: Mayor número de proyectos e interfaces en la solución `.sln`.
