# Redes (Networking) y Puertos

## Diagrama de Red
El servidor de Producción utiliza una red de Docker `crm-network`. 

## Reglas de Firewall (UFW)
Solo los siguientes puertos están expuestos a internet (0.0.0.0/0):
- `80` (HTTP): Exclusivamente para redirección a HTTPS y validación de ACME Challenge (Certbot).
- `443` (HTTPS): Tráfico principal de la API y el Frontend web.

## Puertos Internos Cerrados
Los siguientes puertos están **restringidos** al localhost o a la red de Docker interna y no pueden ser accedidos desde afuera:
- `5432` (PostgreSQL)
- `6379` (Redis)
- `5000` (Kestrel API HTTP)
