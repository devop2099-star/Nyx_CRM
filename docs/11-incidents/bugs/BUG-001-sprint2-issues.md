# BUG-001 — Consolidado de Problemas Resueltos en Sprint 2

## Estado
🟢 Resuelto

## Detección
- **Fecha**: Sprint 2
- **Componentes Afectados**: Auth, Form Validation, FDW Queries

---

# 1. Problemas e Investigaciones

### Caso A: Expiración Prematura de Sesión en Blazor
- **Síntoma**: Los usuarios Asesores perdían el token JWT al navegar entre páginas WASM/Server.
- **Causa Raíz**: El `CustomAuthStateProvider` no persistía correctamente el Refresh Token en el almacenamiento local durante el cambio de circuito Blazor.
- **Solución Aplicada**: Implementación de almacenamiento sincronizado en LocalStorage y lógica de retintento en middleware.

### Caso B: Inconsistencia Responsive en Formulario de PreVentas
- **Síntoma**: En pantallas móviles de Asesores, los campos dinámicos de formularios perdían alineación.
- **Causa Raíz**: Estilos inline sin media queries adecuadas en `AsesorOrderDetail.razor`.
- **Solución Aplicada**: Migración a clases CSS unificadas basadas en el sistema de diseño estandarizado.

---

# 2. Knowledge Base Resultante
- [Guía de Recuperación FDW](../../10-knowledge-base/troubleshooting/fdw-connection-recovery.md)
