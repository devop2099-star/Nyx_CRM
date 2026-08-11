# Troubleshooting: Error 401 Unauthorized por Desincronización de Reloj (Clock Skew)

## Objetivo
Resolver rechazos inesperados de tokens JWT válidos (`IDX10223: Lifetime validation failed. The token is not yet valid`) provocados por diferencias horarias entre el servidor emisor de tokens y los clientes o réplicas.

---

## Síntomas
- El usuario acaba de iniciar sesión exitosamente pero al enviar su primera petición a `/api/orders` recibe un error `401 Unauthorized`.
- En los logs de Serilog aparece:
  `SecurityTokenNotYetValidException: IDX10222: Lifetime validation failed. The token is not yet valid. ValidFrom: '2026-08-10 14:00:05', Current time: '2026-08-10 13:59:58'`.

---

## Causa Raíz
Desalineación de tiempo en el servicio NTP (Network Time Protocol) entre los servidores de aplicación y la máquina del cliente, haciendo que el `nbf` (Not Before) del token sea superior a la hora local del servidor receptor.

---

## Solución

### Paso 1: Configurar ClockSkew en `Program.cs` (.NET 8)
Por defecto, .NET aplica un ClockSkew de 5 minutos, pero si se deshabilitó a `TimeSpan.Zero`, cualquier milisegundo de adelanto causa el fallo. Se recomienda establecer una tolerancia de 30 a 60 segundos:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30) // Margen de tolerancia contra desfase de reloj
        };
    });
```

### Paso 2: Sincronizar NTP en los Servidores Linux
```bash
sudo timedatectl set-ntp on
sudo systemctl restart systemd-timesyncd
timedatectl status
```

---

## Validación
- [ ] El comando `timedatectl status` muestra `System clock synchronized: yes` y `NTP service: active`.
- [ ] Los tokens recién emitidos son aceptados inmediatamente sin errores `401`.
