# Breaking Changes (Cambios Rompedores)

Registro de actualizaciones que requieren intervención manual o que rompen compatibilidad con clientes existentes.

## v1.0.0
- **Base de Datos**: Cambio del tipo de dato `id` en `crm.leads` de `INT` a `UUID`. Requiere migración masiva de la data histórica.
- **API**: Eliminación del endpoint `/api/v1/users/login`, reemplazado por `/api/auth/token`.
