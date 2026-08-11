# Gestión de Secretos

## Entorno Local (Desarrollo)
Se utiliza la herramienta `Secret Manager` (`dotnet user-secrets`) de .NET para no comitear contraseñas.

## Entornos de Servidor (Producción)
Las variables sensibles se inyectan como variables de entorno a través del orquestador de contenedores (Docker Compose / Swarm).

## Secretos Críticos
- `Jwt:Key`: Llave privada para firmar los tokens.
- `ConnectionStrings:DefaultConnection`: Credenciales del usuario de base de datos.
- `Smtp:Password`: Credenciales de correo.

> [!CAUTION]
> Ninguno de estos valores debe existir en el código fuente ni en los archivos de configuración `.json` del repositorio.
