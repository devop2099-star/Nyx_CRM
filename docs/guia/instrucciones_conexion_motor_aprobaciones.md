# Instructivo de Integración para Frontend: Motor Autónomo de Aprobaciones

Este instructivo describe el funcionamiento del motor de aprobaciones **Nyx.ApprovalEngine** y cómo debe conectarse la interfaz de usuario para interactuar con la matriz de firmas por departamento (*Data & Analytics*, *Finanzas*, *Gerencia*, etc.) en tiempo real.

---

## 1. Arquitectura del Motor (Nyx.ApprovalEngine)
El motor de aprobaciones opera como un microservicio independiente en el puerto `5081`:

*   **Matriz de Aprobación Multinivel**: Permite registrar solicitudes de aprobación requeridas por múltiples departamentos (ej: Data & Analytics, Finanzas, Gerencia).
*   **Aprobaciones en Paralelo**: Cada departamento puede revisar, aprobar o rechazar su ítem de firma de forma autónoma e independiente sin requerir un orden secuencial rígido.
*   **Banderas de Bloqueo Granulares**:
    *   `blocksProgress` (`true`): Impide que la entidad (ej: Orden de Venta) avance de etapa si el departamento no ha firmado.
    *   `blocksCommission` (`true`): Retiene el cálculo de comisiones.
    *   `blocksActivation` (`true`): Retiene la orden de alta de servicio.
    *   `blocksLiquidation` (`true`): Retiene la liquidación de pago.

---

## 2. Contratos y Endpoints de la API

El frontend consumirá los endpoints expuestos directamente por el motor o enrutados a través de la API principal (`CRM.ApiHub`).

### A. Consultar Matriz de Aprobaciones Requeridas de una Entidad
Recupera la solicitud activa y el estado de firma por departamento para un pedido o entidad.

*   **URL**: `/api/engine/requests/{entityType}/{entityId}` *(ej: `/api/engine/requests/Order/4821`)*
*   **Método**: `GET`
*   **Respuesta Exitosa (200 OK)**:
    ```json
    [
      {
        "request": {
          "id": 1,
          "entityType": "Order",
          "entityId": "4821",
          "triggerStageId": 3,
          "overallStatus": "PENDING",
          "requestedBy": 12,
          "reason": "Venta requiere aprobación de estructura y descuento especial",
          "createdAt": "2026-08-13T12:00:00Z",
          "updatedAt": "2026-08-13T12:00:00Z"
        },
        "items": [
          {
            "id": 101,
            "approvalRequestId": 1,
            "approvalCatalogId": 1,
            "department": "Data & Analytics",
            "isMandatory": true,
            "blocksProgress": false,
            "blocksCommission": false,
            "blocksActivation": false,
            "blocksLiquidation": false,
            "status": "APPROVED",
            "comments": "Estructura validada correctamente",
            "resolvedBy": 5,
            "resolvedAt": "2026-08-13T12:05:00Z"
          },
          {
            "id": 102,
            "approvalRequestId": 1,
            "approvalCatalogId": 2,
            "department": "Finanzas",
            "isMandatory": true,
            "blocksProgress": false,
            "blocksCommission": true,
            "blocksActivation": false,
            "blocksLiquidation": true,
            "status": "PENDING",
            "comments": null,
            "resolvedBy": null,
            "resolvedAt": null
          },
          {
            "id": 103,
            "approvalRequestId": 1,
            "approvalCatalogId": 3,
            "department": "Gerencia",
            "isMandatory": true,
            "blocksProgress": true,
            "blocksCommission": true,
            "blocksActivation": true,
            "blocksLiquidation": true,
            "status": "PENDING",
            "comments": null,
            "resolvedBy": null,
            "resolvedAt": null
          }
        ]
      }
    ]
    ```

---

### B. Registrar la Firma / Resolución de un Departamento
Permite a un usuario autorizado emitiendo su firma (*Aprobar* o *Rechazar*) para un departamento específico.

*   **URL**: `/api/engine/resolve-item`
*   **Método**: `POST`
*   **Cuerpo (Payload)**:
    ```json
    {
      "approvalItemId": 102,
      "status": "APPROVED",
      "comments": "Descuento autorizado por la gerencia de finanzas",
      "resolvedBy": 8
    }
    ```
    *(Para rechazar, envía `"status": "REJECTED"`)*

*   **Respuesta Exitosa (200 OK)**:
    ```json
    {
      "success": true,
      "itemId": 102,
      "itemStatus": "APPROVED",
      "overallStatus": "PENDING",
      "message": "Aprobación de Finanzas registrada como APPROVED. Estado global: PENDING."
    }
    ```
    *(Nota: El motor recalcula automáticamente el `overallStatus`. Si todas las firmas obligatorias son `APPROVED`, el estado global pasa a `APPROVED`. Si alguna firma obligatoria es `REJECTED`, pasa a `REJECTED`).*

---

### C. Generar una Nueva Solicitud de Aprobación
Permite solicitar explícitamente una aprobación para una entidad o dispararla desde el cambio de etapa.

*   **URL**: `/api/engine/create-request`
*   **Método**: `POST`
*   **Cuerpo (Payload)**:
    ```json
    {
      "entityType": "Order",
      "entityId": "4821",
      "triggerStageId": 3,
      "requestedBy": 12,
      "reason": "Solicitud de descuento comercial especial",
      "metadata": {
        "campaign": "Alarmas"
      }
    }
    ```

---

## 3. UI Recomendada para la Matriz de Aprobaciones

1.  **Estados de Checkbox / Checkmarks**:
    *   `APPROVED`: Muestra un checkbox marcado en verde (`☑ Data & Analytics (estructura — siempre)`).
    *   `PENDING`: Muestra un checkbox desmarcado habilitado para firma (`☐ Finanzas (negocio — dueño del proceso)`).
    *   `REJECTED`: Muestra una marca roja de rechazo (`☒ Gerencia (Rechazado)`).
2.  **Etiquetas Informativas**:
    *   `(estructura — siempre)` para departamentos obligatorios de arquitectura.
    *   `(solo obligatoria si bloquea avance)` para firmas con `blocksProgress = true`.
3.  **Refresco en Vivo**:
    *   Al hacer clic en un departamento para resolver la firma, la vista debe actualizar su estado y rehabilitar/deshabilitar los botones de cambio de etapa según el `overallStatus`.
