# Contexto del Sistema (System Context)

El CRM no opera de forma aislada. Interactúa con diferentes actores y sistemas externos en el ecosistema corporativo.

## Actores (Usuarios)
- **Asesor**: Usuario final del CRM que gestiona leads y realiza ventas.
- **Supervisor**: Usuario que monitorea el desempeño de los asesores, audita ventas y revisa comisiones.
- **Agente Backoffice**: Usuario técnico que recibe las órdenes de venta y realiza las activaciones técnicas.
- **Administrador**: Usuario de TI que gestiona roles, permisos y configuraciones globales.

## Sistemas Externos (Integraciones)

### 1. Base de Datos Legacy (CRM Anterior / ERP)
- **Tecnología**: PostgreSQL.
- **Conexión**: Vía PostgreSQL FDW (Foreign Data Wrapper).
- **Propósito**: Consulta de clientes históricos, verificación de deudas, e integración de datos legacy sin realizar migraciones masivas.

### 2. GLPI (Sistema de Soporte TI)
- **Tecnología**: API REST.
- **Propósito**: Generación automática de tickets cuando ocurre una incidencia técnica reportada por el usuario (ej. fallo en activación) o por el sistema.

### 3. Servidor SMTP Corporativo
- **Tecnología**: SMTP.
- **Propósito**: Envío de notificaciones transaccionales por correo electrónico (alertas, reseteo de contraseñas, confirmación de ventas).
