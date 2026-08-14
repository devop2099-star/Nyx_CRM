# Concepto: Algoritmo de Validación Módulo 10 (Cédula de Identidad)

## Propósito Teórico
El algoritmo **Módulo 10** (variante del algoritmo de Luhn adaptado por el Registro Civil del Ecuador) es una fórmula matemática utilizada para verificar la integridad del número de cédula de 10 dígitos y descartar fraudes o errores tipográficos antes de registrar un nuevo prospecto en el sistema.

---

## 📐 Estructura de la Cédula (10 Dígitos)

```
[ P1 ][ P2 ] [ T ] [ N1 ][ N2 ][ N3 ][ N4 ][ N5 ][ N6 ] [ DV ]
 |________|   |    |____________________________|    |____|
  Provincia   Tipo           Secuencial               Dígito
   (01-24)    (< 6)                                 Verificador
```

1. **Dígitos 1 y 2 (Provincia):** Código provincial válido entre `01` y `24`, o `30` para ecuatorianos en el exterior.
2. **Dígito 3 (Tercer Dígito):** Debe ser menor a `6` para personas naturales (`0` a `5`).
3. **Dígitos 4 al 9:** Número correlativo secuencial.
4. **Dígito 10 (Dígito Verificador):** Resultado del cálculo matemático Módulo 10.

---

## 🧮 Pasos Matemáticos del Algoritmo

1. **Coeficientes Alternados:** Los primeros 9 dígitos se multiplican alternadamente por `[2, 1, 2, 1, 2, 1, 2, 1, 2]`.
2. **Ajuste de Productos:** Si el producto de una multiplicación es **>= 10**, se le resta `9` (ej. `8 * 2 = 16 -> 16 - 9 = 7`).
3. **Suma Total:** Se suman los 9 resultados obtenidos.
4. **Residuo & Resta del Módulo:**
   - Se calcula la decena inmediata superior a la suma total.
   - `Dígito Verificador Esperado = Decena Superior - Suma Total`.
   - Si el resultado es `10`, el dígito verificador es `0`.
5. **Comparación:** Si el dígito verificador calculado es idéntico al décimo dígito ingresado, la cédula es matemáticamente auténtica.

---

## Implementación en el Dominio (`DniDocument.cs`)
Este cálculo se ejecuta en el Value Object inmutable `CRM.Domain.Presales.ValueObjects.DniDocument` antes de persistir cualquier registro en la tabla `crm.leads`.
