# Instructivo de Integración para Frontend: Sistema de Checkpoints Desacoplado

Este instructivo describe el funcionamiento del motor de checkpoints desacoplado y cómo debe conectarse la interfaz de usuario para interactuar con él en tiempo real.

---

## 1. Arquitectura del Motor (Nyx.CheckpointEngine)
El motor de checkpoints funciona mediante un microservicio autónomo: **Nyx.CheckpointEngine** (ejecutándose en el puerto `5080`).

*   **Diseño Reutilizable y Agnóstico**: El motor no posee dependencias directas con el esquema del CRM. Trabaja en base a un esquema genérico de base de datos (`checkpoint_service`) y asocia checkpoints a cualquier elemento mediante un par de campos: `entityType` (ej. `"Order"`, `"Lead"`) y `entityId` (ej. `"4821"`).
*   **Gestión Desacoplada del Rollback**: Cuando se marca un checkpoint como `KO` / `RECHAZADO`, el motor no modifica directamente la tabla de ventas. En su lugar, retorna la instrucción `rollbackRequired: true` junto con la etapa sugerida `rollbackStageId` en el payload JSON. La API del CRM procesa esta respuesta HTTP y aplica los cambios correspondientes localmente en su base de datos.

---

## 2. Contratos y Endpoints de la API

El frontend debe seguir consumiendo la API principal (`CRM.ApiHub`) en el puerto estándar. Los controladores de la API principal aplican seguridad mediante tokens JWT.

### A. Obtener Checkpoints de una Orden
Recupera la lista de checkpoints activos y sus checklists internos correspondientes al pedido.

*   **URL**: `/api/checkpoint/orders/{idOrder}`
*   **Método**: `GET`
*   **Cabeceras**: `Authorization: Bearer <JWT_TOKEN>`
*   **Respuesta Exitosa (200 OK)**:
    ```json
    [
      {
        "checkpoint": {
          "id": 1,
          "entityType": "Order",
          "entityId": "4821",
          "checkpointCatalogId": 1,
          "status": "PENDIENTE",
          "comments": null,
          "checkedBy": null,
          "checkedAt": null,
          "stepsCompleted": 2,
          "checkpointName": "Auditoría de audio",
          "appliesTo": "Alarmas",
          "origin": "INTERNO",
          "blocksProgress": false,
          "blocksCommission": true,
          "blocksActivation": true,
          "blocksLiquidation": false,
          "rollbackStageId": null,
          "triggerOnKoCatalogId": null,
          "ownerDepartment": "Finanzas",
          "requiredStepsCount": 5
        },
        "steps": [
          {
            "orderCheckpointId": 1,
            "stepIndex": 1,
            "isCompleted": true,
            "updatedBy": 12,
            "updatedAt": "2026-08-11T19:30:00Z",
            "stepDescription": "Verificación de la nitidez de la voz del titular"
          },
          {
            "orderCheckpointId": 1,
            "stepIndex": 2,
            "isCompleted": false,
            "updatedBy": null,
            "updatedAt": "2026-08-11T19:30:00Z",
            "stepDescription": "Confirmación del consentimiento explícito"
          }
        ]
      }
    ]
    ```

---

### B. Alternar el Estado de un Paso (Checklist)
Permite marcar un sub-paso de checklist como completado o pendiente. El motor recalcula automáticamente y marca el checkpoint como `APROBADO` si se completan el 100% de los pasos requeridos.

*   **URL**: `/api/checkpoint/{checkpointId}/steps/{stepIndex}/toggle`
*   **Método**: `POST`
*   **Cabeceras**: `Authorization: Bearer <JWT_TOKEN>`
*   **Cuerpo (Payload)**:
    ```json
    {
      "isCompleted": true
    }
    ```
*   **Respuesta Exitosa (200 OK)**:
    ```json
    {
      "message": "Estado del paso del checkpoint actualizado exitosamente."
    }
    ```

---

### C. Aprobar o Rechazar Manualmente un Checkpoint
Permite a los usuarios autorizados (Supervisor, Backoffice o miembros de Finanzas) resolver de forma manual un checkpoint, adjuntando justificaciones.

*   **URL**: `/api/checkpoint/{checkpointId}/status`
*   **Método**: `PUT`
*   **Cabeceras**: `Authorization: Bearer <JWT_TOKEN>`
*   **Cuerpo (Payload)**:
    ```json
    {
      "status": "APROBADO", 
      "comments": "Documentos validados físicamente por el supervisor"
    }
    ```
    *(Usa `"status": "RECHAZADO"` o `"status": "KO"` para rechazar y forzar el rollback del pedido)*
*   **Respuesta Exitosa (200 OK)**:
    ```json
    {
      "message": "Estado de checkpoint actualizado exitosamente."
    }
    ```
    *(Nota: Si el checkpoint posee un `rollback_stage_id` en el catálogo, la API aplicará el retroceso del pedido de forma automática en el backend. El frontend recibirá una respuesta exitosa y deberá refrescar el estado visual de la orden).*
