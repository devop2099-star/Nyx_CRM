# Visión General del Proyecto — CRM Call Center

## 1. Propósito del Sistema
El sistema CRM es una plataforma integral orientada al entorno de Call Center para la gestión, seguimiento, auditoría y activación de productos y servicios de telecomunicaciones/comerciales.

Permite canalizar el ciclo de vida completo del cliente:
1. **Captura y Gestión de Leads / PreVentas**: Registro de prospectos por parte de los Asesores.
2. **Generación de Órdenes de Venta**: Formalización de contratos y anexos.
3. **Validación y Auditoría**: Revisión por parte del área de Supervisión (auditoría de llamadas y formularios).
4. **Procesamiento en Backoffice y Activación**: Gestión de aprobaciones finales y aprovisionamiento.
5. **Cálculo de Comisiones e Indicadores**: Métricas operativas en tiempo real.

---

## 2. Componentes de la Solución Monorepo

| Proyecto | Tipo | Tecnología | Rol Principal |
|---|---|---|---|
| `CRM.ApiHub` | Backend REST API | .NET 8 Web API | Lógica de negocio, persistencia, autenticación JWT, integración FDW PostgreSQL |
| `CRM.WebFrontend` | Web App | Blazor (.NET 8) | Interfaz gráfica para Asesores, Supervisores y Administradores de Backoffice |
| `CRM.WebFrontend.Client` | WASM Frontend | Blazor WebAssembly | Componentes interactivos client-side de la aplicación web |

---

## 3. Roles de Usuario
- **Asesor de Ventas**: Registra llamadas, formularios de pre-venta y gestiona sus prospectos.
- **Supervisor de Operaciones**: Audita grabaciones de audio, revisa solicitudes de pre-venta y aprueba o rechaza órdenes.
- **Analista de Backoffice / Activaciones**: Tramita la activación final de los servicios con proveedores.
- **Administrador del Sistema**: Gestiona usuarios, parámetros del sistema y la Base de Conocimiento (KB).

---

## 4. Límites y Alcance (Lo que NO hace)
- **NO realiza marcación telefónica directa (PBX/SIP)**: Se conecta o recibe referencias de auditoría de audio, pero no actúa como Softphone primario.
- **NO almacena datos bancarios ni tarjetas de crédito en texto plano**: Las transacciones se delegan a las plataformas receptoras.
