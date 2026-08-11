# Matriz de Dependencias & Paquetes NuGet — CRM.ApiHub

Este documento cataloga las dependencias tecnológicas del proyecto .NET 8, especificando su versión exacta, capa arquitectónica de uso y justificación técnica.

---

## 📦 Paquetes NuGet Principales

| Paquete NuGet | Versión | Capa Arquitectónica | Justificación & Uso |
|---|---|---|---|
| `MediatR` | `12.2.0` | **Application** | Implementación del patrón Mediator y desacoplamiento CQRS entre controladores y lógica de negocio. |
| `FluentValidation.AspNetCore` | `11.3.0` | **Application** | Validación declarativa y desacoplada de comandos y consultas antes de llegar al dominio. |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | `8.0.2` | **Infrastructure** | Proveedor oficial de Entity Framework Core para PostgreSQL 16 con soporte nativo para tipos `uuid` y `jsonb`. |
| `StackExchange.Redis` | `2.7.27` | **Infrastructure** | Cliente de alto rendimiento para Redis 7 utilizado en la lista negra de tokens JWT y caché distribuida. |
| `BCrypt.Net-Next` | `4.0.3` | **Domain / Infra** | Algoritmo de derivación de claves con Work Factor 12 para hashing seguro de contraseñas de usuario. |
| `Polly` | `8.3.0` | **Infrastructure** | Políticas de resiliencia, reintentos exponenciales y Circuit Breaker para llamadas HTTP a GLPI y SMTP. |
| `MailKit` | `4.3.0` | **Infrastructure** | Cliente SMTP moderno con soporte completo para STARTTLS (puerto 587) y adjuntos multipart. |
| `Serilog.AspNetCore` | `8.0.1` | **Presentation** | Registro estructurado de logs en formato JSON para trazabilidad de peticiones y observabilidad. |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | `8.0.3` | **Presentation** | Middleware de validación y extracción de claims de tokens JWT en cada petición HTTP entrante. |
| `Microsoft.AspNetCore.SignalR` | `8.0.3` | **Presentation** | Hub de comunicación bidireccional mediante WebSockets para alertas push en tiempo real. |

---

## 🛡️ Reglas de Dependencias entre Capas (Arquitectura Hexagonal)

1. **Domain (Núcleo Puro)**:
   - **Cero dependencias externas**. No hace referencia a Entity Framework, ASP.NET Core ni librerías de infraestructura.
   - Solo contiene entidades, Value Objects, eventos de dominio y contratos de interfaces (`IRepository`, `IUnitOfWork`).

2. **Application (Casos de Uso)**:
   - Depende **únicamente** de la capa de Dominio.
   - Utiliza `MediatR` y `FluentValidation` para orquestar los casos de uso.

3. **Infrastructure (Adaptadores Secundarios)**:
   - Depende de `Domain` y `Application` para implementar las interfaces definidas en el núcleo.
   - Contiene los contextos de base de datos (`AppDbContext`), clientes HTTP externos y repositorios concretos.

4. **Presentation / Web API (Adaptadores Primarios)**:
   - Depende de `Application` y configura la inyección de dependencias (`Program.cs`).
   - Expone endpoints REST y Hubs de SignalR sin contener lógica de negocio directa.
