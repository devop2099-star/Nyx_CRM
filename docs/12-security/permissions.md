# Permisos Granulares

Para escenarios donde el rol no es suficiente (por ejemplo, "Un asesor solo puede editar un Lead si el estado no es 'Cerrado'"), se utilizan **Resource-based Authorization Policies**.

## Políticas Actuales
- `CanEditLeadPolicy`: Verifica que el Lead pertenezca al usuario actual y que su estado lo permita.
- `CanViewOrderPolicy`: Un Asesor solo ve sus órdenes; un Supervisor ve las de su equipo.

*Los Handlers de estas políticas se encuentran en `CRM.ApiHub/Security/Policies/`.*
