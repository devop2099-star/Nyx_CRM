# Gestión de Logs

## Backend (`CRM.ApiHub`)
- **Librería**: Serilog.
- **Destino**: Consola (Docker) y Archivo (rotación diaria).
- **Formato**: JSON (para consumo estructurado) en entornos productivos.

## Visualización
Los logs de contenedores se pueden ver con:
`docker compose logs -f --tail=100 api`

## Política de Retención
Los archivos de log en el servidor se retienen por 14 días. 
Para investigación de incidentes, consultar el sistema centralizado de logs (ej. ELK o Datadog si aplica).
