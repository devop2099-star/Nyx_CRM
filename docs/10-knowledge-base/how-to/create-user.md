# Cómo crear un nuevo usuario manualmente

## Objetivo
Instrucciones para dar de alta un usuario cuando el frontend de administración no está disponible o para un super admin de emergencia.

## Pasos
1. Generar hash de la contraseña (utilizando bcrypt localmente o script de utilería).
2. Conectar a PostgreSQL.
3. Ejecutar:
```sql
INSERT INTO auth.users (id, username, password_hash, role_id, is_active)
VALUES (gen_random_uuid(), 'admin_emergencia', '$2a$12$e...', 1, true);
```
