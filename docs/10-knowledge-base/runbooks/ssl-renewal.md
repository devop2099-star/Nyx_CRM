# Renovación manual de certificados SSL vencidos

## Síntomas
- Alerta en monitoreo: "Certificado SSL expirado".
- Usuarios reportan `ERR_CERT_DATE_INVALID` en el navegador.

## Procedimiento (Mitigación)
1. Conectar al servidor de producción vía SSH.
2. Forzar renovación:
```bash
docker compose run --rm certbot renew --force-renewal
```
3. Reiniciar Nginx para que cargue los nuevos certificados:
```bash
docker compose restart nginx
```
4. Validar desde el navegador (forzar refresco `Ctrl+F5`).
