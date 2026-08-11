# Manual Operativo 360° — Rol Asesor de Ventas

Guía oficial de referencia técnica y procedimental para el rol de **Asesor Comercial** en la plataforma CRM Call Center (compuesto por el backend `CRM.ApiHub` y el frontend `CRM.WebFrontend`).

---

## 🎯 KPIs y Metas Operativas del Asesor

| Indicador (KPI) | Meta Objetivo | Descripción |
|---|---|---|
| **AHT (Average Handling Time)** | `< 4 minutos` | Tiempo promedio de atención por llamada/gestión. |
| **Conversión Comercial** | `> 25%` | Porcentaje de leads calificados convertidos en órdenes de venta. |
| **Efectividad en Auditoría** | `> 90%` | Porcentaje de ventas aprobadas por el supervisor en primer envío. |
| **Duplicidad** | `0%` | Prevención de re-ingreso de cédulas registradas en los últimos 30 días. |

---

## 💻 Módulo 1: Autenticación, Sesión y Atajos

1. Acceder a `https://crm.callcenter.com/login` e ingresar credenciales corporativas.
2. El sistema emite un token **JWT Bearer (60 min)** y un **RefreshToken (7 días)** administrados por `CustomAuthStateProvider.cs`.
3. Redirección automática a la consola de trabajo en `AsesorDashboard.razor`.

### Atajos de Teclado del Dashboard (`AsesorDashboard.razor`)
- `Ctrl + Shift + D`: Alternar vista entre Lista de Leads y Catálogo de Productos.
- `Ctrl + Alt + N`: Abrir formulario modal de "+ Nuevo Prospecto".
- `Ctrl + S`: Guardar borrador actual.
- `Esc`: Cerrar modales flotantes.

---

## 📋 Módulo 2: Captura & Registro de Leads

Al atender una llamada o prospecto comercial:
1. Abrir modal con `Ctrl + Alt + N` o presionar **+ Nuevo Prospecto**.
2. Ingresar tipo y número de documento (Cédula/DNI).
3. Completar Nombres, Apellidos, Teléfono (10 dígitos), Correo Electrónico y Dirección.
4. Presionar **Guardar Lead**. La solicitud quedará en estado `New`.

### Payload API (`POST /api/v1/presales/leads`)
```json
{
  "documentNumber": "0928374651",
  "documentType": "DNI",
  "firstName": "María",
  "lastName": "Fernández",
  "phone": "0991234567",
  "email": "maria.fernandez@example.com",
  "city": "Guayaquil",
  "address": "Av. 9 de Octubre y Pichincha 402",
  "offeredProduct": "Plan Fibra Óptica 300 Mbps"
}
```

---

## 🔍 Módulo 3: Algoritmo DNI & Detector de Duplicados (30 Días)

- **Dígito Verificador**: El backend aplica validación módulo 10. Si el DNI es incorrecto, la API responde `DNI_INVALIDO`.
- **Regla Anti-Duplicados (30 Días)**: Si la cédula o teléfono fueron ingresados en los últimos 30 días, la solicitud se marca como `Duplicate`. No genera comisión doble y enlaza al lead original.

---

## 📝 Módulo 4: Formularios Dinámicos de Calificación

1. Hacer clic en **Calificar Cobertura & Crédito**.
2. Carga del formulario dinámico JSON vía `GET /api/v1/forms/{id}`.
3. Responder preguntas de cobertura de caja dispersora (NAP) y factibilidad de pago.
4. Al guardar (`POST /api/v1/presales/leads/{id}/qualify`), el lead pasa al estado `Qualified`.

---

## 🛒 Módulo 5: Formalización de Orden de Venta

1. En el Lead Calificado, hacer clic en **+ Generar Orden de Venta**.
2. Se despliega la pantalla de orden en `AsesorOrderDetail.razor`.
3. Seleccionar el plan contratado:
   - `PLAN-FIBRA-300M` ($45/mes)
   - `PLAN-FIBRA-500M` ($65/mes)
   - `PLAN-CORP-1GIG` ($120/mes)
4. Confirmar dirección y referencias de instalación.
5. Guardar como borrador (`PendingDocuments`).

---

## 📁 Módulo 6: Carga de Anexos Digitales & Reglas de Validación

### Tabla de Adjuntos Exigidos
| Documento | Formatos | Tamaño Máx. | Requisito para Auditar |
|---|---|---|---|
| Cédula / DNI (Frontal y Posterior) | PDF, PNG, JPG | 10 MB | **Obligatorio (Bloqueante)** |
| Contrato Digital Firmado | PDF | 10 MB | **Obligatorio (Bloqueante)** |
| Planilla de Servicios | PDF, PNG, JPG | 10 MB | Opcional |

- Subida multipart vía `POST /api/v1/orders/{id}/documents`.
- Validación de hash SHA256 e inspección MIME Type.
- Al subir los adjuntos requeridos, se habilita el botón **Enviar a Auditoría** (`PendingAudit`).

---

## 🔄 Módulo 7: Ciclo de Vida de la Orden & Devoluciones

### Matriz de Estados de la Orden
- `Draft`: Borrador inicial.
- `PendingDocuments`: Faltan documentos requeridos.
- `PendingAudit`: Enviada a revisión del Supervisor de Calidad.
- `PendingActivation`: Aprobada en auditoría; en cola de Backoffice.
- `Activated`: **Venta activada exitosamente (Computa para comisión)**.
- `RejectedAudit`: Devuelta por el Supervisor con observaciones.
- `RejectedTechnical`: Devuelta por Backoffice por problemas de cobertura de red.

### Procesar una Devolución (`RejectedAudit`)
1. Notificación SignalR en tiempo real en la pantalla.
2. Abrir la orden devuelta y leer la casilla **Observaciones de Auditoría**.
3. Corregir el dato o subir la foto clara del documento.
4. Presionar **Re-Enviar a Auditoría** (`POST /api/v1/orders/{id}/resubmit`).

---

## 💰 Módulo 8: Consulta de Comisiones & Escalas por Metas

Consulta de saldo acumulado en tiempo real vía `GET /api/v1/commissions/summary`.

### Escala Comercial Mensual
- **1 a 10 Ventas Activadas**: $20.00 / orden.
- **11 a 20 Ventas Activadas**: $25.00 / orden.
- **21 a 30 Ventas Activadas**: $30.00 / orden + **Bono Meta $100.00**.
- **> 30 Ventas Activadas**: $35.00 / orden + **Bono Top Performer $250.00**.

---

## ❓ Módulo 9: Preguntas Frecuentes (FAQs)

#### ¿Por qué el botón "Enviar a Auditoría" está deshabilitado?
Verifica que hayas adjuntado la foto legible de la Cédula (Frontal/Posterior) y el Contrato firmado. El botón se activa solo al cumplir los requisitos bloqueantes.

#### ¿Qué hago si la orden es rechazada por "Audio no coincide"?
Debes contactar nuevamente al cliente, realizar la lectura del script legal registrando la grabación en el sistema y adjuntar el nuevo ID de audio antes de re-enviar.

#### ¿Dónde reporto fallas del sistema durante la llamada?
Presiona `F1` para buscar en la Base de Conocimiento (`KB`) o haz clic en **Reportar Incidencia** para generar un ticket automático en GLPI.
