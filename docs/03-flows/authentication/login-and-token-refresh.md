# Flujo de Autenticación y Refresco de Sesión

## Objetivo
Detallar el intercambio de credenciales y la renovación transparente de tokens JWT entre el frontend Blazor y `CRM.ApiHub`.

---

## Diagrama del Flujo

```mermaid
sequenceDiagram
    autonumber
    actor U as Usuario
    participant FE as CRM.WebFrontend (Blazor)
    participant API as CRM.ApiHub (AuthController)
    participant DB as PostgreSQL

    U->>FE: Ingresa usuario y contraseña
    FE->>API: POST /api/v1/auth/login { username, password }
    API->>DB: Consultar usuario y hash de contraseña
    DB-->>API: Datos del usuario (Validados)
    API->>API: Generar JWT (60m) + RefreshToken (7d)
    API-->>FE: 200 OK { token, refreshToken, expiresAt }
    FE->>FE: Guardar token en Storage / Memory
    FE-->>U: Redirección al Dashboard

    note over FE, API: Transcurrido el tiempo de expiración (60m)
    FE->>API: POST /api/v1/auth/refresh { refreshToken }
    API->>DB: Validar RefreshToken no revocado
    DB-->>API: Válido
    API-->>FE: 200 OK { newJwtToken, newRefreshToken }
```
