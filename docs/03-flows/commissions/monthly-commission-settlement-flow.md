# Flujo de Negocio: Liquidación Mensual de Comisiones Comerciales

## Objetivo
Calcular de forma automatizada, transparente e inmutable el valor a pagar a cada Asesor Comercial al cierre del mes, aplicando el escalafón por tramos de ventas activadas y bonificaciones por cumplimiento de metas.

---

## 1. Actores Involucrados
- **Administrador Comercial / Jefe de Ventas:** Ejecuta el precierre, valida inconsistencias y aprueba la liquidación definitiva.
- **Asesor Comercial:** Consulta el acumulado de sus ventas aprobadas y el estimado de comisiones en tiempo real.
- **Departamento de Nómina (RRHH):** Recibe el archivo de liquidación consolidado en formato CSV/Excel para el pago bancario.
- **Motor de Comisiones (`CommissionEngineService`):** Aplica la lógica de tramos y genera los registros en `crm.commission_settlements`.

---

## 2. Reglas del Escalafón Comercial

| Tramo de Ventas Activadas en el Mes | Comisión Unitaria por Venta | Bono Adicional |
|---|---|---|
| **Tramo 1:** 1 a 10 ventas activadas | **$15.00 USD** c/u | Sin bono adicional |
| **Tramo 2:** 11 a 25 ventas activadas | **$25.00 USD** c/u | Sin bono adicional |
| **Tramo 3 (Meta Superada):** 26 o más ventas | **$35.00 USD** c/u | **+ $150.00 USD** de bono de excelencia |

*Nota: Solo se computan órdenes en estado `Activated` cuya fecha de encendido corresponda al mes calendario en curso.*

---

## 3. Flujo Principal Paso a Paso

```
[ Administrador / Jefe de Ventas ]        [ CommissionEngineService ]         [ crm.commission_settlements ]
                |                                      |                                    |
                |-- 1. Ejecutar Precierre Mensual ---->|                                    |
                |                                      |-- 2. Query Ordenes 'Activated' ----|
                |                                      |-- 3. Calcular Tramos & Bonos       |
                |<-- 4. Retornar Reporte Preliminar ---|                                    |
                |                                      |                                    |
                |-- 5. Revisión & Aprobación Final --->|                                    |
                |                                      |-- 6. Insertar Liquidación Inmutable|
                |                                      |-- 7. Marcar Periodo 'Closed' ------>|
                |<-- 8. Exportar CSV para Nómina ------|
```

1. **Monitoreo Continuo:** A lo largo del mes, cada asesor visualiza en su dashboard el contador de órdenes activadas y su proyección de ingresos.
2. **Corte Mensual (Día 30/31):** El Jefe de Ventas presiona **Calcular Liquidación Preliminar** en el panel de administración.
3. **Ejecución del Algoritmo:**
   - El sistema totaliza las órdenes activadas por cada `advisor_id`.
   - Aplica el escalafón progresivo y calcula el bono de meta si las ventas `>= 26`.
4. **Validación de Anomalías:** El administrador revisa si existen órdenes con disputas o reclamos pendientes.
5. **Cierre Definitivo:** Se genera el registro inmutable con estado `Approved` y se genera el archivo plano para el sistema de nómina.

---

## 4. Tabla de Errores & Manejo de Excepciones

| Código HTTP | Causa | Acción del Sistema / Usuario |
|---|---|---|
| `400 Bad Request` | El periodo mensual ya fue cerrado y aprobado previamente. | El sistema bloquea el recálculo para garantizar inmutabilidad contable. |
| `404 Not Found` | No existen ventas activadas en el periodo seleccionado. | Muestra mensaje: *"No se encontraron órdenes activadas para liquidar en este mes"*. |

---

## 5. Documentación Asociada
- **Módulo de Comisiones:** [docs/02-modules/commissions/index.html](../../02-modules/commissions/index.html)
- **Manual del Asesor:** [docs/08-user-guides/asesor-guide.html](../../08-user-guides/asesor-guide.html)
