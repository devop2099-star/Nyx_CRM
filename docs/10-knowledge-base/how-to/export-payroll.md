# How-To: Exportar Liquidación Mensual de Comisiones para Nómina (RRHH)

## Objetivo
Generar y descargar el archivo plano oficial de comisiones devengadas por la fuerza de ventas en formato CSV estandarizado para su procesamiento en el software de nómina contable.

---

## Cuándo Utilizar esta Guía
- El día 1 de cada mes tras haber aprobado el cierre definitivo del periodo anterior.

---

## Requisitos & Permisos
- Rol requerido: **Admin** o **Jefe de Ventas**.
- El periodo debe encontrarse en estado `Closed` o `Approved`.

---

## Procedimiento Paso a Paso

1. Inicia sesión en el portal con tu cuenta de administrador.
2. Navega a **Reportes & Comisiones** ➔ **Cierre de Periodo**.
3. Selecciona el mes y año a exportar (ej. `Periodo: 2026-07`).
4. Haz clic en el botón **Descargar Archivo para Nómina (.CSV)**.
5. El sistema generará el archivo con la nomenclatura `COMISIONES_CRM_YYYY_MM.csv`.

---

## Estructura del Archivo CSV Generado

```csv
cedula,nombre_completo,total_ventas_activadas,monto_comision_tramo,bono_meta,total_a_pagar
1712345678,Carlos Perez Gomez,28,980.00,150.00,1130.00
0923456789,Maria Lopez Intriago,18,450.00,0.00,450.00
0104567890,Juan Morales Benitez,8,120.00,0.00,120.00
```

---

## Validación Contable
- [ ] La suma de `total_a_pagar` en el archivo CSV coincide exactamente con el monto consolidado en la pantalla de resumen.
- [ ] No existen registros con valores negativos o identificadores duplicados.
