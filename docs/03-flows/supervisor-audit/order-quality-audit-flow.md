# Flujo de Negocio: Auditoría & Control de Calidad de Ventas

## Objetivo
Garantizar que toda orden de venta capturada por un Asesor Comercial cumpla con los estándares legales, veracidad de datos, consentimiento informado en la grabación telefónica y documentación adjunta legible antes de ser despachada a Backoffice.

---

## 1. Actores Involucrados
- **Supervisor de Calidad (Auditor):** Evalúa la orden, escucha la llamada y emite el dictamen (Aprobar / Observar).
- **Asesor Comercial (Vendedor):** Recibe retroalimentación en tiempo real y subsana observaciones si la orden es devuelta.
- **Sistema CRM.ApiHub:** Controla la máquina de estados, emite eventos SignalR y actualiza métricas de comisiones.

---

## 2. Precondiciones
1. La orden de venta se encuentra en estado `PendingAudit`.
2. Existen al menos dos documentos adjuntos: Cédula de Identidad y Contrato Digital Firmado.
3. El ID de grabación de la llamada VoIP (`recording_url` o Asterisk Call ID) está asociado a la orden.

---

## 3. Flujo Principal Paso a Paso

```
[ Asesor Comercial ]             [ CRM.ApiHub ]              [ Supervisor de Calidad ]
         |                              |                                |
         |-- 1. Enviar a Auditoría ---->|                                |
         |                              |-- 2. SignalR Event ------------>| (Notificación Push)
         |                              |                                |-- 3. Abrir AudioAudit.razor
         |                              |                                |-- 4. Escuchar Grabación VoIP
         |                              |                                |-- 5. Calificar Rúbrica (0-100 pts)
         |                              |<-- 6. Enviar Dictamen (>=80) --|
         |                              |
         |                              |-- 7. Estado -> ApprovedAudit
         |<-- 8. SignalR: Orden Aprobada|
```

1. **Recepción en Bandeja de Entrada:** El supervisor ingresa a `SupervisorDashboard.razor` y selecciona una orden de la lista ordenada por orden de llegada (FIFO).
2. **Apertura del Visor de Auditoría:** Se carga la interfaz `AudioAudit.razor` con la información del cliente, plan seleccionado y el reproductor de audio sincronizado.
3. **Escucha & Verificación de Consentimiento:** El auditor reproduce la llamada y verifica los 4 puntos obligatorios de la rúbrica:
   - Identificación formal del asesor y bienvenida corporativa (20 pts).
   - Lectura clara de las condiciones del plan y cláusula de permanencia (30 pts).
   - Expresión explícita del consentimiento del cliente ("Sí, acepto") (30 pts).
   - Validación de la dirección de instalación y número de contacto (20 pts).
4. **Emisión de Calificación:**
   - Si la puntuación total es **>= 80 puntos**, el supervisor presiona **Aprobar Orden**.
   - El sistema cambia el estado a `ApprovedAudit` y la orden pasa automáticamente a la cola de Backoffice.

---

## 4. Flujo Alternativo: Devolución por Observaciones (< 80 puntos o Documento Ilegible)
1. Si la cédula está borrosa o la grabación no contiene el consentimiento explícito:
2. El supervisor selecciona el motivo de rechazo en una lista tipificada (ej. `Ilegible_Cedula`, `Falta_Consentimiento_Audio`).
3. Ingresa un comentario explicativo obligatorio y presiona **Devolver Orden al Asesor**.
4. La orden regresa a estado `PendingDocuments` o `Draft`.
5. El asesor recibe una notificación push inmediata en su panel con el motivo exacto de la devolución.

---

## 5. Tabla de Errores & Manejo de Excepciones

| Código HTTP | Causa | Acción del Sistema / Usuario |
|---|---|---|
| `400 Bad Request` | Puntuación de rúbrica no suma 100 o faltan campos obligatorios. | El frontend valida los campos antes del envío y marca los ítems pendientes en rojo. |
| `409 Conflict` | La orden ya fue auditada por otro supervisor simultáneamente. | Muestra alerta: *"Esta orden ya fue aprobada por otro auditor"* y recarga la bandeja. |
| `404 Not Found` | Archivo de audio de la llamada no encontrado en el servidor VoIP. | Registra advertencia en logs y permite auditar con justificación manual. |

---

## 6. Documentación Visual Asociada
- **Walkthrough Paso a Paso:** [docs/09-visual-docs/walkthroughs/asesor-sales-walkthrough.html](../../09-visual-docs/walkthroughs/asesor-sales-walkthrough.html)
- **Mockup UI del Supervisor:** [docs/09-visual-docs/mockups/supervisor-ui-mockups.html](../../09-visual-docs/mockups/supervisor-ui-mockups.html)
