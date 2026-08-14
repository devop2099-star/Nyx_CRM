# Componentes del Sistema

El CRM se divide en varios componentes lógicos y físicos.

## Componentes Lógicos

### 1. CRM.ApiHub (Backend)
Es la API REST central construida en .NET 8 que expone todas las funcionalidades del sistema. Contiene toda la lógica de negocio, validaciones y acceso a datos.

### 2. CRM.WebFrontend (Frontend)
Aplicación web desarrollada en Blazor Server/WebAssembly que provee la interfaz gráfica para Asesores, Supervisores y Backoffice. Se comunica exclusivamente con el `CRM.ApiHub`.

### 3. Identity Provider
Componente integrado en `CRM.ApiHub` que maneja la autenticación y emisión de tokens JWT.

### 4. Background Workers
Tareas en segundo plano encargadas de procesos asíncronos como el cálculo diferido de comisiones, envío de notificaciones y limpieza de registros.

## Diagrama de Componentes
*(Ver `diagrams/architecture-overview.drawio`)*
