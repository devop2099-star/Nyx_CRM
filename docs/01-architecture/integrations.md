# Arquitectura de Integraciones Externas — CRM.ApiHub

Este documento describe la estrategia de comunicación, desacoplamiento y resiliencia entre el núcleo del CRM y los 4 sistemas satélite principales.

---

## 🗺️ Mapa Arquitectónico de Servicios Satélite

```
+-------------------------------------------------------------------------+
|                              CRM.ApiHub                                 |
|                                                                         |
|  +--------------------+  +--------------------+  +-------------------+  |
|  |  PostgresFdwAdapter|  |   GlpiHttpClient   |  |   SmtpMailAdapter |  |
|  +---------+----------+  +---------+----------+  +---------+---------+  |
+------------|-----------------------|-----------------------|------------+
             | TCP 5432              | HTTPS / Bearer        | TCP 587 TLS
             v                       v                       v
+------------------------+  +--------------------+  +---------------------+
|  Legacy PostgreSQL DB  |  |  GLPI Service Desk |  | Mail Server (SMTP)  |
|  (postgres_fdw driver) |  |  (REST API JSON)   |  | (MailKit STARTTLS)  |
+------------------------+  +--------------------+  +---------------------+
```

---

## 📋 Catálogo de Integraciones

### 1. PostgreSQL Foreign Data Wrapper (`postgres_fdw`)
- **Propósito:** Consulta federada y de solo lectura de clientes históricos y cuentas por cobrar en el sistema ERP legacy.
- **Mecanismo:** Extensión nativa `postgres_fdw` montada en el esquema `legacy_fdw` dentro de la base de datos principal de PostgreSQL 16.
- **Resiliencia:** Vistas materializadas locales con refresco periódico en caso de latencia o corte de red hacia el servidor remoto.

### 2. Mesa de Ayuda GLPI (REST API)
- **Propósito:** Apertura automática y sincronización de tickets de fallas técnicas reportadas por clientes.
- **Protocolo:** HTTP REST sobre TLS 1.3 con cabecera `Authorization: Bearer <token>`.
- **Resiliencia:** Política de reintentos con `Polly` (3 intentos con backoff exponencial) y encolado en base de datos si el servicio no responde.

### 3. Servidor de Correo SMTP Transaccional
- **Propósito:** Envío de copias de contratos de venta en PDF al cliente y notificaciones de liquidaciones a la fuerza de ventas.
- **Protocolo:** SMTP STARTTLS en puerto 587 autenticado.
- **Librería:** `MailKit` en modo asíncrono con cola de despacho en segundo plano (`IHostedService`).

### 4. Telefonía VoIP Asterisk (Audio Streaming)
- **Propósito:** Transmisión de grabaciones de llamadas telefónicas para la auditoría de calidad de los supervisores.
- **Mecanismo:** URLs firmadas de acceso temporal (`Presigned Streaming URLs`) reproducibles desde `AudioPlayer.razor`.
