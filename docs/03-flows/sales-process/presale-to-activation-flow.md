# Flujo E2E: Procesamiento Comercial desde PreVenta hasta Activación

Este documento especifica la secuencia completa del proceso comercial desde la perspectiva del **Asesor de Ventas**, describiendo las transiciones de estado, invocaciones API REST, eventos de dominio y manejo de excepciones en la plataforma CRM Call Center.

---

## 🗺️ Diagrama de Secuencia E2E (Asesor -> Supervisor -> Backoffice)

```mermaid
sequenceDiagram
    autonumber
    actor A as Asesor de Ventas
    participant UI as Blazor WebFrontend
    participant API as CRM.ApiHub
    participant DB as PostgreSQL (crm)
    actor S as Supervisor (Auditoría)
    actor B as Backoffice (Activación)

    Note over A,UI: FASE 1: CAPTURA Y CALIFICACIÓN
    A->>UI: Ingresa DNI del cliente
    UI->>API: POST /api/v1/presales/leads (Verifica DNI & Duplicados)
    API->>DB: Query anti-duplicados 30 días
    DB-->>API: Lead OK (isDuplicate = false)
    API-->>UI: 201 Created (Lead ID, status: New)
    A->>UI: Completa Formulario Dinámico
    UI->>API: POST /api/v1/presales/leads/{id}/qualify
    API-->>UI: 200 OK (status: Qualified)

    Note over A,UI: FASE 2: FORMALIZACIÓN Y DOCUMENTACIÓN
    A->>UI: Convierte a Orden de Venta
    UI->>API: POST /api/v1/orders
    API-->>UI: 201 Created (Order ID, status: PendingDocuments)
    A->>UI: Sube foto Cédula & Contrato
    UI->>API: POST /api/v1/orders/{id}/documents (Multipart)
    API-->>UI: 200 OK (Hash SHA256 verificado)
    A->>UI: Clic "Enviar a Auditoría"
    UI->>API: POST /api/v1/orders/{id}/submit-audit
    API-->>UI: 200 OK (status: PendingAudit)

    Note over S,API: FASE 3: SUPERVISIÓN Y AUDITORÍA
    API-->>S: Notificación SignalR (Nueva orden por auditar)
    S->>API: POST /api/v1/audit/evaluations (Score: 95/100)
    API-->>UI: Evento SalesOrderApproved (status: PendingActivation)

    Note over B,API: FASE 4: ACTIVACIÓN Y COMISIÓN
    B->>API: POST /api/v1/activations/process
    API->>DB: Update status = 'Activated'
    API-->>A: Notificación SignalR: ¡Venta Ganada!
    API-->>DB: Registra comisión en crm.commission_settlements
```

---

## 🔄 Máquina de Estados de la Orden de Venta

```mermaid
stateDiagram-v2
    [*] --> New: Registro de Lead por Asesor
    New --> Qualified: Completa Formulario Cobertura
    New --> Duplicate: Detecta DNI en < 30 días (Bloqueado)
    New --> Discarded: Cliente desiste o sin datos
    Qualified --> Draft: Inicia Orden Comercial
    Draft --> PendingDocuments: Falta foto de Cédula o Contrato
    PendingDocuments --> PendingAudit: Asesor adjunta todos los archivos y presiona "Enviar a Auditoría"
    
    PendingAudit --> PendingActivation: Supervisor aprueba auditoría (Score >= 80)
    PendingAudit --> RejectedAudit: Supervisor devuelve por fotos borrosas o falta de audio
    
    RejectedAudit --> PendingAudit: Asesor corrige adjuntos y Re-Envía
    
    PendingActivation --> Activated: Backoffice completa instalación y aprovisionamiento
    PendingActivation --> RejectedTechnical: Backoffice rechaza por sin puerto NAP
    
    Activated --> [*]: Venta Ganada / Comisión Computada
```

---

## 📋 Detalle Procedimental de las Fases del Asesor

### Fase 1: Recepción y Verificación de Duplicidad (Asesor)
1. El asesor recibe la llamada o contacto del cliente.
2. Presiona `Ctrl + Alt + N` en `AsesorDashboard.razor` e ingresa la cédula.
3. **Validación API**: `POST /api/v1/presales/leads`.
4. Si la respuesta es HTTP 409 (Duplicado), la interfaz bloquea el flujo comercial e indica el ID de la orden original activa.

### Fase 2: Calificación y Formulario Dinámico (Asesor)
1. Si el Lead es nuevo (`New`), el asesor presiona **Calificar Elegibilidad**.
2. Completa las preguntas del formulario dinámico retornado por `GET /api/v1/forms/{id}`.
3. El sistema valida la factibilidad de cobertura y cambia el estado a `Qualified`.

### Fase 3: Formalización y Subida Multipart de Adjuntos (Asesor)
1. El asesor selecciona el plan contratado (`PLAN-FIBRA-300M`, `PLAN-FIBRA-500M`, etc.).
2. Sube la foto del documento de identidad usando la interfaz de `AsesorOrderDetail.razor`.
3. Invoca la API `POST /api/v1/orders/{id}/documents`.
4. Al verificarse los requisitos mínimos, presiona **Enviar a Auditoría**. La orden pasa a `PendingAudit`.

### Fase 4: Manejo de Devoluciones (Asesor)
1. Si el Supervisor rechaza la venta (`Score < 80` o foto ilegible), el asesor recibe una alerta push en tiempo real vía **SignalR**.
2. La orden cambia al estado `RejectedAudit`.
3. El asesor revisa las observaciones del Supervisor en `AsesorOrderDetail.razor`, reemplaza el archivo defectuoso y presiona **Re-Enviar a Auditoría**.

### Fase 5: Confirmación de Activación y Comisión (Asesor)
1. Una vez que Backoffice completa la instalación física, la orden pasa a `Activated`.
2. El sistema envía una notificación al Asesor e incrementa su contador de ventas mensual en el módulo de comisiones (`/commissions/summary`).
