# Especificación de Servidores & Topología de Hardware

Este documento define las características técnicas del hardware, dimensionamiento de recursos y segmentación de red para los entornos de **Producción** y **Staging**.

---

## 🖥️ Topología de Servidores en Producción

| Servidor / Nodo | Rol Operativo | vCPU | Memoria RAM | Almacenamiento | Sistema Operativo | Dirección IP Interna |
|---|---|---|---|---|---|---|
| `srv-crm-app01` | CRM.ApiHub + Blazor WebFrontend | 8 vCores (3.2 GHz) | 16 GB DDR4 | 120 GB NVMe (SO + Docker) | Ubuntu Server 22.04 LTS | `10.10.20.11` |
| `srv-crm-app02` | CRM.ApiHub (Réplica Alta Disponibilidad) | 8 vCores (3.2 GHz) | 16 GB DDR4 | 120 GB NVMe (SO + Docker) | Ubuntu Server 22.04 LTS | `10.10.20.12` |
| `srv-crm-db01` | PostgreSQL 16 Primario (Lectura/Escritura) | 16 vCores (3.4 GHz) | 64 GB DDR4 ECC | 1 TB NVMe RAID 10 (Datos + WAL) | Ubuntu Server 22.04 LTS | `10.10.20.21` |
| `srv-crm-db02` | PostgreSQL 16 Réplica Streaming (Read-Only) | 16 vCores (3.4 GHz) | 64 GB DDR4 ECC | 1 TB NVMe RAID 10 (Réplica) | Ubuntu Server 22.04 LTS | `10.10.20.22` |
| `srv-crm-cache01`| Redis 7 Standalone + Sentinel | 4 vCores | 8 GB DDR4 | 60 GB SSD | Ubuntu Server 22.04 LTS | `10.10.20.31` |
| `srv-crm-edge01` | NGINX Reverse Proxy + SSL Termination | 4 vCores | 8 GB DDR4 | 60 GB SSD | Ubuntu Server 22.04 LTS | `10.10.10.5` (DMZ) |

---

## 🔒 Segmentación de Red (VLANs)

1. **DMZ Externa (`10.10.10.0/24`)**:
   - Aloja el balanceador NGINX con certificados Let's Encrypt / Wildcard corporativo.
   - Único segmento expuesto a los puertos públicos 80 (HTTP) y 443 (HTTPS).

2. **VLAN de Aplicaciones (`10.10.20.0/24`)**:
   - Aloja los contenedores de .NET 8 Kestrel y el servidor Blazor.
   - Solo acepta tráfico proveniente de la DMZ a través de los puertos 5000 y 5001.

3. **VLAN de Persistencia & Datos (`10.10.30.0/24`)**:
   - Aislada completamente de internet.
   - Solo acepta conexiones TCP 5432 desde los servidores de aplicación y TCP 6379 desde Redis.

---

## ⚡ Requisitos Mínimos para Entorno de Desarrollo Local
- **CPU:** 4 núcleos físicos.
- **RAM:** 16 GB recomendados (mínimo 8 GB).
- **Disco:** 20 GB de espacio libre para imágenes Docker de PostgreSQL 16, Redis y SDK de .NET 8.
- **Docker Engine:** v24.0+ con Docker Compose v2.20+.
