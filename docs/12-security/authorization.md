# Autorización

La autorización está basada en **Roles (Role-Based Access Control - RBAC)**.

## Roles Definidos
- `Admin`: Acceso total.
- `Supervisor`: Lectura de todos los asesores, aprobación de auditorías.
- `Asesor`: Lectura/Escritura solo sobre leads y órdenes propias.
- `Backoffice`: Lectura de órdenes de todos, escritura para marcarlas como activadas.

## Implementación
En la API de .NET, se utilizan los atributos `[Authorize(Roles = "Supervisor")]` en los controladores y endpoints.
