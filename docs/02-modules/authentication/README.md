# Módulo de Autenticación y Autorización

## Propósito
Gestiona el acceso seguro de usuarios (Asesores, Supervisores, Backoffice, Administradores) al sistema CRM mediante tokens JWT.

## Componentes Involucrados
- **Backend (`CRM.ApiHub`)**:
  - `AuthController.cs`: Endpoints `/api/v1/auth/login`, `/api/v1/auth/refresh`, `/api/v1/auth/me`.
  - `JwtTokenService.cs`: Generación y validación de tokens JWT.
- **Frontend (`CRM.WebFrontend`)**:
  - `Login.razor`: Vista de inicio de sesión.
  - `CustomAuthStateProvider.cs`: Proveedor de estado de autenticación en Blazor.

## Endpoints del Módulo

| Método | Endpoint | Descripción | Acceso |
|---|---|---|---|
| POST | `/api/v1/auth/login` | Autentica usuario y retorna JWT + Refresh Token | Público |
| POST | `/api/v1/auth/refresh` | Renueva el JWT utilizando un Refresh Token válido | Público |
| GET | `/api/v1/auth/me` | Obtiene el perfil del usuario autenticado actual | Autenticado |

## Reglas de Negocio
1. La clave debe tener mínimo 8 caracteres con complejidad (mayúscula, número, símbolo).
2. Tras 5 intentos fallidos consecutivos, la cuenta se bloquea por 15 minutos.
3. El token JWT tiene una vigencia de 60 minutos; el Refresh Token de 7 días.

## Documentación Visual
- 🌐 [Abrir Guía Visual del Módulo](index.html)

