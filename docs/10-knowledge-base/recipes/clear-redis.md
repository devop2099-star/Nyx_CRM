# Borrar cache de Redis de forma segura

A veces es necesario purgar la cache si se ha subido nueva parametrización que la aplicación no está tomando.

## Comando
```bash
docker compose exec redis redis-cli FLUSHALL
```
> [!CAUTION]
> En entornos de producción, si se usa la misma instancia de Redis para session-state, esto cerrará la sesión de todos los usuarios.
