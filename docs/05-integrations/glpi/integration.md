# Integración con GLPI Ticketing System

## Propósito
Permite escalar casos técnicos e incidentes desde la consola de administración del CRM hacia la plataforma de soporte corporativo GLPI mediante su API REST.

## Autenticación
- **Método**: App-Token + User-Token vía encabezados HTTP:
  - `App-Token: <GLPI_APP_TOKEN>`
  - `Session-Token: <GLPI_SESSION_TOKEN>`

## Endpoints Utilizados
- `POST /apirest.php/initSession`
- `POST /apirest.php/Ticket` — Creación de ticket de soporte técnico.
