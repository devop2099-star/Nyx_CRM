# How-To: Reasignar un Prospecto (Lead) a Otro Asesor Comercial

## Objetivo
Transferir la propiedad de un prospecto en estado `InContact` o `New` de un asesor a otro debido a ausencias, reestructuración de turnos o falta de gestión en más de 48 horas.

---

## Cuándo Utilizar esta Guía
- Un asesor se encuentra de permiso médico o vacaciones.
- Un lead no ha sido contactado en más de 24 horas y debe ser reasignado a un asesor disponible en cola activa.

---

## Requisitos & Permisos
- Rol requerido: **Supervisor** o **Admin**.
- ID del lead a transferir (UUID).
- ID del nuevo asesor de destino.

---

## Procedimiento en Interfaz Web
1. Ingresa a `SupervisorDashboard.razor` y haz clic en la pestaña **Gestión de Leads**.
2. Filtra por el asesor actual o busca por el número de cédula del cliente.
3. Haz clic en el botón de opciones `(...)` al final de la fila del lead y selecciona **Reasignar Lead**.
4. En el modal emergente, selecciona el nuevo asesor en el menú desplegable.
5. Ingresa el motivo de la transferencia (ej. *"Asesor saliente en descanso médico"*) y presiona **Confirmar Reasignación**.

---

## Procedimiento Vía API REST (cURL)

```bash
curl -X PUT "https://crm.callcenter.local/api/leads/a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d/reassign" \
  -H "Authorization: Bearer <TOKEN_SUPERVISOR>" \
  -H "Content-Type: application/json" \
  -d '{
    "new_advisor_id": "f8e7d6c5-b4a3-2109-8765-fedcba987654",
    "reason": "Rebalanceo de carga de trabajo turno tarde"
  }'
```

### Resultado Esperado (HTTP 200 OK)
```json
{
  "lead_id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
  "assigned_to": "f8e7d6c5-b4a3-2109-8765-fedcba987654",
  "status": "Reassigned",
  "updated_at": "2026-08-10T14:30:00Z"
}
```

---

## Validación
- [ ] El nuevo asesor ve aparecer el prospecto en la parte superior de su bandeja con la etiqueta *"Transferido"*.
- [ ] Se registra una entrada en `crm.audit_logs` con el ID del supervisor que autorizó el cambio.
