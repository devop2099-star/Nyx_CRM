# Troubleshooting: Desconexión y Problemas de Reconexión en SignalR WebSockets

## Objetivo
Diagnosticar y solucionar la interrupción de notificaciones push en tiempo real en los paneles de Asesores y Supervisores (`AsesorDashboard.razor`, `SupervisorDashboard.razor`).

---

## Síntomas
- Los asesores no reciben avisos instantáneos cuando una orden es aprobada o devuelta sin tener que refrescar manualmente la página (`F5`).
- En la consola del navegador (`F12`) aparece el error:
  `WebSocket connection to 'wss://crm.callcenter.local/hubs/notifications' failed: Error during WebSocket handshake`.

---

## Causa Raíz Frecuente
1. **Falta de cabeceras Upgrade en NGINX:** El proxy inverso no está configurado para propagar `Upgrade: websocket` y `Connection: Upgrade`.
2. **Timeout de Keep-Alive:** Desconexión silenciosa por inactividad tras 60 segundos de inactividad de red.

---

## Diagnóstico & Solución

### Paso 1: Verificar Configuración del Bloque NGINX
Abre el archivo `/etc/nginx/sites-available/crm.conf` y asegúrate de que la ruta `/hubs/` contenga:

```nginx
location /hubs/ {
    proxy_pass http://crm_api_backend;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection "Upgrade";
    proxy_set_header Host $host;
    proxy_cache_bypass $http_upgrade;
    proxy_read_timeout 3600s;
    proxy_send_timeout 3600s;
}
```

### Paso 2: Recargar NGINX sin Corte de Servicio
```bash
sudo nginx -t && sudo systemctl reload nginx
```

### Paso 3: Verificar Política de Reconexión en el Frontend Blazor
Asegúrate de que la inicialización de la conexión en C# incluya `WithAutomaticReconnect()`:

```csharp
hubConnection = new HubConnectionBuilder()
    .WithUrl(Navigation.ToAbsoluteUri("/hubs/notifications"), options =>
    {
        options.AccessTokenProvider = () => Task.FromResult(authToken);
    })
    .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
    .Build();
```

---

## Validación
- [ ] En la consola del navegador aparece el mensaje: `[SignalR] Connected to NotificationHub via WebSockets`.
- [ ] Al emitir una notificación de prueba desde el backend, la campana de alertas se actualiza instantáneamente sin recargar la página.
