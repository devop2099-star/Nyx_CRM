# Backups (Copias de Seguridad)

## Política de Respaldo
- **Frecuencia**: Diaria a las 02:00 AM.
- **Retención**: 30 días en S3, 7 días locales.
- **Tipo**: Logical backup (pg_dump).

## Script de Ejecución Automática
Un cronjob ejecuta el siguiente script:
```bash
#!/bin/bash
pg_dump -U postgres -h localhost -d crm_db -F c > /backups/crm_db_$(date +%Y%m%d).backup
aws s3 cp /backups/crm_db_$(date +%Y%m%d).backup s3://my-bucket/crm-backups/
```

## Elementos NO respaldados
- Tablas FDW (`legacy_fdw.*`): Estos datos residen en la base de datos externa y deben ser respaldados por las políticas de ese sistema.
