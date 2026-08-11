# Flujo de Negocio: Aprovisionamiento Técnico & Activación en Backoffice

## Objetivo
Despachar la orden aprobada a la cuadrilla técnica en terreno, asignar la caja dispersora de fibra óptica (NAP) y puerto correspondiente, y registrar el encendido definitivo del servicio de telecomunicaciones.

---

## 1. Actores Involucrados
- **Analista de Backoffice:** Gestiona la cola de activaciones en `ActivationBoard.razor`, asigna puertos NAP y coordina con técnicos.
- **Técnico de Campo (Cuadrilla):** Realiza la acometida de fibra óptica e instalación del router ONT en el domicilio del cliente.
- **Sistema CRM.ApiHub:** Bloquea el puerto NAP para evitar colisiones de asignación y actualiza el estado a `Activated`.

---

## 2. Precondiciones
1. La orden se encuentra en estado `ApprovedAudit`.
2. Existen puertos disponibles en la caja NAP seleccionada (puertos 1 a 16).
3. Se dispone del número de serie o dirección MAC del equipo ONT a instalar.

---

## 3. Flujo Principal Paso a Paso

```
[ Supervisor ]               [ Analista Backoffice ]              [ CRM.ApiHub ]              [ Cuadrilla en Campo ]
      |                                |                                |                                |
      |-- 1. Orden Aprobada ---------->|                                |                                |
      |                                |-- 2. Seleccionar Caja NAP ---->|                                |
      |                                |<-- 3. Reservar Puerto (1-16) --|                                |
      |                                |-- 4. Asignar Cuadrilla ---------------------------------------->| (Despacho Ruta)
      |                                |                                |                                |-- 5. Instalar ONT
      |                                |<-- 6. Confirmar Serie ONT & Potencia dBm -----------------------|
      |                                |-- 7. Enviar Confirmación de Encendido ->|
      |                                |                                |-- 8. Estado -> Activated
      |                                |                                |-- 9. Habilitar Comisión
```

1. **Recepción en Backoffice:** El analista visualiza la orden en estado `ApprovedAudit` dentro del tablero Kanban de activaciones.
2. **Asignación de Caja NAP & Puerto:**
   - Se selecciona la caja NAP más cercana al domicilio del cliente.
   - El sistema valida que el puerto seleccionado (1 al 16) esté libre (`is_occupied = false`).
3. **Despacho a Cuadrilla Técnica:**
   - Se asigna el técnico responsable y la franja horaria de instalación (Mañana: 08:00-12:00 / Tarde: 13:00-17:00).
   - El estado de la orden avanza a `InTechnicalProvisioning`.
4. **Instalación & Validación de Potencia Óptica:**
   - El técnico fusiona la fibra y mide la potencia óptica de recepción (umbral válido: `-18 dBm` a `-25 dBm`).
   - Reporta el número de serie del ONT (ej. `ZTEG1234ABCD`).
5. **Confirmación de Encendido:**
   - El analista ingresa el número de serie y la lectura de potencia en `ActivationBoard.razor`.
   - Presiona **Confirmar Encendido y Activar Servicio**.
   - El sistema pasa la orden a estado `Activated` y marca la venta como elegible para liquidación de comisiones.

---

## 4. Flujo Alternativo: Incidencia Técnica en Terreno (Sin Cobertura / Caja Saturada)
1. Si al llegar a la dirección no existe factibilidad técnica o la caja NAP está dañada:
2. El analista marca la orden como **Incidencia Técnica** con motivo tipificado (`NAP_Saturada`, `Zona_Sin_Postes`).
3. El sistema crea automáticamente un ticket en **GLPI** y notifica al cliente para reprogramación o cancelación.
4. El puerto previamente reservado queda liberado automáticamente.

---

## 5. Tabla de Errores & Manejo de Excepciones

| Código HTTP | Causa | Acción del Sistema / Usuario |
|---|---|---|
| `409 Conflict` | El puerto NAP seleccionado ya fue ocupado por otra cuadrilla. | El sistema rechaza la asignación y muestra la lista actualizada de puertos libres. |
| `400 Bad Request` | Potencia óptica fuera del rango permitido (&lt; -27 dBm). | Muestra alerta: *"Nivel de atenuación óptica inaceptable para activación"*. |
| `404 Not Found` | Caja NAP no encontrada en el inventario georreferenciado. | Requiere validación de coordenadas GPS con el equipo de planta externa. |

---

## 6. Documentación Visual Asociada
- **Mockups de Backoffice:** [docs/09-visual-docs/mockups/backoffice-ui-mockups.html](../../09-visual-docs/mockups/backoffice-ui-mockups.html)
- **Módulo de Activaciones:** [docs/02-modules/backoffice-activations/index.html](../../02-modules/backoffice-activations/index.html)
