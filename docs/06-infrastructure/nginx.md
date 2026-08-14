# NGINX (Reverse Proxy)

## Propósito
En el entorno de Producción y Staging, `nginx` actúa como proxy reverso frente a las instancias Kestrel de la API de .NET. 

## Responsabilidades
- **Terminación SSL**: Nginx maneja los certificados TLS y desencripta el tráfico antes de pasarlo al backend en texto plano.
- **Balanceo de Carga**: (Opcional) balancea peticiones si se corren múltiples réplicas de `api`.
- **Reglas de Seguridad**:
  - `X-Frame-Options: DENY`
  - `X-Content-Type-Options: nosniff`
  - Limitar tamaño de payload (client_max_body_size) a 10MB para prevenir ataques de denegación de servicio.

## Ubicación de la Configuración
Los archivos `.conf` se encuentran en el repositorio en `deploy/nginx/`.
