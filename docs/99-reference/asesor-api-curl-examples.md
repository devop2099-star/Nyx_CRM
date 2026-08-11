# Referencia de API & Colección cURL — Rol Asesor de Ventas

Guía de prueba rápida mediante comandos cURL de consola para ejecutar y verificar todos los endpoints REST consumidos por el **Asesor Comercial** en `CRM.ApiHub`.

---

## 🔑 1. Autenticación & Obtención de Token Bearer

### Login de Asesor (`POST /api/v1/auth/login`)
```bash
curl -X POST "https://crm.callcenter.com/api/v1/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "asesor.ventas@callcenter.com",
    "password": "PasswordSeguro2026!",
    "deviceInfo": "cURL Console Test"
  }'
```

---

## 📋 2. Módulo de Pre-Ventas & Leads

### Registrar un Nuevo Lead (`POST /api/v1/presales/leads`)
```bash
curl -X POST "https://crm.callcenter.com/api/v1/presales/leads" \
  -H "Authorization: Bearer <TU_ACCESS_TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "documentNumber": "0928374651",
    "documentType": "DNI",
    "firstName": "María",
    "lastName": "Fernández",
    "phone": "0991234567",
    "email": "maria.fernandez@example.com",
    "city": "Guayaquil",
    "address": "Av. 9 de Octubre y Pichincha 402",
    "offeredProduct": "Plan Fibra Óptica 300 Mbps"
  }'
```

### Consultar Leads Asignados (`GET /api/v1/presales/leads`)
```bash
curl -X GET "https://crm.callcenter.com/api/v1/presales/leads?status=New&page=1&pageSize=20" \
  -H "Authorization: Bearer <TU_ACCESS_TOKEN>"
```

### Calificar Lead con Cuestionario (`POST /api/v1/presales/leads/{id}/qualify`)
```bash
curl -X POST "https://crm.callcenter.com/api/v1/presales/leads/f47ac10b-58cc-4372-a567-0e02b2c3d4e5/qualify" \
  -H "Authorization: Bearer <TU_ACCESS_TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "formId": "FORM-COVERAGE-01",
    "responses": {
      "hasNapCoverage": true,
      "napDistanceMeters": 45,
      "creditCheckApproved": true
    }
  }'
```

---

## 🛒 3. Módulo de Órdenes de Venta & Adjuntos

### Crear Órden de Venta (`POST /api/v1/orders`)
```bash
curl -X POST "https://crm.callcenter.com/api/v1/orders" \
  -H "Authorization: Bearer <TU_ACCESS_TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "leadId": "f47ac10b-58cc-4372-a567-0e02b2c3d4e5",
    "planId": "PLAN-FIBRA-300M",
    "monthlyAmount": 45.00,
    "installationAddress": "Av. 9 de Octubre y Pichincha 402, Apto 3B",
    "notes": "Cliente solicita instalación en horario de la tarde."
  }'
```

### Cargar Archivo Adjunto Cédula (`POST /api/v1/orders/{id}/documents`)
```bash
curl -X POST "https://crm.callcenter.com/api/v1/orders/e1f2a3b4-5c6d-7e8f-9a0b-1c2d3e4f5a6b/documents" \
  -H "Authorization: Bearer <TU_ACCESS_TOKEN>" \
  -F "documentType=DNI_Front" \
  -F "file=@/ruta/local/cedula_frontal.pdf;type=application/pdf"
```

### Enviar Orden a Auditoría del Supervisor (`POST /api/v1/orders/{id}/submit-audit`)
```bash
curl -X POST "https://crm.callcenter.com/api/v1/orders/e1f2a3b4-5c6d-7e8f-9a0b-1c2d3e4f5a6b/submit-audit" \
  -H "Authorization: Bearer <TU_ACCESS_TOKEN>"
```

### Re-Enviar Orden Corregida por Devolución (`POST /api/v1/orders/{id}/resubmit`)
```bash
curl -X POST "https://crm.callcenter.com/api/v1/orders/e1f2a3b4-5c6d-7e8f-9a0b-1c2d3e4f5a6b/resubmit" \
  -H "Authorization: Bearer <TU_ACCESS_TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "correctionNotes": "Se adjunta nueva imagen clara de la cédula de identidad del titular."
  }'
```

---

## 💰 4. Consulta de Comisiones & Ayuda

### Consultar Acumulado de Comisiones (`GET /api/v1/commissions/summary`)
```bash
curl -X GET "https://crm.callcenter.com/api/v1/commissions/summary?period=2026-08" \
  -H "Authorization: Bearer <TU_ACCESS_TOKEN>"
```

### Reportar Incidencia Técnica a GLPI (`POST /api/v1/incidents`)
```bash
curl -X POST "https://crm.callcenter.com/api/v1/incidents" \
  -H "Authorization: Bearer <TU_ACCESS_TOKEN>" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Error de carga en formulario de pre-venta",
    "severity": "Medium",
    "description": "La modal se cierra sola al presionar la tecla Enter."
  }'
```
