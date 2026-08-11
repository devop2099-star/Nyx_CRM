# [Título del Problema / Falla]

## Cuándo Utilizar esta Guía
Utiliza esta guía si experimentas los siguientes síntomas:
- Síntoma 1
- Síntoma 2

---

## Requisitos
- Acceso a servidor / logs / base de datos.

---

## Diagnóstico Paso a Paso

### Paso 1: Verificar el Estado del Servicio
Ejecutar:
```bash
# comando de diagnóstico
```
* **Si el resultado es X**: Continuar con Paso 2.
* **Si el resultado es Y**: Ir a Solución B.

---

## Solución Recomendada

### Paso 1: Ejecutar Mitigación
```bash
# comando de solución
```

### Paso 2: Reiniciar Servicio / Limpiar Caché
```bash
# comando de reinicio
```

---

## Validación de Solución
- [ ] El endpoint responde HTTP 200 OK.
- [ ] No se observan excepciones en los logs.

---

## Qué NO Hacer
- ❌ No reiniciar la base de datos sin previo backup.
- ❌ No modificar credenciales en caliente.

---

## Rollback
En caso de que el procedimiento falle:
1. Revertir cambios ejecutando...
