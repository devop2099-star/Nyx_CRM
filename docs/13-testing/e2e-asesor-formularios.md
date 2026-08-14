# Pruebas End-to-End (E2E) — Módulo & Workflows del Asesor de Ventas

Especificación detallada de escenarios de prueba automatizados con **Playwright** y **xUnit** para verificar los flujos operacionales del Asesor Comercial en la plataforma CRM Call Center (`CRM.WebFrontend` + `CRM.ApiHub`).

---

## 🎯 Cobertura de Pruebas del Asesor

1. **Autenticación & Manejo de Token**: Login exitoso, expiración JWT y refresco silencioso.
2. **Registro de Lead & Validación de DNI**: Verificación del algoritmo del dígito verificador.
3. **Motor Anti-Duplicados (30 Días)**: Verificación de bloqueo cuando la cédula ya existe.
4. **Formularios Dinámicos**: Diligenciamiento de cuestionario de cobertura.
5. **Formalización de Venta & Adjuntos**: Carga multipart de Cédula y Contrato (<10MB).
6. **Manejo de Devolución**: Re-envío a auditoría tras corrección de fotos devueltas por el supervisor.

---

## 🧪 Escenario 1: Captura de Prospecto con DNI Válido

```typescript
import { test, expect } from '@playwright/test';

test.describe('Asesor: Registro de Lead & Validación DNI', () => {
  test.beforeEach(async ({ page }) => {
    // Login inicial del Asesor
    await page.goto('https://crm.callcenter.com/login');
    await page.fill('input[name="username"]', 'asesor.ventas@callcenter.com');
    await page.fill('input[name="password"]', 'PasswordSeguro2026!');
    await page.click('button[type="submit"]');
    await expect(page).toHaveURL('https://crm.callcenter.com/asesor/dashboard');
  });

  test('Debe registrar un nuevo Lead cuando la cédula es válida y sin duplicados', async ({ page }) => {
    // Abrir modal de nuevo prospecto
    await page.click('button:has-text("+ Nuevo Prospecto")');
    
    // Ingresar DNI válido
    await page.fill('input[name="documentNumber"]', '0928374651');
    await page.click('button:has-text("Validar DNI")');
    
    // Validar mensaje de éxito de DNI
    await expect(page.locator('.text-green')).toContainText('Cédula válida');

    // Llenar datos personales
    await page.fill('input[name="firstName"]', 'María');
    await page.fill('input[name="lastName"]', 'Fernández');
    await page.fill('input[name="phone"]', '0991234567');
    await page.fill('input[name="email"]', 'maria.fernandez@example.com');
    
    // Guardar
    await page.click('button:has-text("Guardar Lead")');

    // Confirmar toast de éxito y presencia en la tabla
    await expect(page.locator('.ui-table')).toContainText('María Fernández');
    await expect(page.locator('.ui-tag')).toContainText('New');
  });
});
```

---

## 🧪 Escenario 2: Verificación de Bloqueo por Lead Duplicado (30 Días)

```typescript
test('Debe bloquear el registro si el DNI ingresó en los últimos 30 días', async ({ page }) => {
  await page.click('button:has-text("+ Nuevo Prospecto")');
  
  // Ingresar DNI registrado previamente ayer
  await page.fill('input[name="documentNumber"]', '0911223344');
  await page.click('button:has-text("Validar DNI")');

  // Validar advertencia de duplicidad
  await expect(page.locator('.text-amber')).toContainText('Solicitud Duplicada (Ingresada en los últimos 30 días)');
  
  // Guardar y comprobar estado Duplicate
  await page.click('button:has-text("Guardar Lead")');
  await expect(page.locator('.ui-tag')).toContainText('Duplicate');
});
```

---

## 🧪 Escenario 3: Formalización de Orden & Subida de Documentos (10 MB Limit)

```typescript
test('Debe permitir subir la Cédula y enviar a Auditoría', async ({ page }) => {
  await page.goto('https://crm.callcenter.com/asesor/orders/SO-2026-08-0042');
  
  // Seleccionar archivo DNI
  const fileChooserPromise = page.waitForEvent('filechooser');
  await page.click('button:has-text("Seleccionar Archivo")');
  const fileChooser = await fileChooserPromise;
  await fileChooser.setFiles('./tests/fixtures/cedula_frontal.pdf');

  // Comprobar que el archivo se muestra en la tabla con hash de integridad
  await expect(page.locator('.ui-table')).toContainText('cedula_frontal.pdf');
  await expect(page.locator('.ui-table')).toContainText('Integridad OK');

  // Botón "Enviar a Auditoría" habilitado
  const submitBtn = page.locator('button:has-text("Enviar Orden a Auditoría")');
  await expect(submitBtn).toBeEnabled();
  await submitBtn.click();

  // Estado actualizado
  await expect(page.locator('.header-box')).toContainText('PendingAudit');
});
```

---

## 📊 Matriz de Ejecución QA

| Caso de Prueba | Tipo | Resultado Esperado | Criterio de Aceptación |
|---|---|---|---|
| `TC-ASE-001` | E2E | Login del Asesor | Token JWT recibido en cliente y redirección a Dashboard |
| `TC-ASE-002` | E2E | Validación DNI Módulo 10 | DNI `0928374651` aceptado; DNI `123456` rechazado con mensaje de error |
| `TC-ASE-003` | E2E | Detector Duplicados 30 días | Lead marcado como `Duplicate`, impidiendo cobro doble |
| `TC-ASE-004` | E2E | Subida de Archivo > 10MB | API devuelve HTTP 413 Payload Too Large |
| `TC-ASE-005` | E2E | Envío a Auditoría | Estado cambia a `PendingAudit` y SignalR notifica al Supervisor |
