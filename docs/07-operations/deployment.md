# Guía Operativa de Despliegue (Deployment)

## Requisitos Previos
- .NET 8.0 SDK / Runtime instalado en el servidor objetivo.
- PostgreSQL 16 activo con permisos de creación de tablas/extensiones FDW.
- Nginx configurado como Proxy Reverso con certificado SSL.

---

## 🚀 Pasos para Despliegue en Producción

### 1. Compilación del Backend `CRM.ApiHub`
```bash
dotnet publish CRM.ApiHub/CRM.ApiHub.csproj -c Release -o /var/www/crm-api
```

### 2. Compilación del Frontend `CRM.WebFrontend`
```bash
dotnet publish CRM.WebFrontend/CRM.WebFrontend.csproj -c Release -o /var/www/crm-web
```

### 3. Aplicación de Migraciones de Base de Datos
```bash
dotnet ef database update --project CRM.ApiHub/CRM.ApiHub.csproj
```

### 4. Reinicio de Servicios Systemd / Nginx
```bash
sudo systemctl restart crm-apihub.service
sudo systemctl restart crm-webfrontend.service
sudo systemctl reload nginx
```

---

## 🔄 Procedimiento de Rollback de Emergencia
En caso de detectar fallas críticas post-despliegue:
1. Revertir al ejecutable anterior ubicado en `/var/www/crm-api.bak`.
2. Ejecutar `sudo systemctl restart crm-apihub.service`.
