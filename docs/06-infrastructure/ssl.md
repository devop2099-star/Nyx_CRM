# Certificados SSL / TLS

## Proveedor
Para los entornos públicos (Staging, Prod), se utiliza **Let's Encrypt** con **Certbot**.

## Configuración Automática
El contenedor `certbot` está configurado en un cron para validar y renovar el certificado cada 60 días antes de que expire (que es a los 90 días).

## Comandos de Troubleshooting
- Renovación manual: `certbot renew --force-renewal`
- Verificación en crontab: `crontab -l | grep certbot`
