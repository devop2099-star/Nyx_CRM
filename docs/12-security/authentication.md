# Autenticación

El sistema utiliza **JSON Web Tokens (JWT)** para la autenticación de usuarios.

## Flujo
1. El usuario envía sus credenciales al endpoint `/api/auth/login`.
2. Si son válidas, la API devuelve un JWT firmado (AccessToken) y un RefreshToken.
3. El Frontend almacena el AccessToken en memoria o localStorage y lo envía en el header `Authorization: Bearer <token>` en cada petición.

## Seguridad del Token
- **Algoritmo**: HS256.
- **Expiración**: 15 minutos (AccessToken), 7 días (RefreshToken).
- **Issuer/Audience**: Validados estrictamente.
