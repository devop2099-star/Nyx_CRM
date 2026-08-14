# Recetas de Solución Rápida — Rol Asesor de Ventas

Guía de contingencia y acción inmediata para resolver escenarios anómalos o bloqueos durante la atención telefónica y comercial con los clientes.

---

## ⚡ Receta 1: El cliente es extranjero y no tiene Cédula estándar (DNI Módulo 10)

### Síntoma
Al ingresar el número de identificación del cliente en la modal de creación de Lead, el sistema arroja el error `DNI_INVALIDO`.

### Solución Paso a Paso
1. En la lista desplegable **Tipo de Documento**, selecciona **PASAPORTE** o **RUC / ID EXTRANJERO**.
2. El selector cambiará el patrón de validación desactivando el chequeo Módulo 10.
3. Ingresa la serie alfanumérica del Pasaporte.
4. En el campo **Adjuntos**, deberás subir la copia a color de las páginas principales del Pasaporte o Censo de Extranjería.

---

## ⚡ Receta 2: Error 413 "Payload Too Large" al subir la foto de la Cédula

### Síntoma
La interfaz muestra un mensaje de error en rojo y la foto de la cédula no aparece en la tabla de adjuntos.

### Causa Raíz
El archivo seleccionado supera el límite máximo permitido por la API (`10 MB`).

### Solución Paso a Paso
1. Si la foto fue tomada con cámara móvil en máxima resolución (4K/1080p), comprime el archivo antes de subirlo.
2. Abre la imagen en una herramienta de edición rápida o conviértela a PDF liviano.
3. Verifica que el archivo resultante pese menos de `10 MB` (idealmente entre `500 KB` y `3 MB`).
4. Reintenta la subida.

---

## ⚡ Receta 3: Desconexión de SignalR (Campana de Notificaciones no actualiza)

### Síntoma
El asesor no recibe alertas en tiempo real cuando una orden es devuelta o activada sin refrescar la página manualmente.

### Solución Paso a Paso
1. Comprueba si el indicador de red en la esquina superior derecha del CRM muestra el punto gris o rojo (SignalR Disconnected).
2. Presiona <span class="key-shortcut">F5</span> para refrescar la sesión Blazor.
3. Al recargar, el interceptor re-establecerá el WebSocket enviando el token de autenticación activo.

---

## ⚡ Receta 4: La orden fue devuelta por "Audio Ilegible"

### Síntoma
La orden pasó al estado `RejectedAudit` con la observación: *"La grabación no contiene la aceptación explícita de la tarifa mensual."*

### Solución Paso a Paso
1. Llama nuevamente al cliente para realizar la lectura del script de confirmación de 30 segundos.
2. Graba la llamada y genera la nueva evidencia de audio.
3. En `AsesorOrderDetail.razor`, dirígete a **Adjuntos**, selecciona `Audio_Confirmacion` y sube la nueva grabación.
4. Haz clic en **Re-Enviar a Auditoría**. La orden regresará a la cola del Supervisor sin necesidad de recrear la orden.
