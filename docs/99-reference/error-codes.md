# Referencia: Catálogo de Códigos de Error Propios de la API

## Formato Estándar de Respuesta de Error
Todas las respuestas de error de `CRM.ApiHub` retornan la estructura `ProblemDetails`:

```json
{
  "code": "ERR-AUTH-001",
  "title": "Credenciales inválidas",
  "status": 401,
  "detail": "El usuario o la contraseña especificados son incorrectos."
}
```

---

## Catálogo de Códigos

| Código | Estado HTTP | Descripción | Acción Recomendada |
|---|---|---|---|
| `ERR-AUTH-001` | 401 | Credenciales de usuario inválidas | Verificar usuario/clave |
| `ERR-AUTH-002` | 401 | Token JWT expirado o inválido | Ejecutar refresco de token |
| `ERR-AUTH-003` | 403 | Permisos insuficientes (Rol) | Solicitar permisos al Admin |
| `ERR-ORDER-001` | 400 | Falta documento de identidad adjunto | Cargar documento en orden |
| `ERR-ORDER-002` | 404 | Orden de venta no encontrada | Verificar ID de orden |
| `ERR-FDW-500` | 503 | Timeout o desorganización en FDW | Consultar guía KB de FDW |
